using PangyaAPI.BinaryModels;
using PangyaAPI;
using Py_Game.Client;
using System;
using System.Collections.Generic;
using Py_Game.GameTools;
using Py_Connector.DataBase;
using System.Linq;
using Py_Game.Client.Data;
using PangyaAPI.PangyaPacket;

namespace Py_Game.Functions
{
    public class AchievementCoreSystem
    {
        public void PlayerGetAchievement(GPlayer player, Packet packet)
        {
            PangyaEntities _db = new PangyaEntities();
            int UID;
            var Achievements = new List<TAchievement>();
            var AchievementQuests = new List<TAchievementQuest>();
            var AchievemetCounters = new Dictionary<uint, TAchievementCounter>();
            UID = packet.ReadInt32();

            try
            {
                if (UID == player.GetUID)
                {
                    PlayerSendAchievement(player, player.Achievements, player.AchievementQuests, player.AchievemetCounters);
                }
                else
                {
                    PlayerSendAchievement(player, Achievements, AchievementQuests, AchievemetCounters);
                }
            }
            catch
            {
                if (_db != null)
                {
                    _db.Dispose();
                }
            }
            finally
            {
                if (_db != null)
                {
                    _db.Dispose();
                }
            }
        }
        public void PlayerSendAchievement(GPlayer player, List<TAchievement>
            Achievements, List<TAchievementQuest> AchievementQuests, Dictionary<uint, TAchievementCounter> AchievemetCounters)
        {
            PangyaBinaryWriter Packet = new PangyaBinaryWriter();
            PangyaBinaryWriter Packet2 = new PangyaBinaryWriter();
            UInt32 Count = 0;
            UInt32 CounterQty = 0;

            Packet.Write(new byte[] { 0x2D, 0x02 });
            Packet.WriteUInt32(0);
            Packet.WriteInt32(player.Achievements.Count);
            Packet.WriteInt32(player.Achievements.Count);
            foreach (var Achievement in Achievements)
            {
                Packet.WriteUInt32(Achievement.TypeID);
                Packet.WriteUInt32(Achievement.ID);
                foreach (var Quest in AchievementQuests)
                {
                    if (Achievement.ID == Quest.AchievementIndex)
                    {
                        Count += 1;

                        if (AchievemetCounters.TryGetValue(Quest.CounterIndex, out TAchievementCounter Counter))
                        {
                            CounterQty = Counter.Quantity;
                        }
                        else
                        {
                            CounterQty = 0;
                        }
                        Packet2.WriteUInt32(Quest.AchievementTypeID);
                        Packet2.WriteUInt32(CounterQty);
                        Packet2.WriteUInt32(Quest.SuccessDate);
                    }

                }
            }
            Packet.WriteUInt32(Count);
            Packet.WriteZero(8);
            Packet.Write(Packet2.GetBytes());

            player.SendResponse(Packet.GetBytes());

            player.SendResponse(new byte[] { 0x2C, 0x02, 0x00, 0x00, 0x00, 0x00 });

            Packet.Dispose();
            Packet2.Dispose();
        }
    }
}
