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
    public struct CaddieItem
    {
        [field: MarshalAs(UnmanagedType.Struct)]
        public IFFCommon Base;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x27 + 1)]
        public string MPet;

        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x27 + 1)]
        public string TexTure;
        public UInt16 Price1 { get; set; }
        public UInt16 Price15 { get; set; }
        public UInt16 PriceUN { get; set; }
        public UInt16 Price30 { get; set; }
        public UInt32 Un4 { get; set; }
    }
}
