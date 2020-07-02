using PangyaAPI.BinaryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Py_Game.GameTools.TCompare;
using static Py_Game.GameTools.Tools;
namespace Py_Game.Client.Inventory.Data.Transactions
{
    public class PlayerTransactionData
    {
        public Byte Types { get; set; }
        public uint TypeID { get; set; }
        public uint Index { get; set; }
        public uint PreviousQuan { get; set; }
        public uint NewQuan { get; set; }
        public DateTime DayStart { get; set; }
        // As Unix Datetime
        public DateTime DayEnd { get; set; }
        // As Unix Datetime
        public string UCC { get; set; } = String.Empty;
        public Byte UCCStatus { get; set; }
        public Byte UCCCopyCount { get; set; }
        public ushort C0_SLOT { get; set; }
        public ushort C1_SLOT { get; set; }
        public ushort C2_SLOT { get; set; }
        public ushort C3_SLOT { get; set; }
        public ushort C4_SLOT { get; set; }
        public uint ClubPoint { get; set; }
        public uint WorkshopCount { get; set; }
        public uint CancelledCount { get; set; }
        public uint CardTypeID { get; set; }
        public Byte CharSlot { get; set; }

        public PlayerTransactionData()
        {
            DateTime DefaultDate;

            DefaultDate = new DateTime(1899, 12, 30, 0, 0, 0, 0);
            this.DayStart = DefaultDate;
            this.DayEnd = DefaultDate;
        }

        public byte[] GetInfoData()
        {
            PangyaBinaryWriter result;

            result = new PangyaBinaryWriter();
            result.WriteByte(IfCompare<Byte>(Types <= 0, 0x2, Types));
            result.WriteUInt32(TypeID);
            result.WriteUInt32(Index);
            result.WriteUInt32(IfCompare<UInt32>(DayEnd > DateTime.Now, 1, 0));
            // ## if the item has a period time
            if ((DayEnd > DateTime.Now))
            {
                result.WriteUInt32((uint)UnixTimeConvert(DayStart));
                result.WriteUInt32((uint)UnixTimeConvert(DayEnd));
                result.WriteUInt32((uint)(DayEnd - DayStart).TotalDays);
            }
            else
            {
                result.WriteUInt32(PreviousQuan);
                result.WriteUInt32(NewQuan);
                result.WriteUInt32(NewQuan - PreviousQuan);
            }
            if ((Types == 0xC9))
            {
                result.WriteUInt16(C0_SLOT);
                result.WriteUInt16(C1_SLOT);
                result.WriteUInt16(C2_SLOT);
                result.WriteUInt16(C3_SLOT);
                result.WriteUInt16(C4_SLOT);
            }
            else if ((DayEnd > DayStart))
            {
                result.WriteZero(0x8);
                result.WriteUInt16((ushort)(DayEnd - DayStart).TotalDays);
            }
            else
            {
                result.WriteZero(0xA);
            }
            result.WriteUInt16((ushort)UCC.Length);
            result.WriteStr(UCC, 0x8);
            if (UCC.Length >= 8)
            {
                result.WriteUInt32(UCCStatus);
                result.WriteUInt16(UCCCopyCount);
                result.WriteZero(0x7);
            }
            else if ((Types == 0xCB))
            {
                result.WriteUInt32(CardTypeID);
                result.WriteByte(CharSlot);
            }
            else if ((Types == 0xCC))
            {
                result.WriteUInt32(0);
                result.WriteByte(0);
                result.WriteUInt16(C0_SLOT);
                result.WriteUInt16(C1_SLOT);
                result.WriteUInt16(C2_SLOT);
                result.WriteUInt16(C3_SLOT);
                result.WriteUInt16(C4_SLOT);
                result.WriteUInt32(ClubPoint);
                result.WriteByte(IfCompare<Byte>(WorkshopCount > 0, 0, 0xFF));
                result.WriteUInt32(WorkshopCount);
                result.WriteUInt32(CancelledCount);
            }
            else
            {
                result.WriteUInt32(0);
                result.WriteByte(0);
            }
            return result.GetBytes();
        }
    }
}
