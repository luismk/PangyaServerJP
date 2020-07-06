using System;
using System.Net.Sockets;
using Py_Connector.DataBase;
using System.Linq;
using PangyaAPI.BinaryModels;
using System.Text;
using PangyaAPI.PangyaClient;
using PangyaAPI.Tools;
using PangyaAPI.PangyaPacket;
namespace Py_Login
{
    public class LPlayer : Player
    {
        public LPlayer(TcpClient tcp) : base(tcp)
        {
        }
      
        public void HandleRequestPacket(PangyaPacketsEnum PacketID, Packet ProcessPacket)
        {
            WriteConsole.WriteLine($"[PLAYER_REQUEST_PACKET] --> [{PacketID}, {this.GetLogin}]");
            switch (PacketID)
            {
                case PangyaPacketsEnum.PLAYER_LOGIN:
                    HandlePlayerLogin(ProcessPacket);
                    break;
                case PangyaPacketsEnum.PLAYER_SELECT_SERVER:
                    this.SendGameAuthKey();
                    break;
                case PangyaPacketsEnum.PLAYER_DUPLCATE_LOGIN:
                    this.HandleDuplicateLogin();
                    break;
                case PangyaPacketsEnum.PLAYER_SET_NICKNAME:
                    this.CreateCharacter(ProcessPacket);
                    break;
                case PangyaPacketsEnum.PLAYER_CONFIRM_NICKNAME:
                    this.NicknameCheck(ProcessPacket);
                    break;
                case PangyaPacketsEnum.PLAYER_SELECT_CHARACTER:
                    RequestCharacaterCreate(ProcessPacket);
                    break;
                case PangyaPacketsEnum.PLAYER_RECONNECT:
                    HandlePlayerReconnect(ProcessPacket);
                    break;
                case PangyaPacketsEnum.NOTHING:
                default:
                    {
                        StringBuilder sb = new StringBuilder();

                        for (int i = 0; i < ProcessPacket.GetRemainingData.Length; i++)
                        {
                            if ((i + 1) == ProcessPacket.GetRemainingData.Length)
                            {
                                sb.Append("0x" + ProcessPacket.GetRemainingData[i].ToString("X2") + "");
                            }
                            else
                            {
                                sb.Append("0x" + ProcessPacket.GetRemainingData[i].ToString("X2") + ", ");
                            }
                        }

                        WriteConsole.WriteLine("{Unknown Packet} -> " + sb.ToString(), ConsoleColor.Red);
                        Disconnect();
                    }
                    break;
            }
            
        }

        public bool SetAUTH_KEY_1(string Key1)
        {
            bool result;
            this.GetAuth1 = Key1;
            result = true;
            return result;
        }

        public bool SetAUTH_KEY_2(string Key2)
        {
            bool result;
            this.GetAuth2 = Key2;
            result = true;
            return result;
        }


        public bool SetLogin(string TLogin)
        {
            bool result;
            this.GetLogin = TLogin;
            result = true;
            return result;
        }

        public bool SetNickname(string TNickname)
        {
            bool result;
            this.GetNickname = TNickname;
            result = true;
            return result;
        }

        public bool SetSocket(TcpClient tcp)
        {
            bool result;
            Tcp = tcp;
            result = true;
            return result;
        }


        public bool SetUID(int TUID)
        {
            bool result;
            this.GetUID = (uint)TUID;
            result = true;
            return result;
        }

        public bool SetFirstLogin(byte First)
        {
            bool result;
            GetFirstLogin = First;
            result = true;
            return result;
        }

        private void HandleDuplicateLogin()
        {
            if (this.GetFirstLogin == 0)
            {
                Response.Clear();
                Response.Write(new byte[] { 0x0F, 0x00, 0x00 });
                Response.WritePStr(GetLogin);
                Send(Response.GetBytes());

                Response.Clear();
                Response.Write(new byte[] { 0x01, 0x00 });
                Response.WriteByte(0xD9);
                Response.WriteUInt32(uint.MaxValue);
                Send(Response.GetBytes());
                return;
            }

            if (this.GetFirstLogin == 1)
            {
                this.SendPlayerLoggedOnData();
            }
        }

