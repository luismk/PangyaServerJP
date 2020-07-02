using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Py_Game.IffManager.General;

namespace Py_Game.IffManager.Data.Caddie
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct IffCaddieData
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
