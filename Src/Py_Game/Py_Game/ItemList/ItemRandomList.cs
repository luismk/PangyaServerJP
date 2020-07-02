using PangyaAPI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Py_Game.ItemList
{
    public class ItemRandomClass : List<ItemRandom>, IDisposable
    {
        public List<Supplies> FSupplies { get; set; }
        public List<ItemRandom> FTItem { get; set; }
        public bool FDuplicated = false;
        public ItemRandom PlayerItemDataRadom;
        public System.Random rnd;
        public ItemRandomClass()
        {
            FSupplies = new List<Supplies>();
            FTItem = new List<ItemRandom>();
            FDuplicated = true;
            rnd = new System.Random();
        }

       
        public uint GetAllProb()
        {
            uint prob = 0;
            if (this.Count != 0)
            {
                foreach (var item in this)
                {
                    prob += item.Probs;
                }
            }
            else
            {
                foreach (var item in FTItem)
                {
                    prob += item.Probs;
                }
            }
            return prob;
        }

        public uint GetAllProb(uint TypeID, uint TypeID2, uint TypeID3)
        {
            uint prob = 0;
            if (this.Count != 0)
            {
                foreach (var item in this)
                {
                    if (item.TypeId == TypeID || item.TypeId == TypeID2 || item.TypeId == TypeID3)
                    {
                        prob += item.Probs;
                    }
                }
            }
            else
            {
                foreach (var item in FTItem)
                {
                    prob += item.Probs;
                }
            }
            return prob;
        }

        public void AddItems(ItemRandom itemInfo)
        {
            if (itemInfo.Active)
            {
                this.Add(itemInfo);
            }
            else
            {
                WriteConsole.WriteLine("Falied To add {0} IsNoValid {1}", ConsoleColor.Red, new object[] { itemInfo.TypeId, itemInfo.Active});
            }
        }

        public void AddItems(uint TypeID, uint MaxQuan, uint RareType, ushort Probabilities, bool ItemActive = true)
        {

            ItemRandom Items;
            Items = new ItemRandom
            {
                Active = ItemActive,
                TypeId = TypeID,
                MaxQuantity = MaxQuan,
                Probs = Probabilities,
                RareType = RareType,
            };
            if (ItemActive)
            {
                this.Add(Items);
            }
        }

        public void AddSupply(uint TypeID, uint Quantity = 1)
        {
            Supplies FSupply;
            FSupply = new Supplies
            {
                TypeId = TypeID,
                DelQuantity = (ushort)Quantity
            };
            FSupplies.Add(FSupply);
        }

        public void Arrange()
        {
            foreach (var Item1 in FTItem)
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
                        Item2.RareType  = 0;
                    }
                }
               
            }
        }
        public void SetCanDup(bool Val)
        {
            FDuplicated = Val;
            Restore();
        }

        public void Restore()
        {
            FTItem.Clear();
            foreach (var Items in this)
            {
                FTItem.Add(Items);
            }
        }


        public ItemRandom GetItems(uint TypeID, uint TypeID2, uint TypeID3)
        {
            int RInt = GameTools.MathRand.Rand.Next(1, (int)GetAllProb(TypeID, TypeID2, TypeID3));
            if (!FDuplicated)
            {
                foreach (var Items in FTItem)
                {
                    RInt -= Items.Probs;
                    if (RInt <= 0)
                    {
                        FTItem.Remove(Items);
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
            return null;
        }

        public ItemRandom GetItems()
        {
            int RInt = GameTools.MathRand.Rand.Next(1, (int)GetAllProb());
            if (!FDuplicated)
            {
                foreach (var Items in FTItem)
                {
                    RInt -= Items.Probs;
                    if (RInt <= 0)
                    {
                        FTItem.Remove(Items);
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
                return (uint)FTItem.Count;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // Para detectar chamadas redundantes

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Clear();
                }
                
                disposedValue = true;
            }
        }

        ~ItemRandomClass()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
    public class ItemRandom
    {
        public uint TypeId { get; set; }
        public uint MaxQuantity { get; set; }
        public ushort Probs { get; set; }
        public uint RareType { get; set; }
        public bool Active { get; set; }
    }
    public struct Supplies
    {
        public uint TypeId { get; set; }
        public ushort DelQuantity { get; set; }
    }
}
