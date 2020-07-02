using System;
using System.Runtime.InteropServices;
using PangyaFileCore.Struct;

namespace PangyaFileCore.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Card
    {
        [field: MarshalAs(UnmanagedType.Struct)]
        public IFFCommon Base { get; set; }
        public byte Rarity { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string MPet { get; set; }
        public ushort PowerSlot { get; set; }
        public ushort ControlSlot { get; set; }
        public ushort AccuracySlot { get; set; }
        public ushort SpinSlot { get; set; }
        public ushort CurveSlot { get; set; }
        public UInt16 Effect { get; set; }
        public UInt16 EffectQty { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string AdditionalTexture1;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string AdditionalTexture2;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string AdditionalTexture3;
        public UInt16 Time { get; set; }
        public UInt16 Volumn { get; set; }
        public UInt32 Position { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Unk;
    }    
}
