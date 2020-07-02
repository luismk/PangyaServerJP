using static PangyaFileCore.IffBaseManager;
using static Py_Game.GameTools.Tools;
using static Py_Game.GameTools.PacketCreator;
using System;
using System.Linq;
using Py_Game.Client;
using Py_Game.Defines;
using PangyaAPI;
using Py_Connector.DataBase;
using Py_Game.Data;
using Py_Game.Client.Inventory.Data;
using PangyaAPI.PangyaPacket;
using PangyaAPI.Tools;

namespace Py_Game.Functions
{
    public class GameShopCoreSystem
    {
        public void PlayerEnterGameShop(GPlayer player)
        {
            player.SendResponse(ShowCancelPacket());
        }

        public void PlayerBuyItemGameShop(GPlayer player, Packet packet)
        {
            ShopItemRequest ShopItem;
            uint DeletePang,
                DeleteCookie,
                RentalPrice;

            if (!packet.ReadByte(out byte BuyType))
            { return; }

            if (!packet.ReadUInt16(out ushort BuyTotal))
            { return; }

            switch ((TGAME_SHOP_ACTION)BuyType)
            {
                case TGAME_SHOP_ACTION.Normal:
                    {
                        for (int Count = 0; Count <= BuyTotal - 1; Count++)
                        {
                            ShopItem = (ShopItemRequest)packet.Read(new ShopItemRequest());

                            if (!IffEntry.IsExist(ShopItem.IffTypeId))
                            {
                                player.SendResponse(ShowBuyItemSucceed(TGAME_SHOP.PASSWORD_WRONG));
                                return;
                            }
                            if (!IffEntry.IsBuyable(ShopItem.IffTypeId))
                            {
                                player.SendResponse(ShowBuyItemSucceed(TGAME_SHOP.CANNOT_BUY_ITEM));
                                return;
                            }
                            if (!player.Inventory.Available(ShopItem.IffTypeId, ShopItem.IffQty))
                            {
                                player.SendResponse(ShowBuyItemSucceed(TGAME_SHOP.BUY_FAIL));
                                return;
                            }
                            if (IffEntry.GetPrice(ShopItem.IffTypeId, ShopItem.IffDay) <= 0)
                            {
                                player.SendResponse(ShowBuyItemSucceed(TGAME_SHOP.BUY_FAIL));
                                return;
                            }
                            switch (IffEntry.GetShopPriceType(ShopItem.IffTypeId))
                            {
                                case 0:
                                case 2:
                                case 6:
                                case 32:
                                case 96://Pang
                                    {
                                        DeletePang = IffEntry.GetPrice(ShopItem.IffTypeId, ShopItem.IffDay) * ShopItem.IffQty;
                                        if (!player.RemovePang(DeletePang))
                                        {
                                            player.SendResponse(ShowBuyItemSucceed(TGAME_SHOP.PANG_NOTENOUGHT));
                                            WriteConsole.WriteLine("HandlePlayerBuyItemGameShop: Cannot Delete Player_{0} {1} pang's", new object[] { player.GetLogin, DeletePang });
                                            return;
                                        }
                                    }
                                    break;
                                case 1:
                                case 33:
                                case 97://Cookie
                                    {
                                        DeleteCookie = IffEntry.GetPrice(ShopItem.IffTypeId, ShopItem.IffDay);
                                        if (!player.RemoveCookie(DeleteCookie))
                                        {
                                            player.SendResponse(ShowBuyItemSucceed(TGAME_SHOP.COOKIE_NOTENOUGHT));
                                            WriteConsole.WriteLine("HandlePlayerBuyItemGameShop: Cannot Delete Player_{0} {1} cookie's", new object[] { player.GetLogin, DeleteCookie });
                                            return;
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        if (ShopItem.CookiePrice > 0)
                                        {
                                            player.SendResponse(ShowBuyItemSucceed(TGAME_SHOP.BUY_FAIL));
                                            WriteConsole.WriteLine("CookiePriceType => {0} ", new object[] { IffEntry.GetShopPriceType(ShopItem.IffTypeId) });
                                            return;
                                        }
                                        if (ShopItem.PangPrice > 0)
                                        {
                                            player.SendResponse(ShowBuyItemSucceed(TGAME_SHOP.BUY_FAIL));
                                            WriteConsole.WriteLine("PangPriceType  => {0} ", new object[] { IffEntry.GetShopPriceType(ShopItem.IffTypeId) });
                                            return;
                                        }
                                    }
                                    break;
                            }

                            AddShopItem(player, ShopItem);
                        }
                    }
                    break;
                case TGAME_SHOP_ACTION.Rental:
                    {
                        for (int Count = 0; Count <= BuyTotal - 1; Count++)
                        {

                            ShopItem = (ShopItemRequest)packet.Read(new ShopItemRequest());

                            if (!(GetItemGroup(ShopItem.IffTypeId) == 2))
                            {
                                player.SendResponse(ShowBuyItemSucceed(TGAME_SHOP.ITEM_CANNOT_PURCHASE));
                                return;
                            }

                            if (!IffEntry.IsExist(ShopItem.IffTypeId))
                            {
                                player.SendResponse(ShowBuyItemSucceed(TGAME_SHOP.PASSWORD_WRONG));
                                return;
                            }
                            if (!IffEntry.IsBuyable(ShopItem.IffTypeId))
                            {
                                player.SendResponse(ShowBuyItemSucceed(TGAME_SHOP.CANNOT_BUY_ITEM));
                                return;
                            }
                            if (player.Inventory.IsExist(ShopItem.IffTypeId))
                            {
                                player.SendResponse(ShowBuyItemSucceed(TGAME_SHOP.ITEM_CANNOT_PURCHASE));
                                return;
                            }
                            RentalPrice = IffEntry.GetRentalPrice(ShopItem.IffTypeId);

                            if (RentalPrice <= 0)
                            {
                                player.SendResponse(ShowBuyItemSucceed(TGAME_SHOP.ITEM_CANNOT_PURCHASE));
                                return;
                            }
                            if (!player.RemovePang(RentalPrice))
                            {
                                player.SendResponse(ShowBuyItemSucceed(TGAME_SHOP.PANG_NOTENOUGHT));
                                WriteConsole.WriteLine("PlayerPlayerBuyItemGameShop: Cannot delete Player_{0} {1} pang(s)", new object[] { player.GetNickname, RentalPrice });
                            }
                            AddShopRentItem(player, ShopItem);
                        }
                    }
                    break;
            }
            player.SendResponse(ShowBuyItemSucceed(TGAME_SHOP.BUY_SUCCESS, player.GetPang, player.GetCookie));
        }

       
        void AddShopItem(GPlayer PL, ShopItemRequest shop)
        {
            var ListSet = IffEntry.SetItem.SetList(shop.IffTypeId);
            AddData ItemAddedData;
            AddItem ItemAddData;
            TBuyItem DataBuy;
            //group set item
            if (GetItemGroup(shop.IffTypeId) == 9)
            {
                if (ListSet.Count <= 0)// ## should not be happened
                {
                    PL.SendResponse(ShowBuyItemSucceed(TGAME_SHOP.BUY_FAIL));
                    WriteConsole.WriteLine("Something strange happened (T.T)");
                    return;
                }
                else
                {
                    foreach (var datas in ListSet)
                    {
                        ItemAddData = new AddItem
                        {
                            ItemIffId = datas.FirstOrDefault().Key,
                            Quantity = datas.FirstOrDefault().Value,
                            Transaction = false,
                            Day = 0
                        };
                        ItemAddedData = PL.AddItem(ItemAddData);
                        DataBuy = CheckData(ItemAddedData);
                        PL.SendResponse(ShowBuyItem(ItemAddedData, DataBuy, PL.GetPang, PL.GetCookie));
                    }
                }
            }
            else
            {
                ItemAddData = new AddItem
                {
                    ItemIffId = shop.IffTypeId,
                    Quantity = IffEntry.GetRealQuantity(shop.IffTypeId, shop.IffQty),
                    Transaction = false,
                    Day = shop.IffDay
                };
                ItemAddedData = PL.AddItem(ItemAddData);
                DataBuy = CheckData(ItemAddedData);
                PL.SendResponse(ShowBuyItem(ItemAddedData, DataBuy, PL.GetPang, PL.GetCookie));
            }
        }

        void AddShopRentItem(GPlayer PL, ShopItemRequest ShopItem)
        {
            AddData ItemAddedData;
            TBuyItem DataBuy;
            ItemAddedData = PL.Inventory.AddRent(ShopItem.IffTypeId);

            DataBuy = new TBuyItem
            {
                Flag = 0x6,
                DayTotal = 0x7,
                EndDate = null
            };
            var _db = new PangyaEntities();
            PL.SendResponse(ShowBuyItem(ItemAddedData, DataBuy, PL.GetPang, PL.GetCookie));
        }

        TBuyItem CheckData(AddData AddData)
        {
            TBuyItem Result;

            switch ((TITEMGROUP)GetItemGroup(AddData.ItemTypeID))
            {
                case TITEMGROUP.ITEM_TYPE_CADDIE:
                    {
                        if (AddData.ItemEndDate != null && AddData.ItemEndDate > DateTime.Now)
                        {
                            Result = new TBuyItem
                            {
                                Flag = 4,
                                DayTotal = (ushort)(DaysBetween(AddData.ItemEndDate, DateTime.Now) + 1),
                                EndDate = AddData.ItemEndDate
                            };
                        }
                        else
                        {
                            Result = new TBuyItem
                            {
                                Flag = 0,
                                DayTotal = 0,
                                EndDate = null
                            };
                        }
                    }
                    break;
                case TITEMGROUP.ITEM_TYPE_CADDIE_ITEM:
                    {
                        if (AddData.ItemEndDate != null)
                        {
                            Result = new TBuyItem
                            {
                                Flag = 4,
                                DayTotal = (ushort)(DaysBetween(AddData.ItemEndDate, DateTime.Now) * 24),
                                EndDate = AddData.ItemEndDate
                            };
                        }
                        else
                        {
                            Result = new TBuyItem
                            {
                                Flag = 0,
                                DayTotal = 0,
                                EndDate = null
                            };
                        }

                    }
                    break;
                case TITEMGROUP.ITEM_TYPE_SKIN:
                    {
                        if (AddData.ItemEndDate != null)
                        {
                            Result = new TBuyItem
                            {
                                Flag = 4,
                                DayTotal = (ushort)(DaysBetween(AddData.ItemEndDate, DateTime.Now) + 1),
                                EndDate = AddData.ItemEndDate
                            };
                        }
                        else
                        {
                            Result = new TBuyItem
                            {
                                Flag = 0,
                                DayTotal = 0,
                                EndDate = null
                            };
                        }
                    }
                    break;
                case TITEMGROUP.ITEM_TYPE_MASCOT:
                    {
                        if (AddData.ItemEndDate > DateTime.Now)
                        {
                            Result = new TBuyItem
                            {
                                Flag = 4,
                                DayTotal = (ushort)(DaysBetween(AddData.ItemEndDate, DateTime.Now) + 1),
                                EndDate = AddData.ItemEndDate
                            };
                        }
                        else
                        {
                            Result = new TBuyItem
                            {
                                Flag = 0,
                                DayTotal = 0,
                                EndDate = null
                            };
                        }
                    }
                    break;              
                default:
                    {
                        Result = new TBuyItem
                        {
                            Flag = 0,
                            DayTotal = 0,
                            EndDate = null
                        };
                    }
                    break;
            }
            return Result;
        }       
    }
}
