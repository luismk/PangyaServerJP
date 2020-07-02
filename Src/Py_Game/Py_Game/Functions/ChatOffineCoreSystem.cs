using Py_Game.Client;
using System.Linq;
using PangyaAPI;
using Py_Connector.DataBase;
using PangyaAPI.PangyaPacket;

namespace Py_Game.Functions
{
   public class ChatOffineCoreSystem
    {
        public void PlayerSendChatOffline(GPlayer player)
        {
            var _db = new PangyaEntities();
            var msg_user = _db.ProcGetUserMessage((int)player.GetUID).ToList();
            if (msg_user.Count > 0)
            {
                player.Response.Write(new byte[] { 0xB2, 0x00 });
                player.Response.Write((long)2);
                player.Response.Write(msg_user.Count);
                foreach (var data in msg_user)
                {
                    player.Response.Write(data.uid);//4
                    player.Response.Write((ushort)data.ID_MSG);//6
                    player.Response.WriteStr(data.Nickname, 22);//28
                    player.Response.WriteStr(data.Message, 64);//92
                    player.Response.WriteStr(data.reg_date.ToString(), 17);//109
                }
                player.SendResponse();
            }
        }

        public void PlayerResponseChatOffline(GPlayer player, Packet packet)
        {
            var _db = new PangyaEntities();
            if (!packet.ReadUInt32(out uint From_ID)) { return; }

            if (!packet.ReadPStr(out string Messange)) { return; }

            _db.ProcAddUserMessage((int)player.GetUID, (int)From_ID, Messange);

            player.LoadStatistic();

            player.Response.Write(new byte[] { 0x95, 0x00, 0x11, 0x01 });
            player.Response.Write(0);
            player.Response.Write((long)player.GetPang);
            player.SendResponse();
        }
    }
}
