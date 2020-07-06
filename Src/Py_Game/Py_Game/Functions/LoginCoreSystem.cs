using Py_Game.Client;
using System.Linq;
using PangyaAPI;
using PangyaAPI.BinaryModels;
using static Py_Game.Lobby.Collection.ChannelCollection;
using static Py_Game.GameTools.PacketCreator;
using Py_Connector.DataBase;
using Py_Game.MainServer;
using System;
using PangyaAPI.PangyaPacket;
using PangyaAPI.Tools;

namespace Py_Game.Functions
{
    public class LoginCoreSystem
    {
        public void PlayerLogin(GPlayer PL, Packet packet)
        {
            //GameServer GameServer;


            if (!packet.ReadPStr(out string UserID))
            {
                WriteConsole.WriteLine("[CLIENT_PLAYER]: USER UNKNOWN");
                PL.SendResponse(new byte[] { 0x76, 0x02, 0x2C, 0x01, 0x00, 0x00 }); // ## send code 300
                PL.Close();
                return;
            }

            if (!packet.ReadUInt32(out uint UID))
            {
                WriteConsole.WriteLine("[CLIENT_ERROR]: UID UNKNOWN");
                PL.SendResponse(new byte[] { 0x76, 0x02, 0x2C, 0x01, 0x00, 0x00 }); // ## send code 300
                PL.Close();
                return;
            }

            packet.Skip(6);

            if (!packet.ReadPStr(out string Code1))
            {
                WriteConsole.WriteLine("[CLIENT_ERROR]: AUTHLOGIN UNKNOWN");
                PL.SendResponse(new byte[] { 0x76, 0x02, 0x2C, 0x01, 0x00, 0x00 }); // ## send code 300
                PL.Close();
                return;
            }

            if (!packet.ReadPStr(out string Version))
            {
                WriteConsole.WriteLine("[CLIENT_ERROR]: Client Version Incompartible");
                PL.Send(new byte[] { 0x44, 0x00, 0x0B });
                PL.Close();
                return;
            }

            if (!Program.CheckVersion(Version))
            {
                WriteConsole.WriteLine("[CLIENT_ERROR]: Client Version Incompartible");
                PL.Send(new byte[] { 0x44, 0x00, 0x0B });
                PL.Close();
                return;
            }

            packet.Skip(23);

            if (!packet.ReadPStr(out string Code2))
            {
                WriteConsole.WriteLine("[CLIENT_ERROR]: AUTHGAME UNKNOWN");
                PL.SendResponse(new byte[] { 0x76, 0x02, 0x2C, 0x01, 0x00, 0x00 }); // ## send code 300
                PL.Close();
                return;
            }
            try
            {
                var _db = new PangyaEntities();
                var Query = _db.USP_GAME_LOGIN(UserID, (int)UID, Code1, Code2).FirstOrDefault();

                byte Code = (byte)Query.Code;
                if (Code != 1)
                {
                    PL.SendResponse(new byte[] { 0x76, 0x02, 0x2C, 0x01, 0x00, 0x00 }); // ## send code 300
                    WriteConsole.WriteLine("[CLIENT_ERROR]: PLAYER NULL", ConsoleColor.Red);
                    PL.SetLogin(UserID);
                    PL.Close();
                    return;
                }
                PL.SetLogin(Query.Username);
                PL.SetNickname(Query.Nickname);
                PL.SetSex((byte)Query.Sex);
                PL.SetCapabilities((byte)Query.Capabilities);
                PL.SetUID(UID);
                PL.SetCookie((uint)Query.Cookie);
                PL.LockerPang = (uint)Query.PangLockerAmt;
                PL.LockerPWD = Query.LockerPwd;
                PL.SetAuthKey1(Code1);
                PL.SetAuthKey2(Code2);
                if (Code == 1)
                {
                    PL.LoadStatistic();

                    PL.LoadGuildData();

                    SendJunkPackets(PL);

                    PlayerRequestInfo(PL, Version);
                }
            }
            catch
            {
                PL.Close();
            }
            finally
            {
                packet.Dispose();
            }
        }

