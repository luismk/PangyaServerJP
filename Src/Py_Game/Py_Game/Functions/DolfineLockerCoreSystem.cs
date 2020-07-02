using PangyaAPI;
using Py_Game.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using Py_Connector.DataBase;
using static PangyaFileCore.IffBaseManager;
using Py_Game.Client.Inventory.Data.Warehouse;
using PangyaAPI.PangyaPacket;

namespace Py_Game.Functions
{
    public class DolfineLockerSystem
    {
        class LockerItem
        {
            public decimal TOTAL_PAGE { get; set; }
            public int INVEN_ID { get; set; }
            public int? TypeID { get; set; }
            public string UCC_UNIQE { get; set; }
            public byte UCC_STATUS { get; set; }
            public string UCC_NAME { get; set; }
            public int? UCC_COPY_COUNT { get; set; }
            public string NICKNAME { get; set; }
        }

        public void HandleEnterRoom(GPlayer player)
        {
            if (player.LockerPWD == "0")
            {
                //Chama a primeira criação da senha do dolfine locker
                player.SendResponse(new byte[] { 0x70, 0x01, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00 });
                return;
            }
            else
            {
                player.SendResponse(new byte[] { 0x70, 0x01, 0x00, 0x00, 0x00, 0x00, 0x4C, 0x00, 0x00, 0x00 });
            }
        }

        public void PlayerSetLocker(GPlayer player, Packet packet)
        {

            if (player.LockerPWD != "0")
            {
                return;
            }
            var _db = new PangyaEntities();
            var PwdInput = packet.ReadPStr();
            if (PwdInput.Length >= 4)
            {

                var CODE = _db.ProcSetLockerPwd((int)player.GetUID, PwdInput).First();
                if (CODE == 1)
                {
                    player.LockerPWD = PwdInput;


                    player.SendResponse(new byte[] { 0x76, 0x01, 0x00, 0x00, 0x00, 0x00 });
                }
            }
        }

        public void PlayerOpenLocker(GPlayer player, Packet packet)
        {
            var PwdInput = packet.ReadPStr();
            // senha diferente
            if (player.LockerPWD != PwdInput)
            {
                //senha incorreta 
                player.SendResponse(new byte[] { 0x6C, 0x01, 0x75, 0x00, 0x00, 0x00 });
                return;
            }
            else
                player.SendResponse(new byte[] { 0x6C, 0x01, 0x00, 0x00, 0x00, 0x00 });
        }

        public void PlayerChangeLockerPwd(GPlayer player, Packet packet)
        {
            var OLDPWD = packet.ReadPStr();
            var NEWPWD = packet.ReadPStr();

            // forem diferentes 
            if (player.LockerPWD != OLDPWD)
            {
                player.SendResponse(new byte[] { 0x6C, 0x01, 0x75, 0x00, 0x00, 0x00 });
            }
            if (NEWPWD.Length >= 4)
            {
                var _db = new PangyaEntities();
                var change = _db.ProcSetLockerPwd((int)player.GetUID, NEWPWD).FirstOrDefault();

                if (change != 1)
                    return;

                player.LockerPWD = NEWPWD;


                player.SendResponse(new byte[] { 0x74, 0x01, 0x00, 0x00, 0x00, 0x00, });
            }
        }

