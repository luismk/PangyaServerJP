using Py_Game.Client;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static PangyaFileCore.IffBaseManager;
using Py_Game.Defines;
using Py_Game.Client.Inventory.Data.Transactions;
using Py_Game.GameTools;
using PangyaAPI.PangyaPacket;

namespace Py_Game.Functions
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct TRecycleItemInfo
    {
        public uint ItemTypeID;
        public uint ItemIndex;
        public uint ItemQuantity;
    }
    public class ItemRecycleCoreSystem
    {
        public void PlayerRecycleItem(GPlayer player, Packet packet)
        {
            uint MP = 436208295,
                TikiPang = 0,
                TPCount = 0,
                Mileage = 0,  Count = 0;
            List<TRecycleItemInfo> ItemList;
            TRecycleItemInfo ItemInfo;
         
            ItemList = new List<TRecycleItemInfo>();
            try
            {
                if (!packet.ReadUInt32(out Count))
                {
                    return;
                }

                for (var i = 0; i < Count; i++)
                {
                    ItemInfo = (TRecycleItemInfo)packet.Read(new TRecycleItemInfo());
                    ItemList.Add(ItemInfo);

                    switch (GetItemGroup(ItemInfo.ItemTypeID))
                    {
                        case TITEMGROUP.ITEM_TYPE_CARD:
                            {

                            }
                            break;
                        case TITEMGROUP.ITEM_TYPE_PART:
                            {
                                var part = IffEntry.Part.GetItem(ItemInfo.ItemTypeID);

                                if (part.Base.TypeID > 0)
                                {
                                    TikiPang += part.Base.TikiPang;
                                    TPCount += part.Base.TPCount;
                                    Mileage += part.Base.Mileage;
                                }
                            }
                            break;
                        case TITEMGROUP.ITEM_TYPE_CLUB:
                            break;
                        case TITEMGROUP.ITEM_TYPE_BALL:
                            break;
                        case TITEMGROUP.ITEM_TYPE_USE:
                            break;
                        case TITEMGROUP.ITEM_TYPE_CADDIE:
                            break;
                        case TITEMGROUP.ITEM_TYPE_SKIN:
                            break;
                        case TITEMGROUP.ITEM_TYPE_AUX:
                            break;
                    }
                }
                player.Inventory.ItemTransaction.Add(new PlayerTransactionData()
                {
                    Types = 2,
                    TypeID = MP, // MP ¾ÆÀÌÅÛ
                    Index = 2020, //index item,
                    PreviousQuan = 720,
                    NewQuan = 734,
                    UCC = string.Empty
                });

                foreach (var item in ItemList)
                {
                    player.Inventory.ItemTransaction.Add(new PlayerTransactionData()
                    {
                        Types = 2,
                        TypeID = item.ItemTypeID,
                        Index = item.ItemIndex,
                        PreviousQuan = item.ItemQuantity,
                        NewQuan = item.ItemQuantity -Count,
                        UCC = string.Empty
                    });
                }

                player.SendTransaction();

                player.SendResponse(PacketCreator.ShowReceiveMileage(0, Mileage, TPCount));

            }
            catch
            {
                player.Close();
            }

          
        }

        static TITEMGROUP GetItemGroup(uint TypeID)
        {
            var result = (uint)Math.Round((TypeID & 4227858432) / Math.Pow(2.0, 26.0));

            return (TITEMGROUP)result;
        }
    }
}
