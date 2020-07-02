using PangyaAPI.BinaryModels;
using Py_Game.Data;
using Py_Game.Defines;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Py_Game.Client.Data
{
    public struct GuildData
    {
        public UInt32 ID { get; set; }
        public string Name { get; set; }
        public byte Position { get; set; }
        public string Image { get; set; }
        public uint TotalMember { get; set; }
        public string Notice { get; set; }
        public string Introducing { get; set; }
        public uint LeaderUID { get; set; }
        public string LeaderNickname { get; set; }
        public DateTime? Create_Date { get; set; }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct StatisticData
    {
        public UInt32 Drive { get; set; }
        public UInt32 Putt { get; set; }
        public UInt32 PlayTime { get; set; }
        // Second
        public UInt32 ShotTime { get; set; }
        public float LongestDistance { get; set; }
        public UInt32 Pangya { get; set; }
        public UInt32 TimeOut { get; set; }
        public UInt32 OB { get; set; }
        public UInt32 DistanceTotal { get; set; }
        public UInt32 Hole { get; set; }
        public UInt32 TeamHole { get; set; }
        public UInt32 HIO { get; set; }
        public ushort Bunker { get; set; }
        public UInt32 Fairway { get; set; }
        public UInt32 Albratoss { get; set; }
        public UInt32 Holein { get; set; }
        public UInt32 Puttin { get; set; }
        public float LongestPutt { get; set; }
        public float LongestChip { get; set; }
        public UInt32 EXP { get; set; }
        public byte Level { get; set; }
        public UInt64 Pang { get; set; }
        public UInt32 TotalScore { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x05)]
        public byte[] Score;
        public byte Unknown { get; set; }
        public UInt64 MaxPang0 { get; set; }
        public UInt64 MaxPang1 { get; set; }
        public UInt64 MaxPang2 { get; set; }
        public UInt64 MaxPang3 { get; set; }
        public UInt64 MaxPang4 { get; set; }
        public UInt64 SumPang { get; set; }
        public UInt32 GamePlayed { get; set; }
        public UInt32 Disconnected { get; set; }
        public UInt32 TeamWin { get; set; }
        public UInt32 TeamGame { get; set; }
        public UInt32 LadderPoint { get; set; }
        public UInt32 LadderWin { get; set; }
        public UInt32 LadderLose { get; set; }
        public UInt32 LadderDraw { get; set; }
        public UInt32 LadderHole { get; set; }
        public UInt32 ComboCount { get; set; }
        public UInt32 MaxCombo { get; set; }
        public UInt32 NoMannerGameCount { get; set; }
        public UInt64 SkinsPang { get; set; }
        public UInt32 SkinsWin { get; set; }
        public UInt32 SkinsLose { get; set; }
        public UInt32 SkinsRunHole { get; set; }
        public UInt32 SkinsStrikePoint { get; set; }
        public UInt32 SKinsAllinCount { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x06)]
        public byte[] Unknown1;
        public UInt32 GameCountSeason { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
        public byte[] Unknown2;

        public static StatisticData operator +(StatisticData Left, StatisticData Right)
        {
            var Result = new StatisticData()
            {

                //{ Drive }
                Drive = Left.Drive + Right.Drive,
                //{ Putt}
                Putt = Left.Putt + Right.Putt,
                //{ Player Time Do Nothing }
                PlayTime = Left.PlayTime,
                //{ Shot Time }
                ShotTime = Left.ShotTime + Right.ShotTime
            };
            //{ Longest }
            if (Right.LongestDistance > Left.LongestDistance)
            {
                Result.LongestDistance = Right.LongestDistance;
            }
            else
            {
                Result.LongestDistance = Left.LongestDistance;
            }
            //{ Hit Pangya }
            Result.Pangya = Left.Pangya + Right.Pangya;
            //{ Timeout }
            Result.TimeOut = Left.TimeOut;
            //{ OB }
            Result.OB = Left.OB + Right.OB;
            //{ Total Distance }
            Result.DistanceTotal = Left.DistanceTotal + Right.DistanceTotal;
            //{ Hole Total }
            Result.Hole = Left.Hole + Right.Hole;
            //{ Team Hole }
            Result.TeamHole = Left.TeamHole;
            //{ Hole In One }
            Result.HIO = Left.HIO;
            //{ Bunker }
            Result.Bunker = (ushort)(Left.Bunker + Right.Bunker);
            //{ Fairway }
            Result.Fairway = Left.Fairway + Right.Fairway;
            //{ Albratoss }
            Result.Albratoss = Left.Albratoss + Right.Albratoss;
            //{ Holein ? }
            Result.Holein = Left.Holein + (Result.Hole - Right.Holein);
            //{ Puttin }
            Result.Puttin = Left.Puttin + Right.Puttin;
            //{ Longest Putt }
            if (Right.LongestPutt > Left.LongestPutt)
            {
                Result.LongestPutt = Right.LongestPutt;
            }
            else
            {
                Result.LongestPutt = Left.LongestPutt;
            }
            //{ Longest Chip }
            if (Right.LongestChip > Left.LongestChip)
            {
                Result.LongestChip = Right.LongestChip;
            }
            else
            {
                Result.LongestChip = Left.LongestChip;
            }
            return Result;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct TGameReward
    {
        public bool BestRecovery { get; set; }
        public bool BestChipIn { get; set; }
        public bool BestDrive { get; set; }
        public bool BestSpeeder { get; set; }
        public bool BestLongPutt { get; set; }
        public bool Lucky { get; set; }

        public void Initial()
        {
            BestChipIn = false;
            BestDrive = false;
            BestSpeeder = false;
            BestLongPutt = false;
            Lucky = false;
        }

    } // end TGameReward

    public struct DataGame
    {
        public uint Pang { get; set; }
        public uint BonusPang { get; set; }
        public sbyte Score { get; set; }
        public sbyte ParCount { get; set; }
        public sbyte ShotCount { get; set; }
        public ushort TotalShot { get; set; }
        public bool HoleComplete { get; set; }
        public byte HoleCompletedCount { get; set; }
        public StatisticData Statistic { get; set; }
        public byte Rate { get; set; }
        public UInt32 EXP { get; set; }
        public TGameReward Reward;
        public bool Quited { get; set; }

        public DataGame Initial()
        {
            Statistic = new StatisticData();
            Reward = new TGameReward();
            Pang = 0;
            BonusPang = 0;
            Score = 0;
            ParCount = 0;
            ShotCount = 0;
            TotalShot = 1; // { Total shot default is 1 }
            EXP = 0;
            HoleComplete = false;
            HoleCompletedCount = 0;
            Reward.Initial();
            Quited = false;
            Rate = 0;
            return this;
        }
        public void Reverse()
        {
            Initial();
        }
    }

    public struct VersusData
    {
        public bool LoadComplete { get; set; }
        public bool LoadHole { get; set; }
        public bool LoadAnimation { get; set; }
        public bool ShotSync { get; set; }
        public Double HoleDistance { get; set; }
       // public VSMatch Team { get; set; }
        public UInt32 LastHit { get; set; }
        // as timestamp
        public sbyte LastScore { get; set; }
        public byte Loading { get; set; }
    }

    public class GamePlay
    {
        public UInt32 ConnectionID;
        public UInt32 UID;
        public byte GameSlot;
        public byte Role;
        public bool GameReady;
        public VersusData Versus;
        public uint Playing;
        public bool GameCompleted;
        public Point3D HolePos3D;
        public UInt32 HolePos;
        public Inventory.Data.Action Action;
        public DataGame GameData;
        public ShopItemData GameShop;

        public bool MyTurn { get; set; }

        public void AddWalk(Point3D Pos)
        {
            Action.Vector += Pos;
        }
        public void SetDefault()
        {
            this.GameSlot = 1;
            this.Role = 0;
            this.GameReady = false;
            this.HolePos = 0;
            this.GameCompleted = false;
            this.ConnectionID = 0;
            this.UID = 0;
            this.HolePos3D = new Point3D();
            this.Versus = new VersusData();
            this.GameData = new DataGame().Initial();
            this.Action = new Inventory.Data.Action().Clear();
            this.Versus.LoadAnimation = false;
            this.Versus.ShotSync = false;
            this.Versus.HoleDistance = 0;
            this.Versus.LastHit = 0;
            this.Versus.LastScore = 0;
            this.GameShop = new ShopItemData();
        }
        public GamePlay()
        {
            SetDefault();
        }

        public void UpdateScore(bool Sucess)
        {
            ushort S;

            if (!Sucess)
            {
                S = 5;
            }
            else
            {
                S = (ushort)(GameData.ShotCount - GameData.ParCount);
            }
            Versus.LastScore = (sbyte)S;
            GameData.Score = (sbyte)(GameData.Score + S);
        }
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TShotData
    {
        public uint ConnectionId { get; set; }
        [field: MarshalAs(UnmanagedType.Struct)]
        public Point3D Pos;
        public TShotType ShotType { get; set; }
        public ushort Un1 { get; set; }
        public uint Pang { get; set; }
        public uint BonusPang { get; set; }
        public uint Un2 { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x06)]
        public byte[] MatchData;
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public byte[] ShotDecrypt;

        public TShotData Start()
        {
            Pos = new Point3D();
            return this;
        }
    }

    public struct TAchievement
    {
        public UInt32 ID;
        public UInt32 TypeID;
        public byte AchievementType;
    }

    public struct TAchievementCounter
    {
        public UInt32 ID;
        public UInt32 TypeID;
        public UInt32 Quantity;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct TAchievementQuest
    {
        public UInt32 ID { get; set; }
        public UInt32 AchievementIndex { get; set; }
        public UInt32 AchievementTypeID { get; set; }
        public UInt32 CounterIndex { get; set; }
        public UInt32 SuccessDate { get; set; }// as timestamp
        public UInt32 Total { get; set; }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HoleData
    {
        public uint HolePosition { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x04)]
        public byte[] Unknown { get; set; }
        public byte Par { get; set; }
        //start pos?
        public float A { get; set; }
        public float B { get; set; }
        //hole position
        public float X { get; set; }
        public float Z { get; set; }
    }

    public class HoleDataTest
    {
        public uint HolePosition { get; set; }
        public float Unknown { get; set; }
        public byte Par { get; set; }
        public float A { get; set; }
        public float B { get; set; }
        public float X { get; set; }
        public float Z { get; set; }
    }
    //Reference: https://github.com/hsreina/pangya-server/blob/develop/src/Server/Game/PlayerShopItem.pas#L14
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ShopItem //0xAC = 172
    {
        public uint UnkNownID { get; set; }
        public uint TypeID { get; set; }
        public uint ItemID { get; set; }
        public uint ItemCount { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Un1 { get; set; }
        public uint ItemPrice { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x95)]
        public byte[] Un2 { get; set; }
    }

    public class ShopItemData
    {
        public uint ShopOwnerID { get; set; }
        public string NickName { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public uint UID { get; set; }
        public ulong Pangs { get; set; }
        public List<ShopItem> Items { get; set; }
        public uint CountVisit { get; set; }
        public uint ItemsCount { get { return (uint)Items.Count; } }

        public void Clear()
        {
            new ShopItemData();
        }
        public ShopItemData()
        {
            Items = new List<ShopItem>();
        }
        public void Add(ShopItem items)
        {
            Items.Add(items);
        }

        public bool Remove(ShopItem items)
        {
           return Items.Remove(items);
        }
        public byte[] GetData()
        {
            using (var result = new PangyaBinaryWriter())
            {
                foreach (var item in Items)
                {
                    result.WriteStruct(item);
                }
                return result.GetBytes();
            }
        }
    }
}
