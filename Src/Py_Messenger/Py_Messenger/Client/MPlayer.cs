using PangyaAPI;
using PangyaAPI.PangyaClient;
using PangyaAPI.PangyaPacket;
using PangyaAPI.Tools;
using Py_Connector.DataBase;
using Py_Messenger.Client.Data;
using Py_Messenger.CreatePacket;
using Py_Messenger.Defines;
using System;
using System.Linq;
using System.Net.Sockets;
namespace Py_Messenger.Client
{
    public partial class MPlayer : Player
    {
        #region Handle Player

        public GuildData Guild;
        public ServerProcess ServerInfo { get; set; }
        public MPlayer(TcpClient tcp) : base(tcp)
        {
            _db = new PangyaEntities();
            Guild = new GuildData();
            ServerInfo = new ServerProcess();
        }

        void HandlePlayerLogin(Packet packet)
        {
            #region HandleLogin

            if (!packet.ReadUInt32(out uint UID))
            {
                SendResponse(new byte[] { 0x2F, 0x00, 0x00 });
                return;
            }

            if (!packet.ReadPStr(out string UserID))
            {
                SendResponse(new byte[] { 0x2F, 0x00, 0x00 });
                return;
            }

            try
            {
                var Query = _db.USP_MESSENGER_LOGIN((int)UID, UserID).FirstOrDefault();


                byte Code = (byte)Query.Code;
                if (Code != 0)
                {
                    SendResponse(new byte[] { 0x2F, 0x00, 0x00 });
                    return;
                }
                this.SetLogin(Query.Username);
                this.SetNickname(Query.Nickname);
                this.SetUID(UID);
                this.SetGuildId((uint)Query.GUILD_ID);

                Send(PacketCreator.ShowLogin(UID));
            }
            catch
            {
                packet.Log();
                SendResponse(new byte[] { 0x2F, 0x00, 0x00 });
                _db.Dispose();
                return;
            }

            #endregion
        }

        void HandleServerData(Packet packet)
        {
            #region Handle Lobby Selected
            ServerInfo = (ServerProcess)packet.Read(new ServerProcess());
                       
            SendResponse(PacketCreator.ShowConnectionServer(GetUID, USER_STATUS.IS_ONLINE, ServerInfo));

            SendResponse(PacketCreator.ShowListFriends(Server.Players.Model));
            #endregion
        }

        void HandlePlayerConnected(Packet packet)
        {
            if (!packet.ReadByte(out byte Connected)) { return; }

            switch ((USER_STATUS)Connected)
            {
                case USER_STATUS.IS_ONLINE:
                    {
                        WriteConsole.WriteLine($"PLAYER's ONLINE [{GetNickname}/{(USER_STATUS)Connected}]", ConsoleColor.Green);
                    }
                    break;
                case USER_STATUS.IS_IDLE:
                    {
                        WriteConsole.WriteLine($"PLAYER's OFFLINE [{GetNickname}{(USER_STATUS)Connected}]", ConsoleColor.Red);
                    }
                    break;
                case USER_STATUS.IS_RECONNECT:
                    {
                        WriteConsole.WriteLine($"PLAYER's RECONNECT [{GetNickname} | {(USER_STATUS)Connected}]", ConsoleColor.White);

                        SendResponse(PacketCreator.ShowConnectionServer(GetUID, USER_STATUS.IS_RECONNECT, ServerInfo));
                    }
                    break;
                default:
                    {
                        packet.Log();
                    }
                    break;
            }
            
        }

        void HandlePlayerDisconnect()
        {
            Response.Write(new byte[] { 0x30, 0x00, 0x0F, 0x01 });
            Response.Write(GetUID);
            SendResponse();
        }


        void HandleFindFriend(Packet packet)
        {

            if (!packet.ReadPStr(out string Friend))
            {
                SendResponse(PacketCreator.ShowFindFriend(false, "", 0));
                return;
            }

            var search = (MPlayer)Server.GetPlayerByNickname(Friend);

            if (search == null)
            {
                SendResponse(PacketCreator.ShowFindFriend(false, "", 0));
                return;
            }
            SendResponse(PacketCreator.ShowFindFriend(true, search.GetNickname, search.GetUID));
        }

        void HandleAddFriend(Packet packet)
        {

            if (!packet.ReadUInt32(out uint Friend_ID))
            {
                SendResponse(new byte[] { 0x30, 0x00, 0x04, 0x01, 0x01, 0x00, 0x00, 0x00 });
                return;
            }

            if (!packet.ReadPStr(out string Friend_Nick))
            {
                SendResponse(new byte[]{ 0x30, 0x00, 0x04, 0x01, 0x01, 0x00, 0x00, 0x00 });
                return;
            }

            var GetFriend = (MPlayer)Server.GetPlayerByNickname(Friend_Nick);

            SendResponse(PacketCreator.ShowAddFriend(Friend_ID, Friend_Nick, GetFriend.ServerInfo));

            Response.Write(new byte[] { 0x30, 0x00, 0x09, 0x01 });
            Response.Write(ConnectionID);
            Response.Write(GetUID);
            SendResponse();
        }

        void HandleDeleteFriend(Packet packet)
        {
            if (!packet.ReadUInt32(out uint Friend_ID))
            {
                return;
            }

            if (!packet.ReadPStr(out string Friend_Nick))
            {
                return;
            }

            var GetFriend = (MPlayer)Server.GetPlayerByNickname(Friend_Nick);

            Response.Write(new byte[] { 0x30, 0x00, 0x0B, 0x01 });
            Response.Write(0);
            Response.Write(Friend_ID);

            GetFriend.Response = Response;

            SendResponse();
            GetFriend.SendResponse();
        }
        internal void Close()
        {
            Server.DisconnectPlayer(this);
        }

        #endregion
    }
}
