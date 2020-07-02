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
    public struct Item
    {
        [field: MarshalAs(UnmanagedType.Struct)]
        public IFFCommon Base;
        public UInt32 ItemType { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]

        public string Texture;
        public ushort Power { get; set; }
        public ushort Control { get; set; }
        public ushort Accuracy { get; set; }
        public ushort Spin { get; set; }
        public ushort Curve { get; set; }
        public ushort Unkown { get; set; }
    }
}
