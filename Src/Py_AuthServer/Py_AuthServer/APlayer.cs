using PangyaAPI;
using PangyaAPI.PangyaClient;
using Py_Connector.DataBase;
using System;
using System.Linq;
using System.Net.Sockets;

namespace Py_AuthServer
{
    public class APlayer : Player
    {
        public APlayer(TcpClient tcp) : base(tcp)
        {
            PlayerLoad();
        }

        void PlayerLoad()
        {
            var member = _db.Pangya_Member.Where(c => c.UID == GetUID).FirstOrDefault();
            if (member != null)
            {
                this.GetLogin = member.Username;
                this.GetNickname = member.Nickname;
            }
        }
    }
}
