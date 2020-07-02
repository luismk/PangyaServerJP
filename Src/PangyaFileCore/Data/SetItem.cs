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
    public struct SetItem
    {
        [field: MarshalAs(UnmanagedType.Struct)]
        public IFFCommon Base;
        public UInt32 Total { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0A)]
        public UInt32[] Type{ get; set; }// = new UInt32[9 + 1];
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0A)]
        public UInt16[] Qty { get; set; }// = new UInt16[9 + 1];
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x0B)]
        public string Un1 { get; set; }//{ get; set; }//= new char[0xB + 1];
    }
}
