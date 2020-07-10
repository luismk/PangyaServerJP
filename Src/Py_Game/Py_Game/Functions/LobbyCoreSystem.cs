using System;
using System.Linq;
using Py_Game.Lobby;
using Py_Game.Client;
using PangyaAPI;
using Py_Connector.DataBase;
using static Py_Game.Lobby.Collection.ChannelCollection;
using static Py_Game.GameTools.PacketCreator;
using static Py_Game.GameTools.Tools;
using PangyaAPI.PangyaPacket;

namespace Py_Game.Functions
{
    public class LobbyCoreSystem
    {

        public void PlayerSelectLobby(GPlayer player, Packet packet, bool RequestJoinGameList = false)
        {
            var lp = player.Lobby;

            //Lê Id do Lobby
            if (!packet.ReadByte(out byte lobbyId))
            {
                return;
            }

            var lobby = LobbyList.GetLobby(lobbyId);

            if (lp != null)
            {
                lobby.RemovePlayer(player);
            }

            //Caso o lobby não existir
            if (lobby == null)
            {
                player.SendResponse(new byte[] { 0x95, 0x00, 0x02, 0x01, 0x00 });
                throw new Exception("Player Select Invalid Lobby");
            }
            //Se estiver lotado
            if (lobby.IsFull)
            {
                player.SendResponse(new byte[] { 0x4E, 0x00, 0x02 });
                throw new Exception("Player Selected Lobby Full");
            }
            // ## add player
            if (lobby.AddPlayer(player))
            {
                try
                {
                    if (RequestJoinGameList == false)
                    {
                       // player.SendResponse(new byte[] { 0x95, 0x00, 0x02, 0x01, 0x00 });

                        player.SendResponse(ShowEnterLobby(1));

                        player.SendResponse(new byte[] { 0xF6, 0x01, 0x00, 0x00, 0x00, 0x00 });
                    }
                    if (lp == null)
                    {
                        int Year = DateTime.Now.Year;
                        int Month = DateTime.Now.Month;
                        int Day = DateTime.Now.Day;
                        var _db = new PangyaEntities();
                        var LoginReg = _db.Pangya_Item_Daily_Log.FirstOrDefault(c => c.UID == player.GetUID);

                        if (LoginReg == null)
                        {
                            new LoginDailyRewardSystem().PlayerDailyLoginCheck(player, 0);
                        }
                        else
                        {
                            if (LoginReg.RegDate.Year == Year && LoginReg.RegDate.Month == Month && Day == LoginReg.RegDate.Day)
                            { new LoginDailyRewardSystem().PlayerDailyLoginCheck(player, 1); }
                            else
                            {
                                new LoginDailyRewardSystem().PlayerDailyLoginCheck(player, 0);
                            }
                        }
                    }
                    // ## if request join lobby
                    if (RequestJoinGameList)
                    {
                        //player.SendResponse(new byte[] { 0x95, 0x00, 0x02, 0x01, 0x00 });

                        player.SendResponse(ShowEnterLobby(1));

                        player.SendResponse(new byte[] { 0xF6, 0x01, 0x00, 0x00, 0x00, 0x00 });

                        lobby.JoinMultiplayerGamesList(player);
                    }
                }
                finally
                {

                }
            }
        }

        public void PlayerJoinMultiGameList(GPlayer player, bool GrandPrix = false)
        {
            var lobby = player.Lobby;

            if (lobby == null) return;

            lobby.JoinMultiplayerGamesList(player);

            if (GrandPrix)
            {
                player.SendResponse(new byte[]
               {
                    0x50, 0x02, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02,
                    0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x34, 0x43
               });
            }
            else
            {
                player.SendResponse(new byte[] { 0xF5, 0x00 });
            }
        }

