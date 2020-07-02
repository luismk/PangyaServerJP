using Py_Game.IffManager.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.IffManager.Data.AuxPart
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct IffAuxPartData
    {
        public IFFCommon Base;
        public UInt32 Quantity { get; set; }
        public UInt32 Un1 { get; set; }
        public UInt16 Un2 { get; set; }
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
        public UInt16 Eff1 { get; set; }
        public UInt16 Eff2 { get; set; }
        public UInt16 Eff3 { get; set; }
        public UInt16 Eff4 { get; set; }
        public UInt16 Eff5 { get; set; }
        public UInt16 Eff6 { get; set; }
        public UInt32 AuxPair { get; set; }
    }
}
