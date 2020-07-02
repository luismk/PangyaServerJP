using System;
using System.Runtime.InteropServices;
using PangyaFileCore.Struct;

namespace PangyaFileCore.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Caddie
    {
        [field: MarshalAs(UnmanagedType.Struct)]
        public IFFCommon Base;
        public uint Salary { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x27 + 1)]
        public string MPet;
        public UInt16 C0 { get; set; }
        public UInt16 C1 { get; set; }
        public UInt16 C2 { get; set; }
        public UInt16 C3 { get; set; }
        public UInt16 C4 { get; set; }
        public UInt16 Un4 { get; set; }
    }
}
