using Py_Game.Defines;
using Py_Game.Game;
using Py_Game.Lobby;
using PangyaAPI;
using Py_Game.Client;
using System;
using System.Runtime.InteropServices;
using static Py_Game.GameTools.PacketCreator;
using static PangyaFileCore.IffBaseManager;
using Py_Game.Functions.Mail;
using Py_Game.MainServer;
using PangyaAPI.Auth;
using PangyaAPI.PangyaPacket;
using PangyaAPI.Tools;

namespace Py_Game.Functions
{
    public class GameMasterCoreSystem
    {
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct TNoticePrize
        {
            public uint UN;
            public uint UN2;
            public uint UN3;
            public uint TypeID;
            public uint Qtd;
        }
        public void PlayerGMJoinGame(GPlayer player, Packet packet)
        {
            var PLobby = player.Lobby;
            if (PLobby == null)
            {
                return;
            }

            if (player.GetCapability == 4 || player.GetCapability == 15)
            {
                PLobby.PlayerJoinGame(player, packet);
            }
            else
            {
                player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CANT_CREATE.ShowRoomError());
                return;
            }
        }

        public void PlayerGMCommand(GPlayer player, Packet packet)
        {
            GameBase Game;
            MailSender AddMail;
            Channel PLobby;
            uint ItemTypeID, Quantity, PlayerConnectionID;
            string Nick;
            byte Arg1;

            if (!(player.GetCapability == 4))
            {
                WriteConsole.WriteLine("HandleGMCommands: Player has requested gm command but he is not an admin");
                return;
            }

            if (!packet.ReadUInt16(out ushort CommandId))
            {
                return;
            }

            PLobby = player.Lobby;

            if (PLobby == null)
            {
                return;
            }
            switch ((GM_COMMAND)CommandId)
            {
                case GM_COMMAND.GM_Visibility: //command /visible [on/off] (Ficar Visivel Ou Invisivel)
                    {
                        packet.ReadByte(out Arg1);
                        switch ((TVISIBLE_ACTION)Arg1)
                        {
                            case TVISIBLE_ACTION.Enable: //visibilidade: on
                                {
                                    player.Visible = 4;
                                }
                                break;
                            case TVISIBLE_ACTION.Disable://visibilidade: off                               
                                {
                                    player.Visible = 0;
                                }
                                break;
                        }
                        PLobby.UpdatePlayerLobbyInfo(player);
                        break;
                    }
                case GM_COMMAND.Player_Whisper: //command /whisper  [on/off] 
                    {
                        packet.ReadByte(out Arg1);
                        switch ((TWHISPER_ACTION)Arg1)
                        {
                            case TWHISPER_ACTION.Disable: //whisper: off
                                {
                                }
                                break;
                            case TWHISPER_ACTION.Enable://whiper: on                                 
                                {
                                }
                                break;
                        }
                        player.SendResponse(new byte[] { 0x0F, 0x00 });
                    }
                    break;
                case GM_COMMAND.Player_Lobby: //command /TLobby  [on/off] 
                    {
                        packet.ReadByte(out Arg1);
                        switch (Arg1)
                        {
                            case 0: //lobby: off
                                {
                                }
                                break;
                            case 2://lobby: on                                 
                                {
                                }
                                break;
                        }
                        player.SendResponse(new byte[] { 0x0F, 0x00 });
                    }
                    break;
                case GM_COMMAND.Player_Open: //command /open [nick]
                    {
                        packet.ReadPStr(out Nick);
                        WriteConsole.WriteLine("test =>" + Nick);
                    }
                    break;
                case GM_COMMAND.Player_Close: //command /close [nick]
                    {
                        packet.ReadPStr(out Nick);
                        WriteConsole.WriteLine("test =>" + Nick);
                    }
                    break;
                case GM_COMMAND.Player_Kick: //command /kick [nick] [op]
                    {
                        if (!packet.ReadUInt32(out PlayerConnectionID)) { return; }

                        var client = PLobby.GetPlayerByConnectionId(PlayerConnectionID);

                        if (client == null) return;

                        player.SendResponse(new byte[] { 0x0F, 0x00 });

                        client.Close();
                    }
                    break;
                case GM_COMMAND.Player_Disconnect_By_UID: //command /discon_uid [uid]
                    {
                        if (!packet.ReadUInt32(out PlayerConnectionID)) { return; }

                        var client = PLobby.GetPlayerByConnectionId(PlayerConnectionID);

                        if (client == null) return;

                        client.SendResponse(new byte[] { 0x76, 0x02, 0xFA, 0x00, 0x00, 0x00 });

                        client.Close();

                    }
                    break;
                case GM_COMMAND.Player_Change_GameWind: //Command /wind [spd] [dir] 
                    {
                        packet.ReadByte(out byte WP);

                        packet.ReadByte(out byte WD);

                        Game = PLobby.GetGameHandle(player);

                        if (Game == null) return;

                        if (Game != null && Game.GameType != GAME_TYPE.CHAT_ROOM)
                        {
                            Game.Send(ShowWind(WP, WD));
                        }
                    }
                    break;
                case GM_COMMAND.Player_Change_GameWeather: //Command /weather [type] 'fine', 'rain', 'snow', 'cloud' (Chuva, Neve ...)
                    {
                        packet.ReadByte(out Arg1);

                        Game = PLobby.GetGameHandle(player);

                        if (Game == null) return;

                        Game.Send(ShowWeather(Arg1));
                    }
                    break;
                case GM_COMMAND.Player_GiveItem: //giveitem: /giveitem [nick][typeid][num]
                    {
                        packet.ReadUInt32(out PlayerConnectionID); //meu id de conexão ou do client
                        packet.ReadUInt32(out ItemTypeID); //id do item enviado
                        packet.ReadUInt32(out Quantity); //quantidade de itens enviado

                        if (!IffEntry.IsExist(ItemTypeID))
                        {
                            return;
                        }

                        var Client = (GPlayer)(player.Server).GetClientByConnectionId(PlayerConnectionID);

                        if (null == Client)
                        {
                            return;
                        }
                        AddMail = new MailSender();
                       
                        try
                        {
                            AddMail.Sender = "@GM";
                            AddMail.AddText("GM presents you");
                            AddMail.AddItem(ItemTypeID, Quantity, true);
                            // Add to db
                            AddMail.Send(Client.GetUID);
                            Client.SendMailPopup();

                            player.SendResponse(new byte[] { 0x0F, 0x00 });
                        }
                        finally
                        {
                            AddMail.Dispose();
                        }
                    }
                    break;
                case GM_COMMAND.Player_GoldenBell: //Command goldenbell ID (enviar item para todos da sala)
                    {

                        //id do item enviado
                        if (!packet.ReadUInt32(out ItemTypeID))
                        {
                            return;
                        }
                        //quantidade de itens enviado
                        if (!packet.ReadUInt32(out Quantity))
                        {
                            return;
                        }
                        //Checagem do item 
                        if (!IffEntry.IsExist(ItemTypeID))
                        {
                            return;
                        }

                        Game = PLobby.GetGameHandle(player);

                        if (Game == null)
                        {
                            return;
                        }
                        AddMail = new MailSender();

                        try
                        {                            
                            foreach (var Client in Game.Players)
                            {
                                AddMail.Sender = "@GM";
                                AddMail.AddText("GM presents you");
                                AddMail.AddItem(ItemTypeID, Quantity, true);
                                // Add to db
                                AddMail.Send(Client.GetUID);
                                Client.SendMailPopup();
                            }
                        }
                        finally
                        {
                            AddMail.Dispose();
                        }
                    }
                    break;
                case GM_COMMAND.HioHoleCupScale:
                    {

                    }
                    break;
                case GM_COMMAND.SetMission: //Command /setmission [MISSION_NUM]
                    {
                        packet.ReadByte(out byte MissionID);

                        WriteConsole.WriteLine("SetMission => " + MissionID);
                    }
                    break;
                case GM_COMMAND.MatchMap://Command /matchmap [mapcount]
                    {
                        packet.ReadUInt32(out uint MapCount);

                        WriteConsole.WriteLine("MatchMap => " + MapCount);
                    }
                    break;
                case GM_COMMAND.Notice_Prize:
                    {
                        //List<TNoticePrize> itens;
                        if (!packet.ReadByte(out Arg1))
                        { return; }
                        switch (Arg1)
                        {
                            case 0: //lobby: off
                                {
                                    if (!packet.ReadUInt32(out uint Count))
                                    {
                                        return;
                                    }
                                    for (int i = 0; i < Count; i++)
                                    {
                                        var item = (TNoticePrize)packet.Read(new TNoticePrize());
                                    }
                                }
                                break;
                            case 2://lobby: on                                 
                                {
                                }
                                break;
                        }
                        player.SendResponse(new byte[] { 0x0F, 0x00 });
                    }
                    break;
                default:
                    {
                        WriteConsole.WriteLine("Command ID UNK => " + CommandId);
                        packet.Save();
                    }
                    break;

            }
        }