        public void PlayerGetPangLocker(GPlayer player)
        {
            player.SendLockerPang();
            List<LockerItem> item;

            var _db = new PangyaEntities();
            item = _db.Database.SqlQuery<LockerItem>($"EXEC [dbo].[ProcGetLockerItem] @UID = '{player.GetUID}', @PAGE = '{1}', @PAGE_TOTAL = '{20}'").ToList();

            player.Response.Write(new byte[] { 0x6D, 0x01 });
            if (item.Count == 0)
            {
                player.Response.WriteZero(5);
            }
            else
            {
                ushort TotalPage = (ushort)item.Count;

                player.Response.Write(TotalPage);
                player.Response.Write(TotalPage);
                foreach (var data in item)
                {
                    player.Response.Write((byte)item.Count);
                    player.Response.Write((uint)data.INVEN_ID);
                    player.Response.Write((uint)0);
                    player.Response.Write((uint)data.TypeID);
                    player.Response.Write((uint)0);
                    player.Response.Write(player.GetUID);
                    player.Response.Write((uint)1);
                    player.Response.WriteZero(23);
                    player.Response.WriteStr(data.UCC_UNIQE, 9);
                    player.Response.Write(Convert.ToUInt16(data.UCC_COPY_COUNT ?? 0));
                    player.Response.Write(data.UCC_STATUS);
                    player.Response.WriteZero(0x36);
                    player.Response.WriteStr(data.UCC_NAME, 16);
                    player.Response.WriteZero(0x19);
                    player.Response.WriteStr(data.NICKNAME, 0x16);
                }
            }
            player.SendResponse();
        }

        public void PlayerGetLockerItem(GPlayer player, Packet packet)
        {
            List<LockerItem> item;


            item = new List<LockerItem>();


            var TotalPage = (ushort)Math.Ceiling(a: 20 * 1.0);

            //dados não utilizados
            uint Unknown = packet.ReadUInt32();
            int Pages = packet.ReadUInt16();
            var _db = new PangyaEntities();
            item = _db.Database.SqlQuery<LockerItem>($"EXEC [dbo].[ProcGetLockerItem] @UID = '{player.GetUID}', @PAGE = '{Pages}', @PAGE_TOTAL = '{20}'").ToList();

            player.Response.Write(new byte[] { 0x6D, 0x01 });
            if (item.Count == 0)
            {
                player.Response.WriteZero(5);
            }
            else
            {
                player.Response.Write(TotalPage); // total page
                player.Response.Write((ushort)Pages); //page current   
                player.Response.Write((byte)item.Count);
                foreach (var data in item)
                {
                    player.Response.Write((uint)data.INVEN_ID);
                    player.Response.Write((uint)0);
                    player.Response.Write((uint)data.TypeID);
                    player.Response.Write((uint)0);
                    player.Response.Write(player.GetUID);//??
                    player.Response.WriteZero(0x1B);
                    player.Response.WriteStr(data.UCC_UNIQE, 9);
                    player.Response.Write((ushort?)data.UCC_COPY_COUNT ?? 0);
                    player.Response.Write(data.UCC_STATUS);
                    player.Response.WriteZero(0x36);
                    player.Response.WriteStr(data.UCC_NAME, 16);
                    player.Response.WriteZero(0x19);
                    player.Response.WriteStr(data.NICKNAME, 0x16);
                }
            }
            player.SendResponse();
        }
        // 6B = The process is not yet finished
        // 6C = You have too many items, cannot be put more
        // 6D = item can not be put in locker
        // 6E = item can be expired, cannot be put it locker
        // 6F = Cannot be put the amount of item more than you have
        // 70 = The process is finished // automatically close the locker
        public void PlayerPutItemLocker(GPlayer player, Packet packet)
        {

            //dados não utilizados
            var PlayerID = packet.ReadUInt32();
            packet.Skip(5);
            var TypeID = packet.ReadUInt32();
            var Index = packet.ReadUInt32();

            var GetItem = player.Inventory.ItemWarehouse.GetItem(Index);

            if (null == GetItem)
            {
                player.SendResponse(new byte[] { 0x6E, 0x01, 0x6B, 0x00, 0x00, 0x00 });
                return;
            }

            if (!(GameTools.Tools.GetItemGroup(GetItem.ItemTypeID) == 2))
            {
                player.SendResponse(new byte[] { 0x6E, 0x01, 0x6D, 0x00, 0x00, 0x00 });
                return;
            }
            var _db = new PangyaEntities();

            var invent = _db.USP_INVEN_PUSH((int)player.GetUID, (int)GetItem.ItemTypeID, IffEntry.GetItemName(GetItem.ItemTypeID), (int)GetItem.ItemIndex).FirstOrDefault();

            var Code = invent.Value;


            if (Code != 0)
            {
                player.SendResponse(new byte[] { 0x6E, 0x01, 0x6B, 0x00, 0x00, 0x00 });
                return;
            }

            if (player.Inventory.ItemWarehouse.RemoveItem(GetItem))
            {

                player.SendResponse(new byte[] { 0x39, 0x01, 0x00, 0x00 });


                player.Response.Write(new byte[] { 0xEC, 0x00 });
                player.Response.Write(1);
                player.Response.Write(1);
                player.Response.WriteStr("", 9);
                player.Response.Write(GetItem.ItemTypeID);
                player.Response.Write(GetItem.ItemIndex);
                player.Response.Write(player.GetUID);//quantity
                player.Response.WriteZero(27);
                player.Response.WriteStr(GetItem.ItemUCCUnique, 9);
                player.Response.Write(GetItem.ItemUCCCopyCount ?? 0);
                player.Response.Write(GetItem.ItemUCCStatus ?? 0);
                player.Response.WriteZero(54);
                player.Response.WriteStr(GetItem.ItemUCCName, 16);
                player.Response.WriteZero(25);
                player.Response.WriteStr(GetItem.ItemUCCDrawer, 22);
                player.SendResponse();


                player.Response.Write(new byte[] { 0x6E, 0x01 });
                player.Response.WriteZero(12);
                player.Response.Write(GetItem.ItemTypeID);
                player.Response.Write(GetItem.ItemIndex);
                player.Response.Write(player.GetUID);//quantity
                player.Response.WriteZero(27);
                player.Response.WriteStr(GetItem.ItemUCCUnique, 9);
                player.Response.Write(GetItem.ItemUCCCopyCount ?? 0);
                player.Response.Write(GetItem.ItemUCCStatus ?? 0);
                player.Response.WriteZero(54);
                player.Response.WriteStr(GetItem.ItemUCCName, 16);
                player.Response.WriteZero(25);
                player.Response.WriteStr(GetItem.ItemUCCDrawer, 22);
                player.SendResponse();
            }

        }

