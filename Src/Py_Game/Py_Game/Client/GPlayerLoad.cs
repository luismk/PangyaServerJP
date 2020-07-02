using PangyaAPI.BinaryModels;
using Py_Game.GameTools;
using System;
using System.Runtime.CompilerServices;

namespace Py_Game.Client
{
    public partial class GPlayer
    {
        #region Load Methods

        public void ReloadAchievement()
        {
           
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void LoadStatistic()
        {
            foreach (var Data in Db.ProcGetStatistic((int)GetUID))
            {
                UserStatistic = new Data.StatisticData()
                {
                    Drive = (uint)Data.Drive,
                    Putt = (uint)Data.Putt,
                    PlayTime = (uint)Data.Playtime,
                    ShotTime = (uint)Data.ShotTime,
                    LongestDistance = Data.Longest,
                    Pangya = (uint)Data.Pangya,
                    TimeOut = (uint)Data.Timeout,
                    OB = (uint)Data.OB,
                    DistanceTotal = (uint)Data.Distance,
                    Hole = (uint)Data.Hole,
                    TeamHole = (uint)Data.TeamHole,
                    HIO = (uint)Data.Holeinone,
                    Bunker = (ushort)Data.Bunker,
                    Fairway = (uint)Data.Fairway,
                    Albratoss = (uint)Data.Albatross,
                    Holein = (uint)Data.Holein,
                    Puttin = (uint)Data.PuttIn,
                    LongestPutt = Data.LongestPuttin,
                    LongestChip = Data.LongestChipIn,
                    EXP = (uint)Data.Game_Point,
                    Level = (byte)Data.Game_Level,
                    Pang = (ulong)Data.Pang,
                    TotalScore = (uint)Data.TotalScore,
                    Score = new byte[5] { (byte)Data.BestScore0, (byte)Data.BestScore1, (byte)Data.BestScore2, (byte)Data.BestScore3, (byte)Data.BESTSCORE4 },
                    Unknown = 0,
                    MaxPang0 = (ulong)Data.MaxPang0,
                    MaxPang1 = (ulong)Data.MaxPang1,
                    MaxPang2 = (ulong)Data.MaxPang2,
                    MaxPang3 = (ulong)Data.MaxPang3,
                    MaxPang4 = (ulong)Data.MaxPang4,
                    SumPang = (ulong)Data.SumPang,
                    GamePlayed = (uint)Data.GameCount,
                    Disconnected = (uint)Data.DisconnectGames,
                    TeamWin = (uint)Data.wTeamWin,
                    TeamGame = (uint)Data.wTeamGames,
                    LadderPoint = (uint)Data.LadderPoint,
                    LadderWin = (uint)Data.LadderWin,
                    LadderLose = (uint)Data.LadderLose,
                    LadderDraw = (uint)Data.LadderDraw,
                    LadderHole = (uint)Data.LadderHole,
                    ComboCount = (uint)Data.ComboCount,
                    MaxCombo = (uint)Data.MaxComboCount,
                    NoMannerGameCount = (uint)Data.NoMannerGameCount,
                    SkinsPang = (ulong)Data.SkinsPang,
                    SkinsWin = (uint)Data.SkinsWin,
                    SkinsLose = (uint)Data.SkinsLose,
                    SkinsRunHole = (uint)Data.SkinsRunHoles,
                    SkinsStrikePoint = (uint)Data.SkinsStrikePoint,
                    SKinsAllinCount = (uint)Data.SkinsAllinCount,
                    Unknown1 = new byte[6] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF },
                    GameCountSeason = (uint)Data.GameCountSeason,
                    Unknown2 = new byte[8] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, }
                };
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void LoadGuildData()
        {
            foreach (var Data in Db.ProcGuildGetPlayerData((int)GetUID, 0))
            {
                GuildInfo = new Data.GuildData
                {
                    Name = Data.GUILD_NAME,
                    ID = (uint)Data.GUILD_INDEX,
                    Position = Data.GUILD_POSITION,
                    Image = Data.GUILD_IMAGE,
                    Introducing = Data.GUILD_INTRODUCING,
                    LeaderNickname = Data.GUILD_LEADER_NICKNAME,
                    Notice = Data.GUILD_NOTICE,
                    LeaderUID = (uint)Data.GUILD_LEADER_UID,
                    TotalMember = (uint)Data.GUILD_TOTAL_MEMBER,
                    Create_Date = Data.GUILD_CREATE_DATE
                };
            }

            if (GuildInfo.LeaderUID == 0)
            {
                GuildInfo.LeaderUID = uint.MaxValue;
            }
        }
        #endregion  
    }
}
