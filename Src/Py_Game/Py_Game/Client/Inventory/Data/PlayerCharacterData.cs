using Py_Connector.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.Client.Inventory.Data.Character
{
    public class PlayerCharacterData
    {
        public UInt32 TypeID { get; set; }
        public UInt32 Index { get; set; }
        public UInt16 HairColour { get; set; }
        public UInt16 GiftFlag { get; set; }
        public UInt32[] EquipTypeID { get; set; } = new UInt32[24];
        public UInt32[] EquipIndex { get; set; } = new UInt32[24];
        public UInt32 AuxPart { get; set; }
        public UInt32 AuxPart2 { get; set; }
        public uint AuxPart3 { get; set; }
        public uint AuxPart4 { get; set; }
        public uint AuxPart5 { get; set; }
        public UInt32 FCutinIndex { get; set; }
        public Byte Power { get; set; }
        public Byte Control { get; set; }
        public Byte Impact { get; set; }
        public Byte Spin { get; set; }
        public Byte Curve { get; set; }
        public Byte MasteryPoint { get; set; }
        public bool NEEDUPDATE { get; set; }

        public bool UpgradeSlot(Byte Slot)
        {
            switch (Slot)
            {
                case 0:
                    this.Power += 1;
                    break;
                case 1:
                    this.Control += 1;
                    break;
                case 2:
                    this.Impact += 1;
                    break;
                case 3:
                    this.Spin += 1;
                    break;
                case 4:
                    this.Curve += 1;
                    break;
                default:
                    return false;
            }
            this.NEEDUPDATE = true;
            return true;
        }

        public bool DowngradeSlot(Byte Slot)
        {
            switch (Slot)
            {
                case 0:
                    if ((this.Power <= 0))
                    {
                        return false;
                    }
                    this.Power -= 1;
                    break;
                case 1:
                    if ((this.Control <= 0))
                    {
                        return false;
                    }
                    this.Control -= 1;
                    break;
                case 2:
                    if ((this.Impact <= 0))
                    {
                        return false;
                    }
                    this.Impact -= 1;
                    break;
                case 3:
                    if ((this.Spin <= 0))
                    {
                        return false;
                    }
                    this.Spin -= 1;
                    break;
                case 4:
                    if ((this.Curve <= 0))
                    {
                        return false;
                    }
                    this.Curve -= 1;
                    break;
            }
            this.NEEDUPDATE = true;
            return true;
        }

        public uint GetPangUpgrade(byte Slot)
        {
            const uint POWPANG = 2100, CONPANG = 1700, IMPPANG = 2400, SPINPANG = 1900, CURVPANG = 1900;

            switch (Slot)
            {
                case 0:
                    return ((this.Power * POWPANG) + POWPANG);
                case 1:
                    return ((this.Control * CONPANG) + CONPANG);
                case 2:
                    return ((this.Impact * IMPPANG) + IMPPANG);
                case 3:
                    return ((this.Spin * SPINPANG) + SPINPANG);
                case 4:
                    return ((this.Curve * CURVPANG) + CURVPANG);
            }
            return 0;
        }

        public void Update(PlayerCharacterData info)
        {
            HairColour = info.HairColour;
            Power = info.Power;
            Control = info.Control;
            Impact = info.Impact;
            Spin = info.Spin;
            Curve = info.Curve;
            FCutinIndex = info.FCutinIndex;
            EquipTypeID = info.EquipTypeID;
            EquipIndex = info.EquipIndex;
            AuxPart = info.AuxPart;
            AuxPart2 = info.AuxPart2;
            NEEDUPDATE = true;
        }

        public void SaveChar(uint UID)
        {
            StringBuilder SQLString;
            if (NEEDUPDATE)
            {
                SQLString = new StringBuilder();

                SQLString.Append('^');
                SQLString.Append(Index);
                SQLString.Append('^');
                SQLString.Append(Power);
                SQLString.Append('^');
                SQLString.Append(Control);
                SQLString.Append('^');
                SQLString.Append(Impact);
                SQLString.Append('^');
                SQLString.Append(Spin);
                SQLString.Append('^');
                SQLString.Append(Curve);
                SQLString.Append('^');
                SQLString.Append(FCutinIndex);
                SQLString.Append('^');
                SQLString.Append(HairColour);
                SQLString.Append('^');
                SQLString.Append(AuxPart);
                SQLString.Append('^');
                SQLString.Append(AuxPart2);
                var Table4 = $"Exec dbo.USP_SAVE_CHARACTER_STATS  @UID = '{(int)UID}', @ITEMSTR = '{SQLString}'";
                var _db = new PangyaEntities();

                _db.Database.SqlQuery<PangyaEntities>(Table4).FirstOrDefault();

                SQLString.Clear();
            }
        }

        internal string GetStringCharInfo()
        {
            StringBuilder SQLString;
            SQLString = new StringBuilder();
            try
            {
                SQLString.Append('^');
                SQLString.Append(Index);
                for (int i = 0; i <= 23; i++)
                {
                    SQLString.Append('^');
                    SQLString.Append(EquipTypeID[i]);
                }
                for (int i = 0; i <= 23; i++)
                {
                    SQLString.Append('^');
                    SQLString.Append(EquipIndex[i]);
                }
                SQLString.Append(',');

                return SQLString.ToString();
            }
            finally
            {
                SQLString = null;
            }
        }
    }
}
