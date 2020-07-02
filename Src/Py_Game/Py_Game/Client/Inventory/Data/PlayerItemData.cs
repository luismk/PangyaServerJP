using Py_Game.Defines;
using System;
using System.Text;
using static Py_Game.GameTools.Tools;
using static Py_Game.GameTools.TCompare;
using PangyaAPI.BinaryModels;
using Py_Connector.DataBase;
using static PangyaFileCore.IffBaseManager;
using static Py_Game.Data.ClubData;
using Py_Game.Client.Data;

namespace Py_Game.Client.Inventory.Data.Warehouse
{
    public class PlayerItemData
    {
        public UInt32 ItemIndex { get; set; }

        public UInt32 ItemTypeID { get; set; }

        public UInt16 ItemC0 { get; set; }

        public UInt16 ItemC1 { get; set; }

        public UInt16 ItemC2 { get; set; }

        public UInt16 ItemC3 { get; set; }

        public UInt16 ItemC4 { get; set; }

        public string ItemUCCUnique { get; set; }

        public string ItemUCCName { get; set; }

        public byte? ItemUCCStatus { get; set; }
        public string ItemUCCDrawer { get; set; }

        public UInt32? ItemUCCDrawerUID { get; set; }

        public UInt16? ItemUCCCopyCount { get; set; }

        public UInt32? ItemClubPoint { get; set; }

        public UInt32? ItemClubWorkCount { get; set; }

        public UInt32? ItemClubPointLog { get; set; }

        public UInt32? ItemClubPangLog { get; set; }

        public UInt16 ItemC0Slot { get; set; }

        public UInt16 ItemC1Slot { get; set; }

        public UInt16 ItemC2Slot { get; set; }

        public UInt16 ItemC3Slot { get; set; }

        public UInt16 ItemC4Slot { get; set; }

        public UInt32? ItemClubSlotCancelledCount { get; set; }

        public Byte ItemGroup { get; set; }

        public byte ItemIsValid { get; set; }

        public byte? ItemFlag { get; set; }

        public uint ItemHourLeft { get; set; }
        public DateTime? ItemEndDate { get; set; }

        public DateTime? ItemRegDate { get; set; }

        public bool ItemNeedUpdate { get; set; }

        public PlayerItemData()
        {

        }

        public PlayerItemData(ProcGetItemWarehouse_Result info)
        {
            if (info.HOURLEFT == null)
            {
                info.HOURLEFT = -1;
            }
            if (info.C0_SLOT == null)
            {
                info.C0_SLOT = 0;
                info.C1_SLOT = 0;
                info.C2_SLOT = 0;
                info.C3_SLOT = 0;
                info.C4_SLOT = 0;
                info.CLUB_SLOT_CANCEL = 0;
                info.CLUB_WORK_COUNT = 0;
            }

            ItemIndex = (uint)info.IDX;
            ItemTypeID = (uint)info.TYPEID;
            ItemC0 = IfCompare<ushort>(info.C0 == null, 0, (ushort)info.C0);
            ItemC1 = IfCompare<ushort>(info.C1 == null, 0, (ushort)info.C1);
            ItemC2 = IfCompare<ushort>(info.C2 == null, 0, (ushort)info.C2);
            ItemC3 = IfCompare<ushort>(info.C3 == null, 0, (ushort)info.C3);
            ItemC4 = IfCompare<ushort>(info.C4 == null, 0, (ushort)info.C4);
            ItemUCCUnique = info.UCC_UNIQE;
            ItemUCCStatus = info.UCC_STATUS;
            ItemUCCDrawer = info.UCC_DRAWER;
            ItemUCCDrawerUID = (uint?)info.UCC_DRAWER_UID;
            ItemUCCName = info.UCC_NAME;
            ItemUCCCopyCount = (ushort?)info.UCC_COPY_COUNT;
            ItemClubPoint = (uint?)info.CLUB_POINT;
            ItemClubWorkCount = (uint?)info.CLUB_WORK_COUNT;
            ItemClubPointLog = (uint?)info.CLUB_POINT_TOTAL_LOG;
            ItemClubPangLog = (uint?)info.CLUB_UPGRADE_PANG_LOG;
            ItemC0Slot = (ushort)info.C0_SLOT;
            ItemC1Slot = (ushort)info.C1_SLOT;
            ItemC2Slot = (ushort)info.C2_SLOT;
            ItemC3Slot = (ushort)info.C3_SLOT;
            ItemC4Slot = (ushort)info.C4_SLOT;
            ItemClubSlotCancelledCount = (uint?)info.CLUB_SLOT_CANCEL;
            ItemGroup = (byte)GetItemGroup((uint)info.TYPEID);
            ItemFlag = info.Flag;
            ItemRegDate = info.RegDate;
            ItemEndDate = info.DateEnd;
            ItemIsValid = 1;
            ItemHourLeft = (uint)info.HOURLEFT;     
        }

