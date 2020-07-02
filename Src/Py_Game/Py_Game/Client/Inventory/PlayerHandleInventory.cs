using PangyaAPI.BinaryModels;
using Py_Game.Defines;
using Py_Connector.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static PangyaFileCore.IffBaseManager;
using static Py_Game.GameTools.TCompare;
using static System.Math;
using Py_Game.Client.Inventory.Data;
using Py_Game.Client.Inventory.Data.Warehouse;
using Py_Game.Client.Inventory.Data.Character;
using Py_Game.Client.Inventory.Data.Card;
using Py_Game.Client.Inventory.Data.Caddie;
using Py_Game.Client.Inventory.Data.Mascot;
using Py_Game.Client.Inventory.Data.Transactions;
namespace Py_Game.Client.Inventory
{
    public partial class PlayerInventory
    {
        #region Methods Array of Byte
        public byte[] GetToolbar()
        {
            PangyaBinaryWriter Reply;

            Reply = new PangyaBinaryWriter();

            Reply.Write(new byte[] { 0x72, 0x00 });
            Reply.Write(GetEquipData());
            return Reply.GetBytes();
        }
        public byte[] GetCharData()
        {
            return ItemCharacter.GetCharData(CharacterIndex);
        }
        public byte[] GetCharData(uint Index)
        {

            return ItemCharacter.GetCharData(Index);
        }
        public byte[] GetMascotData()
        {
            PlayerMascotData MascotInfo;
            MascotInfo = ItemMascot.GetMascotByIndex(MascotIndex);
            if ((MascotInfo != null))
            {
                return MascotInfo.GetMascotInfo();
            }
            return new byte[0x3E];
        }

        public byte[] GetTrophyInfo()
        {
            return ItemTrophies.GetTrophy();
        }
        /// <summary>
        /// GetSize 116 bytes
        /// </summary>
        /// <returns></returns>
        public byte[] GetEquipData()
        {
            var result = new PangyaBinaryWriter();

            result.WriteUInt32(CaddieIndex);
            result.WriteUInt32(CharacterIndex);
            result.WriteUInt32(ClubSetIndex);
            result.WriteUInt32(BallTypeID);//16
            result.Write(ItemSlot.GetItemSlot());//56
            result.WriteUInt32(BackGroundIndex);
            result.WriteUInt32(FrameIndex);
            result.WriteUInt32(StickerIndex);
            result.WriteUInt32(SlotIndex);
            result.WriteUInt32(0);//UNKNOWN, value = 0
            result.WriteUInt32(TitleIndex);
            result.WriteStruct(ItemDecoration);//104
            result.WriteUInt32(MascotIndex);
            result.WriteUInt32(Poster1);
            result.WriteUInt32(Poster2);//116
            return result.GetBytes();
        }
        /// <summary>
        /// GetCharacter(513 bytes), GetCaddie(25 bytes),ClubSet(28 bytes), Mascot(62 bytes), Total Size 634 
        /// </summary>
        /// <returns>Select(634 array of byte)</returns>
        public byte[] GetEquipInfo()
        {
            var Response = new PangyaBinaryWriter();
            Response.Write(GetCharData());
            Response.Write(GetCaddieData());
            Response.Write(GetClubData());
            Response.Write(GetMascotData());
            return Response.GetBytes();
        }

        public byte[] GetClubData()
        {
            PlayerItemData ClubInfo;
            ClubInfo = ItemWarehouse.GetItem(this.ClubSetIndex);
            if ((ClubInfo == null))
            {
                return new byte[28];
            }
            return ClubInfo.GetClubInfo();
        }

        public byte[] GetCaddieData()
        {
            PlayerCaddieData CaddieInfo;
            CaddieInfo = ItemCaddie.GetCaddieByIndex(CaddieIndex);
            if (!(CaddieInfo == null))
            {
                return CaddieInfo.GetCaddieInfo();
            }
            return new byte[0x19];
        }
        // transaction
        public byte[] GetTransaction()
        {
            return ItemTransaction.GetTran();
        }

        public byte[] GetDecorationData()
        {
            using (var result = new PangyaBinaryWriter())
            {
                result.WriteStruct(ItemDecoration);
                return result.GetBytes();
            }
        }
        public byte[] GetGolfEQP()
        {
            using (var Packet = new PangyaBinaryWriter())
            {
                Packet.WriteUInt32(BallTypeID);
                Packet.WriteUInt32(ClubSetIndex);
                return Packet.GetBytes();
            }
        }

        #endregion

        #region Methods Bool
        // poster


        public bool SetCutInIndex(uint CharIndex, uint CutinIndex)
        {
            if (CutinIndex == 0)
            {
                return true;
            }
            var Item = ItemWarehouse.GetItem(CutinIndex, TGET_ITEM.gcIndex);
            var CharType = ItemCharacter.GetChar(CharIndex, Defines.CharType.bIndex);
            if (Item == null)
            {
                return false;
            }
            if (CharType == null)
            {
                return false;
            }
            CharType.FCutinIndex = Item.ItemIndex;
            ItemCharacter.UpdateCharacter(CharType);
            return true;
        }
        public bool SetPoster(uint Poster1, uint Poster2)
        {
            this.Poster1 = Poster1;
            this.Poster2 = Poster2;
            return true;
        }

