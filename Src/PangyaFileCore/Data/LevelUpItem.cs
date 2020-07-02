using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PangyaFileCore.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct LevelUpItem
    {
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] UN;
        public byte UN01;
        public uint Index { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public byte[] UN0;
        public ushort Level { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] ItemTypeID;
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] Quantity;
        public uint UN2 { get; set; }
        public uint UN3 { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 132)]
        public string Name;
    }
}
