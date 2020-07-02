using System;
using Py_Game.Defines;
using static PangyaFileCore.IffBaseManager;
using Py_Game.Client.Data;
using PangyaFileCore.Data;

namespace Py_Game.Data
{
    public class ClubData
    {
        public static ClubStatus GetClubStatus(UInt32 ID)
        {
            ClubStatus result = new ClubStatus();
            ClubSet ClubInfo = new ClubSet();
            if (!IffEntry.Club.LoadItem(ID, ref ClubInfo))
            {
                return result;
            }
            result.Power = ClubInfo.MaxPow;
            result.Control = ClubInfo.MaxCon;
            result.Impact = ClubInfo.MaxImp;
            result.Spin = ClubInfo.MaxSpin;
            result.Curve = ClubInfo.MaxCurve;
            result.ClubType = (ECLUBTYPE)ClubInfo.ClubType;
            result.ClubSPoint = (byte)ClubInfo.ClubSPoint;
            return result;
        }
        public static ClubStatus GetClubMaxStatus(uint TypeID)
        {
            return GetClubStatus(TypeID);
        }

        public static ClubStatus PlayerGetClubSlotLeft(uint ID, ClubStatus ClubPlayerData, bool IsRankUp = false)
        {
            ClubStatus ClubMaxSlot, ClubData;
            ClubMaxSlot = new ClubStatus();
            ClubData = GetClubStatus(ID);

            switch (ClubData.ClubType)
            {
                case ECLUBTYPE.TYPE_BALANCE:
                    {
                        switch (ClubData.GetClubTotal(ClubPlayerData, IsRankUp))
                        {
                            case 30: //balance e
                            case 31:
                            case 32:
                            case 33:
                            case 34:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 14,
                                        Control = 12,
                                        Impact = 12,
                                        Spin = 5,
                                        Curve = 5
                                    };
                                }
                                break;
                            case 35://balance D
                            case 36:
                            case 37:
                            case 38:
                            case 39:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 15,
                                        Control = 12,
                                        Impact = 13,
                                        Spin = 6,
                                        Curve = 6
                                    };
                                }
                                break;
                            case 40://balance C
                            case 41:
                            case 42:
                            case 43:
                            case 44:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 16,
                                        Control = 12,
                                        Impact = 14,
                                        Spin = 6,
                                        Curve = 6
                                    };
                                }
                                break;
                            case 45://balance b
                            case 46:
                            case 47:
                            case 48:
                            case 49:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 18,
                                        Control = 13,
                                        Impact = 15,
                                        Spin = 7,
                                        Curve = 7
                                    };
                                }
                                break;
                            case 50://balance A
                            case 51:
                            case 52:
                            case 53:
                            case 54:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 20,
                                        Control = 13,
                                        Impact = 16,
                                        Spin = 8,
                                        Curve = 8
                                    };
                                }
                                break;
                            case 55:
                            case 56:
                            case 57:
                            case 58:
                            case 59:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 20,
                                        Control = 13,
                                        Impact = 16,
                                        Spin = 8,
                                        Curve = 8
                                    };
                                }
                                break;
                        }
                    }
                    break;
                case ECLUBTYPE.TYPE_POWER:
                    {
                        switch (ClubData.GetClubTotal(ClubPlayerData, IsRankUp))
                        {
                            case 30: //balance e
                            case 31:
                            case 32:
                            case 33:
                            case 34:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 0,
                                        Control = 0,
                                        Impact = 0,
                                        Spin = 0,
                                        Curve = 0
                                    };
                                }
                                break;
                            case 35://balance D
                            case 36:
                            case 37:
                            case 38:
                            case 39:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 16,
                                        Control = 12,
                                        Impact = 13,
                                        Spin = 6,
                                        Curve = 6
                                    };
                                }
                                break;
                            case 40://balance C
                            case 41:
                            case 42:
                            case 43:
                            case 44:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 16,
                                        Control = 12,
                                        Impact = 14,
                                        Spin = 6,
                                        Curve = 6
                                    };
                                }
                                break;
                            case 45://Power D
                            case 46:
                            case 47:
                            case 48:
                            case 49:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 17,
                                        Control = 13,
                                        Impact = 14,
                                        Spin = 6,
                                        Curve = 6
                                    };
                                }
                                break;
                            case 50://balance A
                            case 51:
                            case 52:
                            case 53:
                            case 54:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 20,
                                        Control = 13,
                                        Impact = 16,
                                        Spin = 8,
                                        Curve = 8
                                    };
                                }
                                break;
                            case 55:
                            case 56:
                            case 57:
                            case 58:
                            case 59:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 20,
                                        Control = 13,
                                        Impact = 16,
                                        Spin = 8,
                                        Curve = 8
                                    };
                                }
                                break;
                                //default:
                                //    {
                                //        ClubMaxSlot = ClubPlayerData;
                                //    }
                                //    break;
                        }
                    }
                    break;
                case ECLUBTYPE.TYPE_CONTROL:
                    {
                        switch (ClubData.GetClubTotal(ClubPlayerData, IsRankUp))
                        {
                            case 30: //Control E
                            case 31:
                            case 32:
                            case 33:
                            case 34:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 0,
                                        Control = 0,
                                        Impact = 0,
                                        Spin = 0,
                                        Curve = 0
                                    };
                                }
                                break;
                            case 35://Control D
                            case 36:
                            case 37:
                            case 38:
                            case 39:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 16,
                                        Control = 12,
                                        Impact = 13,
                                        Spin = 6,
                                        Curve = 6
                                    };
                                }
                                break;
                            case 40://Control C
                            case 41:
                            case 42:
                            case 43:
                            case 44:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 16,
                                        Control = 13,
                                        Impact = 14,
                                        Spin = 6,
                                        Curve = 6
                                    };
                                }
                                break;
                            case 45://Control B
                            case 46:
                            case 47:
                            case 48:
                            case 49:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 18,
                                        Control = 13,
                                        Impact = 15,
                                        Spin = 6,
                                        Curve = 6
                                    };
                                }
                                break;
                            case 50://Control A
                            case 51:
                            case 52:
                            case 53:
                            case 54:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 20,
                                        Control = 13,
                                        Impact = 16,
                                        Spin = 8,
                                        Curve = 8
                                    };
                                }
                                break;
                            case 55:
                            case 56:
                            case 57:
                            case 58:
                            case 59:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 20,
                                        Control = 13,
                                        Impact = 16,
                                        Spin = 8,
                                        Curve = 8
                                    };
                                }
                                break;

                        }
                    }
                    break;
                case ECLUBTYPE.TYPE_SPIN:
                    {
                        switch (ClubData.GetClubTotal(ClubPlayerData, IsRankUp))
                        {
                            case 30: //Control E
                            case 31:
                            case 32:
                            case 33:
                            case 34:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 0,
                                        Control = 0,
                                        Impact = 0,
                                        Spin = 0,
                                        Curve = 0
                                    };
                                }
                                break;
                            case 35://Control D
                            case 36:
                            case 37:
                            case 38:
                            case 39:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 15,
                                        Control = 12,
                                        Impact = 13,
                                        Spin = 7,
                                        Curve = 6
                                    };
                                }
                                break;
                            case 40://Control C
                            case 41:
                            case 42:
                            case 43:
                            case 44:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 16,
                                        Control = 13,
                                        Impact = 14,
                                        Spin = 7,
                                        Curve = 6
                                    };
                                }
                                break;
                            case 45://Control B
                            case 46:
                            case 47:
                            case 48:
                            case 49:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 18,
                                        Control = 13,
                                        Impact = 15,
                                        Spin = 8,
                                        Curve = 7
                                    };
                                }
                                break;
                            case 50://Control A
                            case 51:
                            case 52:
                            case 53:
                            case 54:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 20,
                                        Control = 13,
                                        Impact = 16,
                                        Spin = 8,
                                        Curve = 8
                                    };
                                }
                                break;
                            case 55:
                            case 56:
                            case 57:
                            case 58:
                            case 59:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 20,
                                        Control = 13,
                                        Impact = 16,
                                        Spin = 8,
                                        Curve = 8
                                    };
                                }
                                break;

                        }
                    }
                    break;
                case ECLUBTYPE.TYPE_SPECIAL:
                    {
                        switch (ClubData.GetClubTotal(ClubPlayerData, IsRankUp))
                        {
                            case 30: //Control E
                            case 31:
                            case 32:
                            case 33:
                            case 34:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 0,
                                        Control = 0,
                                        Impact = 0,
                                        Spin = 0,
                                        Curve = 0
                                    };
                                }
                                break;
                            case 35://Control D
                            case 36:
                            case 37:
                            case 38:
                            case 39:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 17,
                                        Control = 13,
                                        Impact = 14,
                                        Spin = 6,
                                        Curve = 6
                                    };
                                }
                                break;
                            case 40://Control C
                            case 41:
                            case 42:
                            case 43:
                            case 44:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 19,
                                        Control = 13,
                                        Impact = 15,
                                        Spin = 7,
                                        Curve = 7
                                    };
                                }
                                break;
                            case 45://Control B
                            case 46:
                            case 47:
                            case 48:
                            case 49:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 21,
                                        Control = 13,
                                        Impact = 17,
                                        Spin = 8,
                                        Curve = 8
                                    };
                                }
                                break;
                            case 50://Control A
                            case 51:
                            case 52:
                            case 53:
                            case 54:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 22,
                                        Control = 14,
                                        Impact = 18,
                                        Spin = 9,
                                        Curve = 9
                                    };
                                }
                                break;
                            case 55:
                            case 56:
                            case 57:
                            case 58:
                            case 59:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 22,
                                        Control = 14,
                                        Impact = 18,
                                        Spin = 9,
                                        Curve = 9
                                    };
                                }
                                break;
                            default:
                                {
                                    ClubMaxSlot = new ClubStatus
                                    {
                                        Power = 0,
                                        Control = 0,
                                        Impact = 0,
                                        Spin = 0,
                                        Curve = 0
                                    };
                                }
                                break;

                        }
                    }
                    break;
            }

            var Result = ClubMaxSlot - ClubData.GetClubPlayer(ClubPlayerData);
            return Result;
        }

        public static sbyte PlayerGetSlotUpgrade(uint TypeID, uint Quantity, ClubStatus ClubPlayerData)
        {
            const byte RandTo = 30;
            byte RandInt;
            sbyte Index;
            Random rnd;
            bool Check()
            {
                return (ClubPlayerData.Power > 0) || (ClubPlayerData.Control > 0) || (ClubPlayerData.Impact > 0) || (ClubPlayerData.Spin > 0) || (ClubPlayerData.Curve > 0);
            }
            if (!Check())
            {
                return -1;
            }
            rnd = new Random();
            RandInt = (byte)(rnd.Next(100) + 1);
            switch (TypeID)
            {
                case 0x7C800026:// Orihakon
                    switch (Quantity)
                    {
                        case 1:
                            if ((RandInt < RandTo) && (ClubPlayerData.Impact > 0))
                            {
                                return 2;
                            }
                            break;
                        case 2:
                            if ((RandInt < RandTo) && (ClubPlayerData.Curve > 0))
                            {
                                return 4;
                            }
                            break;
                        case 3:
                            if ((RandInt < RandTo) && (ClubPlayerData.Power > 0))
                            {
                                return 0;
                            }
                            break;
                        case 4:
                            if ((RandInt < RandTo) && (ClubPlayerData.Spin > 0))
                            {
                                return 3;
                            }
                            break;
                        case 5:
                            if ((RandInt < RandTo) && (ClubPlayerData.Control > 0))
                            {
                                return 1;
                            }
                            break;
                    }
                    break;
                case 0x7C800041:// Soren
                    switch (Quantity)
                    {
                        case 0:
                            if (ClubPlayerData.Impact > 0)
                            {
                                return 2;
                            }
                            break;
                        case 1:
                            if (ClubPlayerData.Curve > 0)
                            {
                                return 4;
                            }
                            break;
                        case 2:
                            if (ClubPlayerData.Power > 0)
                            {
                                return 0;
                            }
                            break;
                        case 3:
                            if (ClubPlayerData.Spin > 0)
                            {
                                return 3;
                            }
                            break;
                        case 4:
                            if (ClubPlayerData.Control > 0)
                            {
                                return 1;
                            }
                            break;
                    }
                    break;
            }
            while (true)
            {
                for (Index = 0; Index < 5; Index++)
                {
                    RandInt = (byte)(rnd.Next(0, 20) + 1);
                    if (ClubPlayerData.GetClubArray()[Index] > 0 && RandInt <= 20)
                    {
                        return Index;
                    }                    
                }
                return -1;
            }
        }

        public static TClubUpgradeRank PlayerGetClubRankUPData(uint ID, ClubStatus ClubPlayerData)
        {
            TClubUpgradeRank result;
            ClubStatus ClubData;


            ClubData = GetClubStatus(ID);
            result.ClubPoint = 0;
            result.ClubCurrentRank = 0;
            result.ClubSPoint = ClubData.ClubSPoint;
            result.ClubSlotLeft = PlayerGetClubSlotLeft(ID, ClubPlayerData, true);
            switch (ClubData.ClubType)
            {
                case ECLUBTYPE.TYPE_BALANCE:
                case ECLUBTYPE.TYPE_POWER:
                case ECLUBTYPE.TYPE_SPIN:
                    switch (ClubData.GetClubTotal(ClubPlayerData, false))
                    {
                        case 59:
                            result.ClubPoint = 0;
                            result.ClubCurrentRank = 5;
                            break;
                        case 54:
                            result.ClubPoint = 68000;
                            result.ClubCurrentRank = 4;
                            break;
                        case 49:
                            result.ClubPoint = 20200;
                            result.ClubCurrentRank = 3;
                            break;
                        case 44:
                            result.ClubPoint = 11000;
                            result.ClubCurrentRank = 2;
                            break;
                        case 39:
                            result.ClubPoint = 2500;
                            result.ClubCurrentRank = 1;
                            break;
                        case 34:
                            result.ClubPoint = 900;
                            result.ClubCurrentRank = 0;
                            break;
                    }
                    break;
                case ECLUBTYPE.TYPE_CONTROL:
                    switch (ClubData.GetClubTotal(ClubPlayerData, false))
                    {
                        case 59:
                            result.ClubPoint = 0;
                            result.ClubCurrentRank = 5;
                            break;
                        case 54:
                            result.ClubPoint = 75000;
                            result.ClubCurrentRank = 4;
                            break;
                        case 49:
                            result.ClubPoint = 32500;
                            result.ClubCurrentRank = 3;
                            break;
                        case 44:
                            result.ClubPoint = 15000;
                            result.ClubCurrentRank = 2;
                            break;
                        case 39:
                            result.ClubPoint = 4800;
                            result.ClubCurrentRank = 1;
                            break;
                        case 34:
                            result.ClubPoint = 0;
                            result.ClubCurrentRank = 0;
                            break;
                    }
                    break;
                case ECLUBTYPE.TYPE_SPECIAL:
                    switch (ClubData.GetClubTotal(ClubPlayerData, false))
                    {
                        case 59:
                            result.ClubPoint = 0;
                            result.ClubCurrentRank = 5;
                            break;
                        case 54:
                            result.ClubPoint = 90000;
                            result.ClubCurrentRank = 4;
                            break;
                        case 49:
                            result.ClubPoint = 35000;
                            result.ClubCurrentRank = 3;
                            break;
                        case 44:
                            result.ClubPoint = 17600;
                            result.ClubCurrentRank = 2;
                            break;
                        case 39:
                            result.ClubPoint = 5300;
                            result.ClubCurrentRank = 1;
                            break;
                        case 34:
                            result.ClubPoint = 0;
                            result.ClubCurrentRank = 0;
                            break;
                    }
                    break;
            }
            return result;
        }
    }
}
