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
    public struct Part
    {
        [field: MarshalAs(UnmanagedType.Struct)]
        public IFFCommon Base;//{ get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string MPet;
        public UInt32 UCCType { get; set; }
        public UInt32 SlotCount { get; set; }
        public UInt32 Un1 { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Texture1;//{ get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Texture2;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Texture3;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Texture4;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Texture5;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Texture6;
        public UInt16 C0 { get; set; }
        public UInt16 C1 { get; set; }
        public UInt16 C2 { get; set; }
        public UInt16 C3 { get; set; }
        public UInt16 C4 { get; set; }
        public UInt16 Slot1 { get; set; }
        public UInt16 Slot2 { get; set; }
        public UInt16 Slot3 { get; set; }
        public UInt16 Slot4 { get; set; }
        public UInt16 Slot5 { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] Blank;
        public UInt32 Un2 { get; set; }
        public UInt32 Un3 { get; set; }
        public UInt32 RentPang { get; set; }
        public UInt32 Un4 { get; set; }
    }
}
