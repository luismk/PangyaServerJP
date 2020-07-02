using Py_Game.Client;
using PangyaAPI.BinaryModels;
using System;
using Py_Game.GameTools.Json;
using System.IO;
using Py_Game.ItemList;
using PangyaAPI;
using Py_Game.Client.Inventory.Data;
using PangyaAPI.Tools;

namespace Py_Game.Functions.MiniGames
{
    public class PapelSystem
    {
        private static ItemRandomClass RandomWeight { get; set; }
        
        static PapelSystem()
        {
            ItemRandom ItemInfo;
            uint[] SupplyTypeID;
            RandomWeight = new ItemRandomClass();

            try
            {
                if (File.Exists("Json/BlackPapelData.json"))
                {
                    SupplyTypeID = new uint[3];
                    foreach (var item in JsonValue.ParseFile("Json/BlackPapelData.json")["BlackPapelData"].AsJsonArray)
                    {
                        SupplyTypeID[0] = (uint)item["SupplyTypeID"].AsInteger;
                        SupplyTypeID[1] = (uint)item["SupplyTypeID2"].AsInteger;
                        SupplyTypeID[2] = (uint)item["SupplyTypeID3"].AsInteger;
                        foreach (var Reward in item["Items"].AsJsonArray)
                        {
                            ItemInfo = new ItemRandom()
                            {
                                TypeId = (uint)Reward["TypeID"].AsInteger,
                                MaxQuantity = (uint)Reward["MaxQuan"].AsInteger,
                                RareType = (uint)Reward["TypeRare"].AsInteger,
                                Probs = (ushort)Reward["Probability"].AsInteger,
                                Active = Reward["Valid"].AsBoolean
                            };

                            RandomWeight.AddItems(ItemInfo);
                        }
                    }

                    RandomWeight.AddSupply(SupplyTypeID[0]);
                    RandomWeight.AddSupply(SupplyTypeID[1]);
                    RandomWeight.AddSupply(SupplyTypeID[2]);
                    RandomWeight.SetCanDup(false);
                }
                else
                {
                    throw new Exception("BlackPapelItemList: BlackPapelData.json file in folder not found !");
                }
            }
            catch (Exception ex)
            {
                WriteConsole.WriteLine(ex.Message, ConsoleColor.Red);
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        public PapelSystem(GPlayer player)
        {
            OpenRareShop(player);
        }
       public static void OpenRareShop(GPlayer player)
        {
            player. Response.Write(new byte[] { 0x0B, 0x01 });//01 = ativado, 00 = desativo
            player.Response.Write(uint.MaxValue);
            player.Response.Write(uint.MaxValue);
            player.Response.Write(0);
            player.SendResponse();
        }
        

        public static void PlayNormalPapel(GPlayer player)
        {
            var rnd = new Random();
            byte Count, IQuantity;
            uint Stuff;
            AddData ItemData;
            PangyaBinaryWriter packet;


            packet = new PangyaBinaryWriter();
            try
            {
                ItemData = new AddData();
                Stuff = 0;
                foreach (var Supply in RandomWeight.FSupplies)
                {
                    ItemData = player.Inventory.Remove(Supply.TypeId, Supply.DelQuantity);
                    if (ItemData.Status)
                    {
                        Stuff = ItemData.ItemIndex;
                        break;
                    }
                }

                if (!ItemData.Status)
                {
                    if (!player.RemovePang(900))
                    {
                        player.SendResponse(new byte[] { 0x1B, 0x02, 0x7A, 0x73, 0x28, 0x00 });
                        return;
                    }
                    Stuff = 0;
                }

                if (rnd.Next(0, 21) + 1 >= 20)
                {
                    // 3 4 5
                    Count = (byte)rnd.Next(3, 6);
                }
                else
                {
                    Count = (byte)rnd.Next(2, 4);
                }
                // 2 3
                packet.WriteUInt32(Stuff);
                packet.WriteUInt32(Count);
                for (var I = 1; I <= Count; I++)
                {
                    var Reward = RandomWeight.GetItems();

                    if (Reward == null)
                    {
                        player.SendResponse(new byte[] { 0x6C, 0x02, 0x7A, 0x73, 0x28, 0x00 });
                        WriteConsole.WriteLine($" Error");
                        break;
                    }

                    if (new Random(0x64).Next() <= 20)
                    {
                        var QNT = rnd.Next((int)Reward.MaxQuantity) + 1;
                        IQuantity = (byte)QNT;
                    }
                    else
                    {
                        IQuantity = 1;
                    }
                    var Item = new AddItem
                    {
                        ItemIffId = Reward.TypeId,
                        Quantity = IQuantity,
                        Transaction = true,
                        Day = 0
                    };
                    ItemData = player.AddItem(Item);

                    packet.WriteInt32(rnd.Next(3));
                    packet.WriteUInt32(ItemData.ItemTypeID);
                    packet.WriteUInt32(ItemData.ItemIndex);
                    packet.WriteUInt32(IQuantity);
                    packet.WriteUInt32(Reward.RareType);
                }
                packet.WriteUInt64(player.GetPang);
                packet.WriteUInt64(player.GetCookie);

                player.SendTransaction();

                player.SendResponse(new byte[] { 0x1B, 0x02, 0x00, 0x00, 0x00, 0x00 }, packet.GetBytes());

                RandomWeight.Restore();

                //{ **Achievement * * }
                //{ **Add Papel Counter ** }
                //player.AddAchivementQuest(1816133706, 1);
                //player.SendAchievement();
            }
            finally
            {
                packet.Dispose();
            }
        }

        public static void PlayBigPapel(GPlayer player)
        {
            var rnd = new Random();
            byte Count, IQuantity;
            AddData ItemData;
            PangyaBinaryWriter packet;


            packet = new PangyaBinaryWriter();
            try
            {
                ItemData = new AddData();
                if (!player.RemovePang(10000))
                {
                    player.SendResponse(new byte[] { 0x6C, 0x02, 0x7A, 0x73, 0x28, 0x00 });
                    WriteConsole.WriteLine($" {player.GetLogin} esta sem pangs");
                    return;
                }

                player.SendPang();


                Count = (byte)rnd.Next(4, 10);

                packet.WriteUInt32(Count);

                for (var I = 1; I <= Count; I++)
                {
                    var Reward = RandomWeight.GetItems();
                    if (rnd.Next(0x64) <= 20)
                    {
                        // 3 4 5
                        IQuantity = (byte)rnd.Next(3, 6);
                    }
                    else
                    { // 5 6 7 8 9
                        IQuantity = (byte)rnd.Next(5, 10);
                    }

                    if ((Reward.RareType == 1) || (Reward.RareType == 2))
                    {
                        IQuantity = 1;
                    }
                    var Item = new AddItem
                    {
                        ItemIffId = Reward.TypeId,
                        Quantity = IQuantity,
                        Transaction = true,
                        Day = 0
                    };

                    ItemData = player.AddItem(Item);
                    packet.WriteInt32(rnd.Next(3));
                    packet.WriteUInt32(ItemData.ItemTypeID);
                    packet.WriteUInt32(ItemData.ItemIndex);
                    packet.WriteUInt32(IQuantity);
                    packet.WriteUInt32(Reward.RareType);
                }
                packet.WriteUInt64(player.GetPang);
                packet.WriteUInt64(player.GetCookie);

                player.SendTransaction();

                player.SendResponse(new byte[] { 0x6C, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, }, packet.GetBytes());

                RandomWeight.Restore();
            }
            finally
            {
                packet.Dispose();
            }
        }
    }
}
