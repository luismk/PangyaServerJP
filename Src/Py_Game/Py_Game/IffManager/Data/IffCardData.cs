using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Py_Game.IffManager.General;
using Py_Game.Defines;

namespace Py_Game.IffManager.Data.Card
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct IffCardData
    {
        [field: MarshalAs(UnmanagedType.Struct)]
        public IFFCommon Base;
        public byte Rarity;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string MPet;
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

    public class CardPack
    {
        public TPACKCARD CardTypePack { get; set; }
        public byte Quantity { get; set; }
    }

    public class CardData
    {
        public uint TypeID { get; set; }
        public uint Quantity { get; set; }
        public CardData(uint ID)
        {
            TypeID = ID;
            Quantity = 1;
        }
        public CardData()
        {
        }
    }
}
