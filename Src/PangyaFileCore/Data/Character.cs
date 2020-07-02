using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PangyaFileCore.Struct;

namespace PangyaFileCore.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Character
    {
        [field: MarshalAs(UnmanagedType.Struct)]
        public IFFCommon Base;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string MPet;// = new char[0x27 + 1];
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Texture1;// = new char[0x27 + 1];
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Texture2;//{ get; set; }// = new char[0x27 + 1];
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Texture3;//  = new char[0x27 + 1];
        public ushort C0 { get; set; }
        public ushort C1 { get; set; }
        public ushort C2 { get; set; }
        public ushort C3 { get; set; }
        public ushort C4 { get; set; }
        public byte Slot1 { get; set; }
        public byte Slot2 { get; set; }
        public byte Slot3 { get; set; }
        public byte Slot4 { get; set; }
        public byte Slot5 { get; set; }
        public byte Un1 { get; set; }
        public float MasteryProb { get; set; }
        public byte Stat1 { get; set; }
        public byte Stat2 { get; set; }
        public byte Stat3 { get; set; }
        public byte Stat4 { get; set; }
        public byte Stat5 { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Texture4;// = new char[0x27 + 1];
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Un2;// = new char[0x2 + 1];
    }
}
