using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PangyaFileCore.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MemorialCoinItem
    {
        public uint Enable { get; set; }

        public uint TypeID { get; set; }
        public uint CoinType { get; set; }
        public uint Value_1 { get; set; }
        public uint Value_2 { get; set; }
        public uint Gacha_Num { get; set; }
        public uint Pool { get; set; }
        public uint ItemType { get; set; }
        public uint Value_3 { get; set; }
        public uint Value_4 { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 23)]
        public string UN;
    }
}
