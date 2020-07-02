using PangyaAPI.BinaryModels;
using Py_Connector.DataBase;
using Py_Game.Client.Inventory.Data.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.Client.Inventory.Collection
{
    public class CardCollection : List<PlayerCardData>
    {
        //Constructor  Create()
        public CardCollection(int PlayerUID)
        {
            Build(PlayerUID);
        }

        public int CardAdd(PlayerCardData Value)
        {
            Value.CardNeedUpdate = false;
            Add(Value);
            return Count;
        }     

        void Build(int UID)
        {
            var _db = new PangyaEntities();
            foreach (var info in _db.ProcGetCard(UID))
            {
                var card = new PlayerCardData()
                {
                    CardIndex = (uint)info.CARD_IDX,
                    CardTypeID = (uint)info.CARD_TYPEID,
                    CardQuantity = (uint)info.QTY,
                    CardIsValid = (byte)info.VALID,
                    CardNeedUpdate = false
                };
                this.Add(card);
            }
        }
        public byte[] Build()
        {
            PangyaBinaryWriter Packet;

            Packet = new PangyaBinaryWriter();
            try
            {
                Packet.Write(new byte[] { 0x38, 0x01 });
                Packet.WriteUInt32(0);
                Packet.WriteUInt16((ushort)Count);
                foreach (var Card in this)
                {
                    Packet.Write(Card.GetCardInfo());
                }
                return Packet.GetBytes();
            }
            finally
            {

            }
        }
        public PlayerCardData GetCard(UInt32 ID)
        {
            foreach (PlayerCardData Card in this)
            {
                if ((Card.CardIndex == ID) && (Card.CardQuantity >= 1) && (Card.CardIsValid == 1))
                {
                    return Card;
                }
            }
            return null;
        }

        public PlayerCardData GetCard(UInt32 ID, UInt32 Quantity)
        {
            foreach (PlayerCardData Card in this)
            {
                if ((Card.CardTypeID == ID) && (Card.CardQuantity >= Quantity) && (Card.CardIsValid == 1))
                {
                    return Card;
                }
            }
            return null;
        }

        public PlayerCardData GetCard(UInt32 TypeID, UInt32 Index, UInt32 Quantity)
        {
            foreach (PlayerCardData Card in this)
            {
                if ((Card.CardTypeID == TypeID) && (Card.CardIndex == Index) && (Card.CardQuantity >= Quantity) && (Card.CardIsValid == 1))
                {
                    return Card;
                }
            }
            return null;
        }

        public string GetSqlUpdateCard()
        {
            StringBuilder SQLString;
            SQLString = new StringBuilder();

            try
            {
                foreach (PlayerCardData Cards in this)
                {
                    if (Cards.CardNeedUpdate)
                    {
                        SQLString.Append(Cards.GetSqlUpdateString());
                        // ## set update to false when request string
                        Cards.CardNeedUpdate = false;

                    }
                }
                return SQLString.ToString();
            }
            finally
            {
                SQLString.Clear();
            }
        }

        public bool IsExist(UInt32 TypeID, UInt32 Index, UInt32 Quantity)
        {
            foreach (PlayerCardData Card in this)
            {
                if (Card.Exist(TypeID, Index, Quantity))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
