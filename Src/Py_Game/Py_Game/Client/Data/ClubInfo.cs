using Py_Game.Client.Inventory.Data.Warehouse;
using Py_Game.Defines;
using System;
namespace Py_Game.Client.Data
{
    public class ClubStatus
    {
        public ushort Power { get; set; }
        public ushort Control { get; set; }
        public ushort Impact { get; set; }
        public ushort Spin { get; set; }
        public ushort Curve { get; set; }
        public ECLUBTYPE ClubType { get; set; }
        public byte ClubSPoint { get; set; }

        // ClubStatus
        public byte[] GetClubArray()
        {
            byte[] result = new byte[5];
            result[0] = (byte)Power;
            result[1] = (byte)Control;
            result[2] = (byte)Impact;
            result[3] = (byte)Spin;
            result[4] = (byte)Curve;
            return result;
        }

        public ClubStatus GetClubPlayer(ClubStatus ClubPlayerData)
        {
            ClubStatus result;
            result = this + ClubPlayerData;
            return result;
        }

        public int GetClubTotal(ClubStatus ClubPlayerData, bool IsRankUp)
        {
            int result;
            result = (Power +  Control +  Impact +  Spin + Curve + ClubPlayerData.Power + ClubPlayerData.Control +  ClubPlayerData.Impact + ClubPlayerData.Spin + ClubPlayerData.Curve);
            if (IsRankUp)
            {
                result += 1;
            }
            return result;
        }
        

        public static ClubStatus operator -(ClubStatus X, ClubStatus Y)
        {
            ClubStatus result = new ClubStatus()
            {
                Power = Convert.ToUInt16(Y.Power - X.Power),
                Control = Convert.ToUInt16(Y.Control - X.Control),
                Impact = Convert.ToUInt16(Y.Impact - X.Impact),
                Spin = Convert.ToUInt16(Y.Spin - X.Spin),
                Curve = Convert.ToUInt16(Y.Curve - X.Curve)
            };

            return result;
        }

        public static ClubStatus operator +(ClubStatus X, ClubStatus Y)
        {
            ClubStatus result = new ClubStatus()
            {
                Power = Convert.ToUInt16(X.Power + Y.Power),
                Control = Convert.ToUInt16(X.Control + Y.Control),
                Impact = Convert.ToUInt16(X.Impact + Y.Impact),
                Spin = Convert.ToUInt16(X.Spin + Y.Spin),
                Curve = Convert.ToUInt16(X.Curve + Y.Curve)
            };
            return result;
        }

    } // end ClubStatus

    public struct TClubUpgradeData
    {
        public bool Able;
        public UInt32 Pang;
    } // end TClubUpgradeData

    public class TClubUpgradeTemporary
    {
        public PlayerItemData PClub;
        public sbyte UpgradeType;
        public byte Count;
        public TClubUpgradeTemporary()
        {
            PClub = new PlayerItemData();
        }
        // TClubUpgradeTemporary
        public void Clear()
        {
            PClub = null;
            UpgradeType = -1;
        }

    } // end TClubUpgradeTemporary

    public struct TClubUpgradeRank
    {
        public UInt32 ClubPoint;
        public byte ClubSPoint;
        public byte ClubCurrentRank;
        public ClubStatus ClubSlotLeft;
    } // end TClubUpgradeRank
}