        public void PlayerTalkItemLocker(GPlayer player, Packet packet)
        {
            PlayerItemData Item;

            var count = packet.ReadByte();//count item
            var Index = packet.ReadInt32();//id do item

            var _db = new PangyaEntities();
            var invent = _db.USP_INVEN_POP((int)player.GetUID, Index).First();

            if (invent.ERROR == null || invent.ERROR != 0)
            {
                player.SendResponse(new byte[] { 0x6f, 0x01, 0x6B, 0x00, 0x00, 0x00 });
                return;
            }
            else
            {
                Item = new PlayerItemData();

                Item.CreateNewItem();

                Item.ItemIndex = (uint)invent.ITEM_ID;
                Item.ItemTypeID = (uint)invent.TYPEID;
                Item.ItemC0 = (ushort)invent.C0;
                Item.ItemC1 = (ushort)invent.C1;
                Item.ItemC2 = (ushort)invent.C2;
                Item.ItemC3 = (ushort)invent.C3;
                Item.ItemC4 = (ushort)invent.C4;
                Item.ItemEndDate = invent.DateEnd;
                Item.ItemFlag = invent.FLAG;
                Item.ItemUCCUnique = invent.UCC_UNIQE;
                Item.ItemUCCStatus = invent.UCC_STATUS;
                Item.ItemUCCName = invent.UCC_NAME;
                Item.ItemUCCDrawerUID = (uint?)invent.UCC_DRAWER_UID;
                Item.ItemUCCDrawer = invent.UCC_DRAWER_NICKNAME;
                Item.ItemUCCCopyCount = (ushort?)invent.UCC_COPY_COUNT;
                // Add to inventory
                player.Inventory.ItemWarehouse.Add(Item);

                player.Response.Write(new byte[] { 0xEC, 0x00 });
                player.Response.Write((byte)1);
                player.Response.Write(0);
                player.Response.Write(player.GetPang);
                player.Response.WriteZero(8);
                player.Response.Write(Item.ItemTypeID);
                player.Response.Write(Item.ItemIndex);
                player.Response.Write(player.GetUID);//quantity
                player.Response.WriteZero(27);
                player.Response.WriteStr(Item.ItemUCCUnique, 9);
                player.Response.Write(Item.ItemUCCCopyCount ?? 0);
                player.Response.Write(Item.ItemUCCStatus ?? 0);
                player.Response.WriteZero(54);
                player.Response.WriteStr(Item.ItemUCCName, 16);
                player.Response.WriteZero(25);
                player.Response.WriteStr(Item.ItemUCCDrawer, 16);
                player.Response.WriteZero(6);
                player.Response.Write((byte)3);
                player.Response.Write(player.GetUID);
                player.Response.Write(Item.ItemTypeID);
                player.Response.Write(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                player.Response.Write(1);
                player.Response.WriteZero(6);
                player.Response.Write(1);
                player.Response.WriteZero(0x0E);
                player.Response.Write((byte)2);
                player.Response.WriteStr(Item.ItemUCCName, 16);
                player.Response.WriteZero(25);
                player.Response.WriteStr(Item.ItemUCCUnique, 9);
                player.Response.Write(Item.ItemUCCStatus ?? 0);
                player.Response.Write(Item.ItemUCCCopyCount ?? 0);
                player.Response.WriteStr(Item.ItemUCCDrawer, 16);
                player.Response.WriteZero(0x4E);
                player.Response.Write(0);
                player.Response.Write(0);
                player.SendResponse();


                player.Response.Write(new byte[] { 0x6F, 0x01 });
                player.Response.Write(0);
                player.Response.Write(Index);
                player.Response.Write(0);
                player.Response.Write(Item.ItemIndex);
                player.Response.Write(player.GetUID);//quantity
                player.Response.Write(Item.ItemTypeID);
                player.Response.WriteZero(27);
                player.Response.WriteStr(Item.ItemUCCUnique, 9);
                player.Response.Write(Item.ItemUCCCopyCount ?? 0);
                player.Response.Write(Item.ItemUCCStatus ?? 0);
                player.Response.WriteZero(54);
                player.Response.WriteStr(Item.ItemUCCName, 16);
                player.Response.WriteZero(25);
                player.Response.WriteStr(Item.ItemUCCDrawer, 22);
                player.SendResponse();
            }
        }

        public void PlayerPangControlLocker(GPlayer player, Packet packet)
        {
            //120 =você inseriu um valor maior do que o permitido
            //111 = valor de entrada maior do que o que você tem
            //100 falhou
            //101 falied
            void SendCode(uint Code = 0)
            {
                player.Response.Write(new byte[] { 0x71, 0x01, });
                player.Response.Write(Code);
                player.SendResponse();
            }
            var Action = packet.ReadByte();
            var Pang = packet.ReadUInt64();

            bool Check()
            {
                return player.GetPang <= 0 || Pang <= 0;
            }


            try
            {
                if (!Check())
                {
                    SendCode(110);
                }
                else
                {
                    SendCode();
                    switch (Action)
                    {
                        case 0: //puxa pangs 
                            {
                                try
                                {
                                    if (player.RemoveLockerPang((uint)Pang))
                                    {
                                        player.AddPang((uint)Pang);
                                    }
                                }
                                catch
                                {
                                    SendCode(100);
                                    return;
                                }
                            }
                            break;
                        case 1:  //guarda pangs
                            {
                                try
                                {
                                    if (player.RemovePang((uint)Pang))
                                    {
                                        player.AddLockerPang((uint)Pang);
                                    }
                                }
                                catch
                                {
                                    SendCode(100);
                                    return;
                                }
                            }
                            break;
                    }

                    //reload pangs(reenvia os pangs na caixa de pangs do client)
                    player.SendPang();

                    //SendPangs in dolfine(envia os pangs na caixa do dolfine)
                    player.SendLockerPang();
                    var _db = new PangyaEntities();
                    _db.ProcSavePersonalLog((int)player.GetUID, Action, (int)player.LockerPang);
                }
            }
            catch
            {
                SendCode(100);
            }
        }
    }
}