        private void HandlePlayerLogin(Packet ClientPacket)
        {
            string Nickname, Auth1, Auth2;
            Byte Code, Banned, FirstSet;
            int UID;
            USP_LOGIN_SERVER_Result UspLoginServer = null;
            if (Program.Server.IsUnderMaintenance)
            {
                Send(new byte[] { 0x01, 0x00, 0xE3, 0x48, 0xD2, 0x4D, 0x00 });
                Disconnect();
                return;
            }

            if (!ClientPacket.ReadPStr(out string User))
            {
                return;
            }

            if (!ClientPacket.ReadPStr(out string Pwd))
            {
                return;
            }

            try
            {
                Auth1 = RandomAuth(7);
                Auth2 = RandomAuth(7);

                if(Pwd.Length == 32)
                {
                   var result = _db.USP_LOGIN_SERVER_US(User, Pwd, GetAddress, Auth1, Auth2).FirstOrDefault();
                    UspLoginServer = new USP_LOGIN_SERVER_Result
                    {
                        CODE = result.CODE,
                        FirstSet = result.FirstSet,
                        Nickname = result.Nickname,
                        IDState = result.IDState,
                        Logon = result.Logon, UID = result.UID
                    };
                }
                if (Pwd.Length <= 32)
                {
                    UspLoginServer = _db.USP_LOGIN_SERVER(User, Pwd, GetAddress, Auth1, Auth2).FirstOrDefault();
                }
                Code = (byte)UspLoginServer.CODE;
                // {-- USER NOT FOUND --}
                var OUTCODE = (Code == 0) && (UspLoginServer.FirstSet == null);
                if (OUTCODE || Code == 5)
                {
                    Send(new byte[] { 0x01, 0x00, 0xE3, 0x6F, 0xD2, 0x4D, 0x00, });
                    Disconnect();
                    return;
                }

                // {-- PASSWORD ERROR --}
                if (Code == 6)
                {
                    Response.Clear();
                    Response.Write(new byte[] { 0x01, 0x00, 0xE3, 0x5B, 0xD2, 0x4D, 0x00, });
                    Send(Response.GetBytes());
                    Disconnect();
                    return;
                }

                Banned = (byte)UspLoginServer.IDState;

                if (Banned > 0)
                {
                    Response.Clear();
                    Response.Write(new byte[] { 0x01, 0x00, 0xE3, 0xF4, 0xD1, 0x4D, 0x00, });
                    Send(Response.GetBytes());
                    Disconnect();
                    return;
                }

                FirstSet = (byte)UspLoginServer.FirstSet;
                UID = UspLoginServer.UID;
                Nickname = UspLoginServer.Nickname;


                this.SetLogin(User);
                this.SetUID(UID);
                this.SetNickname(Nickname);
                this.SetAUTH_KEY_1(Auth1);
                this.SetAUTH_KEY_2(Auth2);
                this.SetFirstLogin(FirstSet);

               // Server.AuthServer.Send(new PangyaAPI.Auth.AuthPacket() { ServerType = PangyaAPI.Auth.AuthClientTypeEnum.LoginServer,ID = PangyaAPI.Auth.AuthPacketEnum.RECEIVES_USER_UID, Message = new { ID = UID} }) ;

                if (UspLoginServer.Logon == 1)
                {
                    Send(new byte[] { 0x01, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00 });
                    return;
                }

                if (string.IsNullOrEmpty(Nickname))
                {
                    Send(new byte[] { 0x01, 0x00, 0x0D8, 0x00, 0x00, 0x00, 0x00 });
                    return;
                }

                if (FirstSet == 0)
                {
                    Response.Clear();
                    Response.Write(new byte[] { 0x0F, 0x00, 0x00 });
                    Response.WritePStr(User);
                    Send(Response.GetBytes());

                    Response.Clear();
                    Response.Write(new byte[] { 0x01, 0x00 });
                    Response.WriteByte(0xD9);
                    Response.WriteUInt32(uint.MaxValue);
                    Send(Response.GetBytes());
                    return;
                }
            }
            catch
            {
                Send(new byte[] { 0x01, 0x00, 0xE3, 0x6F, 0xD2, 0x4D, 0x00, });
                Disconnect();
                return;
            }

            //new login
            if (GetFirstLogin == 1)
            {
                this.SendPlayerLoggedOnData();
            }
        }

        private void SendPlayerLoggedOnData()
        {
            GetCapability = (byte)_db.Pangya_Member.First(c => c.UID == GetUID).Capabilities;
            GetLevel = (byte)_db.Pangya_User_Statistics.First(c => c.UID == GetUID).Game_Level;

            Response.Clear();
            Response.WriteUInt16(0x0010);
            Response.WritePStr(GetAuth1);//AuthKeyLogin
            Send(Response.GetBytes());

            Response.Clear();
            Response.Write(new byte[] { 0x01, 0x00, 0x00, });
            Response.WritePStr(GetLogin);
            Response.WriteUInt32(GetUID);
            Response.WriteUInt32(GetCapability);//Capacity
            Response.WriteUInt32(GetLevel); // Level
            Response.WriteUInt32(10);
            Response.WriteUInt16(12);
            Response.WritePStr(GetNickname);
            Send(Response.GetBytes());

            // ## GameServer
            byte[] Game = GameServerList();
            Send(Game);

            Response.Clear();
            Response.Write(new byte[] { 0x06, 0x00 });
            foreach (var data in _db.ProcGetMacro((int)GetUID))
            {
                Response.WriteStr(data.Macro1, 64);
                Response.WriteStr(data.Macro2, 64);
                Response.WriteStr(data.Macro3, 64);
                Response.WriteStr(data.Macro4, 64);
                Response.WriteStr(data.Macro5, 64);
                Response.WriteStr(data.Macro6, 64);
                Response.WriteStr(data.Macro7, 64);
                Response.WriteStr(data.Macro8, 64);
                Response.WriteStr(data.Macro9, 64);
            }
            Send(Response.GetBytes());

            // ## Messenger  
            byte[] Messanger = MessangerServerList();
            Send(Messanger);
        }

