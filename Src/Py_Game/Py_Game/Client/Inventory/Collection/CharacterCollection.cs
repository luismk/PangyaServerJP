using PangyaAPI.BinaryModels;
using Py_Connector.DataBase;
using Py_Game.Defines;
using Py_Game.Client.Inventory.Data.Character;
using System;
using System.Collections.Generic;
using System.Text;

namespace Py_Game.Client.Inventory.Collection
{
    public class CharacterCollection : List<PlayerCharacterData>
    {
        public uint UID;
        CardEquipCollection fCard = null;
        public CardEquipCollection Card
        {
            get
            {
                return fCard;
            }
            set
            {
                fCard = value;
            }
        }


        public CharacterCollection(int PlayerUID)
        {
            UID = (uint)PlayerUID;
            Build(PlayerUID);
        }
        

        void Build(int UID)
        {
            var _db = new PangyaEntities();
            foreach (var info in _db.ProcGetCharacter(UID))
            {

                var character = new PlayerCharacterData()
                {
                    TypeID = (uint)info.TYPEID,
                    Index = (uint)info.CID,
                    HairColour = (ushort)info.HAIR_COLOR,
                    GiftFlag = (ushort)info.GIFT_FLAG,
                    Power = (byte)info.POWER,
                    Control = (byte)info.CONTROL,
                    Impact = (byte)info.IMPACT,
                    Spin = (byte)info.SPIN,
                    Curve = (byte)info.CURVE,
                    FCutinIndex = (uint)info.CUTIN,
                    NEEDUPDATE = false,
                    AuxPart = (uint)info.AuxPart,
                    AuxPart2 = (uint)info.AuxPart2,
                };
                for (int i = 0; i < 24; i++)
                {
                    var valorPropriedade = info.GetType().GetProperty($"PART_TYPEID_{i + 1}").GetValue(info, null);
                    character.EquipTypeID[i] = Convert.ToUInt32(valorPropriedade);
                }

                for (int i = 0; i < 24; i++)
                {
                    var valorPropriedade = info.GetType().GetProperty($"PART_IDX_{i + 1}").GetValue(info, null);
                    character.EquipIndex[i] = Convert.ToUInt32(valorPropriedade);
                }
                Add(character);
            }
        }

        public int CharacterAdd(PlayerCharacterData Value)
        {
            Value.NEEDUPDATE = true;
            foreach (var chars in this)
            {
                if (chars.AuxPart > 0 && chars.AuxPart2 > 0)
                {
                    Value.AuxPart = chars.AuxPart;
                    Value.AuxPart2 = chars.AuxPart;
                    break;
                }
            }
            Value = CharacterPartDefault(Value);
            Add(Value);
            return Count;
        }

        private PlayerCharacterData CharacterPartDefault(PlayerCharacterData character)
        {
            //var _db = new PangyaEntities();

            //foreach (var info in _db.Pangya_Character_Part_Default.Where(c=> c.Char_TypeID == character.TypeID).ToList())
            //{
            //    for (int i = 0; i < 24; i++)
            //    {
            //        var valorPropriedade = info.GetType().GetProperty($"Parts_{i + 1}").GetValue(info, null);
            //        character.EquipTypeID[i] = Convert.ToUInt32(valorPropriedade);
            //    }
            //}
            return character;
        }


        public void UpdateCharacter(PlayerCharacterData character)
        {
            foreach (var Char in this)
            {
                if (Char.Index == character.Index && Char.TypeID == character.TypeID)
                {
                    Char.Update(character);
                }
            }
        }

        public byte[] CreateChar(PlayerCharacterData CharData, byte[] CardMap)
        {
            PangyaBinaryWriter Packet;

            Packet = new PangyaBinaryWriter();
            try
            {
                Packet.Write(CharData.TypeID);
                Packet.Write(CharData.Index);
                Packet.Write(CharData.HairColour);
                Packet.Write(CharData.GiftFlag);
                for (var Index = 0; Index < 24; Index++)
                {
                    Packet.Write(CharData.EquipTypeID[Index]);
                }
                for (var Index = 0; Index < 24; Index++)
                {
                    Packet.Write(CharData.EquipIndex[Index]);
                }
                Packet.WriteZero(216);
                Packet.Write(CharData.AuxPart);// anel da esquerda
                Packet.Write(CharData.AuxPart2);// anel da direita
                Packet.Write(CharData.AuxPart3);
                Packet.Write(CharData.AuxPart4);
                Packet.Write(CharData.AuxPart5);
                Packet.WriteUInt32(CharData.FCutinIndex); // CUTIN IDX
                Packet.WriteZero(12);
                Packet.WriteByte(CharData.Power);
                Packet.WriteByte(CharData.Control);
                Packet.WriteByte(CharData.Impact);
                Packet.WriteByte(CharData.Spin);
                Packet.WriteByte(CharData.Curve);
                Packet.WriteInt32(CharData.MasteryPoint);
                Packet.Write(CardMap, 40);
                Packet.WriteUInt32(0);
                Packet.WriteUInt32(0);
                return Packet.GetBytes();
            }
            finally
            {
                Packet.Dispose();
            }
        }

