using Py_Connector.DataBase;
using  PangyaAPI.BinaryModels;
using System.Linq;
using Py_Game.Client;
using Py_Game.Client.Inventory;
using PangyaAPI;
using Py_Game.Client.Inventory.Data.Warehouse;
using PangyaAPI.PangyaPacket;

namespace Py_Game.Functions
{
    public class SelfDesignCoreSystem
    {
        public void PlayerRequestUploadKey(GPlayer player, Packet packet)
        {
            byte Option;
            uint ITEMID;
            PlayerItemData ItemUCC;
            var db = new PangyaEntities();
            Option = packet.ReadByte();
            switch (Option)
            {
                case 0:
                    {
                        var typeID = packet.ReadInt32();//meu uid
                        packet.Skip(1);// Skip for ununsed data
                        ITEMID = packet.ReadUInt32();
                        ItemUCC = player.Inventory.ItemWarehouse.GetUCC(ITEMID);
                        if (ItemUCC == null)
                        {
                            return;
                        }

                        var Query = db.USP_UCC_REQUEST_UPLOAD((int)player.GetUID,(int)ITEMID).FirstOrDefault();

                        if (!(Query.CODE == 1))
                        {
                            return;
                        }
                        player.Response.Write(new byte[] { 0x53, 0x01, Option });
                        player.Response.Write((byte)1); // Unknown now
                        player.Response.WriteUInt32(ITEMID);
                        player.Response.WritePStr(Query.UCCKEY);
                        player.Response.Write((byte)1); // Unknown now
                        player.SendResponse();
                    }
                    break;

            }
        }

