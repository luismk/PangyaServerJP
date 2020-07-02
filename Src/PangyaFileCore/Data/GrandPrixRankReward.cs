using System.Runtime.InteropServices;

namespace PangyaFileCore.Data
{
    public struct GrandPrixRankReward
    {
        public uint Enabled { get; set; }
        public uint TypeID { get; set; }
        public uint Rank { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public uint[] RewardTypeID;
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public uint[] Quantity;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Unknown;
        public uint Trophy { get; set; }
    }
}
