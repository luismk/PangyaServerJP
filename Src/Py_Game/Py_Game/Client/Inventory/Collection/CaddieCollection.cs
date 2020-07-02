using PangyaAPI.BinaryModels;
using Py_Connector.DataBase;
using Py_Game.Client.Inventory.Data.Caddie;
using System;
using System.Collections.Generic;
using System.Text;

namespace Py_Game.Client.Inventory.Collection
{
    public class CaddieCollection : List<PlayerCaddieData>
    {
        public CaddieCollection(int PlayerUID)
        {
            Build(PlayerUID);
        }
        // SerialPlayerCaddieData
        public int CadieAdd(PlayerCaddieData Value)
        {
            Value.CaddieNeedUpdate = false;
            Add(Value);
            return Count;
        }

        
        void Build(int UID)
        {
            var _db = new PangyaEntities();
            foreach (var info in _db.ProcGetCaddies(UID))
            {
                if (info.DAY_LEFT == null)
                    info.DAY_LEFT = 0;

                var SkinHour = (info.SKIN_HOUR_LEFT == null ? (ushort)0 : (ushort)info.SKIN_HOUR_LEFT);
                var caddie = new PlayerCaddieData()
                {
                    CaddieIdx = (uint)info.CID,
                    CaddieTypeId = (uint)info.TYPEID,
                    CaddieSkin = (uint)info.SKIN_TYPEID,
                    CaddieSkinEndDate = GameTools.TCompare.IfCompare(info.SKIN_END_DATE == null, DateTime.MinValue, info.SKIN_END_DATE),
                    CaddieLevel = info.cLevel,
                    CaddieExp = (uint)info.EXP,
                    CaddieType = (byte)info.RentFlag,
                    CaddieDay = (ushort)info.DAY_LEFT,//DIAS RESTANTES
                    CaddieSkinDay = SkinHour,
                    CaddieAutoPay = (ushort)info.TriggerPay,
                    CaddieDateEnd = (DateTime)info.END_DATE,
                    CaddieNeedUpdate = false
                };
                CadieAdd(caddie);
            }
        }


        public byte[] Build()
        {
            PangyaBinaryWriter Reply;

            using (Reply = new PangyaBinaryWriter())
            {
                Reply.Write(new byte[] { 0x71, 0x00 });
                Reply.WriteUInt16((ushort)Count);
                Reply.WriteUInt16((ushort)Count);
                foreach (PlayerCaddieData CaddieInfo in this)
                {
                    Reply.Write(CaddieInfo.GetCaddieInfo());
                }
                return Reply.GetBytes();
            }
        }

        public byte[] BuildExpiration()
        {
            PangyaBinaryWriter Reply;

            using (Reply = new PangyaBinaryWriter())
            {
                Reply.Write(new byte[] { 0xD4, 0x00 });
                foreach (PlayerCaddieData CaddieInfo in this)
                {
                    Reply.Write(CaddieInfo.GetExpirationNotice());
                }
                return Reply.GetBytes();
            }
        }
        public byte[] GetCaddie()
        {
            PangyaBinaryWriter Result;
            Result = new PangyaBinaryWriter();
            try
            {
                foreach (PlayerCaddieData CaddieInfo in this)
                {
                    Result.Write(CaddieInfo.GetCaddieInfo());
                }
                return Result.GetBytes();
            }
            finally
            {
                Result.Dispose();
            }
        }

        public bool IsExist(UInt32 TypeId)
        {
            foreach (PlayerCaddieData CaddieInfo in this)
            {
                if ((CaddieInfo.CaddieTypeId == TypeId))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanHaveSkin(UInt32 SkinTypeId)
        {
            foreach (PlayerCaddieData CaddieInfo in this)
            {
                if (CaddieInfo.Exist(SkinTypeId))
                {
                    return true;
                }
            }
            return false;
        }

        public PlayerCaddieData GetCaddieByIndex(UInt32 Index)
        {
            foreach (PlayerCaddieData CaddieInfo in this)
            {
                if (CaddieInfo.CaddieIdx == Index)
                {
                    return CaddieInfo;
                }
            }
            return null;
        }

        public PlayerCaddieData GetCaddieBySkinId(UInt32 SkinTypeId)
        {
            foreach (PlayerCaddieData CaddieInfo in this)
            {
                if (CaddieInfo.Exist(SkinTypeId))
                {
                    return CaddieInfo;
                }
            }
            return null;
        }


        public string GetSqlUpdateCaddie()
        {
            StringBuilder SQLString;
            SQLString = new StringBuilder();
            try
            {
                foreach (PlayerCaddieData CaddieInfo in this)
                {
                    if (CaddieInfo.CaddieNeedUpdate)
                    {
                        SQLString.Append(CaddieInfo.GetSQLUpdateString());
                        // update to false when get string
                        CaddieInfo.CaddieNeedUpdate = false;
                    }
                }
                return SQLString.ToString();
            }
            finally
            {
                SQLString = null;
            }
        }
    }
}
