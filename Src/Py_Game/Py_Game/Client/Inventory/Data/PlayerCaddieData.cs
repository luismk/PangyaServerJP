using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PangyaAPI.BinaryModels;
using static Py_Game.GameTools.Tools;
namespace Py_Game.Client.Inventory.Data.Caddie
{
    public class PlayerCaddieData
    {
        public UInt32 CaddieIdx { get; set; }
        public UInt32 CaddieTypeId { get; set; }
        public UInt32 CaddieSkin { get; set; }
        public DateTime? CaddieSkinEndDate { get; set; }
        public Byte CaddieLevel { get; set; }
        public UInt32 CaddieExp { get; set; }
        public Byte CaddieType { get; set; }
        public UInt16 CaddieDay { get; set; }
        public UInt16 CaddieSkinDay { get; set; }
        public Byte CaddieUnknown { get; set; }
        public UInt16 CaddieAutoPay { get; set; }
        public DateTime CaddieDateEnd { get; set; }
        public bool CaddieNeedUpdate { get; set; }
        // Caddies
        public byte[] GetCaddieInfo()
        {
            PangyaBinaryWriter Packet;
            Packet = new PangyaBinaryWriter();
            try
            {
                Packet.WriteUInt32(CaddieIdx);
                Packet.WriteUInt32(CaddieTypeId);
                Packet.WriteUInt32(CaddieSkin);
                Packet.WriteByte(CaddieLevel);
                Packet.WriteUInt32(CaddieExp);
                Packet.WriteByte(CaddieType);
                Packet.WriteUInt16(CaddieDay);
                Packet.WriteUInt16(CaddieSkinDay);
                Packet.WriteByte(0);
                Packet.WriteUInt16(CaddieAutoPay);
                return Packet.GetBytes();
            }
            finally
            {
                Packet.Dispose();
            }
        }

        public byte[] GetExpirationNotice()
        {
            using (var Packet = new PangyaBinaryWriter())
            {
                if (CaddieType == 2 && CaddieDay == 0 || CaddieDay >= 65435)
                {
                    Packet.WriteUInt32(1);
                    Packet.WriteUInt32(CaddieIdx);
                    Packet.WriteUInt32(CaddieTypeId);
                    Packet.WriteUInt32(CaddieSkin);
                    Packet.WriteByte(CaddieLevel);
                    Packet.WriteUInt32(CaddieExp);
                    Packet.WriteByte(CaddieType);
                    Packet.WriteUInt16(CaddieDay);
                    Packet.WriteUInt16(CaddieSkinDay);
                    Packet.WriteZero(0);
                    Packet.WriteUInt16(CaddieAutoPay);
                }
                return Packet.GetBytes();
            }
        }

        public string GetSQLUpdateString()
        {
            StringBuilder SQLString;
            SQLString = new StringBuilder();
            try
            {
                SQLString.Append('^');
                SQLString.Append(CaddieIdx);
                SQLString.Append('^');
                SQLString.Append(CaddieSkin);
                SQLString.Append('^');
                SQLString.Append(GetSQLTime(CaddieSkinEndDate));
                SQLString.Append('^');
                SQLString.Append(CaddieAutoPay);
                SQLString.Append(',');
                // close for next player
                return SQLString.ToString();
            }
            finally
            {
                SQLString = null;
            }
        }
        public bool Update()
        {
            CaddieNeedUpdate = true;

            return true;
        }
        public void UpdateCaddieSkin(UInt32 SkinTypeId, UInt32 Period)
        {

            CaddieSkin = SkinTypeId;
            if ((CaddieSkinEndDate == DateTime.MinValue) || (CaddieSkinEndDate < DateTime.Now))
            {
                CaddieSkinEndDate = DateTime.Now.AddDays(Convert.ToDouble(Period));
                return;
            }
            CaddieSkinEndDate = CaddieSkinEndDate.Value.AddDays(Convert.ToDouble(Period));
        }
        
        public bool Exist(uint SkinTypeId)
        {
            return (CaddieTypeId == GetCaddieTypeIDBySkinID(SkinTypeId));
        }
    }
}
