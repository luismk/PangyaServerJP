using System;
using System.Linq;
using System.Text;
using Py_Connector.DataBase;
using Py_Game.Client.Inventory.Data.ItemDecoration;
using Py_Game.Client.Inventory.Data.Slot;

using Py_Game.Client.Inventory.Collection;
namespace Py_Game.Client.Inventory
{
    public partial class PlayerInventory
    {
        public ItemDecorationData ItemDecoration;
        public ItemSlotData ItemSlot { get; set; }
        public WarehouseCollection ItemWarehouse { get; set; }
        public CaddieCollection ItemCaddie { get; set; }
        public CharacterCollection ItemCharacter { get; set; }
        public MascotCollection ItemMascot { get; set; }
        public CardCollection ItemCard { get; set; }
        public CardEquipCollection ItemCardEquip { get; set; }
        public FurnitureCollection ItemRoom { get; set; }
        public TrophyCollection ItemTrophies { get; set; }
        public TrophySpecialCollection ItemTrophySpecial { get; set; }
        public TrophyGPCollection ItemTrophyGP { get; set; }
        public TransactionsCollection ItemTransaction { get; set; }
        public uint UID { get; set; }
        public uint CharacterIndex { get; set; }
        public uint CaddieIndex { get; set; }
        public uint MascotIndex { get; set; }
        public uint BallTypeID { get; set; }
        public uint ClubSetIndex { get; set; }
        public uint CutinIndex { get; set; }
        public uint TitleIndex { get; set; }
        public uint BackGroundIndex { get; set; }
        public uint FrameIndex { get; set; }
        public uint StickerIndex { get; set; }
        public uint SlotIndex { get; set; }
        public uint Poster1 { get; set; }
        public uint Poster2 { get; set; }
        public uint TranCount
        {
            get { return (ItemTransaction.Count); }
        }

        public PlayerInventory(UInt32 TUID)
        {
            UID = TUID;
            ItemCardEquip = new CardEquipCollection((int)UID);
            ItemCharacter = new CharacterCollection((int)UID);
            ItemMascot = new MascotCollection((int)UID);
            ItemWarehouse = new WarehouseCollection((int)UID);
            ItemCaddie = new CaddieCollection((int)UID);
            ItemCard = new CardCollection((int)UID);
            ItemTransaction = new TransactionsCollection();
            ItemRoom = new FurnitureCollection((int)UID);
            ItemSlot = new ItemSlotData();
            ItemDecoration = new ItemDecorationData();
            ItemTrophies = new TrophyCollection();
            ItemTrophyGP = new TrophyGPCollection();
            ItemTrophySpecial = new TrophySpecialCollection();
            ItemSlotData Items;
            var _db = new PangyaEntities();
            foreach (var info in _db.ProcGetToolbar((int)UID))
            {
                Items = new ItemSlotData
                {
                    Slot1 = (uint)info.ITEM_SLOT_1,
                    Slot2 = (uint)info.ITEM_SLOT_2,
                    Slot3 = (uint)info.ITEM_SLOT_3,
                    Slot4 = (uint)info.ITEM_SLOT_4,
                    Slot5 = (uint)info.ITEM_SLOT_5,
                    Slot6 = (uint)info.ITEM_SLOT_6,
                    Slot7 = (uint)info.ITEM_SLOT_7,
                    Slot8 = (uint)info.ITEM_SLOT_8,
                    Slot9 = (uint)info.ITEM_SLOT_9,
                    Slot10 = (uint)info.ITEM_SLOT_10,
                };

                ItemSlot.SetItemSlot(Items);
                SetTitleIndex((uint)info.Skin_1);
                SetCharIndex((uint)info.CHARACTER_ID);
                SetCaddieIndex((uint)info.CADDIE);
                SetBallTypeID((uint)info.BALL_ID);
                SetClubSetIndex((uint)info.CLUB_ID);
                SetMascotIndex((uint)info.MASCOT_ID);
                SetPoster((uint)info.POSTER_1, (uint)info.POSTER_2);
            }

            ItemCharacter.Card = ItemCardEquip;
        }
    