        public void PlayerAfterUploaded(GPlayer player, Packet packet)
        {
            var db = new PangyaEntities();
            byte Option;
            byte Cases;
            uint TypeId;
            uint UCC_IDX;
            string UCC_UNIQUE;
            string UCC_NAME;
            PlayerItemData Item = null;
            TSaveUCC UCC_SAVE = new TSaveUCC();
            Option = packet.ReadByte();
            switch (Option)
            {
                // Save Permanently
                case 0:
                    {

                        TypeId = packet.ReadUInt32();
                        UCC_UNIQUE = packet.ReadPStr();//key?
                        UCC_NAME = packet.ReadPStr();

                        Item = player.Inventory.ItemWarehouse.GetUCC(TypeId, UCC_UNIQUE);

                        if (Item == null)
                        {
                            Item = player.Inventory.ItemWarehouse.GetUCC(TypeId, UCC_UNIQUE);
                            return;
                        }
                        if (!(Item == null))
                        {
                            Item.ItemUCCStatus = 1;
                            Item.ItemUCCName = UCC_NAME;
                            Item.ItemUCCDrawerUID = (uint)player.GetUID;
                            Item.ItemNeedUpdate = false;
                            UCC_SAVE.UID = (uint)player.GetUID;
                            UCC_SAVE.UCCIndex = Item.ItemIndex;
                            UCC_SAVE.UCCName = UCC_NAME;
                            UCC_SAVE.UCCStatus = (byte)Item.ItemUCCStatus;
                            UCC_SAVE.UccDrawerUID = (uint)player.GetUID;
                            // SAVE TO DATABASE
                            SaveUCC(UCC_SAVE);
                        }
                        player.Response.Write(new byte[] { 0x2E, 0x01, 0x00, 0x01 });
                        player.Response.WriteUInt32(Item.ItemIndex);
                        player.Response.WriteUInt32(Item.ItemTypeID);
                        player.Response.WritePStr(Item.ItemUCCUnique);
                        player.Response.WritePStr(UCC_NAME);
                        player.SendResponse();
                        break;
                    }
                // UCC INFO
                case 1:
                    {
                        UCC_IDX = packet.ReadUInt32();
                        Cases = packet.ReadByte();


                        if ((UCC_IDX == 0))
                        {
                            player.SendResponse(new byte[] { 0x2E, 0x01, 0x04 });
                            return;
                        }
                        try
                        {
                            var data = db.ProcGetUCCData((int)UCC_IDX).ToList();
                            if (data.Count <= 0)
                            {
                                return;
                            }

                            player.Response.Write(new byte[] { 0x2E, 0x01, 0x01 });
                            foreach (var Query in data)
                            {
                                player.Response.WriteInt32(Query.TYPEID);
                                player.Response.WritePStr(Query.UCC_UNIQE);
                                player.Response.WriteByte(1);
                                player.Response.WriteInt32(Query.item_id);
                                player.Response.WriteInt32(Query.TYPEID);
                                player.Response.WriteZero(0xF);
                                player.Response.WriteByte(1);
                                player.Response.WriteZero(0x10);
                                player.Response.WriteByte(2);
                                player.Response.WriteStr(Query.UCC_NAME, 0x10);
                                player.Response.WriteZero(0x19);
                                player.Response.WriteStr(Query.UCC_UNIQE, 0x9);
                                player.Response.WriteByte((byte)Query.UCC_STATUS);
                                player.Response.WriteUInt16((ushort)Query.UCC_COPY_COUNT);
                                player.Response.WriteStr(Query.Nickname, 0x10);
                                player.Response.WriteZero(0x56);
                            }
                            player.SendResponse();
                        }
                        finally
                        {
                            db = null;

                        }
                        break;
                    }
                // COPY UCC
                case 2:
                    {
                        TypeId = packet.ReadUInt32();
                        UCC_UNIQUE = packet.ReadPStr();
                        packet.Skip(2);
                        UCC_IDX = packet.ReadUInt32();


                        // IDX TO COPY
                        Item = player.Inventory.ItemWarehouse.GetUCC(TypeId, UCC_UNIQUE, true);
                        if (Item == null)
                        {
                            return;
                        }
                        db = new PangyaEntities();
                        try
                        {
                            //ProcSaveUCCCopy
                            var Query = db.ProcSaveUCCCopy((int)player.GetUID, (int)TypeId, UCC_UNIQUE, (int)UCC_IDX).First();

                            if (Query.Code == 0)
                            {
                                return;
                            }
                            player.Response.Write(new byte[] { 0x2E, 0x01, 0x02 });
                            player.Response.WriteUInt32(TypeId);
                            player.Response.WritePStr(UCC_UNIQUE);
                            player.Response.Write(new byte[] { 0x01, 0x00 }); // UNKNOWN YET                          
                            player.Response.WriteUInt32(UCC_IDX);
                            player.Response.WriteInt32(Query.ITEM_ID);
                            player.Response.WriteInt32(Query.TYPEID);
                            player.Response.WritePStr(Query.UCC_UNIQE);
                            player.Response.Write((ushort)Query.UCC_COPY_COUNT);
                            player.Response.Write((byte)1);
                            player.SendResponse();
                        }
                        finally
                        {
                            db = null;
                        }
                        break;
                    }
                // SAVE TEMPARARILY
                case 3:
                    {
                        TypeId = packet.ReadUInt32();
                        UCC_UNIQUE = packet.ReadPStr();

                        player.Response.Write(new byte[] { 0x2E, 0x01 });
                        player.Response.Write(Option);
                        player.Response.WriteUInt32(TypeId);
                        player.Response.WritePStr(UCC_UNIQUE);
                        Item = player.Inventory.ItemWarehouse.GetUCC(TypeId, UCC_UNIQUE);
                        if (Item == null)
                        {
                            player.Response.Write((byte)0);
                        }
                        if (!(Item == null))
                        {
                            Item.ItemUCCStatus = 2;
                            Item.ItemNeedUpdate = true;
                            player.Response.Write((byte)1);
                        }
                        player.SendResponse();
                        break;
                    }
            }
        }

        class TSaveUCC
        {
            public uint UID { get; set; }
            public uint UCCIndex { get; set; }
            public string UCCName { get; set; }
            public byte UCCStatus { get; set; }
            public uint UccDrawerUID { get; set; }
        }

        static void SaveUCC(TSaveUCC Data)
        {
            var db = new PangyaEntities();

            var Query = db.ProcSaveUCC((int)Data.UID, (int)Data.UCCIndex, Data.UCCName, Data.UCCStatus, (int)Data.UccDrawerUID);
        }
    }
}
