using PangyaAPI.BinaryModels;
using Py_Game.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Py_Game.GameTools.TCompare;
using static Py_Game.GameTools.Tools;
using static PangyaFileCore.IffBaseManager;
namespace Py_Game.Client.Inventory.Data.CardEquip
{
   public class PlayerCardEquipData
    {
        public uint ID { get; set; }
        public uint CID { get; set; }
        public uint CHAR_TYPEID { get; set; }
        public uint CARD_TYPEID { get; set; }
        public byte SLOT { get; set; }
        public byte FLAG { get; set; }
        public DateTime REGDATE { get; set; }
        public DateTime ENDDATE { get; set; }
        public byte VALID { get; set; }
        public bool NEEDUPDATE { get; set; }

        public bool CheckCard(uint ID, uint CardSlot)
        {
            return (CID == ID) && (SLOT == CardSlot) && (FLAG == 0) && (VALID == 1);
        }

        public UInt32 GetType(UInt32 TypeID)
        {
            switch (GetCardType(TypeID))
            {
                case CARDTYPE.tNormal:
                    return 0;
                case CARDTYPE.tCaddie:
                    return 1;
                case CARDTYPE.tNPC:
                    return 5;
                case CARDTYPE.tSpecial:
                    return 2;
                default:
                    return 0;
            }
        }

        internal byte[] CardEquipInfo()
        {
            PangyaBinaryWriter result;

            result = new PangyaBinaryWriter();
            var P = IffEntry.Card.GetSPCL(CARD_TYPEID);
            result.WriteUInt32(0);
            result.WriteUInt32(CARD_TYPEID);
            result.WriteUInt32(CHAR_TYPEID);
            result.WriteUInt32(CID);
            if (P != null)
            {
                result.WriteUInt32(P.First().Key);
                result.WriteUInt32(P.First().Value);
            }
            else
            {
                result.WriteUInt32(0);
                result.WriteUInt32(0);
            }
            result.WriteUInt32(SLOT);
            if (CID == 0)
            {
                result.WriteZero(0x10);
                result.WriteZero(0x10);
            }
            else
            {
                result.Write(GetFixTime(REGDATE), 0x10);
                result.Write(GetFixTime(ENDDATE), 0x10);
            }
            result.WriteUInt32(GetType(CARD_TYPEID));
            result.WriteByte(1);
            return result.GetBytes();
        }
        public string GetSqlUpdateString()
        {
            StringBuilder SQLString;
            SQLString = new StringBuilder();
            try
            {
                SQLString.Append('^');
                SQLString.Append(ID);
                SQLString.Append('^');
                SQLString.Append(CARD_TYPEID);
                SQLString.Append('^');
                SQLString.Append(ENDDATE);
                SQLString.Append('^');
                SQLString.Append(VALID);
                SQLString.Append(',');
                // close for next player
                return SQLString.ToString();
            }
            finally
            {
                SQLString = null;
            }
        }
    }
}
