using PangyaAPI.BinaryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.Client.Inventory.Data.Slot
{
    public struct ItemSlotData
    {
        public uint Slot1 { get; set; }
        public uint Slot2 { get; set; }
        public uint Slot3 { get; set; }
        public uint Slot4 { get; set; }
        public uint Slot5 { get; set; }
        public uint Slot6 { get; set; }
        public uint Slot7 { get; set; }
        public uint Slot8 { get; set; }
        public uint Slot9 { get; set; }
        public uint Slot10 { get; set; }
        // TItemsUsing

        public ItemSlotData Clear()
        {
            return new ItemSlotData();
        }
        /// <summary>
        /// GetSize 40 Bytes
        /// </summary>
        /// <returns></returns>
        public byte[] GetItemSlot()
        {
            PangyaBinaryWriter Result;

            Result = new PangyaBinaryWriter();
            try
            {
                Result.WriteStruct(this);
                return Result.GetBytes();
            }
            finally
            {
                Result.Dispose();
            }
        }

        public bool SetItemSlot(ItemSlotData SlotData)
        {
            this = SlotData;         
            return true;
        }

        public void Remove()
        {
            if (this.Slot10 > 0)
            {
                this.Slot10 = 0;
                return;
            }
            else if (this.Slot9 > 0)
            {
                this.Slot9 = 0;
                return;
            }
            else if (this.Slot8 > 0)
            {
                this.Slot8 = 0;
                return;
            }
            else if (this.Slot7 > 0)
            {
                this.Slot7 = 0;
                return;
            }
            else if (this.Slot6 > 0)
            {
                this.Slot6 = 0;
                return;
            }
            else if (this.Slot5 > 0)
            {
                this.Slot5 = 0;
                return;
            }
            else if (this.Slot4 > 0)
            {
                this.Slot4 = 0;
                return;
            }
            else if (this.Slot3 > 0)
            {
                this.Slot3 = 0;
                return;
            }
            else if (this.Slot2 > 0)
            {
                this.Slot2 = 0;
                return;
            }
            else if (this.Slot1 > 0)
            {
                this.Slot1 = 0;
                return;
            }

        }
        internal bool Exist()
        {
            return (this.Slot1 > 0 &&
            this.Slot2 > 0 &&
            this.Slot3 > 0 &&
            this.Slot4 > 0 &&
            this.Slot5 > 0 &&
            this.Slot6 > 0 &&
            this.Slot7 > 0 &&
            this.Slot8 > 0);
        }
    }
}