        public bool AddClubPoint(uint amount)
        {
            if (!(ItemClubPoint > 99999)) return false;
            ItemClubPoint += amount;
            Update();
            return true;
        }

        public bool Update(PlayerItemData item)
        {
            this.ItemC1 = item.ItemC1;
            this.ItemC2 = item.ItemC2;
            this.ItemC3 = item.ItemC3;
            this.ItemC4 = item.ItemC4;
            this.ItemUCCName = item.ItemUCCName;
            this.ItemUCCStatus = item.ItemUCCStatus;
            this.ItemUCCDrawer = item.ItemUCCDrawer;
            this.ItemUCCDrawerUID = item.ItemUCCDrawerUID;
            this.ItemClubPoint = item.ItemClubPoint;
            this.ItemClubWorkCount = item.ItemClubWorkCount;
            this.ItemClubPointLog = item.ItemClubPointLog;
            this.ItemClubPangLog = item.ItemClubPangLog;
            this.ItemC0Slot = item.ItemC0Slot;
            this.ItemC1Slot = item.ItemC1Slot;
            this.ItemC2Slot = item.ItemC2Slot;
            this.ItemC3Slot = item.ItemC3Slot;
            this.ItemC4Slot = item.ItemC4Slot;
            this.ItemClubSlotCancelledCount = item.ItemClubSlotCancelledCount;
            return true;
        }

        public bool ClubAddSlot(byte AddType, byte Count = 1)
        {
            if (!(GetItemGroup(ItemTypeID) == 4)) return false;

            ItemClubWorkCount += 1;
            switch (AddType)
            {
                case 0:
                    {
                        ItemC0Slot++;
                        Count++;
                    }
                    break;
                case 1:
                    {
                        ItemC1Slot++;
                        Count++;
                    }
                    break;
                case 2:
                    {
                        ItemC2Slot++;
                        Count++;
                    }
                    break;
                case 3:
                    {
                        ItemC3Slot++;
                        Count++;
                    }
                    break;
                case 4:
                    {
                        ItemC0Slot++;
                        Count++;
                    }
                    break;
            }
            Update();
            return true;
        }

        public bool RemoveClubPoint(uint amount)
        {
            if (ItemClubPoint < amount) return false;

            ItemClubPoint -= amount;
            Update();
            return true;
        }

        public bool RemoveClubSlot(byte RemoveType, byte Count = 1)
        {
            if (!(GetItemGroup(ItemTypeID) == 4)) return false;

            ItemClubWorkCount--;

            if (ItemClubSlotCancelledCount >= 5)
            {
                return false;
            }
            ItemClubSlotCancelledCount++;

            switch (RemoveType)
            {
                case 0:
                    {
                        ItemC0Slot -= Count;
                    }
                    break;
                case 1:
                    {
                        ItemC1Slot -= Count;
                    }
                    break;
                case 2:
                    {
                        ItemC2Slot -= Count;
                    }
                    break;
                case 3:
                    {
                        ItemC3Slot -= Count;
                    }
                    break;
                case 4:
                    {
                        ItemC0Slot -= Count;
                    }
                    break;
            }
            Update();
            return true;
        }

