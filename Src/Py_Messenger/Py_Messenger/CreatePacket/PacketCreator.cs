using PangyaAPI;
using PangyaAPI.BinaryModels;
using PangyaAPI.PangyaClient;
using Py_Messenger.Client;
using Py_Messenger.Client.Data;
using Py_Messenger.Defines;
using System.Collections.Generic;
namespace Py_Messenger.CreatePacket
{
    public static class PacketCreator
    {

        public static byte[] ShowLogin(uint UID)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[] { 0x2F, 0x00, 0x00 });
                Response.Write(UID); //uid player
                return Response.GetBytes();
            }
        }

        //Response.Write(new byte[] { 0x30, 0x00, 0x15, 0x01 });
        //Response.Write(GetUID); //uid player
        //Response.Write(1); //sempre aparece 4
        //Response.WriteByte(1);
        //Response.Write(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, });
        //Response.Write(ServerInfo.GameServerID); // porta que player se conectou
        //Response.Write(ServerInfo.LobbyId); //id do canal 
        //Response.WriteStr(ServerInfo.LobbyName, 64);
        public static byte[] ShowConnectionServer(uint UID, USER_STATUS typeConnection,ServerProcess server)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[] { 0x30, 0x00, 0x15, 0x01 });
                Response.Write(UID); 
                Response.Write((uint)typeConnection);
                Response.WriteByte(1);
                Response.Write(server.GameID);
                Response.Write(server.Unknown);
                Response.Write(server.GameServerID);
                Response.Write(server.LobbyId);
                Response.WriteStr(server.LobbyName, 64);
                return Response.GetBytes();
            }
        }
        public static byte[] ShowListFriends(List<Player> players)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[] { 0x30, 0x00, 0x02, 0x01, 0x01 });
                Response.Write((ushort)players.Count);//count
                Response.Write((ushort)players.Count);//count
                foreach (MPlayer player in players)
                {
                    Response.WriteStr(player.GetNickname, 22);
                    Response.WriteStr("FRIEND", 22);//out apelido 
                    Response.Write(player.GetUID);
                    Response.Write(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                    Response.Write(0);
                    Response.Write(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                    Response.WriteZero(12);
                    Response.Write(new byte[] { 0xFF, 0xFF, 0xe3, 0x00, 0x00, 0x00 });//troca por => 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 
                    Response.Write(player.ServerInfo.GameServerID); // porta que player se conectou
                    Response.WriteStr(player.ServerInfo.LobbyName, 64);
                    Response.Write(new byte[] { 0x05, 0xFF, 0x0E, 0x0C, 0x01 });
                }              
                return Response.GetBytes();
            }
        }

        public static byte[] ShowDeleteFriend(uint FriendUID)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[] { 0x30, 0x00, 0x0B, 0x01 });
                Response.Write(0);
                Response.Write(FriendUID);
                return Response.GetBytes();
            }
        }

        public static byte[] ShowChatText(uint UID, string NickName, string Message)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[] { 0x30, 0x00, 0x13, 0x01 });
                Response.Write(UID);
                Response.WritePStr(NickName);
                Response.WritePStr(Message);
                return Response.GetBytes();
            }
        }

        public static byte[] ShowChangeFriendSubNick(uint UID, string NickName)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[] { 0x30, 0x00, 0x19, 0x01 });
                Response.Write(0);
                Response.Write(UID);
                Response.WritePStr(NickName);
                return Response.GetBytes();
            }
        }


        public static byte[] ShowFindFriend(bool IsExist,string NickName, uint UID)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[] { 0x30, 0x00, 0x17, 0x01 });
                if (IsExist)
                {
                    Response.Write(0);
                    Response.WritePStr(NickName);
                    Response.Write(UID);
                }
                else 
                {
                    Response.Write(2);                    
                }
                return Response.GetBytes();
            }
        }

        public static byte[] ShowAddFriend(uint Friend_UID, string Friend_Nick, ServerProcess ServerInfo)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[]  {  0x30, 0x00, 0x04, 0x01 });
                Response.Write(0);
                Response.WriteStr("Pangs(e59)", 22);
                Response.WriteStr(Friend_Nick, 11); //apelido do amigo
                Response.Write(Friend_UID); //meu uid
                Response.Write(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });//un 1
                Response.Write(0);//2
                Response.Write(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });//3
                Response.Write(0);//4
                Response.Write(0);//5
                Response.Write(0);//6
                Response.Write(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });//flag?
                Response.Write(new byte[] { 0xFF, 0xFF });
                Response.Write(ServerInfo.GameServerID);
                Response.Write(ServerInfo.LobbyId); //id do canal?
                Response.WriteStr(ServerInfo.LobbyName, 64);
                Response.Write(new byte[] { 0x03, 0xFF, 0x00, 0x0A, 0x01 });
                return Response.GetBytes();
            }
        }
    }
}
