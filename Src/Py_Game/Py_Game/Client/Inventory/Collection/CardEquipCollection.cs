using PangyaAPI.BinaryModels;
using Py_Connector.DataBase;
using Py_Game.Client.Inventory.Data.CardEquip;
using Py_Game.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Py_Game.Client.Inventory.Collection
{
    public class CardEquipCollection : List<PlayerCardEquipData>
    {
        public CardEquipCollection(int PlayerUID)
        {
            Build(PlayerUID);
        }
        void Build(int UID)
        {
            var _db = new PangyaEntities();
            foreach (var info in _db.ProcGetCardEquip(UID))
            {
                var cardequip = new PlayerCardEquipData()
                {
                    ID = (uint)info.ID,
                    CID = (uint)info.CID,
                    CHAR_TYPEID = (uint)info.CHAR_TYPEID,
                    CARD_TYPEID = (uint)info.CARD_TYPEID,
                    SLOT = (byte)info.SLOT,
                    FLAG = (byte)info.FLAG,
                    REGDATE = (DateTime)info.REGDATE,
                    ENDDATE = (DateTime)info.ENDDATE,
                    VALID = 1,
                    NEEDUPDATE = false
                };

                AddCard(cardequip);
            }
        }

        public void AddCard(PlayerCardEquipData P)
        {
            this.Add(P);
        }

        public byte[] Build()
        {
            PangyaBinaryWriter result;
            result = new PangyaBinaryWriter();

            result.Write(new byte[] { 0x37, 0x01 });
            result.WriteUInt16((ushort)this.Count);
            foreach (PlayerCardEquipData C in this)
            {
                result.Write(C.CardEquipInfo());
            }
            return result.GetBytes();
        }

        // CHARACTER CARD
        public PlayerCardEquipData GetCard(UInt32 CID, UInt32 SLOT)
        {
            foreach (PlayerCardEquipData result in this)
            {
                if (result.CheckCard(CID, SLOT))
                {
                    return result;
                }
            }
            return null;
        }



        public Dictionary<bool, PlayerCardEquipData> UpdateCard(UInt32 UID, UInt32 CID, UInt32 CHARTYPEID, UInt32 CARDTYPEID, byte SLOT, byte FLAG, byte TIME)
        {
            PlayerCardEquipData UP;
            UP = null;
            foreach (PlayerCardEquipData P in this)
            {
                switch (FLAG)
                {
                    case 0:
                        if ((P.CID == CID) && (P.CHAR_TYPEID == CHARTYPEID) && (P.SLOT == SLOT) && (P.FLAG == 0) && (P.VALID == 1))
                        {
                            UP = P;
                            break;
                        }
                        break;
                    case 1:
                        if ((P.CID == CID) && (P.CARD_TYPEID == CARDTYPEID) && (P.SLOT == SLOT) && (P.FLAG == 1) && (P.ENDDATE > DateTime.Now) && (P.VALID == 1))
                        {
                            UP = P;
                            break;
                        }
                        break;
                }
            }
            if (UP == null)
            {
                try
                {

                    var _db = new PangyaEntities();
                    var card = _db.USP_ADD_CARD_EQUIP((int)UID, (int)CID, (int)CHARTYPEID, (int)CARDTYPEID, SLOT, FLAG, TIME).FirstOrDefault();
                    if (!(card.CODE == 0))
                    {
                        return new Dictionary<bool, PlayerCardEquipData>() { { false, null } };
                    }

                    this.Clear();
                    Build((int)UID);
                    return new Dictionary<bool, PlayerCardEquipData>() { { true, new PlayerCardEquipData()
                    {
                        ID = (uint)card.OUT_INDEX,
                        CID = (uint)card.CID,
                        CHAR_TYPEID = (uint)card.CHARTYPEID,
                        CARD_TYPEID = (uint)card.CARDTYPEID,
                        SLOT = (byte)card.SLOT,
                        REGDATE = card.REGDATE,
                        ENDDATE = (DateTime)card.ENDDATE,
                        FLAG = (byte)card.FLAG,
                        VALID = 1,
                        NEEDUPDATE = false
                    } } };
                }
                finally
                {
                }
            }
            else
            {
                UP.CARD_TYPEID = CARDTYPEID;
                UP.NEEDUPDATE = true;
                if (FLAG == 1)
                {
                    UP.ENDDATE = DateTime.Now.AddMinutes(TIME);
                }
            }
            return new Dictionary<bool, PlayerCardEquipData>() { { true, UP } };
        }

        public byte[] MapCard(UInt32 CID)
        {
            TPCards TC;
            PangyaBinaryWriter Packet;

            TC = new TPCards();
            foreach (var PC in this)
            {
                if (PC.CID == CID)
                {
                    TC.Card[PC.SLOT] = PC.CARD_TYPEID;
                }
            }
            Packet = new PangyaBinaryWriter();
            try
            {
                Packet.WriteUInt32(TC.Card[1]);
                Packet.WriteUInt32(TC.Card[2]);
                Packet.WriteUInt32(TC.Card[3]);
                Packet.WriteUInt32(TC.Card[4]);
                Packet.WriteUInt32(TC.Card[5]);
                Packet.WriteUInt32(TC.Card[6]);
                Packet.WriteUInt32(TC.Card[7]);
                Packet.WriteUInt32(TC.Card[8]);
                Packet.WriteUInt32(TC.Card[9]);
                Packet.WriteUInt32(TC.Card[10]);
                return Packet.GetBytes();
            }
            finally
            {
                Packet.Dispose();
            }
        }

        public string GetSqlUpdateCardEquip()
        {
            StringBuilder SQLString;
            SQLString = new StringBuilder();
            try
            {
                foreach (var P in this)
                {
                    if (P.NEEDUPDATE)
                    {
                        SQLString.Append(P.GetSqlUpdateString());

                        // ## set update to false when request string
                        P.NEEDUPDATE = false;
                    }
                }
                return SQLString.ToString();
            }
            finally
            {

                SQLString.Clear();
            }
        }
    }
}
