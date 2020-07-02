using PangyaAPI.BinaryModels;
using Py_Connector.DataBase;
using Py_Game.Client.Inventory.Data.Mascot;
using System;
using System.Collections.Generic;
using System.Text;

namespace Py_Game.Client.Inventory.Collection
{
    public class MascotCollection : List<PlayerMascotData>
    {
        public MascotCollection(int PlayerUID)
        {
            Build(PlayerUID);
        }
        // SerialPlayerMascotData
        public int MascotAdd(PlayerMascotData Value)
        {
            Value.MascotNeedUpdate = false;
            Add(Value);
            return Count;
        }
        
        void Build(int UID)
        {
            var _db = new PangyaEntities();
            foreach (var info in _db.ProcGetMascot(UID))
            {
                var mascot = new PlayerMascotData()
                {
                    MascotIndex = (uint)info.MID,
                    MascotTypeID = (uint)info.MASCOT_TYPEID,
                    MascotMessage = info.MESSAGE,
                    MascotEndDate = (DateTime)info.DateEnd,
                    MascotDayToEnd = (ushort)info.END_DATE_INT,//DIAS PARA FINALIZAR O MASCOT 
                    MascotIsValid = 1,
                    MascotNeedUpdate = false
                };
                this.MascotAdd(mascot);
            }
        }
        public byte[] Build()
        {
            PangyaBinaryWriter Packet;

            using (Packet = new PangyaBinaryWriter())
            {
                Packet.Write(new byte[] { 0xE1, 0x00 });
                Packet.WriteByte((byte)Count);
                foreach (var Mascot in this)
                {
                    Packet.Write(Mascot.GetMascotInfo());
                }
                return Packet.GetBytes();
            }

        }
        public PlayerMascotData GetMascotByIndex(UInt32 MascotIndex)
        {
            foreach (PlayerMascotData Mascot in this)
            {
                if ((Mascot.MascotIndex == MascotIndex) && (Mascot.MascotEndDate > DateTime.MinValue))
                {
                    return Mascot;
                }
            }
            return null;
        }

        public PlayerMascotData GetMascotByTypeId(UInt32 MascotTypeId)
        {
            foreach (PlayerMascotData Mascot in this)
            {
                if ((Mascot.MascotTypeID == MascotTypeId) && (Mascot.MascotEndDate > DateTime.Now))
                {
                    return Mascot;
                }
            }
            return null;
        }

        public bool MascotExist(UInt32 TypeId)
        {
            foreach (PlayerMascotData Mascot in this)
            {
                if ((Mascot.MascotTypeID == TypeId) && (Mascot.MascotEndDate > DateTime.Now))
                {
                    return true;
                }
            }
            return false;
        }

        public string GetSqlUpdateMascots()
        {

            StringBuilder SQLString;
            SQLString = new StringBuilder();
            try
            {
                foreach (var mascot in this)
                {
                    if (mascot.MascotNeedUpdate)
                    {
                        SQLString.Append(mascot.GetSqlUpdateString());
                        // ## set update to false when request string
                        mascot.MascotNeedUpdate = false;
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
