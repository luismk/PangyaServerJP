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
    public struct Mascot
    {
        [field: MarshalAs(UnmanagedType.Struct)]
        public IFFCommon Base;

        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Texture1;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Texture2;
        public ushort Price1 { get; set; }
        public ushort Price7 { get; set; }
        public ushort UN0 { get; set; }
        public ushort Price30 { get; set; }
        public byte C0 { get; set; }
        public byte C1 { get; set; }
        public byte C2 { get; set; }
        public byte C3 { get; set; }
        public byte C4 { get; set; }
        public byte Slot1 { get; set; }
        public byte Slot2 { get; set; }
        public byte Slot3 { get; set; }
        public byte Slot4 { get; set; }
        public byte Slot5 { get; set; }
        public uint Effect1 { get; set; }
        public uint Effect2 { get; set; }
        public uint Effect3 { get; set; }
        public UInt16 UN1 { get; set; }
        public UInt16 UN2 { get; set; }

        public ushort GetDay()
        {
            return 7;
        }
    }
}
