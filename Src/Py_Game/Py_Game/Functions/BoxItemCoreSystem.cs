using System;
using System.Collections.Generic;
using Py_Game.Client;
using PangyaAPI;
using Py_Game.Client.Inventory;
using static Py_Game.GameTools.ErrorCode;
using Py_Game.GameTools;
using static Py_Game.GameTools.PacketCreator;
using Py_Game.ItemList;
using Py_Game.Functions.Mail;
using PangyaAPI.Auth;
using Py_Game.Client.Inventory.Data;
using Py_Game.Data;
using PangyaAPI.PangyaPacket;
using PangyaAPI.Tools;

namespace Py_Game.Functions
{
    public class BoxItemCoreSystem
    {
        protected BoxItemList BoxItem = new BoxItemList();
        #region CONSTRUCTOR
        public BoxItemCoreSystem()
        {
            BoxItem = new BoxItemList();           
        }
        #endregion
        public void PlayerOpenBox(GPlayer player, Packet packet)
        {
            TRewardInfo Reward;
            TBoxInfo BoxInfo;
            AddData RemoveItemData;
            AddData AddItemData;
            AddItem Item;
            string Param = "";
            List<object> Lists;
            object APoint;
            MailSender MailSender;
            Lists = new List<object>();
            try
            {
                if (!packet.ReadUInt32(out uint BoxIffTypeID))
                {
                    return;
                }

                AddItemData = new AddData();

                BoxInfo = BoxItem.GetBoxInfo(BoxIffTypeID);
                // # Box doens't exist
                if ((BoxInfo == null))
                {
                    player.Write(OPEN_BOX_NULL);
                    WriteConsole.WriteLine($"HandlePlayerOpenBox: Cannot find system\'s box to open { BoxIffTypeID }", ConsoleColor.Red);
                    return;
                }
                // # Player does not have this box
                if (!player.Inventory.IsExist(BoxInfo.BoxTypeID))
                {
                    player.Write(OPEN_BOX_NULL);
                    WriteConsole.WriteLine($"HandlePlayerOpenBox: Player hasn\'t had this box { BoxIffTypeID }", ConsoleColor.Red);
                    return;
                }
                // # Special item to delete # use for openned cube
                if ((BoxInfo.SupplyTypeID > 0))
                {
                    if (!player.Inventory.IsExist(BoxInfo.SupplyTypeID))
                    {
                        player.Write(OPEN_BOX_NULL);
                        WriteConsole.WriteLine($"HandlePlayerOpenBox: Can\'t find player\'s supply { BoxInfo.SupplyTypeID }", ConsoleColor.Red);
                        return;
                    }
                }
                // # delete box
                RemoveItemData = player.Inventory.Remove(BoxInfo.BoxTypeID, BoxInfo.BoxQuantity, false);
                // # add to list
                APoint = new TItemData();
                ((TItemData)APoint).TypeID = RemoveItemData.ItemTypeID;
                ((TItemData)APoint).ItemIndex = RemoveItemData.ItemIndex;
                ((TItemData)APoint).ItemQuantity = RemoveItemData.ItemNewQty;
                Lists.Add((TItemData)APoint);
                // # end
                // # if supply typeid is specified
                if (BoxInfo.SupplyTypeID > 0)
                {
                    // # delete supplyment # use for key's spin cube
                    RemoveItemData = player.Inventory.Remove(BoxInfo.SupplyTypeID, BoxInfo.SupplyQuantity, false);
                    // # add to list
                    APoint = new TItemData();
                    ((TItemData)APoint).TypeID = RemoveItemData.ItemTypeID;
                    ((TItemData)APoint).ItemIndex = RemoveItemData.ItemIndex;
                    ((TItemData)APoint).ItemQuantity = RemoveItemData.ItemNewQty;
                    Lists.Add((TItemData)APoint);
                    // # end
                }
                // # if special reward is specified
                if ((BoxInfo.SpecialRewardTypeID > 0))
                {
                    Item = new AddItem()
                    {
                        ItemIffId = BoxInfo.SpecialRewardTypeID,
                        Quantity = BoxInfo.SpecialRewardQuantity,
                        Transaction = false,
                        Day = 0
                    };
                    AddItemData = player.AddItem(Item);
                    if ((AddItemData.ItemNewQty > 1))
                    {
                        // # add to list
                        APoint = new TItemData();

                        ((TItemData)APoint).TypeID = AddItemData.ItemTypeID;
                        ((TItemData)APoint).ItemIndex = AddItemData.ItemIndex;
                        ((TItemData)APoint).ItemQuantity = AddItemData.ItemNewQty;
                        Lists.Add((TItemData)APoint);
                        // # end
                    }
                }
                // # send data to player
                player.Write(ShowBoxItem(Lists));
                // # clear
                Lists.Clear();
                // # send #$AA               
                if ((AddItemData.ItemNewQty == 1))
                {
                    APoint = new TItemData();
                    ((TItemData)(APoint)).TypeID = AddItemData.ItemTypeID;
                    ((TItemData)(APoint)).ItemIndex = AddItemData.ItemIndex;
                    ((TItemData)(APoint)).ItemQuantity = AddItemData.ItemNewQty;
                    Lists.Add((TItemData)APoint);
                }
                // send result
                player.Write(ShowBoxNewItem(Lists, player.GetPang, player.GetCookie));
                while (true)
                {
                    Reward = BoxItem.GetItemBox(BoxIffTypeID);
                    if (!Reward.Duplicated)
                    {
                        // if this item can have only one ea
                        if (!player.Inventory.IsExist(Reward.TypeId))
                        {
                            break;
                        }
                    }
                    else if (Reward.Duplicated)
                    {
                        break;
                    }
                }
                if (Reward.TypeId <= 0)
                {
                    player.Write(BOX_REWARD_NIL);
                    WriteConsole.WriteLine($"HandlePlayerOpenBox: Reward is nil with box typeid: { BoxIffTypeID }");
                    return;
                }
                if (Reward.Announce)
                {
                    // generate param
                    Param = string.Format(" You got items from !! BoxAnnounce = <PARAMS> <BOX_TYPEID> {0} </ BOX_TYPEID> <NICKNAME> {1} </ NICKNAME> <TYPEID> {2}</ TYPEID> <QTY> {3} </ QTY>", new object[] { BoxIffTypeID, player.GetNickname, Reward.TypeId, Reward.Quantity });

                    var Auth = MainServer.Program._server.AuthServer;

                    if (Auth != null)
                    {
                        Auth.Send(new AuthPacket() {ID = AuthPacketEnum.SERVER_RELEASE_BOXRANDOM, Message = new { GetMessage = Param } });
                    }
                    else
                    {
                        player.Server.Notice(Param);
                    }
                }
                try
                {
                    MailSender = new MailSender
                    {
                        Sender = "@BoxSystem"
                    };
                    MailSender.AddText(BoxInfo.BoxParam);
                    MailSender.AddItem(Reward.TypeId, Reward.Quantity);
                    MailSender.Send(player.GetUID);
                    player.SendMailPopup();
                }
                finally
                {
                    MailSender = null;
                }
                player.Write(ShowBoxItem(BoxIffTypeID, Reward.TypeId, Reward.Quantity));
            }
            catch
            {
                player.Close();
            }
            finally
            {
                Lists.Clear();
            }
        }
    }
}
