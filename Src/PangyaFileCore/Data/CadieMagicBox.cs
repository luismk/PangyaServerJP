using System;
using System.Runtime.InteropServices;
using PangyaFileCore.Definitions;
using PangyaFileCore.Struct;
namespace PangyaFileCore.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CadieMagicBox
    {
        public uint MagicID { get; set; }
        public uint Enabled { get; set; }
        public uint Sector { get; set; }
        public CadieBoxEnum BoxType { get; set; }
        public uint Level { get; set; }
        public uint Un1 { get; set; }
        public uint TypeID { get; set; }
        public uint Quatity { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public UInt32[] TradeID;
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public UInt32[] TradeQuantity;
        public UInt32 BoxID { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Name;
        [field: MarshalAs(UnmanagedType.Struct)]
        public SystemTime DateStart;
        [field: MarshalAs(UnmanagedType.Struct)]
        public SystemTime EndTime;
    }
}
