using System;
using System.Linq;
using System.Collections.Generic;
using Py_Game.Client.Inventory;
using PangyaAPI;
using Py_Game.Client;
using static PangyaFileCore.IffBaseManager;
using static Py_Game.GameTools.Tools;
using Py_Game.Client.Inventory.Data;
using Py_Game.Data;
using PangyaAPI.PangyaPacket;
using PangyaAPI.Tools;

namespace Py_Game.Functions
{
    public class CaddieMagicBoxSystem
    {
       
        //struct TMagic
        //{
        //    public ushort Page;
        //    public uint Sum;
        //    public ushort MagicID;
        //    public byte ItemCount;
        //    public uint ItemTypeID;
        //    public uint ItemIndex;
        //}
        public void PlayerMagicBox(GPlayer PL, Packet Packet)
        {
            uint MagicSum;
            ushort MagicID;
            byte SumItem;
            TMagicList MGItem;
            List<TMagicList> MGCList;
            List<Dictionary<uint, uint>> MagicList;
            Dictionary<uint, uint> GetItem;
            AddData ItemData;
            AddItem AddItemData;

            MGCList = new List<TMagicList>();
            try
            {
                MagicID = Packet.ReadUInt16();
                MagicSum = Packet.ReadUInt32();
                SumItem = Packet.ReadByte();

                MagicList = IffEntry.MgicBox.GetMagicTrade((uint)MagicID + 1);

                foreach (var data in MagicList)
                {
                    MGItem = new TMagicList
                    {
                        TypeID = data.First().Key,
                        Quantity = data.First().Value * MagicSum,
                    };
                    Packet.Skip(4);
                    MGItem.Index = Packet.ReadUInt32();
                    // ## if item didn't exist
                    if (!PL.Inventory.IsExist(MGItem.TypeID, MGItem.Index, MGItem.Quantity))
                    {
                        PL.SendResponse(new byte[] { 0x2F, 0x02, 0xB3, 0xF9, 0x56, 0x00 });// ## delete item fail or item didn't exist
                        return;
                    }
                    // ## add to list
                    MGCList.Add(MGItem);
                }

                foreach (var GetMGCList in MGCList)
                {
                    if (!PL.Inventory.Remove(GetMGCList.TypeID, GetMGCList.Index, GetMGCList.Quantity).Status)
                    {
                        PL.SendResponse(new byte[] { 0x2F, 0x02, 0x01, 0x00, 0x00, 0x00 });
                        WriteConsole.WriteLine("PlayerMagicBox: fail to delete PL's item while check true");
                        return;
                    }
                }

                GetItem = IffEntry.MgicBox.GetItem((uint)MagicID + 1);


                foreach (var data in GetItem)
                {
                    AddItemData = new AddItem
                    {
                        ItemIffId = data.Key,
                        Quantity = data.Value * MagicSum,
                        Transaction = true,
                        Day = 0
                    };


                    ItemData = AddItem(PL, AddItemData);

                    // ## send tran
                    PL.SendTransaction();

                    Packet.Write(new byte[] { 0x2F, 0x02 });
                    Packet.WriteUInt32(0);
                    Packet.WriteUInt32(MagicID);
                    Packet.WriteUInt32(1);
                    Packet.WriteUInt32(ItemData.ItemTypeID);
                    Packet.WriteUInt32(ItemData.ItemIndex);
                    Packet.WriteUInt32(data.Value * MagicSum);
                    Packet.WriteUInt32(ItemData.ItemNewQty);
                    Packet.WriteUInt32(0);
                    PL.SendResponse(Packet.GetBytes());

                }
            }
            finally
            {
            }
        }
        static AddData AddItem(GPlayer PL, AddItem ItemData)
        {
            List<Dictionary<UInt32, UInt32>> ListSet;
            AddItem ItemAddData;
            AddData Result;

            // ## GROUP SET ITEM
            if (GetItemGroup(ItemData.ItemIffId) == 9)
            {
               ListSet = IffEntry.SetItem.SetList(ItemData.ItemIffId);

                if (ListSet.Count <= 0)
                {
                    // ## should not be happened
                    return new AddData();
                }
                foreach (var _enum in ListSet)
                {
                    var data = _enum.First();

                    ItemAddData = new AddItem
                    {
                        ItemIffId = data.Key,
                        Quantity = data.Value,
                        Transaction = true,
                        Day = 0
                    };
                    PL.AddItem(ItemAddData);
                }
                Result = new AddData
                {
                    Status = true,
                    ItemIndex = uint.MaxValue,
                    ItemTypeID = ItemData.ItemIffId,
                    ItemOldQty = 0,
                    ItemNewQty = 1,
                    ItemUCCKey = string.Empty,
                    ItemFlag = 0,
                    ItemEndDate = DateTime.MinValue
                };
                return Result;
            }
            else
            { return PL.AddItem(ItemData); }
        }
    }
}
