using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Py_Messenger.Client.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GuildData
    {
        public uint GuildID { get; set; }
        public string GuildName { get; set; }
        public byte GuildPosition { get; set; }
        public string GuildImage { get; set; }
        public uint GuildTotalMember { get; set; }
        public string GuildNotice { get; set; }
        public string GuildIntroducing { get; set; }
        public uint GuildLeaderUID { get; set; }
        public string GuildLeaderNickname { get; set; }
        public DateTime? Guild_Create_Date { get; set; }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ServerProcess
    {
        public ushort GameID { get; set; }
        public uint Unknown { get; set; }
        public uint GameServerID { get; set; }
        public byte LobbyId { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string LobbyName { get; set; }
    }
}
