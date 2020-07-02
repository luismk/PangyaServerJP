using PangyaAPI.BinaryModels;
using Py_Connector.DataBase;
using Py_Game.Defines;
using Py_Game.Client.Inventory.Data.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Py_Game.GameTools.Tools;
namespace Py_Game.Client.Inventory.Collection
{
    public class WarehouseCollection : List<PlayerItemData>
    {
        public WarehouseCollection(int PlayerUID)
        {
            Build(PlayerUID);
        }

        void Build(int UID)
        {
            var _db = new PangyaEntities();
            foreach (var info in _db.ProcGetItemWarehouse(UID))
            {
                if (info.HOURLEFT == null || info.C0_SLOT == null)
                {
                    info.HOURLEFT = 0;
                    info.C0_SLOT = 0;
                    info.C1_SLOT = 0;
                    info.C2_SLOT = 0;
                    info.C3_SLOT = 0;
                    info.C4_SLOT = 0;
                    info.CLUB_SLOT_CANCEL = 0;
                    info.CLUB_WORK_COUNT = 0;
                }
                var item = new PlayerItemData(info);
                Add(item);
            }
        }

        public byte[] Build()
        {
            using (var result = new PangyaBinaryWriter())
            {
                result.Write(new byte[] { 0x73, 0x00 });
                result.WriteUInt16(Convert.ToUInt16(Count));
                result.WriteUInt16(Convert.ToUInt16(Count));
                foreach (var item in this)
                {
                    result.Write(item.GetItems());
                }
                return result.GetBytes();
            }
        }
        public int ItemAdd(PlayerItemData Value)
        {
            Value.ItemNeedUpdate = false;
            Add(Value);
            return Count;
        }


        public PlayerItemData GetUCC(uint ItemIdx)
        {
            var item = this.Where(c => c.ItemIndex == ItemIdx && c.ItemUCCUnique.Length >= 8);
            if (item.Any()) { return item.FirstOrDefault(); }

            return null;
        }

        public PlayerItemData GetUCC(uint TypeID, string UCCUnique, bool Status)
        {
            var item = this.Where(c => c.ItemTypeID == TypeID && c.ItemUCCUnique == UCCUnique && c.ItemUCCStatus == 1);
            if (item.Any()) { return item.FirstOrDefault(); }

            return null;
        }
        public PlayerItemData GetUCC(uint TypeID, string UCCUnique)
        {
            var item = this.Where(c => c.ItemTypeID == TypeID && c.ItemUCCUnique == UCCUnique && c.ItemUCCStatus >= 0);
            if (item.Any()) { return item.FirstOrDefault(); }

            return null;
        }
        public PlayerItemData GetItem(uint Index)
        {
            foreach (var Items in this)
            {
                if ((Items.ItemIndex == Index) && (Items.ItemIsValid == 1))
                {
                    return Items;
                }
            }
            return null;
        }
        public PlayerItemData GetItem(uint ID, TGET_ITEM type)
        {
            switch (type)
            {
                case TGET_ITEM.gcTypeID:
                    {
                        var Item = this.Where(c => c.ItemTypeID == ID);

                        if (Item.Any())
                        {
                            return Item.FirstOrDefault();
                        }
                        else
                        {
                            return null;
                        }
                    }
                case TGET_ITEM.gcIndex:
                    {
                        var Item = this.Where(c => c.ItemTypeID == ID);

                        if (Item.Any())
                        {
                            return Item.FirstOrDefault();
                        }
                        else
                        {
                            return null;
                        }
                    }
            }

            return null;
        }

        public PlayerItemData GetItem(uint TypeID, uint Quantity)
        {
            foreach (var Items in this)
            {
                if (Items.ItemTypeID == TypeID && Items.ItemC0 >= Quantity && Items.ItemIsValid == 1)
                {
                    return Items;
                }
            }
            return null;
        }

        public PlayerItemData GetItem(uint TypeID, uint Index, uint Quantity)
        {
            foreach (var Items in this)
            {
                if (Items.ItemTypeID == TypeID && (Items.ItemIndex == Index) && Items.ItemC0 >= Quantity && Items.ItemIsValid == 1)
                {
                    return Items;
                }
            }
            return null;
        }


        public PlayerItemData GetClub(uint ID, TGET_CLUB type)
        {
            switch (type)
            {
                case TGET_CLUB.gcTypeID:
                    {
                        var ClubInfo = this.Where(c => c.ItemTypeID == ID && c.ItemGroup == (byte)TITEMGROUP.ITEM_TYPE_CLUB);

                        if (ClubInfo.Any())
                        {
                            return ClubInfo.FirstOrDefault();
                        }
                        else
                        {
                            return null;
                        }
                    }
                case TGET_CLUB.gcIndex:
                    {
                        var ClubInfo = this.Where(c => c.ItemIndex == ID && c.ItemGroup == (byte)TITEMGROUP.ITEM_TYPE_CLUB);

                        if (ClubInfo.Any())
                        {
                            return ClubInfo.FirstOrDefault();
                        }
                        else
                        {
                            return null;
                        }
                    }
            }

            return null;
        }