        public void PlayerGMSendNotice(GPlayer player, Packet packet)
        {
            if (!(player.GetCapability == 4))
            {
               throw new Exception(" GM was trying to Messages but he is not an admin");
            }
            if (!packet.ReadPStr(out string Messages)) { return; }

            Program.SendAuth(new AuthPacket() { ID = AuthPacketEnum.SERVER_RELEASE_NOTICE_GM, Message = new { GetNick = player.GetNickname, mensagem = Messages } });
        }

        public void PlayerGMChangeIdentity(GPlayer player, Packet packet)
        {
            if (!(player.GetCapability == 4))
            {
                throw new Exception(" GM was trying to Type but he is not an admin");
            }

            if (!packet.ReadUInt32(out uint Mode)) { return; }

            if (!packet.ReadPStr(out string PlayerName)) { return; }


            player.Response.Write(new byte[] { 0x9A, 0x00 });
            switch (Mode)
            {
                case 128:
                    {
                        player.Visible = byte.MaxValue;
                    }
                    break;
                case 4294967295:
                    {
                        player.Visible = 4;
                    }
                    break;
            }
            if (player.GetLogin == PlayerName)
            {
                player.Response.WriteUInt64(player.Visible);
            }
            else
            {
                player.Response.WriteUInt64(0);
            }
            player.SendResponse();

            player.SendResponse(new byte[] { 0x0F, 0x00 });
        }

        public void PlayerGMDestroyRoom(GPlayer player, Packet packet)
        {
            if (!(player.GetCapability == 4))
            {
                new Exception("HandleGMDestroyRoom: GM was trying to destroy a room but he is not an admin");
            }
            if (!packet.ReadUInt16(out ushort GameID)) { return; }
            var PLobby = player.Lobby;
            var GameHandle = PLobby[GameID];
            if (GameHandle == null)
            {
                return;
            }
            GameHandle.DestroyRoom();
        }

        public void PlayerGMDisconnectUserByConnectID(GPlayer player, Packet packet)
        {
            if (!(player.GetCapability == 4))
            {
                new Exception("HandleGMDisconnectUserByConnectId: GM was trying to disconnect a player but he is not an admin");
            }

            if (!packet.ReadUInt32(out uint PlayerConnection)) { return; }

            var client = (GPlayer)((player.Server)).GetClientByConnectionId(PlayerConnection);

            if (client == null)
                return;

            client.SendResponse(new byte[] { 0x76, 0x02, 0xFA, 0x00, 0x00, 0x00 });

            client.Close();
        }
    }
}