        public bool AddQuantity(uint Value)
        {
            ItemC0 += (ushort)Value;

            Update();
            return true;
        }

        public bool ClubAddStatus(TCLUB_STATUS Slot)
        {
            Update();
            switch (Slot)
            {
                case TCLUB_STATUS.csPower:
                    {
                        ItemC0Slot += 1;
                    }
                    break;
                case TCLUB_STATUS.csControl:
                    {
                        ItemC1Slot += 1;
                    }
                    break;
                case TCLUB_STATUS.csImpact:
                    {
                        ItemC2Slot += 1;
                    }
                    break;
                case TCLUB_STATUS.csSpin:
                    {
                        ItemC3Slot += 1;
                    }
                    break;
                case TCLUB_STATUS.csCurve:
                    {
                        ItemC0Slot += 1;
                    }
                    break;
            }
            return true;
        }

        public bool ClubRemoveStatus(TCLUB_STATUS Slot)
        {
            Update();
            switch (Slot)
            {
                case TCLUB_STATUS.csPower:
                    {
                        if (ItemC0 > 0)
                        {
                            ItemC0 -= 1;
                            return true;
                        }
                        return false;
                    }
                case TCLUB_STATUS.csControl:
                    {
                        if (ItemC1 > 0)
                        {
                            ItemC1 -= 1;
                            return true;
                        }
                        return false;
                    }
                case TCLUB_STATUS.csImpact:
                    {
                        if (ItemC2 > 0)
                        {
                            ItemC2 -= 1;
                            return true;
                        }
                        return false;
                    }
                case TCLUB_STATUS.csSpin:
                    {
                        if (ItemC3 > 0)
                        {
                            ItemC3 -= 1;
                            return true;
                        }
                        return false;
                    }
                case TCLUB_STATUS.csCurve:
                    {
                        if (ItemC4 > 0)
                        {
                            ItemC4 -= 1;
                            return true;
                        }
                        return false;
                    }
            }
            return true;
        }

        public bool ClubSetCanReset()
        {
            if (!(ItemGroup == (byte)TITEMGROUP.ITEM_TYPE_CLUB)) return false;

            return true;
        }

        public bool ClubSetReset()
        {
            if (!(ItemGroup == (byte)TITEMGROUP.ITEM_TYPE_CLUB)) return false;

            ItemC0 = 0;
            ItemC1 = 0;
            ItemC2 = 0;
            ItemC3 = 0;
            ItemC4 = 0;

            ItemC0Slot = 0;
            ItemC1Slot = 0;
            ItemC2Slot = 0;
            ItemC3Slot = 0;
            ItemC4Slot = 0;

            ItemClubSlotCancelledCount = 0;

            ItemClubPointLog = 0;
            ItemClubPangLog = 0;
            Update();
            return true;
        }

        public TClubUpgradeData ClubSlotAvailable(TCLUB_STATUS Slot)
        {
            const uint Power = 2100, Con = 1700, Impact = 2400, Spin = 1900, Curve = 1900;


            var ClubData = GetClubMaxStatus(ItemTypeID);

            switch (Slot)
            {
                case TCLUB_STATUS.csPower:
                    {
                        if (ItemC0 < (ClubData.Power + this.ItemC0Slot))
                        {
                            var result = new TClubUpgradeData()
                            {
                                Able = true,
                                Pang = (ItemC0 * Power) + Power
                            };
                            return result;
                        }
                    }
                    break;
                case TCLUB_STATUS.csControl:
                    {
                        if (ItemC1 < (ClubData.Control + ItemC1Slot))
                        {
                            var result = new TClubUpgradeData()
                            {
                                Able = true,
                                Pang = (ItemC1 * Con) + Con
                            };
                            return result;
                        }
                    }
                    break;
                case TCLUB_STATUS.csImpact:
                    {
                        if (ItemC2 < (ClubData.Impact + ItemC2))
                        {
                            var result = new TClubUpgradeData()
                            {
                                Able = true,
                                Pang = (ItemC2 * Impact) + Impact
                            };
                            return result;
                        }
                    }
                    break;
                case TCLUB_STATUS.csSpin:
                    {
                        if (ItemC3 < (ClubData.Spin + ItemC3))
                        {
                            var result = new TClubUpgradeData()
                            {
                                Able = true,
                                Pang = (ItemC3 * Spin) + Spin
                            };
                            return result;
                        }
                    }
                    break;
                case TCLUB_STATUS.csCurve:
                    {
                        if (ItemC4 < (ClubData.Curve + ItemC4))
                        {
                            var result = new TClubUpgradeData()
                            {
                                Able = true,
                                Pang = (ItemC4 * Curve) + Curve
                            };
                            return result;
                        }
                    }
                    break;
            }

            return new TClubUpgradeData();
        }


