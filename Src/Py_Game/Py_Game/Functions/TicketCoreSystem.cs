using Py_Game.Client;
using PangyaAPI;
using PangyaAPI.Auth;
using Py_Game.MainServer;
using PangyaAPI.PangyaPacket;

namespace Py_Game.Functions
{
    public class TicketCoreSystem
    {
        public void PlayerCheckTickerCookies(GPlayer player)
        {
            if (player.GetCookie < 300)
            {
                player.SendResponse(new byte[] { 0xCB, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, });
            }
            else
                player.SendResponse(new byte[] { 0xCA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, });
        }
        public void PlayerNoticeTicker(GPlayer player, Packet packet)
        {
            if (!packet.ReadPStr(out string Message)) { return; }

            //autenfica a permissão
            if (!(player.RemoveCookie(300)))
            {
                player.SendResponse(new byte[] { 0xCB, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, });
                return;
            }

            var Auth = Program._server;
            if (Auth != null)
            {
                Program.SendAuth(new AuthPacket() { ID = AuthPacketEnum.SERVER_RELEASE_TICKET, Message = new { GetNickName = player.GetNickname, GetMessage = Message } });
                player.SendCookies();
            }
            else
            {
                using (var result = new PangyaAPI.BinaryModels.PangyaBinaryWriter())
                {
                    result.Write(new byte[] { 0xC9, 0x00 });
                    result.WritePStr(player.GetNickname);
                    result.WritePStr(Message);
                    player.SendToAll(result.GetBytes());
                }
                player.SendCookies();
            }
        }
    }
}
