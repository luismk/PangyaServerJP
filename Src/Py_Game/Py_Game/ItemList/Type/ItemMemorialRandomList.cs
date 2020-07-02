using PangyaAPI;
using Py_Game.GameTools.Json;
using System;
using System.Collections.Generic;
using System.IO;
using static PangyaFileCore.IffBaseManager;
namespace Py_Game.ItemList.Type
{
    public class MemorialItemCollection : List<MemorialItem>
    {
        public List<SpecialItem> ItemNormal { get; set; }

        protected List<MemorialItem> ItemRare { get; set; }
        public bool FDuplicated = false;
        public MemorialItemCollection()
        {
            ItemNormal = new List<SpecialItem>();
            FDuplicated = true;
        }

        public void LoadObject()
        {
            ItemRare = this;
        }
        public void AddNormalItems(SpecialItem itemInfo)
        {
            ItemNormal.Add(itemInfo);
        }

        public void AddRareItems(MemorialItem info)
        {
            Add(info);
        }

        public MemorialItemCollection GetRare(UInt32 CoinTypeID, byte Pooling)
        {
            MemorialItemCollection result;
            result = new MemorialItemCollection();
            switch (Pooling)
            {
                case 0:
                    foreach (var Item in this)
                    {
                        result.AddRareItems(Item);
                    }
                    break;
                default:
                    foreach (var Item in this)
                    {
                        if (Item.Pooling == Pooling)
                        {
                            result.AddRareItems(Item);
                        }
                    }
                    break;
            }
            result.SetCanDup(false);
            result.Arrange();
            return result;
        }

        public void SetCanDup(bool Val)
        {
            FDuplicated = Val;
            Restore();
        }

        public void Restore()
        {
            ItemRare.Clear();
            foreach (var Items in this)
            {
                ItemRare.Add(Items);
            }
        }

        public void Arrange()
        {
            foreach (var Item1 in this)
            {
                foreach (var Item2 in this)
                {
                    if (Item1.RareType > Item2.RareType)
                    {
                        Item2.RareType = uint.MaxValue;
                    }
                    else if (Item1.RareType < Item2.RareType)
                    {
                        Item2.RareType = 1;
                    }
                    else
                    {
                        Item2.RareType = 0;
                    }
                }

            }
        }

        public List<SpecialItem> GetNormal(UInt32 TypeID)
        {
            Random rnd;
            byte PairNum;
            List<SpecialItem> result = new List<SpecialItem>();
            switch (TypeID)
            {
                case 436208242:
                    rnd = new Random();
                    PairNum = (byte)rnd.Next(8, 14);
                    break;
                default:
                    rnd = new Random();
                    PairNum = (byte)rnd.Next(1, 7);
                    break;
            }
            foreach (SpecialItem SpecialItem in ItemNormal)
            {
                if (SpecialItem.Number == PairNum)
                {
                    result.Add(SpecialItem);
                }
            }
            return result;
        }
        public MemorialItem GetItems()
        {
            int RInt = GameTools.MathRand.Rand.Next(700);
            if (!FDuplicated)
            {
                foreach (var Items in this)
                {
                    RInt -= Items.Probs;
                    if (RInt <= 0)
                    {
                        this.Remove(Items);
                        return Items;
                    }
                }
            }
            else if (FDuplicated)
            {
                foreach (var Items in this)
                {
                    RInt -= Items.Probs;
                    if (RInt <= 0)
                    {
                        return Items;
                    }
                }
            }
            return GetItems();
        }

        public uint GetLeft()
        {
            if (FDuplicated)
            {
                return (uint)Count;
            }
            else
            {
                return (uint)ItemRare.Count;
            }
        }
    }
    public class MemorialItem
    {
        public string Name { get; set; }
        public uint TypeId { get; set; }
        public uint MaxQuantity { get; set; }
        public ushort Probs { get; set; }
        public uint RareType { get; set; }
        public bool Active { get; set; }

        public byte Pooling { get; set; }
    }
    public class SpecialItem
    {
        public string Name { get; set; }
        public byte Number { get; set; }
        public UInt32 TypeID { get; set; }
        public UInt32 Quantity { get; set; }
        public uint Prob { get; set; }
        public uint RareType { get; set; }
        public SpecialItem()
        {
            RareType = uint.MaxValue;

            var rnd = new Random();
            Prob = (uint)Math.Abs((sbyte)(rnd.Next() % 30));
        }
    }
    public class CoinType
    {
        public uint TypeId { get; set; }
        public ushort DelQuantity { get; set; }
    }
}