        public void CreateNewItem()
        {
            this.ItemC1 = 0;
            this.ItemC2 = 0;
            this.ItemC3 = 0;
            this.ItemC4 = 0;
            this.ItemUCCName = string.Empty;
            this.ItemUCCStatus = 0;
            this.ItemUCCDrawer = string.Empty;
            this.ItemUCCDrawerUID = 0;
            this.ItemClubPoint = 0;
            this.ItemClubWorkCount = 0;
            this.ItemClubPointLog = 0;
            this.ItemClubPangLog = 0;
            this.ItemC0Slot = 0;
            this.ItemC1Slot = 0;
            this.ItemC2Slot = 0;
            this.ItemC3Slot = 0;
            this.ItemC4Slot = 0;
            this.ItemClubSlotCancelledCount = 0;
            this.ItemNeedUpdate = false;
            this.ItemIsValid = 1;
            this.ItemNeedUpdate = false;
            this.ItemFlag = 0;
            this.ItemEndDate = DateTime.Now;
            switch (GetItemGroup(this.ItemTypeID))
            {
                case 4:
                case 2:
                    {
                        this.ItemC0 = 0;
                    }
                    break;
            }
        }

        public void DeleteItem()
        {
            this.ItemIsValid = 0;
            this.ItemNeedUpdate = true;
        }

        public byte[] GetClubInfo()
        {
            var ClubData = GetClubMaxStatus(ItemTypeID);
            var Packet = new PangyaBinaryWriter();

            try
            {
                Packet.WriteUInt32(ItemIndex);
                Packet.WriteUInt32(ItemTypeID);
                Packet.WriteUInt16(ItemC0);
                Packet.WriteUInt16(ItemC1);
                Packet.WriteUInt16(ItemC2);
                Packet.WriteUInt16(ItemC3);
                Packet.WriteUInt16(ItemC4);
                Packet.WriteUInt16(Convert.ToUInt16(ClubData.Power + this.ItemC0Slot));
                Packet.WriteUInt16(Convert.ToUInt16(ClubData.Control + this.ItemC1Slot));
                Packet.WriteUInt16(Convert.ToUInt16(ClubData.Impact + this.ItemC2Slot));
                Packet.WriteUInt16(Convert.ToUInt16(ClubData.Spin + this.ItemC3Slot));
                Packet.WriteUInt16(Convert.ToUInt16(ClubData.Curve + ItemC4Slot));
                var GetBytes = Packet.GetBytes();
                return GetBytes;
            }
            finally
            {
                Packet.Dispose();
            }
        }

        public uint GetClubPoint()
        {
            return (uint)this.ItemClubPoint;
        }

