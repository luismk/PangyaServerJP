using Py_Game.Client;
using System.Collections.Generic;
using System.Linq;
using static PangyaFileCore.IffBaseManager;
using static Py_Game.GameTools.Tools;
using static Py_Game.GameTools.MathRand;
using System;
using Py_Game.ItemList;
using Py_Game.Client.Inventory.Data;
using PangyaAPI.PangyaPacket;
using PangyaAPI.Tools;

namespace Py_Game.Functions.MiniGames
{
    public class MemorialSystem
    {
        protected ItemMemorial Items;
        public MemorialSystem()
        {
            Items = new ItemMemorial();
        }
        public void PlayMemorialGacha(GPlayer player, Packet packet)
        {
            uint RandInt, TypeID;

            TypeID = packet.ReadUInt32();

            if (!IffEntry.MemorialCoin.IsExist(TypeID))
            {
                player.SendResponse(new byte[] { 0x64, 0x02, 0x85, 0x073, 0x55, 0x00 });
                WriteConsole.WriteLine("PlayerPlayerMemorialGacha: coin was not found", ConsoleColor.Red);
            }

            var RemoveData = player.Inventory.Remove(TypeID, 1);

            if (!RemoveData.Status)
            {
                player.SendResponse(new byte[] { 0x64, 0x02, 0x85, 0x073, 0x55, 0x00 });
                WriteConsole.WriteLine("PlayerPlayerMemorialGacha: Player don''t have that coin TypeID", ConsoleColor.Red);
            }
            RandInt = Rand.NextUInt(150);

            if (RandInt <= 20)
            {
                GetRareItem(player, TypeID);
            }
            else
            {
                GetNormalItem(player, TypeID);
            }
        }

        protected void GetRareItem(GPlayer player, uint TypeID)
        {
            AddItem AddItem;
            //For normal item
            //Form Rare item
            ItemRandomClass RareItem;
            ItemRandom PRare;
            List<Dictionary<uint, uint>> ListSet;
            try
            {
                RareItem = Items.GetRareItem(IffEntry.MemorialCoin.GetPool(TypeID));
                while (true)
                {
                    PRare = null;
                    if (RareItem.GetLeft() <= 0)
                        break;

                    PRare = RareItem.GetItems();

                    if (!player.Inventory.IsExist(PRare.TypeId))
                        break;
                }

                if (PRare == null)
                {
                    player.SendResponse(new byte[] { 0x64, 0x02, 0xAD, 0x0F9, 0x56, 0x00 });
                    WriteConsole.WriteLine("PlayerPlayerMemorialGacha: object is Null", ConsoleColor.Red);
                }

                if (GetItemGroup(PRare.TypeId) == 9)
                {
                    ListSet = IffEntry.SetItem.SetList(PRare.TypeId);

                    if (ListSet.Count <= 0)
                    {
                        player.SendResponse(new byte[] { 0x64, 0x02, 0xAD, 0x0F9, 0x56, 0x00 });
                        WriteConsole.WriteLine("PlayerPlayerMemorialGacha: Failed", ConsoleColor.Red);
                        // ## should not be happened
                        return;
                    }
                    foreach (var data in ListSet)
                    {
                        AddItem = new AddItem()
                        {
                            ItemIffId = data.Keys.FirstOrDefault(),
                            Quantity = data.Values.FirstOrDefault(),
                            Transaction = true,
                            Day = 0// ## set should not be limited time in their set
                        };
                        player.AddItem(AddItem);
                    }

                }
                else
                {
                    AddItem = new AddItem
                    {
                        ItemIffId = PRare.TypeId,
                        Quantity = PRare.MaxQuantity,
                        Transaction = true,
                        Day = 0,
                    };
                    player.AddItem(AddItem);
                }

                // ## send transaction
                player.SendTransaction();

                // 0 = ITEM COMUM
                // 1 = ITEM NORMAL ASA FECHADA
                // 2 = ITEM NORMAL ASA ABERTA
                // 3 = ITEM RARO ASA FECHADA
                // 4 = ITEM RARO ASA ABERTA
                player.Response.Write(new byte[] { 0x64, 0x02 });
                player.Response.Write(0);
                player.Response.Write(1);
                player.Response.Write(PRare.RareType);
                player.Response.WriteUInt32(PRare.TypeId);
                player.Response.WriteUInt32(PRare.MaxQuantity);
                player.SendResponse();
            }
            catch
            {
                player.Close();
            }
        }


        protected void GetNormalItem(GPlayer player, uint TypeID)
        {
            try
            {
                var NormalItem = Items.GetNormalItem(TypeID);
                // ## add to item list
                foreach (var PNormal in NormalItem)
                {
                    var NormalAddItem = new AddItem
                    {
                        ItemIffId = PNormal.TypeID,
                        Quantity = PNormal.Quantity,
                        Transaction = true,
                        Day = 0
                    };
                    // ## add to warehouse
                    player.AddItem(NormalAddItem);
                }
                // ## send transaction
                player.SendTransaction();

                // 0 = ITEM COMUM
                // 1 = ITEM NORMAL ASA FECHADA
                // 2 = ITEM NORMAL ASA ABERTA
                // 3 = ITEM RARO ASA FECHADA
                // 4 = ITEM RARO ASA ABERTA

                player.Response.Write(new byte[] { 0x64, 0x02 });
                player.Response.Write(0);
                player.Response.Write(NormalItem.Count);
                foreach (var PNormal in NormalItem)
                {
                    player.Response.WriteUInt32(PNormal.RareType); // ALTERAR AQUI  TIPO DE ITEM
                    player.Response.WriteUInt32(PNormal.TypeID);
                    player.Response.WriteUInt32(PNormal.Quantity);
                }
                player.SendResponse();
            }
            catch
            {
                player.Close();
            }
        }
    }
}
