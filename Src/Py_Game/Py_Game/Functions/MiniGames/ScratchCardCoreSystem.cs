using System;
using System.IO;
using Py_Game.Client;
using PangyaAPI;
using PangyaAPI.BinaryModels;
using Py_Game.GameTools.Json;
using Py_Game.ItemList;
using Py_Game.Client.Inventory.Data;
using Py_Game.Client.Inventory.Data.Transactions;
using PangyaAPI.PangyaPacket;
using PangyaAPI.Tools;

namespace Py_Game.Functions.MiniGames
{
    public class ScratchCardSystem
    {
        static ItemRandomClass RandomWeight { get; set; }

        static ScratchCardSystem()
        {
            ItemRandom ItemInfo;
            uint[] SupplyTypeID;
            RandomWeight = new ItemRandomClass();

            try
            {
                if (File.Exists("Json/ScratchCard.json"))
                {
                    SupplyTypeID = new uint[4];
                    foreach (var item in JsonValue.ParseFile("Json/ScratchCard.json")["ScratchCardData"].AsJsonArray)
                    {
                        SupplyTypeID[0] = (uint)item["SupplyTypeID"].AsInteger;
                        SupplyTypeID[1] = (uint)item["SupplyTypeID2"].AsInteger;
                        SupplyTypeID[2] = (uint)item["SupplyTypeID3"].AsInteger;
                        SupplyTypeID[3] = (uint)item["SupplyTypeID4"].AsInteger;
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
                    RandomWeight.AddSupply(SupplyTypeID[3]);
                    RandomWeight.SetCanDup(false);
                }
                else
                {
                    throw new Exception(" ScratchCard.json file in folder not found !");
                }
            }
            catch (Exception ex)
            {
                WriteConsole.WriteLine(ex.Message, ConsoleColor.Red);
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        public ScratchCardSystem(GPlayer player)
        {
            OpenScratchCard(player);
        }

        static void OpenScratchCard(GPlayer player)
        {
            player.SendResponse(new byte[] { 0xEB, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00 });
        }

        public static void PlayerScratchCardSerial(GPlayer player, Packet packet)
        {

            if (!packet.ReadUInt32(out uint serialSize))
            {
                return;
            }

            if (!packet.ReadPStr(out string serial, serialSize))
            {
                return;
            }

            if (serialSize != 13)
            {

            }
            //2500118
            //player.SendResponse(new byte[] { 0xDE, 0x00, 0x16, 0x26, 0x26, 0x00 }); 
            // The server seem to alway answer that with any wrong serial
            // Serial seem broken in original Pangya
            // O servidor parece sempre responder isso com qualquer serial errado
            // Serial parece quebrado no original Pangya

            // Old server data was(Os dados antigos do servidor eram)
            player.Response.Write(new byte[] { 0xDE, 0x00 });
            player.Response.Write(2500118);
            //player.Response.Write(0);// código de retorno 0 sucesso, 1 usado, 2 inválidos, 3 expirados etc ...
            //player.Response.Write(1);// item ID, is test?
            player.SendResponse();

            var item = player.Inventory.ItemWarehouse.GetItem(402653188, Defines.TGET_ITEM.gcTypeID);

            if (item != null)
            {
                //PlayerTransactionData Tran;

                //Tran = new PlayerTransactionData() { Types = 2, Index = (uint)item.ItemIndex, TypeID = item.ItemTypeID, PreviousQuan = item.ItemC0, NewQuan = (uint)item.ItemC0 + 1, UCC = "" };

                //player.Inventory.ItemTransaction.Add(Tran);

                //player.SendTransaction();
            }
        }

        public static void PlayerPlayScratchCard(GPlayer player)
        {
            uint Count, I, IQuantity = 0;
            AddData ItemData;
            ItemRandom Reward;
            AddItem AddItemData;
            PangyaBinaryWriter Packet;
            Random rnd;

            try
            {
                Packet = new PangyaBinaryWriter();
                ItemData = new AddData();
                rnd = new Random();
                foreach (var Supply in RandomWeight.FSupplies)
                {
                    ItemData = player.Inventory.Remove(Supply.TypeId, Supply.DelQuantity);
                    if (ItemData.Status)
                    {
                        break;
                    }
                }
                if (!ItemData.Status)// ## item can't be deleted
                {
                    player.SendResponse(new byte[] { 0xDD, 0x00, 0xE4, 0xC6, 0x2D, 0x00, });
                    return;
                }

                if (new Random(0x64).Next() < 10)
                {
                    Count = 2;
                }
                else
                {
                    Count = 1;
                }

                Packet.WriteUInt32(Count);
                for (I = 1; I <= Count; I++)
                {
                    Reward = RandomWeight.GetItems();
                    if (rnd.Next(0x64) < 10)
                    {
                        IQuantity = Reward.MaxQuantity;
                    }
                    else
                    {
                        IQuantity = 1;
                    }
                    AddItemData = new AddItem
                    {
                        ItemIffId = Reward.TypeId,
                        Quantity = IQuantity,
                        Transaction = true,
                        Day = 0
                    };
                    ItemData = player.AddItem(AddItemData);

                    if (ItemData.ItemTypeID == 0)
                    {
                        player.SendResponse(new byte[] { 0xDD, 0x00, 0x05, 0x00, 0x00, 0x00, });
                        return;
                    }
                    Packet.Write(0);
                    Packet.WriteUInt32(ItemData.ItemTypeID);
                    Packet.WriteUInt32(ItemData.ItemIndex);
                    Packet.WriteUInt32(IQuantity);
                    Packet.Write(1);//rare item?
                }

                player.SendTransaction();

                player.SendResponse(new byte[] { 0xDD, 0x00, 0x00, 0x00, 0x00, 0x00 }, Packet.GetBytes());

                RandomWeight.Restore();
            }
            catch
            {
                player.Close();
            }

        }
    }
}