        public bool IsExist(uint TypeID, uint Index, uint Quantity)
        {
            switch (GetPartGroup(TypeID))
            {
                case 5:
                case 6:
                    // ## normal and ball
                    return ItemWarehouse.IsNormalExist(TypeID, Index, Quantity);
                case 2:
                    // ## part
                    return ItemWarehouse.IsPartExist(TypeID, Index, Quantity);
                case 0x1:
                    // ## card
                    return ItemCard.IsExist(TypeID, Index, Quantity);
            }
            return false;
        }

        // item exists?
        public bool IsExist(uint TypeId)
        {
            List<Dictionary<uint, uint>> ListSet;
            switch (GetPartGroup(TypeId))
            {
                case 2:
                    return ItemWarehouse.IsPartExist(TypeId);
                case 5:
                case 6:
                    return ItemWarehouse.IsNormalExist(TypeId);
                case 9:
                    ListSet = IffEntry.SetItem.SetList(TypeId);
                    try
                    {
                        if (ListSet.Count <= 0)
                        {
                            return false;
                        }
                        foreach (var __Enum in ListSet)
                        {
                            if (this.IsExist(__Enum.First().Key))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                    finally
                    {
                        ListSet.Clear();
                    }
                case 14:
                    return ItemWarehouse.IsSkinExist(TypeId);
            }
            return false;
        }

        public bool Available(uint TypeID, uint Quantity)
        {

            var ListSet = IffEntry.SetItem.SetList(TypeID);

            switch ((TITEMGROUP)GetPartGroup(TypeID))
            {
                case TITEMGROUP.ITEM_TYPE_SETITEM:
                    {
                        if (ListSet.Count <= 0)
                        { return false; }

                        else
                        {
                            foreach (var data in ListSet)
                            {
                                Available(data.Keys.FirstOrDefault(), data.Values.FirstOrDefault());
                            }
                            return true;
                        }
                    }
                case TITEMGROUP.ITEM_TYPE_CHARACTER:
                    {
                        return true;
                    }
                case TITEMGROUP.ITEM_TYPE_HAIR_STYLE:
                    {
                        return true;
                    }
                case TITEMGROUP.ITEM_TYPE_PART:
                    {
                        return true;
                    }
                case TITEMGROUP.ITEM_TYPE_CLUB:
                    {
                        if (ItemWarehouse.IsClubExist(TypeID))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                case TITEMGROUP.ITEM_TYPE_AUX:
                case TITEMGROUP.ITEM_TYPE_BALL:
                case TITEMGROUP.ITEM_TYPE_USE:
                    {
                        if (GetQuantity(TypeID) + Quantity > 32767)
                        {
                            return false;
                        }
                        return true;
                    }
                case TITEMGROUP.ITEM_TYPE_CADDIE:
                    {
                        if (ItemCaddie.IsExist(TypeID))
                        {
                            return false;
                        }
                        return true;
                    }
                case TITEMGROUP.ITEM_TYPE_CADDIE_ITEM:
                    {
                        if (ItemCaddie.CanHaveSkin(TypeID))
                        {
                            return true;
                        }
                        return false;
                    }
                case TITEMGROUP.ITEM_TYPE_SKIN:
                    {
                        if (ItemWarehouse.IsSkinExist(TypeID))
                        {
                        }
                        return true;
                    }
                case TITEMGROUP.ITEM_TYPE_MASCOT:
                    {
                        return true;
                    }
                case TITEMGROUP.ITEM_TYPE_CARD:
                    {
                        return true;
                    }

            }
            return false;
        }

        public bool SetMascotText(uint MascotIdx, string MascotText)
        {
            PlayerMascotData Mascot;
            Mascot = ItemMascot.GetMascotByIndex(MascotIdx);
            if (!(Mascot == null))
            {
                Mascot.SetText(MascotText);
                return true;
            }
            return false;
        }
        // caddie system
        public bool SetCaddieIndex(uint Index)
        {
            PlayerCaddieData Caddie;
            if (Index == 0)
            {
                CaddieIndex = 0;
                return true;
            }
            Caddie = ItemCaddie.GetCaddieByIndex(Index);
            if (Caddie == null)
            {
                return false;
            }
            CaddieIndex = Caddie.CaddieIdx;
            return true;
        }

        // mascot system
        public bool SetMascotIndex(uint Index)
        {
            PlayerMascotData Mascot;
            if (Index == 0)
            {
                MascotIndex = 0;
                return true;
            }
            Mascot = ItemMascot.GetMascotByIndex(Index);
            if (Mascot == null)
            {
                return false;
            }
            MascotIndex = Mascot.MascotIndex;
            return true;
        }

        public bool SetCharIndex(uint CharID)
        {
            PlayerCharacterData Char;
            Char = ItemCharacter.GetChar(CharID, CharType.bIndex);
            if (Char == null)
            {
                return false;
            }
            CharacterIndex = CharID;
            return true;
        }

        public bool SetBackgroudIndex(uint typeID)
        {
            var Get = ItemWarehouse.GetItem(typeID, 1);
            if (Get == null)
            {
                return false;
            }
            ItemDecoration.BackGroundTypeID = typeID;
            BackGroundIndex = Get.ItemIndex;
            return true;
        }

        public bool SetrameIndex(uint typeID)
        {
            var Get = ItemWarehouse.GetItem(typeID, 1);
            if (Get == null)
            {
                return false;
            }
            ItemDecoration.FrameTypeID = typeID;
            FrameIndex = Get.ItemIndex;
            return true;
        }


        public bool SetStickerIndex(uint typeID)
        {
            var Get = ItemWarehouse.GetItem(typeID, 1);
            if (Get == null)
            {
                return false;
            }
            ItemDecoration.StickerTypeID = typeID;
            StickerIndex = Get.ItemIndex;
            return true;
        }
        public bool SetSlotIndex(uint typeID)
        {
            var Get = ItemWarehouse.GetItem(typeID, 1);
            if (Get == null)
            {
                return false;
            }
            ItemDecoration.SlotTypeID = typeID;
            SlotIndex = Get.ItemIndex;
            return true;
        }

        public bool SetTitleIndex(uint ID)
        {
            var Get = ItemWarehouse.GetItem(ID);
            if (Get == null)
            {
                return false;
            }
            ItemDecoration.TitleTypeID = Get.ItemTypeID;
            TitleIndex = Get.ItemIndex;
            return true;
        }
        public bool SetDecoration(uint background, uint frame, uint sticker, uint slot, uint un, uint title)
        {
            if (SetBackgroudIndex(background) || SetrameIndex(frame) || SetStickerIndex(sticker) || SetSlotIndex(slot) || SetTitleIndex(title))
            {
                ItemDecoration.UnknownTypeID = un;
                return true;
            }
            return false;
        }

        // club system
        public bool SetClubSetIndex(uint Index)
        {
            PlayerItemData Club;
            Club = ItemWarehouse.GetItem(Index);
            if ((Club == null) || (!(GetItemGroup(Club.ItemTypeID) == 0x4)))
            {
                return false;
            }
            this.ClubSetIndex = Index;
            return true;
        }

        public bool SetGolfEQP(uint BallTypeID, uint ClubSetIndex)
        {
            return (this.SetBallTypeID(BallTypeID) || this.SetClubSetIndex(ClubSetIndex));
        }

        public bool SetBallTypeID(uint TypeID)
        {
            PlayerItemData Ball;
            Ball = ItemWarehouse.GetItem(TypeID, 1);
            if ((Ball == null) || (!(GetItemGroup(Ball.ItemTypeID) == 0x5)))
            {
                return false;
            }
            this.BallTypeID = TypeID;
            return true;
        }

        #endregion

        #region Methods UInt32

        [MethodImpl(MethodImplOptions.NoInlining)]
        public uint GetItemGroup(uint TypeID)
        {
            var result = (uint)Round((TypeID & 4227858432) / Pow(2.0, 26.0));

            return result;
        }

        public uint DaysBetween(DateTime? d1, DateTime d2)
        {
            TimeSpan span = d1.Value.Subtract(d2);
            return Convert.ToUInt32(span.Days);
        }

        public uint GetTitleTypeID()
        {
            return ItemDecoration.TitleTypeID;
        }
        public uint GetCharTypeID()
        {
            PlayerCharacterData CharInfo;
            CharInfo = ItemCharacter.GetChar(CharacterIndex, CharType.bIndex);
            if (!(CharInfo == null))
            {
                return CharInfo.TypeID;
            }
            return 0;
        }

        public uint GetCutinIndex()
        {
            PlayerCharacterData CharInfo;
            CharInfo = ItemCharacter.GetChar(CharacterIndex, CharType.bIndex);
            if (!(CharInfo == null))
            {
                return CharInfo.FCutinIndex;
            }
            return 0;
        }


        public uint GetMascotTypeID()
        {
            PlayerMascotData MascotInfo;
            MascotInfo = ItemMascot.GetMascotByIndex(MascotIndex);
            if (!(MascotInfo == null))
            {
                return MascotInfo.MascotTypeID;
            }
            return 0;
        }

        public uint GetQuantity(uint TypeId)
        {
            switch (GetPartGroup(TypeId))
            {
                case 5:
                case 6:
                    // Ball And Normal
                    return ItemWarehouse.GetQuantity(TypeId);
                default:
                    return 0;
            }
        }

        uint GetPartGroup(uint TypeID)
        {
            uint result;
            result = (uint)Round((TypeID & 4227858432) / Pow(2.0, 26.0));
            return result;
        }
        #endregion

        #region Methods GetItem
        public PlayerItemData GetUCC(uint ItemIdx)
        {
            foreach (PlayerItemData ItemUCC in ItemWarehouse)
            {
                if ((ItemUCC.ItemIndex == ItemIdx) && (ItemUCC.ItemUCCUnique.Length >= 8))
                {
                    return ItemUCC;
                }
            }
            return null;
        }

        //THIS IS USE OR UCC THAT ALREADY PAINTED
        public PlayerItemData GetUCC(uint TypeId, string UCC_UNIQUE, bool Status)
        {
            foreach (PlayerItemData ItemUCC in ItemWarehouse)
            {
                if ((ItemUCC.ItemTypeID == TypeId) && (ItemUCC.ItemUCCUnique == UCC_UNIQUE) && (ItemUCC.ItemUCCStatus == 1))
                {
                    return ItemUCC;
                }
            }
            return null;
        }

        //THIS IS USE OR UCC THAT ALREADY {NOT PAINTED}
        public PlayerItemData GetUCC(uint TypeID, string UCC_UNIQUE)
        {
            foreach (PlayerItemData ItemUCC in ItemWarehouse)
            {
                if ((ItemUCC.ItemTypeID == TypeID) && (ItemUCC.ItemUCCUnique == UCC_UNIQUE) && !(ItemUCC.ItemUCCStatus == 1))
                {
                    return ItemUCC;
                }
            }
            return null;
        }

        public PlayerCharacterData GetCharacter(uint TypeID)
        {
            PlayerCharacterData Character;
            Character = ItemCharacter.GetChar(TypeID, CharType.bTypeID);
            if (!(Character == null))
            {
                return Character;
            }
            return null;
        }
        #endregion

        #region Methods AddItems
        public AddData AddItem(AddItem ItemAddData)
        {
            Object TPlayerItemData;
            AddData Result;

            Result = new AddData() { Status = false };


            if (UID == 0)
            {
                return Result;
            }
            switch ((TITEMGROUP)GetPartGroup(ItemAddData.ItemIffId))
            {
                case TITEMGROUP.ITEM_TYPE_CHARACTER:
                    {
                        TPlayerItemData = ItemCharacter.GetChar(ItemAddData.ItemIffId, CharType.bTypeID);

                        if (TPlayerItemData == null)
                        {
                            return AddItemToDB(ItemAddData);
                        }

                        else if (!(TPlayerItemData != null))
                        {
                            Result.Status = true;
                            Result.ItemIndex = ((PlayerCharacterData)(TPlayerItemData)).Index;
                            Result.ItemTypeID = ((PlayerCharacterData)(TPlayerItemData)).TypeID;
                            Result.ItemOldQty = 1;
                            Result.ItemNewQty = 1;
                            Result.ItemUCCKey = string.Empty;
                            Result.ItemFlag = 0;
                            Result.ItemEndDate = null;

                            if (ItemAddData.Transaction)
                                ItemTransaction.AddChar(2, (PlayerCharacterData)TPlayerItemData);
                        }
                    }
                    break;
                case TITEMGROUP.ITEM_TYPE_HAIR_STYLE:
                    {
                        var IffHair = IffEntry.GetByHairColor(ItemAddData.ItemIffId);
                        var character = ItemCharacter.GetCharByType((byte)IffHair.CharType);
                        if (character != null)
                        {
                            character.HairColour = IffHair.HairColor;
                            character.Update(character);
                            Result.Status = true;
                            Result.ItemIndex = character.Index;
                            Result.ItemTypeID = ItemAddData.ItemIffId;
                            Result.ItemOldQty = 0;
                            Result.ItemNewQty = 1;
                            Result.ItemUCCKey = null;
                            Result.ItemFlag = 0;
                            Result.ItemEndDate = null;
                        }
                    }
                    break;
                case TITEMGROUP.ITEM_TYPE_PART:
                    {
                        return AddItemToDB(ItemAddData);
                    }
                case TITEMGROUP.ITEM_TYPE_CLUB:
                    {
                        return AddItemToDB(ItemAddData);
                    }
                case TITEMGROUP.ITEM_TYPE_AUX:
                case TITEMGROUP.ITEM_TYPE_BALL:
                case TITEMGROUP.ITEM_TYPE_USE:
                    {
                        TPlayerItemData = ItemWarehouse.GetItem(ItemAddData.ItemIffId, 1);
                        if (TPlayerItemData != null)
                        {

                            Result.Status = true;
                            Result.ItemIndex = ((PlayerItemData)(TPlayerItemData)).ItemIndex;
                            Result.ItemTypeID = ((PlayerItemData)(TPlayerItemData)).ItemTypeID;
                            Result.ItemOldQty = ((PlayerItemData)(TPlayerItemData)).ItemC0;
                            Result.ItemNewQty = ((PlayerItemData)(TPlayerItemData)).ItemC0 + ItemAddData.Quantity;
                            Result.ItemUCCKey = ((PlayerItemData)(TPlayerItemData)).ItemUCCUnique;
                            Result.ItemFlag = (byte)((PlayerItemData)(TPlayerItemData)).ItemFlag;
                            Result.ItemEndDate = null;
                            //Add
                            ((PlayerItemData)(TPlayerItemData)).AddQuantity(ItemAddData.Quantity);

                            if (ItemAddData.Transaction)
                            {
                                ItemTransaction.AddItem(0x02, (PlayerItemData)TPlayerItemData, ItemAddData.Quantity);
                            }
                        }

                        else if (TPlayerItemData == null)
                        {
                            return AddItemToDB(ItemAddData);
                        }
                    }
                    break;
                case TITEMGROUP.ITEM_TYPE_CADDIE:
                    {
                        return AddItemToDB(ItemAddData);
                    }
                case TITEMGROUP.ITEM_TYPE_CADDIE_ITEM:
                    {
                        TPlayerItemData = ItemCaddie.GetCaddieBySkinId(ItemAddData.ItemIffId);

                        if (!(TPlayerItemData == null))
                        {
                            ((PlayerCaddieData)(TPlayerItemData)).Update();
                            ((PlayerCaddieData)(TPlayerItemData)).UpdateCaddieSkin(ItemAddData.ItemIffId, ItemAddData.Day);
                            Result.Status = true;
                            Result.ItemIndex = ((PlayerCaddieData)(TPlayerItemData)).CaddieIdx;
                            Result.ItemTypeID = ((PlayerCaddieData)(TPlayerItemData)).CaddieSkin;
                            Result.ItemOldQty = 1;
                            Result.ItemNewQty = 1;
                            Result.ItemUCCKey = string.Empty;
                            Result.ItemFlag = 0;
                            Result.ItemEndDate = DateTime.Now.AddDays(ItemAddData.Day);
                        }
                    }
                    break;
                case TITEMGROUP.ITEM_TYPE_SKIN:
                    {
                        return AddItemToDB(ItemAddData);
                    }
                case TITEMGROUP.ITEM_TYPE_MASCOT:
                    {
                        TPlayerItemData = ItemMascot.GetMascotByTypeId(ItemAddData.ItemIffId);

                        if (TPlayerItemData != null)
                        {
                            ((PlayerMascotData)(TPlayerItemData)).AddDay(ItemAddData.Day);
                            Result.Status = true;
                            Result.ItemIndex = ((PlayerMascotData)(TPlayerItemData)).MascotIndex;
                            Result.ItemTypeID = ((PlayerMascotData)(TPlayerItemData)).MascotTypeID;
                            Result.ItemOldQty = 1;
                            Result.ItemNewQty = 1;
                            Result.ItemUCCKey = "";
                            Result.ItemFlag = 0;
                            Result.ItemEndDate = ((PlayerMascotData)(TPlayerItemData)).MascotEndDate;
                        }
                        else if (TPlayerItemData == null)
                        {
                            return AddItemToDB(ItemAddData);
                        }
                    }
                    break;

                case TITEMGROUP.ITEM_TYPE_CARD:
                    {
                        TPlayerItemData = ItemCard.GetCard(ItemAddData.ItemIffId, 1);

                        if (TPlayerItemData == null)
                        {
                            AddItemToDB(ItemAddData);
                        }
                        else if (TPlayerItemData != null)
                        {
                            Result.Status = true;
                            Result.ItemIndex = ((PlayerCardData)(TPlayerItemData)).CardIndex;
                            Result.ItemTypeID = ((PlayerCardData)(TPlayerItemData)).CardTypeID;
                            Result.ItemOldQty = ((PlayerCardData)(TPlayerItemData)).CardQuantity;
                            Result.ItemNewQty = ((PlayerCardData)(TPlayerItemData)).CardQuantity + ItemAddData.Quantity;
                            Result.ItemUCCKey = string.Empty;
                            Result.ItemFlag = 0;
                            Result.ItemEndDate = null;

                            ((PlayerCardData)(TPlayerItemData)).AddQuantity(ItemAddData.Quantity);

                            if (ItemAddData.Transaction)
                                ItemTransaction.AddCard(0x02, (PlayerCardData)TPlayerItemData, ItemAddData.Quantity);
                        }
                    }
                    break;
            }
            return Result;
        }

        public AddData AddRent(uint TypeID, ushort Day = 7)
        {
            object PRent;
            AddData Result;

            Result = new AddData() { Status = false };

            if (!(GetItemGroup(TypeID) == 2))
            {
                return Result;
            }
            var _db = new PangyaEntities();
            var Add = _db.ProcAddRent((int)UID, (int)TypeID, Day).ToList();
            if (Add.Count <= 0)
                return Result;
            foreach (var data in Add)
            {
                PRent = new PlayerItemData();

                ((PlayerItemData)(PRent)).ItemIndex = (uint)data.ITEM_INDEX;
                ((PlayerItemData)(PRent)).ItemTypeID = (uint)data.ITEM_TYPEID;
                ((PlayerItemData)(PRent)).ItemC0 = 0;
                ((PlayerItemData)(PRent)).ItemUCCUnique = string.Empty;
                ((PlayerItemData)(PRent)).CreateNewItem();
                ((PlayerItemData)(PRent)).ItemFlag = (byte)data.ITEM_FLAG;
                ((PlayerItemData)(PRent)).ItemEndDate = data.ITEM_DATE_END;
                ItemWarehouse.ItemAdd((PlayerItemData)(PRent));

                Result.Status = true;
                Result.ItemIndex = ((PlayerItemData)(PRent)).ItemIndex;
                Result.ItemTypeID = ((PlayerItemData)(PRent)).ItemTypeID;
                Result.ItemOldQty = 0;
                Result.ItemNewQty = 1;
                Result.ItemUCCKey = ((PlayerItemData)(PRent)).ItemUCCUnique;
                Result.ItemFlag = (byte)((PlayerItemData)(PRent)).ItemFlag;
                Result.ItemEndDate = ((PlayerItemData)(PRent)).ItemEndDate;
            }

            return Result;
        }

        public AddData AddItemToDB(AddItem ItemAddData)
        {
            Object TPlayerItemData;
            PlayerTransactionData Tran;
            AddData Result;

            Result = new AddData() { Status = false };
            var _db = new PangyaEntities();
            var additem = _db.ProcAddItem((int)UID, (int)ItemAddData.ItemIffId, (int)ItemAddData.Quantity, IfCompare<byte>(IffEntry.IsSelfDesign(ItemAddData.ItemIffId), 1, 0), IffEntry.GetItemTimeFlag(ItemAddData.ItemIffId, ItemAddData.Day), (int)ItemAddData.Day).ToList();
            if (additem.Count > 0)
            {
                var dbdata = additem.FirstOrDefault();
                
                Tran = new PlayerTransactionData() { Types = 2, Index = (uint)dbdata.IDX, TypeID =(uint)dbdata.iffTypeId, PreviousQuan = 0, NewQuan = (uint)dbdata.Quantity, UCC = dbdata.UCC_KEY };

                ItemTransaction.Add(Tran);
                try
                {
                    switch ((TITEMGROUP)GetPartGroup(ItemAddData.ItemIffId))
                    {
                        case TITEMGROUP.ITEM_TYPE_CHARACTER:
                            {
                                TPlayerItemData = new PlayerCharacterData();

                                ((PlayerCharacterData)(TPlayerItemData)).Index = (uint)dbdata.IDX;
                                ((PlayerCharacterData)(TPlayerItemData)).TypeID = (uint)dbdata.iffTypeId;
                                ((PlayerCharacterData)(TPlayerItemData)).HairColour = 0;
                                ((PlayerCharacterData)(TPlayerItemData)).GiftFlag = 0;
                                ItemCharacter.CharacterAdd((PlayerCharacterData)(TPlayerItemData));

                                CharacterIndex = (uint)dbdata.IDX;
                                Result = new AddData()
                                {
                                    Status = true,
                                    ItemIndex = ((PlayerCharacterData)(TPlayerItemData)).Index,
                                    ItemTypeID = ((PlayerCharacterData)(TPlayerItemData)).TypeID,
                                    ItemOldQty = 1,
                                    ItemNewQty = 1,
                                    ItemUCCKey = string.Empty,
                                    ItemFlag = 0,
                                    ItemEndDate = DateTime.MinValue,
                                };
                            }
                            break;

                        case TITEMGROUP.ITEM_TYPE_AUX:
                        case TITEMGROUP.ITEM_TYPE_PART:
                        case TITEMGROUP.ITEM_TYPE_CLUB:
                        case TITEMGROUP.ITEM_TYPE_BALL:
                        case TITEMGROUP.ITEM_TYPE_USE:
                            {
                                TPlayerItemData = new PlayerItemData();
                                ((PlayerItemData)(TPlayerItemData)).ItemIndex = (uint)dbdata.IDX;
                                ((PlayerItemData)(TPlayerItemData)).ItemTypeID = (uint)dbdata.iffTypeId;
                                ((PlayerItemData)(TPlayerItemData)).ItemC0 = (ushort)dbdata.Quantity;
                                ((PlayerItemData)(TPlayerItemData)).ItemUCCUnique = dbdata.UCC_KEY;
                                ((PlayerItemData)(TPlayerItemData)).CreateNewItem();
                                // Add to inventory list
                                ItemWarehouse.ItemAdd((PlayerItemData)(TPlayerItemData));
                                // Set the result data
                                Result = new AddData()
                                {
                                    Status = true,
                                    ItemIndex = ((PlayerItemData)(TPlayerItemData)).ItemIndex,
                                    ItemTypeID = ((PlayerItemData)(TPlayerItemData)).ItemTypeID,
                                    ItemOldQty = 0,
                                    ItemNewQty = ItemAddData.Quantity,
                                    ItemUCCKey = ((PlayerItemData)(TPlayerItemData)).ItemUCCUnique,
                                    ItemFlag = 0,
                                    ItemEndDate = null,
                                };
                            }
                            break;
                        case TITEMGROUP.ITEM_TYPE_CADDIE:
                            {
                                TPlayerItemData = new PlayerCaddieData();
                                ((PlayerCaddieData)(TPlayerItemData)).CaddieIdx = (uint)dbdata.IDX;
                                ((PlayerCaddieData)(TPlayerItemData)).CaddieTypeId = (uint)dbdata.iffTypeId;
                                ((PlayerCaddieData)(TPlayerItemData)).CaddieDateEnd = (DateTime)dbdata.END_DATE;
                                ((PlayerCaddieData)(TPlayerItemData)).CaddieAutoPay = 0;
                                ((PlayerCaddieData)(TPlayerItemData)).CaddieType = (byte)dbdata.Flag;
                                // Add caddie to inventory list
                                ItemCaddie.CadieAdd((PlayerCaddieData)(TPlayerItemData));
                                // set the result data
                                Result = new AddData()
                                {
                                    Status = true,
                                    ItemIndex = ((PlayerCaddieData)(TPlayerItemData)).CaddieIdx,
                                    ItemTypeID = ((PlayerCaddieData)(TPlayerItemData)).CaddieTypeId,
                                    ItemOldQty = 0,
                                    ItemNewQty = 1,
                                    ItemUCCKey = string.Empty,
                                    ItemFlag = ((PlayerCaddieData)(TPlayerItemData)).CaddieType,
                                    ItemEndDate = null,
                                };
                            }
                            break;
                        case TITEMGROUP.ITEM_TYPE_SKIN:
                            {
                                TPlayerItemData = new PlayerItemData();
                                ((PlayerItemData)(TPlayerItemData)).ItemIndex = (uint)dbdata.IDX;
                                ((PlayerItemData)(TPlayerItemData)).ItemTypeID = (uint)dbdata.iffTypeId;
                                ((PlayerItemData)(TPlayerItemData)).ItemC0 = (ushort)ItemAddData.Quantity;
                                ((PlayerItemData)(TPlayerItemData)).ItemUCCUnique = dbdata.UCC_KEY;
                                ((PlayerItemData)(TPlayerItemData)).ItemFlag = (byte)dbdata.Flag;
                                ((PlayerItemData)(TPlayerItemData)).ItemEndDate = dbdata.END_DATE;
                                ((PlayerItemData)(TPlayerItemData)).ItemIsValid = 1;
                                // Add to inventory list
                                ItemWarehouse.ItemAdd((PlayerItemData)(TPlayerItemData));
                                // Set the result data
                                Result = new AddData()
                                {
                                    Status = true,
                                    ItemIndex = ((PlayerItemData)(TPlayerItemData)).ItemIndex,
                                    ItemTypeID = ((PlayerItemData)(TPlayerItemData)).ItemTypeID,
                                    ItemOldQty = 0,
                                    ItemNewQty = ItemAddData.Quantity,
                                    ItemUCCKey = ((PlayerItemData)(TPlayerItemData)).ItemUCCUnique,
                                    ItemFlag = (byte)((PlayerItemData)(TPlayerItemData)).ItemFlag,
                                    ItemEndDate = ((PlayerItemData)(TPlayerItemData)).ItemEndDate,
                                };
                            }
                            break;
                        case TITEMGROUP.ITEM_TYPE_CARD:
                            {
                                TPlayerItemData = new PlayerCardData();
                                ((PlayerCardData)(TPlayerItemData)).CardIndex = (uint)dbdata.IDX;
                                ((PlayerCardData)(TPlayerItemData)).CardTypeID = (uint)dbdata.iffTypeId;
                                ((PlayerCardData)(TPlayerItemData)).CardQuantity = ItemAddData.Quantity;
                                ((PlayerCardData)(TPlayerItemData)).CardIsValid = 1;
                                ((PlayerCardData)(TPlayerItemData)).CardNeedUpdate = false;
                                // ## add to card
                                ItemCard.CardAdd((PlayerCardData)(TPlayerItemData));
                                // set the result data
                                Result = new AddData()
                                {
                                    Status = true,
                                    ItemIndex = ((PlayerCardData)(TPlayerItemData)).CardIndex,
                                    ItemTypeID = ((PlayerCardData)(TPlayerItemData)).CardTypeID,
                                    ItemOldQty = 0,
                                    ItemNewQty = ((PlayerCardData)(TPlayerItemData)).CardQuantity,
                                    ItemUCCKey = string.Empty,
                                    ItemFlag = 0,
                                    ItemEndDate = null,
                                };
                            }
                            break;
                        case TITEMGROUP.ITEM_TYPE_MASCOT:
                            {
                                TPlayerItemData = new PlayerMascotData();
                                ((PlayerMascotData)(TPlayerItemData)).MascotIndex = (uint)dbdata.IDX;
                                ((PlayerMascotData)(TPlayerItemData)).MascotTypeID = (uint)dbdata.iffTypeId;
                                ((PlayerMascotData)(TPlayerItemData)).MascotMessage = "Pangya !";
                                ((PlayerMascotData)(TPlayerItemData)).MascotIsValid = 1;
                                ((PlayerMascotData)(TPlayerItemData)).MascotNeedUpdate = false;
                                ((PlayerMascotData)(TPlayerItemData)).MascotEndDate = (DateTime)dbdata.END_DATE;

                                ((PlayerMascotData)(TPlayerItemData)).MascotDayToEnd = (ushort)(DaysBetween(dbdata.END_DATE, DateTime.Now));
                                // ## add to card
                                ItemMascot.MascotAdd((PlayerMascotData)(TPlayerItemData));
                                // set the result data
                                Result = new AddData()
                                {
                                    Status = true,
                                    ItemIndex = ((PlayerMascotData)(TPlayerItemData)).MascotIndex,
                                    ItemTypeID = ((PlayerMascotData)(TPlayerItemData)).MascotTypeID,
                                    ItemOldQty = 0,
                                    ItemNewQty = 1,
                                    ItemUCCKey = string.Empty,
                                    ItemFlag = 4,
                                    ItemEndDate = DateTime.Now.AddDays(ItemAddData.Day + 1),
                                };
                            }
                            break;
                    }
                }
                catch
                {
                    var player = (GPlayer)MainServer.Program._server.GetPlayerByUID(UID);
                    if (player != null)
                    {
                        player.Close();
                    }
                }
            }
            // ## resulted
            return Result;
        }
        #endregion

        #region RemoveItems
        public AddData Remove(uint ItemIffId, uint Quantity, bool Transaction = true)
        {
            AddData ItemDeletedData;
            PlayerItemData Items;
            PlayerCardData Cards;
            PlayerTransactionData Tran;
            ItemDeletedData = new AddData() { Status = false };
            if (UID <= 0)
            { return ItemDeletedData; }

            if (ItemIffId <= 0 && Quantity <= 0)
            { return ItemDeletedData; }


            switch ((TITEMGROUP)GetPartGroup(ItemIffId))
            {
                case TITEMGROUP.ITEM_TYPE_CLUB:
                case TITEMGROUP.ITEM_TYPE_USE:
                    {
                        Items = ItemWarehouse.GetItem(ItemIffId, Quantity);

                        if (!(Items == null))
                        {
                            ItemDeletedData.Status = true;
                            ItemDeletedData.ItemIndex = Items.ItemIndex;
                            ItemDeletedData.ItemTypeID = Items.ItemTypeID;
                            ItemDeletedData.ItemOldQty = Items.ItemC0;
                            ItemDeletedData.ItemNewQty = Items.ItemC0 - Quantity;
                            ItemDeletedData.ItemUCCKey = Items.ItemUCCUnique;
                            ItemDeletedData.ItemFlag = 0;
                            ItemDeletedData.ItemEndDate = null;
                            if (Transaction)
                            {
                                Tran = new PlayerTransactionData() { UCC = "", Types = 2, TypeID = Items.ItemTypeID, Index = Items.ItemIndex, PreviousQuan = Items.ItemC0, NewQuan = Items.ItemC0 - Quantity };
                                ItemTransaction.Add(Tran);
                            }

                            // update item info
                            Items.RemoveQuantity(Quantity);
                        }
                        return ItemDeletedData;
                    }
                case TITEMGROUP.ITEM_TYPE_CARD:
                    {
                        Cards = ItemCard.GetCard(ItemIffId, Quantity);

                        if (!(Cards == null))
                        {
                            ItemDeletedData.Status = true;
                            ItemDeletedData.ItemIndex = Cards.CardIndex;
                            ItemDeletedData.ItemTypeID = Cards.CardTypeID;
                            ItemDeletedData.ItemOldQty = Cards.CardQuantity;
                            ItemDeletedData.ItemNewQty = Cards.CardQuantity - Quantity;
                            ItemDeletedData.ItemUCCKey = string.Empty;
                            ItemDeletedData.ItemFlag = 0;
                            ItemDeletedData.ItemEndDate = null;
                            if (Transaction)
                            {
                                Tran = new PlayerTransactionData() { UCC = "", Types = 2, TypeID = Cards.CardTypeID, Index = Cards.CardIndex, PreviousQuan = Cards.CardQuantity, NewQuan = Cards.CardQuantity - Quantity };
                                ItemTransaction.Add(Tran);
                            }
                        }
                        // update item info
                        Cards.RemoveQuantity(Quantity);
                        return ItemDeletedData;
                    }
            }
            ItemDeletedData.SetData(false, 0, 0, 0, 0, string.Empty, 0, DateTime.Now);
            return (ItemDeletedData);
        }

        public AddData Remove(uint ItemIffId, uint Index, uint Quantity, bool Transaction = true)
        {
            AddData ItemDeletedData;
            PlayerItemData Items;
            PlayerCardData Cards;
            PlayerTransactionData Tran;
            ItemDeletedData = new AddData() { Status = false };
            if (UID <= 0)
            { return ItemDeletedData; }

            if (ItemIffId <= 0 && Quantity <= 0)
            { return ItemDeletedData; }


            switch ((TITEMGROUP)GetPartGroup(ItemIffId))
            {
                case TITEMGROUP.ITEM_TYPE_CLUB:
                case TITEMGROUP.ITEM_TYPE_USE:
                    {
                        Items = ItemWarehouse.GetItem(ItemIffId, Index, Quantity);

                        if (!(Items == null))
                        {
                            ItemDeletedData.Status = true;
                            ItemDeletedData.ItemIndex = Items.ItemIndex;
                            ItemDeletedData.ItemTypeID = Items.ItemTypeID;
                            ItemDeletedData.ItemOldQty = Items.ItemC0;
                            ItemDeletedData.ItemNewQty = Items.ItemC0 - Quantity;
                            ItemDeletedData.ItemUCCKey = Items.ItemUCCUnique;
                            ItemDeletedData.ItemFlag = 0;
                            ItemDeletedData.ItemEndDate = null;
                            if (Transaction)
                            {
                                Tran = new PlayerTransactionData() { UCC = "", Types = 2, TypeID = Items.ItemTypeID, Index = Items.ItemIndex, PreviousQuan = Items.ItemC0, NewQuan = Items.ItemC0 - Quantity };
                                ItemTransaction.Add(Tran);
                            }

                        }
                        // update item info
                        Items.RemoveQuantity(Quantity);
                        return ItemDeletedData;
                    }
                case TITEMGROUP.ITEM_TYPE_PART:
                    {
                        Items = ItemWarehouse.GetItem(ItemIffId, Index, 0); // ## part should be zero

                        if (!(Items == null))
                        {
                            ItemDeletedData.Status = true;
                            ItemDeletedData.ItemIndex = Items.ItemIndex;
                            ItemDeletedData.ItemTypeID = Items.ItemTypeID;
                            ItemDeletedData.ItemOldQty = 1;
                            ItemDeletedData.ItemNewQty = 0;
                            ItemDeletedData.ItemUCCKey = Items.ItemUCCUnique;
                            ItemDeletedData.ItemFlag = 0;
                            ItemDeletedData.ItemEndDate = null;
                            if (Transaction)
                            {
                                Tran = new PlayerTransactionData() { UCC = "", Types = 2, TypeID = Items.ItemTypeID, Index = Items.ItemIndex, PreviousQuan = 1, NewQuan = 0 };
                                ItemTransaction.Add(Tran);
                            }
                        }
                        // update item info
                        Items.RemoveQuantity(Quantity);
                        return ItemDeletedData;
                    }
                case TITEMGROUP.ITEM_TYPE_CARD:
                    {
                        Cards = ItemCard.GetCard(ItemIffId, Index, Quantity);

                        if (!(Cards == null))
                        {
                            ItemDeletedData.Status = true;
                            ItemDeletedData.ItemIndex = Cards.CardIndex;
                            ItemDeletedData.ItemTypeID = Cards.CardTypeID;
                            ItemDeletedData.ItemOldQty = Cards.CardQuantity;
                            ItemDeletedData.ItemNewQty = Cards.CardQuantity - Quantity;
                            ItemDeletedData.ItemUCCKey = string.Empty;
                            ItemDeletedData.ItemFlag = 0;
                            ItemDeletedData.ItemEndDate = null;
                            if (Transaction)
                            {
                                Tran = new PlayerTransactionData() { UCC = "", Types = 2, TypeID = Cards.CardTypeID, Index = Cards.CardIndex, PreviousQuan = Cards.CardQuantity, NewQuan = Cards.CardQuantity - Quantity };
                                ItemTransaction.Add(Tran);
                            }
                        }
                        // update item info
                        Cards.RemoveQuantity(Quantity);
                        return ItemDeletedData;
                    }
            }
            ItemDeletedData.SetData(false, 0, 0, 0, 0, string.Empty, 0, DateTime.Now);
            return (ItemDeletedData);
        }
        #endregion
    }
}
