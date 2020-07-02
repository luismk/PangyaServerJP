using System;
using System.Runtime.InteropServices;

namespace Py_Game.Data
{
    public class TMagicList
    {
        public uint TypeID { get; set; }
        public uint Index { get; set; }
        public uint Quantity { get; set; }
    }

    public struct TBuyItem
    {
        public byte Flag;
        public ushort DayTotal;
        public DateTime? EndDate;
    }

    public class TItemData
    {
        public UInt32 TypeID;
        public UInt32 ItemIndex;
        public UInt32 ItemQuantity;
    }
    public class TPCards
    {
        public UInt32[] Card { get; set; }

        public TPCards()
        {
            Card = new UInt32[0x0B];
        }
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
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CharacterData
    {
        public UInt32 TypeID { get; set; }
        public UInt32 Index { get; set; }
        public UInt16 HairColour { get; set; }
        public UInt16 GiftFlag { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public UInt32[] EquipTypeID { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public UInt32[] EquipIndex { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 212)]
        public byte[] Unknown { get; set; }
        public UInt32 AuxPart { get; set; }
        public UInt32 AuxPart2 { get; set; }
        public uint AuxPart3 { get; set; }
        public uint AuxPart4 { get; set; }
        public uint AuxPart5 { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Unknown2 { get; set; }

        public UInt32 FCutinIndex { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] Unknown3 { get; set; }

        public Byte Power { get; set; }
        public Byte Control { get; set; }
        public Byte Impact { get; set; }
        public Byte Spin { get; set; }
        public Byte Curve { get; set; }
    }

    public struct CutinInfoData
    {
        public UInt32 UID { get; set; }
        public UInt32 Unknown { get; set; }
        public UInt16 Unknown2 { get; set; }
        public UInt32 TypeID { get; set; }
        public UInt32 Type { get; set; }
    }
}
