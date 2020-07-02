using PangyaAPI;
using PangyaAPI.PangyaClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Messenger.Client
{
    public partial class MPlayer : Player
    {
        public bool SetLogin(string name)
        {
            GetLogin = name;
            return true;
        }

        public bool SetNickname(string nick)
        {
            GetNickname = nick;
            return true;
        }

        public bool SetUID(uint ID)
        {
            GetUID = ID;
            return true;
        }

        public bool SetGuildId(uint GuildId)
        {
            this.Guild.GuildID = GuildId;
            return true;
        }
    }
}