        void PlayerRequestInfo(GPlayer PL, string ServerVersion)
        {
            #region HandlePlayer
            PangyaBinaryWriter Reply;

            var Inventory = PL.Inventory;

            #region PlayerLogin
            PL.SendMainPacket(ServerVersion);
            #endregion

            #region PlayerCharacterInfo
            PL.SendResponse(Inventory.ItemCharacter.Build());
            #endregion

            #region PlayerCaddieInfo            
            PL.SendResponse(Inventory.ItemCaddie.Build());
            #endregion

            #region PlayerWarehouseInfo
            PL.SendResponse(Inventory.ItemWarehouse.Build());
            #endregion

            #region PlayerMascotsInfo            
            PL.SendResponse(Inventory.ItemMascot.Build());
            #endregion

            #region PlayerToolBarInfo
            PL.SendResponse(Inventory.GetToolbar());
            #endregion

            #region PlayerLobbyListInfo
            PL.SendResponse(LobbyList.Build(true));
            #endregion

            #region Map Rate
            PL.SendResponse(ShowLoadMap());
            #endregion

            #region PlayerAchievement
            PL.ReloadAchievement();

            PL.SendAchievementCounter();

            PL.SendAchievement();
            #endregion

            #region Call Messeger Server
            new MessengerServerCoreSystem().PlayerCallMessengerServer(PL);
            #endregion

            #region PlayerCardInfo
            PL.SendResponse(Inventory.ItemCard.Build());

            PL.SendResponse(new byte[] { 0x36, 0x01 });
            #endregion

            #region PlayerCardEquipInfo
            PL.SendResponse(Inventory.ItemCardEquip.Build());

            PL.SendResponse(new byte[] { 0x81, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00 });
            #endregion

            #region PlayerCookies
            PL.SendCookies();
            #endregion

            #region TROPHY
            Reply = new PangyaBinaryWriter();
            Reply.Write(Inventory.ItemTrophies.Build(5));
            PL.SendResponse(Reply.GetBytes());

            Reply = new PangyaBinaryWriter();
            Reply.Write(Inventory.ItemTrophies.Build(0));
            PL.SendResponse(Reply.GetBytes());

            Reply = new PangyaBinaryWriter();
            Reply.Write(Inventory.ItemTrophySpecial.Build(5));
            PL.SendResponse(Reply.GetBytes());

            Reply = new PangyaBinaryWriter();
            Reply.Write(Inventory.ItemTrophySpecial.Build(0));
            PL.SendResponse(Reply.GetBytes());

            Reply = new PangyaBinaryWriter();
            Reply.Write(Inventory.ItemTrophyGP.Build(5));
            PL.SendResponse(Reply.GetBytes());

            Reply = new PangyaBinaryWriter();
            Reply.Write(Inventory.ItemTrophyGP.Build(0));
            PL.SendResponse(Reply.GetBytes());
            #endregion

            #region StatisticInfo
            Reply = new PangyaBinaryWriter();
            Reply.Write(new byte[] { 0x58, 01, 0x00 });
            Reply.Write(PL.GetUID);
            Reply.Write(PL.Statistic());
            PL.SendResponse(Reply.GetBytes());
            #endregion

            #region MailGiftBox
            PL.SendMailPopup();
            #endregion

            #region ChatOffLine
            // PL.SendChatOffline();
            #endregion

            #region Check Exist Item 467664918
            if (PL.Inventory.GetQuantity(467664918) == 1)
            { PL.Assist = 1; }
            #endregion

            #region PlayerGetMessengerServerInfo
            new MessengerServerCoreSystem().PlayerConnectMessengerServer(PL);
            #endregion
            #endregion
        }

        public void SendJunkPackets(GPlayer PL)
        {
            PL.SendResponse(new byte[] { 0x44, 0x00, 0xD3, 0x00 });

            PL.SendResponse(ShowLoadServer(0x01));

            PL.SendResponse(ShowLoadServer(0x03));

            PL.SendResponse(ShowLoadServer(0x09));

            PL.SendResponse(ShowLoadServer(0x07));

            PL.SendResponse(ShowLoadServer(0x0B));

            PL.SendResponse(ShowLoadServer(0x0D));

            PL.SendResponse(ShowLoadServer(0x17));

            PL.SendResponse(ShowLoadServer(0x0F));

            PL.SendResponse(ShowLoadServer(0x13));

            PL.SendResponse(ShowLoadServer(0x1E));

            PL.SendResponse(ShowLoadServer(0x19));

            PL.SendResponse(ShowLoadServer(0x1B));

            PL.SendResponse(ShowLoadServer(0x12));

            PL.SendResponse(ShowLoadServer(0x14));

            new TutorialCoreSystem(PL);

            PL.SendResponse(ShowLoadServer(0x1D));
        }
    }
}