        public ClubStatus GetClubSlotStatus()
        {
            ClubStatus Result = new ClubStatus();
            var club = IffEntry.Club.GetItem(ItemTypeID);
            if (club.Base.TypeID == ItemTypeID)
            {
                if (ItemC0Slot == 0)
                {
                    Result.Power = club.C0;
                }
                if (ItemC1Slot == 0)
                {
                    Result.Control = club.C1;
                }
                if (ItemC2Slot == 0)
                {
                    Result.Impact = club.C2;
                }
                if (ItemC3Slot == 0)
                {
                    Result.Spin = club.C3;
                }
                if (ItemC4Slot == 0)
                {
                    Result.Curve = club.C4;
                }
                Result.ClubType = (ECLUBTYPE)club.ClubType;
            }
            else
            {
                Result.Power = this.ItemC0Slot;
                Result.Control = this.ItemC1Slot;
                Result.Impact = this.ItemC2Slot;
                Result.Spin = this.ItemC3Slot;
                Result.Curve = this.ItemC4Slot;
            }
            return Result;
        }

        public byte[] GetItems()
        {
            using (var result = new PangyaBinaryWriter())
            {
                result.Write(ItemIndex);
                result.Write(ItemTypeID);
                result.Write(ItemHourLeft);
                if ((TITEMGROUP)ItemGroup == TITEMGROUP.ITEM_TYPE_CLUB)
                {
                    result.Write(ItemC0Slot);
                    result.Write(ItemC1Slot);
                    result.Write(ItemC2Slot);
                    result.Write(ItemC3Slot);
                    result.Write(ItemC4Slot);//22
                    result.Write((byte)0);
                    result.Write(ItemFlag ?? 0);
                    result.Write(UnixTimeConvert(ItemRegDate)); //UNIXTIME campo RegDate
                    result.Write(UnixTimeConvert(ItemEndDate)); //UNIXTIME campo EndDate
                    result.Write((byte)2);
                    result.WriteStr(IsUCCNull(ItemUCCName), 16);
                    result.WriteZero(25);
                    result.WriteStr(IsUCCNull(ItemUCCUnique), 9);//chave do SD
                    result.Write(ItemUCCStatus ?? 0);
                    result.Write(ItemUCCCopyCount ?? 0);
                    result.WriteStr(IsUCCNull(ItemUCCDrawer), 16);
                    result.WriteZero(70);//70
                    result.Write(Convert.ToUInt32(ItemClubPoint ?? 0));//74
                    result.Write(Convert.ToUInt32(ItemClubSlotCancelledCount ?? 0));// 78
                    result.Write((long)ItemClubWorkCount);//86
                }
                else
                {
                    result.Write(ItemC0);
                    result.Write(ItemC1);
                    result.Write(ItemC2);
                    result.Write(ItemC3);
                    result.Write(ItemC4);
                    result.Write((byte)0);
                    result.Write(ItemFlag ?? 0);
                    result.Write(UnixTimeConvert(ItemRegDate)); //UNIXTIME campo RegDate
                    result.Write(UnixTimeConvert(ItemEndDate)); //UNIXTIME campo EndDate
                    result.Write((byte)2);
                    result.WriteStr(IsUCCNull(ItemUCCName), 16);
                    result.WriteZero(25);
                    result.WriteStr(IsUCCNull(ItemUCCUnique), 9);//chave do SD
                    result.Write(ItemUCCStatus ?? 0);
                    result.Write(ItemUCCCopyCount ?? 0);
                    result.WriteStr(IsUCCNull(ItemUCCDrawer), 16);
                    result.WriteZero(86);
                }
                return result.GetBytes();
            }
        }

        public bool RemoveQuantity(uint Value)
        {
            if (ItemTypeID == 467664918)
            {
                return true;
            }
            ItemC0 -= (ushort)Value;
            if (ItemC0 <= 0)
            {
                ItemIsValid = 0;
            }
            Update();
            return true;
        }
        public void Renew()
        {
            this.ItemEndDate = DateTime.Now.AddDays(7);
            this.ItemFlag = 0x60;
            this.ItemNeedUpdate = true;
        }