        private void SendGameAuthKey()
        {
            Response.Clear();
            Response.Write(new byte[] { 0x03, 0x00 });
            Response.WriteInt32(0);
            Response.WritePStr(GetAuth2);
            Send(Response.GetBytes());
        }

        void HandlePlayerReconnect(Packet packet)
        {

            packet.ReadPStr(out string Username);
            packet.ReadUInt32(out uint UID);
            packet.ReadPStr(out string AuthKey_Game);
            SetAUTH_KEY_1(RandomAuth(7));


            Response.Clear();
            Response.WriteUInt16(0x0010);
            Response.WritePStr(GetAuth1);//AuthKeyLogin
            Send(Response);

            byte[] Game = GameServerList();
            Send(Game);
        }

        private void CreateCharacter(Packet ClientPacket)
        {

            if (!ClientPacket.ReadPStr(out string Nickname))
            {
                return;
            }
            SetNickname(Nickname);


            var check = Nickname == GetNickname;

            Response.Clear();
            Response.Write(new byte[] { 0x01, 0x00,
                0xD9//US = D9, TH = DA
            });
            Send(Response.GetBytes());
        }

        private void NicknameCheck(Packet ClientPacket)
        {
            Byte Code;

            if (!ClientPacket.ReadPStr(out string Nickname))
            {
                return;
            }

            var Query = _db.USP_NICKNAME_CHECK(Nickname).First();

            Code = (byte)Query;

            if ((Code == 0) || (Code == 2))
            {
                Response.Clear();
                Response.Write(new byte[] { 0x0E, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x21, 0xD2, 0x4D, 0x00 });
                this.Send(Response);
                return;
            }

            if (Code == 1)
            {
                Response.Clear();
                Response.Write(new byte[] { 0x0E, 0x00 });
                Response.WriteUInt32(0);
                Response.WritePStr(Nickname);
                Send(Response.GetBytes());
            }
        }

        private void RequestCharacaterCreate(Packet ClientPacket)
        {
            if (!ClientPacket.ReadInt32(out int CHAR_TYPEID))
            {
                return;
            }
            if (!ClientPacket.ReadUInt16(out ushort HAIR_COLOR))
            {
                return;
            }

            try
            {
                _db.Database.SqlQuery<PangyaEntities>($" EXEC [dbo].[USP_FIRST_CREATION] @UID = '{GetUID}', @CHAR_TYPEID = '{CHAR_TYPEID}', @HAIRCOLOUR = '{HAIR_COLOR}', @NICKNAME = '{GetNickname}'").FirstOrDefault();

                GetFirstLogin = (byte)_db.Pangya_Member.First(c => c.UID == (int)GetUID).FirstSet;
                if (GetFirstLogin == 1)
                {
                    SendPlayerLoggedOnData();
                }
                else
                {
                    Disconnect();
                    return;
                }
            }
            catch
            {
                this.Disconnect();
            }
        }

        byte[] GameServerList()
        {
            using (var result = new PangyaBinaryWriter())
            {
                result.Write(new byte[] { 0x02, 0x00 });
                result.WriteByte((byte)_db.ProcGetGameServer().Count());//count servers 
                foreach (var data in _db.ProcGetGameServer())
                {
                    result.WriteStr(data.Name, 40);
                    result.WriteInt32(data.ServerID);//serverID
                    result.WriteInt32(data.MaxUser);//max user
                    result.WriteInt32(data.UsersOnline);
                    result.WriteStr(data.IP, 18);
                    result.WriteInt32(data.Port);//port 
                    result.WriteInt32(data.Property);
                    result.WriteUInt32(0); // Angelic Number
                    result.WriteUInt16((ushort)data.ImgEvent);//Flag event
                    result.WriteUInt16(0);
                    result.WriteInt32(100);
                    result.WriteUInt16(data.ImgNo);//Icon Server        
                }
                return result.GetBytes();
            }
        }

        byte[] MessangerServerList()
        {
            using (var result = new PangyaBinaryWriter())
            {
                result.Write(new byte[] { 0x09, 0x00 });
                result.WriteByte((byte)_db.ProcGetMessengerServer().Count());//count servers 
                foreach (var server in _db.ProcGetMessengerServer())
                {
                    result.WriteStr(server.Name, 40);
                    result.WriteInt32(server.ServerID);
                    result.WriteInt32(server.MaxUser);
                    result.WriteInt32(server.UsersOnline);
                    result.WriteStr(server.IP, 18);
                    result.WriteInt32(server.Port);
                    result.WriteInt32(4096);
                    result.WriteZero(14);
                }
                return result.GetBytes();
            }
        }

        string RandomAuth(ushort Count)
        {
            return Guid.NewGuid().ToString()
                .ToUpper()
                .Replace("-", string.Empty).Substring(0, Count);
        }
    }
}