        public void PlayerLeaveMultiGamesList(GPlayer player, bool GrandPrix = false)
        {
            var lobby = player.Lobby;

            if (lobby == null) return;

            lobby.LeaveMultiplayerGamesList(player);

            if (GrandPrix)
            {
                player.SendResponse(new byte[]
               {
                    0x51, 0x02, 0x00, 0x00, 0x00, 0x00
               });
            }
            else
            {
                player.SendResponse(new byte[] { 0xF6, 0x00 });
            }
        }

        public void PlayerChat(GPlayer player, Packet packet)
        {
            var PLobby = player.Lobby;
            if (PLobby == null)
            {
                return;
            }
            packet.ReadPStr(out string Nickname);
            packet.ReadPStr(out string Messages);

            if (!(Nickname == player.GetNickname))
            {
                return;
            }
            PLobby.PlayerSendChat(player, Messages);
        }

        public void PlayerWhisper(GPlayer player, Packet packet)
        {
            Channel PLobby;

            PLobby = player.Lobby;
            if (PLobby == null)
            {
                return;
            }

            if (!packet.ReadPStr(out string Nickname))
            {

            }

            if (!packet.ReadPStr(out string Messages))
            {

            }

            PLobby.PlayerSendWhisper(player, Nickname, Messages);
        }

        public void PlayerChangeNickname(GPlayer player, Packet packet)
        {
            if (!packet.ReadPStr(out string nick)) { return; }

            if (nick.Length < 4 || nick.Length > 16)
            {
                ShowChangeNickName(1);
                return;
            }

            if (player.GetCookie < 1500)
            {
                ShowChangeNickName(4);
                throw new Exception($"Player not have cookies enough: {player.GetCookie}");
            }

            var CODE = 1;
            //Nickname duplicate
            if (CODE == 2 || CODE == 0)
            {
                ShowChangeNickName(2);
                return;
            }
            //Sucess
            if (CODE == 1)
            {
                ShowChangeNickName(0, nick);

                player.SetNickname(nick);
                //se não for gm ou A.I
                if (player.GetCapability != 4 || player.GetCapability != 15)
                {
                    player.RemoveCookie(500);//debita 

                    player.SendCookies();
                }

                var lobby = player.Lobby;
                if (lobby != null)
                {
                    lobby.UpdatePlayerLobbyInfo(player);
                }
            }
        }

        public void PlayerCreateGame(GPlayer player, Packet packet)
        {
            var PLobby = player.Lobby;
            if (PLobby == null && player.Game != null)
            {
                return;
            }
            PLobby.PlayerCreateGame(player, packet);
        }

        public void PlayerLeaveGame(GPlayer player)
        {
            var PLobby = player.Lobby;
            if (PLobby == null)
            {
                return;
            }
            PLobby.PlayerLeaveGame(player);
        }

        public void PlayerLeaveGP(GPlayer player)
        {
            var PLobby = player.Lobby;
            if (PLobby == null)
            {
                return;
            }
            PLobby.PlayerLeaveGP(player);
        }

        public void PlayerJoinGame(GPlayer player, Packet packet)
        {
            var PLobby = player.Lobby;
            if (PLobby == null)
            {
                return;
            }
            PLobby.PlayerJoinGame(player, packet);
        }

        public void PlayerGetLobbyInfo(GPlayer player)
        {
            player.Response.Write(LobbyList.GetBuildServerInfo());
            player.SendResponse();
        }

        public void PlayerGetGameInfo(GPlayer player, Packet packet)
        {
            var PLobby = player.Lobby;
            if (PLobby == null)
            {
                return;
            }
            PLobby.PlayerRequestGameInfo(player, packet);
        }

        public void PlayerEnterGP(GPlayer player, Packet packet)
        {
            var PLobby = player.Lobby;
            if (PLobby == null)
            {
                return;
            }
            PLobby.PlayerJoinGrandPrix(player, packet);
        }

        public void PlayerGetTime(GPlayer player)
        {
            player.Response.Write(new byte[] { 0xBA, 0x00 });
            player.Response.Write(GameTime());
            player.SendResponse();
        }
    }
}
