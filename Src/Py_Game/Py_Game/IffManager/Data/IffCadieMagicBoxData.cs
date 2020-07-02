using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Py_Game.IffManager.General;

namespace Py_Game.IffManager.Data.CadieMagicBox
{
    public enum CadieBoxEnum : int
    {
        MascotType = -1,
        PartOrSomethingElse = -2,
        NURI = 0,
        HANA = 1,
        AZER = 2,
        CESILLIA = 3,
        MAX = 4,
        KOOH = 5,
        ARIN = 6,
        KAZ = 7,
        LUCIA = 8,
        NELL = 9,
        SPIKA = 10,
        NURI_R = 11,
        HANA_R = 12,
        UNKNOWN = 13,
        CESILLIA_R = 14
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct IffCadieMagicBoxData
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
