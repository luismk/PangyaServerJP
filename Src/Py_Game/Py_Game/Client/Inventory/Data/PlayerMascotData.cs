using PangyaAPI.BinaryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Py_Game.GameTools.TCompare;
using static Py_Game.GameTools.Tools;
namespace Py_Game.Client.Inventory.Data.Mascot
{
    public class PlayerMascotData
    {
        public UInt32 MascotIndex { get; set; }
        public UInt32 MascotTypeID { get; set; }
        public string MascotMessage { get; set; }
        public DateTime MascotEndDate { get; set; }
        public ushort MascotDayToEnd { get; set; }
        public Byte MascotIsValid { get; set; }
        public bool MascotNeedUpdate { get; set; }

        // Mascots
        public void AddDay(UInt32 DayTotal)
        {
            this.MascotNeedUpdate = true;
            if ((this.MascotEndDate == DateTime.MinValue) || (this.MascotEndDate < DateTime.Now))
            {
                this.MascotEndDate = DateTime.Now.AddDays(Convert.ToDouble(DayTotal));
                return;
            }
            this.MascotEndDate = this.MascotEndDate.AddDays(Convert.ToDouble(DayTotal));

            Update();
        }

        public byte[] GetMascotInfo()
        {

            PangyaBinaryWriter Packet;
            Packet = new PangyaBinaryWriter();
            using (Packet = new PangyaBinaryWriter())
            {
                Packet.WriteUInt32(MascotIndex);
                Packet.WriteUInt32(MascotTypeID);
                Packet.WriteZero(5);
                Packet.WriteStr(MascotMessage, 16);
                Packet.WriteZero(14);
                Packet.WriteUInt16(MascotIsValid);
                Packet.Write(GetFixTime(MascotEndDate));
                Packet.WriteByte(0);
                var result = Packet.GetBytes();
                return result;
            }
        }

        public bool Update()
        {
            this.MascotNeedUpdate = true;
            return true;
        }
        public void SetText(string Text)
        {
            this.MascotMessage = Text;
            Update();
        }

        internal string GetSqlUpdateString()
        {
            StringBuilder SQLString;
            SQLString = new StringBuilder();
            try
            {
                SQLString.Append('^');
                SQLString.Append(MascotIndex);
                SQLString.Append('^');
                SQLString.Append(MascotTypeID);
                SQLString.Append('^');
                SQLString.Append(MascotMessage);
                SQLString.Append('^');
                SQLString.Append(MascotIsValid);
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