        public PlayerCharacterData GetCharByType(byte charType)
        {
            switch ((CharTypeByHairColor)charType)
            {
                case CharTypeByHairColor.Nuri:
                    return GetChar(67108864, CharType.bTypeID);
                case CharTypeByHairColor.Hana:
                    return GetChar(67108865, CharType.bTypeID);
                case CharTypeByHairColor.Azer:
                    return GetChar(67108866, CharType.bTypeID);
                case CharTypeByHairColor.Cecilia:
                    return GetChar(67108867, CharType.bTypeID);
                case CharTypeByHairColor.Max:
                    return GetChar(67108868, CharType.bTypeID);
                case CharTypeByHairColor.Kooh:
                    return GetChar(67108869, CharType.bTypeID);
                case CharTypeByHairColor.Arin:
                    return GetChar(67108870, CharType.bTypeID);
                case CharTypeByHairColor.Kaz:
                    return GetChar(67108871, CharType.bTypeID);
                case CharTypeByHairColor.Lucia:
                    return GetChar(67108872, CharType.bTypeID);
                case CharTypeByHairColor.Nell:
                    return GetChar(67108873, CharType.bTypeID);
                case CharTypeByHairColor.Spika:
                    return GetChar(67108874, CharType.bTypeID);
                case CharTypeByHairColor.NR:
                    return GetChar(67108875, CharType.bTypeID);
                case CharTypeByHairColor.HR:
                    return GetChar(67108876, CharType.bTypeID);
                case CharTypeByHairColor.CR:
                    return GetChar(67108878, CharType.bTypeID);
            }
            return null;
        }

        public PlayerCharacterData GetChar(UInt32 ID, CharType GetType)
        {
            switch (GetType)
            {
                case CharType.bTypeID:
                    foreach (PlayerCharacterData Char in this)
                    {
                        if (Char.TypeID == ID)
                        {
                            return Char;
                        }
                    }
                    return null;
                case CharType.bIndex:
                    foreach (PlayerCharacterData Char in this)
                    {
                        if (Char.Index == ID)
                        {
                            return Char;
                        }
                    }
                    return null;
            }
            return null;
        }

        public byte[] GetCharData(UInt32 CID)
        {
            foreach (PlayerCharacterData Char in this)
            {
                if (Char.Index == CID)
                {
                    return CreateChar(Char, Card.MapCard(CID));
                }
            }
            return new byte[513];
        }

        public byte[] Build()
        {
            PangyaBinaryWriter Packet;
            Packet = new PangyaBinaryWriter();
            try
            {
                Packet.Write(new byte[] { 0x70, 0x00 });
                Packet.WriteUInt16((ushort)this.Count);
                Packet.WriteUInt16((ushort)this.Count);
                foreach (PlayerCharacterData Char in this)
                {
                    Packet.Write(CreateChar(Char, Card.MapCard(Char.Index)));
                }
                return Packet.GetBytes();
            }
            finally
            {
                Packet.Dispose();
            }
        }
        /// <summary>
        /// String usada para salvar dados do Character, Status + Equipamentos
        /// </summary>
        /// <param name="UID">Player UID</param>
        /// <returns></returns>
        public string GetSqlUpdateCharacter()
        {
            StringBuilder SQLString;
            SQLString = new StringBuilder();
            try
            {
                foreach (PlayerCharacterData Char in this)
                {
                    if (Char.NEEDUPDATE)
                    {
                        SQLString.Append(Char.GetStringCharInfo());//string com informações do equipmento do char  
                    }
                    Char.SaveChar(UID);
                    Char.NEEDUPDATE = false;//seta como falso, para não causa erros ao salvar item
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