        public bool SetItemInformations(PlayerItemData Info)
        {
            ItemIndex = Info.ItemIndex;
            ItemTypeID = Info.ItemTypeID;
            ItemHourLeft = Info.ItemHourLeft;
            ItemC0 = Info.ItemC0;
            ItemC1 = Info.ItemC1;
            ItemC2 = Info.ItemC2;
            ItemC3 = Info.ItemC3;
            ItemC4 = Info.ItemC4;
            ItemUCCUnique = Info.ItemUCCUnique;
            ItemUCCStatus = Info.ItemUCCStatus;
            ItemUCCDrawer = Info.ItemUCCDrawer;
            ItemUCCDrawerUID = Info.ItemUCCDrawerUID;
            ItemUCCName = Info.ItemUCCName;
            ItemUCCCopyCount = Info.ItemUCCCopyCount;
            ItemClubPoint = Info.ItemClubPoint;
            ItemClubWorkCount = Info.ItemClubWorkCount;
            ItemClubPointLog = Info.ItemClubPointLog;
            ItemClubPangLog = Info.ItemClubPangLog;
            ItemC0Slot = Info.ItemC0Slot;
            ItemC1Slot = Info.ItemC1Slot;
            ItemC2Slot = Info.ItemC2Slot;
            ItemC3Slot = Info.ItemC3Slot;
            ItemC4Slot = Info.ItemC4Slot;
            ItemClubSlotCancelledCount = Info.ItemClubSlotCancelledCount;
            ItemGroup = (byte)GetItemGroup(Info.ItemTypeID);
            ItemRegDate = Info.ItemRegDate;
            ItemEndDate = Info.ItemEndDate;
            ItemIsValid = Info.ItemIsValid;
            ItemNeedUpdate = false;
            return true;
        }

        public void Update()
        {
            if (!ItemNeedUpdate)
            { ItemNeedUpdate = true; }
        }

        public string GetSqlUpdateString()
        {
            StringBuilder SQLString;
            SQLString = new StringBuilder();

            try
            {
                SQLString.Append('^');
                SQLString.Append(ItemIndex);
                SQLString.Append('^');
                SQLString.Append(ItemC0);
                SQLString.Append('^');
                SQLString.Append(ItemC1);
                SQLString.Append('^');
                SQLString.Append(ItemC2);
                SQLString.Append('^');
                SQLString.Append(ItemC3);
                SQLString.Append('^');
                SQLString.Append(ItemC4);
                SQLString.Append('^');
                SQLString.Append(ItemIsValid);
                SQLString.Append('^');
                SQLString.Append(IfCompare<byte>(IffEntry.IsSelfDesign(ItemTypeID), 1, 0));
                SQLString.Append('^');
                SQLString.Append(ItemUCCStatus ?? 0);
                SQLString.Append('^');
                SQLString.Append(ItemUCCUnique ?? "0");
                SQLString.Append('^');
                SQLString.Append(GetSQLTime(ItemEndDate));
                SQLString.Append('^');
                SQLString.Append(ItemFlag ?? 0);
                SQLString.Append('^');
                // { CLUB SET DATA }
                SQLString.Append(ItemClubPoint);
                SQLString.Append('^');
                SQLString.Append(ItemClubWorkCount);
                SQLString.Append('^');
                SQLString.Append(ItemClubPointLog);
                SQLString.Append('^');
                SQLString.Append(ItemClubPangLog);
                SQLString.Append('^');
                SQLString.Append(ItemC0Slot);
                SQLString.Append('^');
                SQLString.Append(ItemC1Slot);
                SQLString.Append('^');
                SQLString.Append(ItemC2Slot);
                SQLString.Append('^');
                SQLString.Append(ItemC3Slot);
                SQLString.Append('^');
                SQLString.Append(ItemC4Slot);
                SQLString.Append('^');
                SQLString.Append(ItemClubSlotCancelledCount);
                SQLString.Append('^');
                SQLString.Append(IfCompare<byte>(GetItemGroup(ItemTypeID) == 4, 1, 0));
                SQLString.Append(',');
                // close for next player
                var data = SQLString.ToString();
                return data;
            }
            finally
            {
                SQLString = null;
            }
        }
    }
}
