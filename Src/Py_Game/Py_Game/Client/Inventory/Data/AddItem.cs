using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.Client.Inventory.Data
{
    public struct AddItem
    {
        public uint ItemIffId { get; set; }
        public Boolean Transaction { get; set; }
        public uint Quantity { get; set; }
        public uint Day { get; set; }
    }
    public struct AddData
    {
        public Boolean Status { get; set; }
        public uint ItemIndex { get; set; }
        public uint ItemTypeID { get; set; }
        public uint ItemOldQty { get; set; }
        public uint ItemNewQty { get; set; }
        public string ItemUCCKey { get; set; }
        public Byte ItemFlag { get; set; }
        public DateTime? ItemEndDate { get; set; }
        // AddData
        public void SetData(Boolean FStatus, uint FItemIndex, uint FItemTypeId, uint FItemOldQty, uint FItemNewQty, string FItemUCCKey, Byte FItemFlag, DateTime FItemEndDate)
        {
            this.Status = FStatus;
            this.ItemIndex = FItemIndex;
            this.ItemTypeID = FItemTypeId;
            this.ItemOldQty = FItemOldQty;
            this.ItemNewQty = FItemNewQty;
            this.ItemUCCKey = FItemUCCKey;
            this.ItemFlag = FItemFlag;
            this.ItemEndDate = FItemEndDate;
        }

        public AddData Create()
        {
            SetData(false, 0, 0,0 ,0, string.Empty,0, DateTime.MinValue);
            return this;
        }
    }
}