        public PlayerItemData GetItem(uint ID, TGET_CLUB type, uint Quantity = 0)
        {
            switch (type)
            {
                case TGET_CLUB.gcTypeID:
                    {
                        var Item = this.Where(c => c.ItemTypeID == ID && c.ItemC0 >= Quantity && c.ItemIsValid == 1);

                        if (Item.Any())
                        {
                            return Item.FirstOrDefault();
                        }
                    }
                    break;
                case TGET_CLUB.gcIndex:
                    {
                        var Item = this.Where(c => c.ItemIndex == ID && c.ItemC0 >= Quantity && c.ItemIsValid == 1);

                        if (Item.Any())
                        {
                            return Item.FirstOrDefault();
                        }
                        break;
                    }
            }

            return null;
        }


        public bool IsSkinExist(uint typeID)
        {
            var Items = this.FirstOrDefault(c => c.ItemTypeID == typeID && c.ItemIsValid == 1);


            if (Items != null && GetItemGroup(Items.ItemTypeID) == 14)
            {
                return true;
            }
            return false;
        }

        public bool IsClubExist(uint typeID)
        {
            var Items = this.FirstOrDefault(c => c.ItemTypeID == typeID && c.ItemIsValid == 1);
            if (Items != null && GetItemGroup(Items.ItemTypeID) == 4)
            {
                return true;
            }
            return false;
        }

        public bool IsNormalExist(uint typeID)
        {
            var Items = this.Where(c => c.ItemTypeID == typeID && c.ItemC0 > 0 && c.ItemIsValid == 1);
            return Items.Any();
        }
        public bool IsNormalExist(uint typeID, uint index, uint Quantity)
        {
            var Items = this.Where(c => c.ItemTypeID == typeID && c.ItemIndex == index && c.ItemC0 >= Quantity && c.ItemIsValid == 1);
            return Items.Any();
        }

        public bool IsPartExist(uint typeID)
        {
            var Items = this.Where(c => c.ItemTypeID == typeID && c.ItemIsValid == 1);

            return Items.Any();
        }

        public bool IsHairStyleExist(int typeID)
        {
            var Items = this.Where(c => c.ItemTypeID == typeID && c.ItemIsValid == 1);

            return Items.Any();
        }

        public bool IsPartExist(uint typeID, uint index, uint Quantity)
        {
            var Items = this.Where(c => c.ItemTypeID == typeID && c.ItemIndex == index && c.ItemIsValid == 1);

            return Items.Any();
        }

        public uint GetQuantity(uint TypeID)
        {
            var Items = this.FirstOrDefault(c => c.ItemTypeID == TypeID);

            if (Items != null)
            {
                return Items.ItemC0;
            }
            else
            {
                return 0;
            }
        }

        public string GetSqlUpdateItems()
        {
            StringBuilder SQLString;
            SQLString = new StringBuilder();

            foreach (var Items in this)
            {
                if (Items.ItemNeedUpdate)
                {
                    SQLString.Append(Items.GetSqlUpdateString());
                    Items.ItemNeedUpdate = false;
                }
            }
            return SQLString.ToString();
        }

      
        public List<PlayerItemData> GetClubData()
        {
            return this.Where(c => c.ItemGroup == (byte)TITEMGROUP.ITEM_TYPE_CLUB && c.ItemNeedUpdate == true).ToList();
        }

        internal bool RemoveItem(uint TypeId, uint Count)
        {
            switch (GetItemGroup(TypeId))
            {
                case 5:
                case 6:
                    {
                        foreach (var Items in this)
                        {
                            if (Items.ItemTypeID == TypeId && Items.ItemC0 >= Count && Items.ItemIsValid == 1)
                            {
                                Items.ItemC0 -= (ushort)Count;

                                if (Items.ItemC0 == 0)
                                {
                                    Items.ItemIsValid = 0;
                                }
                            }
                            return true;
                        }
                    }
                    break;
            }
            return false;
        }
        public void Update(PlayerItemData Item)
        {
            foreach (var upgrade in this)
            {
                if (upgrade.ItemIndex == Item.ItemIndex && upgrade.ItemTypeID == Item.ItemTypeID)
                {
                    upgrade.Update(Item);
                }
            }
        }
        internal bool RemoveItem(PlayerItemData Item)
        {
            return this.Remove(Item);
        }
    }
}