        // PlayerSave
        public void Save(PangyaEntities _db)
        {
            // # PLAYER TOOLBAR UPDATE
            _db.Database.SqlQuery<PangyaEntities>($"Exec dbo.USP_SAVE_TOOLBAR  @UID = '{UID}', @ITEMSTR = '{GetSqlUpdateToolbar()}'").FirstOrDefault();

            // #PLAYER ITEM UPDATE
            _db.Database.SqlQuery<PangyaEntities>($"Exec dbo.USP_SAVE_ITEM  @UID = '{UID}', @ITEMSTR = '{ItemWarehouse.GetSqlUpdateItems()}'").FirstOrDefault();

            // #PLAYER CADDIE UPDATE
            _db.Database.SqlQuery<PangyaEntities>($"Exec dbo.USP_SAVE_CADDIE  @UID = '{UID}', @ITEMSTR = '{ItemCaddie.GetSqlUpdateCaddie()}'").FirstOrDefault();

            // #PLAYER CHARACTER AND EQUIP UPDATE
            _db.Database.SqlQuery<PangyaEntities>($"Exec dbo.USP_SAVE_CHARACTER_EQUIP  @UID = '{UID}', @EQUIPSTR = '{ItemCharacter.GetSqlUpdateCharacter()}'").FirstOrDefault();

            // #PLAYER CARD UPDATE
            _db.Database.SqlQuery<PangyaEntities>($"Exec dbo.USP_SAVE_CARD  @UID = '{UID}', @ITEMSTR = '{ItemCard.GetSqlUpdateCard()}'").FirstOrDefault();

            // #PLAYER CARD EQUIP UPDATE
            _db.Database.SqlQuery<PangyaEntities>($"Exec dbo.USP_SAVE_CARD_EQUIP  @UID = '{UID}', @ITEMSTR = '{ItemCardEquip.GetSqlUpdateCardEquip()}'").FirstOrDefault();

            // #PLAYER MASCOT UPDATE
            _db.Database.SqlQuery<PangyaEntities>($"Exec dbo.USP_SAVE_MASCOT  @UID = '{UID}', @ITEMSTR = '{ItemMascot.GetSqlUpdateMascots()}'").FirstOrDefault();

            _db.Dispose();
        }

        public string GetSqlUpdateToolbar()
        {
            StringBuilder SQLString;
            SQLString = new StringBuilder();

            SQLString.Append('^');
            SQLString.Append(CharacterIndex);
            SQLString.Append('^');
            SQLString.Append(CaddieIndex);
            SQLString.Append('^');
            SQLString.Append(MascotIndex);
            SQLString.Append('^');
            SQLString.Append(BallTypeID);
            SQLString.Append('^');
            SQLString.Append(ClubSetIndex);
            SQLString.Append('^');
            SQLString.Append(ItemSlot.Slot1);
            SQLString.Append('^');
            SQLString.Append(ItemSlot.Slot2);
            SQLString.Append('^');
            SQLString.Append(ItemSlot.Slot3);
            SQLString.Append('^');
            SQLString.Append(ItemSlot.Slot4);
            SQLString.Append('^');
            SQLString.Append(ItemSlot.Slot5);
            SQLString.Append('^');
            SQLString.Append(ItemSlot.Slot6);
            SQLString.Append('^');
            SQLString.Append(ItemSlot.Slot7);
            SQLString.Append('^');
            SQLString.Append(ItemSlot.Slot8);
            SQLString.Append('^');
            SQLString.Append(ItemSlot.Slot9);
            SQLString.Append('^');
            SQLString.Append(ItemSlot.Slot10);
            SQLString.Append('^');
            SQLString.Append(ItemDecoration.BackGroundTypeID);
            SQLString.Append('^');
            SQLString.Append(ItemDecoration.FrameTypeID);
            SQLString.Append('^');
            SQLString.Append(ItemDecoration.StickerTypeID);
            SQLString.Append('^');
            SQLString.Append(ItemDecoration.SlotTypeID);
            SQLString.Append('^');
            SQLString.Append(ItemDecoration.UnknownTypeID);//is zero, for typeID unknown
            SQLString.Append('^');
            SQLString.Append(ItemDecoration.TitleTypeID);
            SQLString.Append(',');
            // close for next player
            return SQLString.ToString();
        }

    }
}