using System;
using System.Collections.Generic;
using PangyaAPI.BinaryModels;
using Py_Game.Client.Inventory.Data.Transactions;
using Py_Game.Client.Inventory.Data.Character;
using Py_Game.Client.Inventory.Data.Card;
using Py_Game.Client.Inventory.Data.Warehouse;
using System.Linq;

namespace Py_Game.Client.Inventory.Collection
{
    public class TransactionsCollection : List<PlayerTransactionData>
    {
        public new uint Count
        {
            get { return (uint)this.ToList().Count; }
        }

        public void AddInfo(PlayerTransactionData Tran)
        {
            this.Add(Tran);
        }
       
        public void AddChar(Byte ShowType, PlayerCharacterData Char)
        {
            PlayerTransactionData Tran;
            if ((Char == null))
            {
                return;
            }
            Tran = new PlayerTransactionData()
            {
                Types = ShowType,
                TypeID = Char.TypeID,
                Index = Char.Index,
                PreviousQuan = 0,
                NewQuan = 0,
                UCC = String.Empty
            };
            this.AddInfo(Tran);
        }

        public void AddItem(Byte ShowType, PlayerItemData Item, UInt32 Add)
        {
            PlayerTransactionData Tran;
            if ((Item == null))
            {
                return;
            }
            Tran = new PlayerTransactionData()
            {
                Types = ShowType,
                TypeID = Item.ItemTypeID,
                Index = Item.ItemIndex,
                PreviousQuan = Item.ItemC0 - Add,
                NewQuan = Item.ItemC0,
                UCC = string.Empty
            };
            this.AddInfo(Tran);
        }

    
        public void AddCard(Byte ShowType, PlayerCardData Card, UInt32 Add)
        {
            PlayerTransactionData Tran;
            if ((Card == null))
            {
                return;
            }
            Tran = new PlayerTransactionData()
            {
                Types = ShowType,
                TypeID = Card.CardTypeID,
                Index = Card.CardIndex,
                PreviousQuan = Card.CardQuantity - Add,
                NewQuan = Card.CardQuantity,
                UCC = string.Empty
            };
            this.AddInfo(Tran);
        }

        public void AddCharStatus(Byte ShowType, PlayerCharacterData Char)
        {
            PlayerTransactionData Tran;
            if ((Char == null))
            {
                return;
            }
            Tran = new PlayerTransactionData()
            {
                Types = ShowType,
                TypeID = Char.TypeID,
                Index = Char.Index,
                PreviousQuan = 0,
                NewQuan = 0,
                UCC = string.Empty,
                C0_SLOT = Char.Power,
                C1_SLOT = Char.Control,
                C2_SLOT = Char.Impact,
                C3_SLOT = Char.Spin,
                C4_SLOT = Char.Curve
            };
            this.AddInfo(Tran);
        }

        public void AddClubSystem(PlayerItemData Item)
        {
            PlayerTransactionData Tran;
            if ((Item == null))
            {
                return;
            }
            Tran = new PlayerTransactionData()
            {
                Types = 0xCC,
                TypeID = Item.ItemTypeID,
                Index = Item.ItemIndex,
                PreviousQuan = 0,
                NewQuan = 0,
                UCC = string.Empty,
                C0_SLOT = Item.ItemC0Slot,
                C1_SLOT = Item.ItemC1Slot,
                C2_SLOT = Item.ItemC2Slot,
                C3_SLOT = Item.ItemC3Slot,
                C4_SLOT = Item.ItemC4Slot,
                ClubPoint = (uint)Item.ItemClubPoint,
                WorkshopCount = (uint)Item.ItemClubWorkCount,
                CancelledCount = (uint)Item.ItemClubSlotCancelledCount
            };
            this.AddInfo(Tran);
        }


        public byte[] GetTran()
        {
            PangyaBinaryWriter result;
            PlayerTransactionData Tran;

            result = new PangyaBinaryWriter();

            result.Write(new byte[] { 0x16, 0x02 });
            result.WriteInt32((new Random()).Next());//number random
            result.WriteUInt32(this.Count);
            foreach (PlayerTransactionData TranObj in this)
            {
                if (!(TranObj is PlayerTransactionData))
                {
                    continue;
                }
                Tran = TranObj;
                result.Write(Tran.GetInfoData());
            }
            this.Clear();
            return result.GetBytes();
        }
    }
}
