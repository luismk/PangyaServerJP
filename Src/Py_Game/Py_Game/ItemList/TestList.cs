using Py_Game.GameTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Py_Game.ItemList
{
    public class RandomItemList : List<ItemData>
    {
        int COUNT_ITEM;
        int NUMBER1;
        int NUMBER2;
        public RandomItemList()
        {
            AddItems(1, "Spin Mastery", 402653184, 3, 1);
            AddItems(2, "Curve Mastery", 402653185, 3, 1);
            AddItems(3, "Strength Boost", 402653188, 3, 1);
            AddItems(4, "Miracle Sign", 402653189, 3, 1);
            AddItems(5, "Generic Lucky Pangya", 402653191, 5, 1);
            AddItems(6, "Generic Nerve Stabilizer", 402653192, 5, 1);
            AddItems(7, "Duostar Lucky Pangya", 402653194, 5, 1);
            AddItems(8, "Duostar Nerve Stabilizer", 402653195, 5, 1);
            AddItems(9, "Silent Wind", 402653190, 3, 1);
            AddItems(10, "Mulligan Rose", 402653198, 5, 1);
            AddItems(11, "Power Calipers", 402653193, 50, 1);
            AddItems(12, "Time Booster", 436207633, 50, 1);
            AddItems(13, "Duostar PS", 402653200, 3, 1);
            AddItems(14, "Duostar SS", 402653201, 3, 1);
            AddItems(15, "Duostar LS", 402653202, 3, 1);
            AddItems(16, "Power Potion", 402653223, 3, 1);
            AddItems(17, "Double Strength Boost", 402653230, 3, 1);
            AddItems(18, "Power Milk", 402653221, 3, 1);
            AddItems(19, "Safety", 402653224, 3, 1);
            AddItems(20, "Papel Box", 436208136, 1, 2);
            AddItems(21, "Lucky Pangya", 402653186, 2, 1);
            AddItems(22, "Nerve Stabilizer", 402653187, 1, 1);
            AddItems(23, "Stength Stabilizers", 402653196, 2, 1);
            AddItems(24, "Silent Nerve Stabilizer", 402653228, 2, 1);
            AddItems(25, "Safe Silent", 402653229, 2, 1);
            AddItems(26, "Wind Strength Boost", 402653231, 2, 1);
            AddItems(27, "Replay Tape", 436207616, 2, 1);
            AddItems(28, "Papel Shop Coupon(Gift)", 436207657, 2, 2);
            AddItems(29, "Scratchy Card (Event)", 436207664, 2, 1);
            AddItems(30, "Scratchy Card Ticket(E)", 436207667, 1, 1);
            AddItems(31, "Partial Scratchy Card", 436207677, 3, 1);
            AddItems(32, "Auto Caliper", 436207680, 50, 1);
            AddItems(33, "Scratchy Card", 436207779, 2, 1);
            AddItems(34, "Grand Prix Ticket", 436208228, 1, 1);
        }

        
        public void AddItems(int Id, string name, int TypeID, int Quantity, int Type)
        {
            this.Add(new ItemData { ID = Id, Name = name, ItemTypeID = TypeID, ItemQuantity = Quantity, ItemType = Type });
        }

        public ItemData GetItem()
        {
            ItemData result;


            COUNT_ITEM = this.Where(c => c.ItemTypeID > 1).Count();

            result = new ItemData();
            if (new Random().Next(COUNT_ITEM, 200) + (Math.Abs(Checksum(Guid.NewGuid().ToString()))) % 20 > 100)
            {
                NUMBER1 = 1 + (Math.Abs(Checksum(Guid.NewGuid().ToString())) % COUNT_ITEM);
            }
            else if (NUMBER1 < 100)
            {
                NUMBER1 = 1 + (Math.Abs(Checksum(Guid.NewGuid().ToString())) % COUNT_ITEM);
            }

            if (NUMBER1 > COUNT_ITEM || NUMBER1 <= 0)
            {
                if (new Random().Next(COUNT_ITEM, 200) + (Math.Abs(Checksum(Guid.NewGuid().ToString()))) % 20 > 100)
                {
                    NUMBER2 = 1 + (Math.Abs(Checksum(Guid.NewGuid().ToString())) % COUNT_ITEM);
                }
                else
                {
                    NUMBER2 = Convert.ToInt32(MathRand.Rand.NextUInt(Convert.ToUInt32(COUNT_ITEM & 10)) + (Math.Abs(Checksum(Guid.NewGuid().ToString())) % 20));
                }
            }

            if (this.Any(c => c.ID == 1 + NUMBER1) == true)
            {
               result =  this.Where(c => c.ID == 1 + NUMBER1).First();
            }

            else if (this.Any(c => c.ID == 1 + NUMBER1) == false)
            {
                COUNT_ITEM = this.Where(c => c.ItemType == 2).Count();

                for (int i = 0; i < i + 1; i++)
                {
                    if (result.ItemType == 1)
                    {
                        NUMBER2 = new Random().Next(1, COUNT_ITEM);
                        if (this.Where(c => c.ID == NUMBER2 && c.ItemType >= 2).Any())
                        {
                            result = this.Where(c => c.ID == NUMBER2).First();
                            break;
                        }
                    }
                }
            }
            return result;
        }
        static int Checksum(string dataToCalculate)
        {
            byte[] byteToCalculate = Encoding.ASCII.GetBytes(dataToCalculate);
            int checksum = 0;
            foreach (byte chData in byteToCalculate)
            {
                checksum += chData;
            }
            checksum &= 0xff;
            return checksum;
        }
        public static int UnixTime(DateTime unixtime)
        {
            TimeSpan timeSpan = (unixtime - new DateTime(1970, 1, 1, 0, 0, 0));
            return (int)timeSpan.TotalSeconds;
        }
    }

    public class ItemData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ItemTypeID { get; set; }
        public int ItemQuantity { get; set; }
        public int ItemType { get; set; }
    }

    public struct ItemsSupplies
    {
        public uint TypeID { get; set; }
        public ushort DelQuantity { get; set; }
    }
}
