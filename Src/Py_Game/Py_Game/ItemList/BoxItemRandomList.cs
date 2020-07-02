using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Py_Game.GameTools.Json;
using PangyaAPI.Tools;

namespace Py_Game.ItemList
{
    public class BoxItemList : List<TBoxInfo>, IDisposable
    {
        protected List<TBoxInfo> FBoxItem;
        protected bool FDuplicated = false;
        public BoxItemList()
        {
            FBoxItem = new List<TBoxInfo>();
            FDuplicated = true;

            try
            {
                if (File.Exists("Json/BoxRandomData.txt"))
                {
                    var JsonValues = JsonValue.ParseFile("Json/BoxRandomData.txt")["BoxData"].AsJsonArray;
                    foreach (var AMember in JsonValues)
                    {
                        var Box = new TBoxInfo
                        {
                            BoxName = AMember["BoxName"].AsString,

                            BoxParam = AMember["BoxParam"].AsString,

                            BoxType = (uint)AMember["BoxType"].AsInteger,

                            BoxTypeID = (uint)AMember["BoxTypeID"].AsInteger,

                            BoxQuantity = (uint)AMember["BoxQuantity"].AsInteger,

                            SupplyTypeID = (uint)AMember["SupplyTypeID"].AsInteger,

                            SupplyQuantity = (uint)AMember["SupplyQuantity"].AsInteger,

                            SpecialRewardTypeID = (uint)AMember["SpecialRewardTypeID"].AsInteger,

                            SpecialRewardQuantity = (uint)AMember["SpecialRewardQuantity"].AsInteger
                        };
                        foreach (var BMember in AMember["RewardItems"].AsJsonArray)
                        {
                            var Reward = new TRewardInfo
                            {
                                Name = BMember["Name"].AsString,
                                TypeId = Convert.ToUInt32(BMember["TypeID"].AsInteger),

                                Quantity = (uint)BMember["Quantity"].AsInteger,

                                Prob = (uint)BMember["Probability"].AsInteger,
                                Type = (byte)BMember["BoxType"].AsInteger,
                                RareType = (byte)BMember["RareType"].AsInteger,

                                Duplicated = BMember["CanDuplicated"].AsBoolean,

                                Announce = BMember["Announce"].AsBoolean
                            };
                            Box.RewardList.Add(Reward);
                        }
                        Add(Box);
                    }
                }
                else
                {
                    throw new Exception("BoxItemList: BoxRandomData.json file in folder not found !");
                }
            }
            catch (Exception ex)
            {
                WriteConsole.WriteLine(ex.Message, ConsoleColor.Red);
                Console.ReadKey();
                Environment.Exit(0);
            }
            finally
            {
                this.SetCanDup(false);
            }
        }

        ~BoxItemList()
        {
            Dispose(true);
        }
        public bool Remove(uint ID, TRewardInfo item)
        {
           this.Single(c => c.BoxTypeID == ID).Remove(item);

            FBoxItem.Single(c => c.BoxTypeID == ID).Remove(item);
            return true;
        }

        public void SetCanDup(bool Val)
        {
            FDuplicated = Val;
            Restore();
        }

        public void Restore()
        {
            FBoxItem.Clear();
            foreach (var Items in this)
            {
                FBoxItem.Add(Items);
                if (Items.RewardList.Where(c=> c.TypeId == 335544325).Any())
                {

                }
            }
        }
        public uint GetAllProb()
        {
            uint prob = 0;
            if (this.Count != 0)
            {
                foreach (var item in this)
                {
                    foreach (var info in item.RewardList)
                    {
                        prob += info.Prob;
                    }
                }
            }
            else
            {
                foreach (var item in FBoxItem)
                {
                    foreach (var info in item.RewardList)
                    {
                        prob += info.Prob;
                    }
                }
            }
            return prob;
        }
        public TBoxInfo GetBoxInfo(uint BoxTypeId)
        {
            foreach (var Box in this)
            {
                if (Box.BoxTypeID == BoxTypeId)
                {
                    return Box;
                }
            }
            return null;
        }

        public TRewardInfo GetItemBox(uint BoxTypeId)
        {
            int RInt;

            RInt = (int)FBoxItem.Where(c => c.BoxTypeID == BoxTypeId).First().GetProb();
            foreach (TBoxInfo Box in FBoxItem.Where(c => c.BoxTypeID == BoxTypeId))
            {
                foreach (TRewardInfo Reward in Box.RewardList)
                {
                    if (Box.BoxType == Reward.Type)
                    {
                        RInt -= (int)Reward.Prob;
                    }
                    if (RInt <= 0)
                    {
                        Remove(BoxTypeId, Reward);
                        return Reward;
                    }
                }
            }
            return new TRewardInfo();
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    public class TBoxInfo
    {
        public string BoxName { get; set; }
        public string BoxParam { get; set; }
        public uint BoxType { get; set; }//tipo de box
        public uint BoxTypeID { get; set; }
        public uint BoxQuantity { get; set; }
        public uint SupplyTypeID { get; set; }
        public uint SupplyQuantity { get; set; }
        public uint SpecialRewardTypeID { get; set; }
        public uint SpecialRewardQuantity { get; set; }  // USE IN SPIN CUBE [KEY]
        public List<TRewardInfo> RewardList { get; set; }

        public TBoxInfo()
        {
            // # Create box reward lists
            RewardList = new List<TRewardInfo>();
        }
        public uint GetProb()
        {
            uint prob = 0;
            if (this.RewardList.Count != 0)
            {
                foreach (var item in RewardList)
                {
                    if (BoxType == item.Type)
                    {
                        prob += item.Prob;
                    }
                }
            }
            return prob;
        }

        public void Remove(TRewardInfo item)
        {
            RewardList.Remove(item);
        }
    }
    public struct TRewardInfo
    {
        public string Name { get; set; }
        public uint TypeId { get; set; }
        public uint Quantity { get; set; }
        public uint Prob { get; set; }
        public byte Type { get; set; }
        public byte RareType { get; set; }
        public bool Duplicated { get; set; }
        public bool Announce { get; set; }
    }
}
