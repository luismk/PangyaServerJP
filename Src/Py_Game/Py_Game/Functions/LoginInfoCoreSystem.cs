using Py_Game.GameTools;
using PangyaAPI;
using Py_Game.Client;
using System;
using System.Linq;
using Py_Connector.DataBase;
using static Py_Game.GameTools.JunkPacket;
using System.Collections.Generic;
using Py_Game.Data;
using PangyaAPI.PangyaPacket;

namespace Py_Game.Functions
{
    public class LoginInfoCoreSystem
    {
        public void HandleUserInfo(GPlayer player, Packet packet)
        {
            //Ler UID do player baseado no Login do Player, e a Sessão 
            #region Leitura do packet
            var UID = packet.ReadUInt32();
            var session = packet.ReadByte();
            if (UID > 0)
            {
                player.SearchUID = UID;
            }
            else
            {
                UID = player.SearchUID;
            }
            #endregion

            var Client = (GPlayer)player.Server.GetPlayerByUID(UID);

            //Check
            if (Client != null)
            {
                PlayerOnline(Client, player, session);
            }
            else
            {
                PlayerOffLine(player, (int)UID, session);
            }
        }

        protected void PlayerOnline(GPlayer GetPlayer, GPlayer player, byte Session)
        {
            var Inventory = GetPlayer.Inventory;

            #region PlayerGetUserInfo
            player.Response.Write(new byte[] { 0x57, 0x01, Session });
            player.Response.Write(GetPlayer.GetUID);
            player.Response.Write(GetPlayer.GetLoginInfo());
            player.Response.Write(0); //guild points
            player.SendResponse();
            #endregion

            #region PlayerGetCharacterInfo
            player.Response.Write(new byte[] { 0x5E, 0x01 });
            player.Response.Write(GetPlayer.GetUID);
            player.Response.Write(Inventory.GetCharData());
            #endregion

            #region PlayerGetToolbarInfo
            player.Response.Write(new byte[] { 0x56, 0x01, Session });
            player.Response.Write(GetPlayer.GetUID);
            player.Response.Write(Inventory.GetEquipData());
            #endregion

            #region PlayerGetStatisticsInfo
            player.Response.Write(new byte[] { 0x58, 0x01, Session });
            player.Response.Write(GetPlayer.GetUID);
            player.Response.Write(GetPlayer.Statistic());
            player.SendResponse();
            #endregion

            #region PlayerGetGuildInfo
            player.Response.Write(new byte[] { 0x5D, 0x01 });
            player.Response.WriteUInt64(GetPlayer.GetUID);
            player.Response.Write(GetPlayer.GetGuildInfo());
            player.Response.Write(Tools.GetFixTime(GetPlayer.GuildInfo.Create_Date));
            player.SendResponse();
            #endregion

            //#region PlayerGetRecordInfo(Natural)
            //player.Response.Write(new byte[] { 0x5C, 0x01, 0x33, });
            //player.Response.Write(GetPlayer.GetUID);
            //player.Response.Write(21);
            //player.Response.Write(JunkMapStatistic2);
            //player.SendResponse();
            //#endregion

            //#region PlayerGetRecordInfo(GP)
            //player.Response.Write(new byte[] { 0x5C, 0x01, 0x34, });
            //player.Response.Write(GetPlayer.GetUID);
            //player.Response.Write(21);//code
            //player.Response.Write(JunkMapStatistic2);
            //player.SendResponse();
            //#endregion

            //#region PlayerGetInfoUnknown
            //player.Response.Write(new byte[] { 0x5B, 0x01, Session });
            //player.Response.Write(GetPlayer.GetUID);
            //player.Response.WriteUInt16(0);
            //player.SendResponse();
            //#endregion

            //#region PlayerTrophyInfo(Special)
            //player.Response.Write(new byte[] { 0x5A, 0x01, Session });
            //player.Response.Write(GetPlayer.GetUID);
            //player.Response.Write(Inventory.ItemTrophySpecial.GetInfo());
            //player.SendResponse();
            //#endregion

            //#region PlayerTrophyInfo(N)
            //player.Response.Write(new byte[] { 0x59, 0x01, Session });
            //player.Response.Write(GetPlayer.GetUID);
            //player.Response.Write(Inventory.ItemTrophies.GetTrophy());
            //player.SendResponse();
            //#endregion

            //#region PlayerRecordInfo(N)
            //player.Response.Write(new byte[] { 0x5C, 0x01, Session });
            //player.Response.Write(GetPlayer.GetUID);
            //player.Response.Write(21);
            //player.Response.Write(JunkMapStatistic2);
            //player.SendResponse();
            //#endregion

            //#region PlayerTrophyInfo(GP)
            //player.Response.Write(new byte[] { 0x57, 0x02, Session });
            //player.Response.Write(GetPlayer.GetUID);
            //player.Response.Write(Inventory.ItemTrophyGP.GetInfo());
            //player.SendResponse();
            //#endregion

            #region PlayerResultInfo
            player.Response.Write(new byte[] { 0x89, 0x00, 0x01, 0x00, 0x00, 0x00, Session });
            player.Response.Write(GetPlayer.GetUID);
            player.SendResponse();
            #endregion
        }

        protected void PlayerOffLine(GPlayer GetClient, int UID, byte Session)
        {
          //  var _db = new PangyaEntities();
          ////  List<ProcGetCardEquip_One_Result> CardEquip;
          //  TPCards TCard;
          //  //List<ProcGetCardEquip_One_Result> CardEquip;
          //  //List<ProcGetCharacter_One_Result> Character;

          //  var player = GetClient;
          //  var member = _db.ProcGet_UserInfo(UID).ToList();

          //  if (member.Count <= 0)
          //  {
          //      player.Response.Write(new byte[] { 0x89, 0x00, 0x02, 0x00, 0x00, 0x00, Session });
          //      player.Response.Write(UID);
          //      player.SendResponse();
          //      WriteConsole.WriteLine("HandlePlayerGetInfo: " + UID, ConsoleColor.Red);
          //      return;
          //  }
          //  #region PlayerGetUserInfo
          //  player.Response.Write(new byte[] { 0x57, 0x01, Session });
          //  player.Response.Write(UID);
          //  player.Response.Write(ushort.MaxValue);
          //  foreach (var data in member)
          //  {
          //      player.Response.WriteStr(data.Username, 22);
          //      player.Response.WriteStr(data.Nickname, 22);
          //      player.Response.WriteStr(data.GUILD_NAME, 17);
          //      player.Response.WriteStr(data.GUILD_IMAGE, 9);
          //      player.Response.WriteZero(7);
          //      player.Response.Write(0);
          //      player.Response.Write(0);
          //      player.Response.Write(0);
          //      player.Response.WriteZero(12);
          //      player.Response.Write(data.GUILDINDEX ?? 0);
          //      player.Response.Write(0);
          //      player.Response.Write((ushort)0);
          //      player.Response.Write(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
          //      player.Response.WriteZero(144);
          //      player.Response.Write(UID);
          //      player.Response.Write(0); //guild points
          //  }
          //  player.SendResponse();
          //  #endregion

          //  #region PlayerCharacterInfo
          //  CardEquip = _db.ProcGetCardEquip_One(UID).ToList();

          //  TCard = new TPCards();
          //  player.Response.Write(new byte[] { 0x5E, 0x01 });
          //  player.Response.Write(UID);
          //  foreach (var character in _db.ProcGetCharacter_One(UID))
          //  {
          //      player.Response.Write(character.TYPEID);
          //      player.Response.Write(character.CID);
          //      player.Response.Write(character.HAIR_COLOR ?? 0);
          //      player.Response.Write(character.GIFT_FLAG ?? 0);
          //      player.Response.Write(character.PART_TYPEID_1 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_2 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_3 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_4 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_5 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_6 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_7 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_8 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_9 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_10 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_11 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_12 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_13 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_14 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_15 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_16 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_17 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_18 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_19 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_21 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_22 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_23 ?? 0);
          //      player.Response.Write(character.PART_TYPEID_24 ?? 0);
          //      player.Response.Write(character.PART_IDX_1 ?? 0);
          //      player.Response.Write(character.PART_IDX_2 ?? 0);
          //      player.Response.Write(character.PART_IDX_3 ?? 0);
          //      player.Response.Write(character.PART_IDX_4 ?? 0);
          //      player.Response.Write(character.PART_IDX_5 ?? 0);
          //      player.Response.Write(character.PART_IDX_6 ?? 0);
          //      player.Response.Write(character.PART_IDX_7 ?? 0);
          //      player.Response.Write(character.PART_IDX_8 ?? 0);
          //      player.Response.Write(character.PART_IDX_9 ?? 0);
          //      player.Response.Write(character.PART_IDX_10 ?? 0);
          //      player.Response.Write(character.PART_IDX_11 ?? 0);
          //      player.Response.Write(character.PART_IDX_12 ?? 0);
          //      player.Response.Write(character.PART_IDX_13 ?? 0);
          //      player.Response.Write(character.PART_IDX_14 ?? 0);
          //      player.Response.Write(character.PART_IDX_15 ?? 0);
          //      player.Response.Write(character.PART_IDX_16 ?? 0);
          //      player.Response.Write(character.PART_IDX_17 ?? 0);
          //      player.Response.Write(character.PART_IDX_18 ?? 0);
          //      player.Response.Write(character.PART_IDX_19 ?? 0);
          //      player.Response.Write(character.PART_IDX_21 ?? 0);
          //      player.Response.Write(character.PART_IDX_22 ?? 0);
          //      player.Response.Write(character.PART_IDX_23 ?? 0);
          //      player.Response.Write(character.PART_IDX_24 ?? 0);
          //      player.Response.WriteZero(216);
          //      player.Response.Write(character.AUXPART);// anel da esquerda
          //      player.Response.Write(character.AUXPART1);// anel da direita
          //      player.Response.WriteZero(12);
          //      player.Response.Write(character.CUTIN ?? 0);
          //      player.Response.WriteZero(12);
          //      player.Response.Write(character.POWER ?? 0);//power
          //      player.Response.Write(character.CONTROL ?? 0);//control
          //      player.Response.Write(character.IMPACT ?? 0);//impact
          //      player.Response.Write(character.SPIN ?? 0); //spin
          //      player.Response.Write(character.CURVE ?? 0); //curve
          //      player.Response.Write(0);
          //      if (CardEquip.Count > 0)
          //      {
          //          foreach (var PC in CardEquip)
          //          {
          //              if (PC.CID == character.CID)
          //              { TCard.Card[PC.SLOT ?? 0] = (uint)PC.CARD_TYPEID; }
          //              player.Response.WriteUInt32(TCard.Card[0]);
          //              player.Response.WriteUInt32(TCard.Card[1]);
          //              player.Response.WriteUInt32(TCard.Card[2]);
          //              player.Response.WriteUInt32(TCard.Card[3]);
          //              player.Response.WriteUInt32(TCard.Card[4]);
          //              player.Response.WriteUInt32(TCard.Card[5]);
          //              player.Response.WriteUInt32(TCard.Card[6]);
          //              player.Response.WriteUInt32(TCard.Card[7]);
          //              player.Response.WriteUInt32(TCard.Card[8]);
          //              player.Response.WriteUInt32(TCard.Card[9]);
          //          }
          //      }
          //      else
          //      {
          //          player.Response.WriteZero(40);
          //      }
          //      player.Response.Write((uint)0);
          //      player.Response.Write((uint)0);
          //  }
          //  #endregion

          //  #region PlayerToolbarInfo
          //  player.Response.Write(new byte[] { 0x56, 0x01, Session });
          //  player.Response.Write(UID);
          //  foreach (var Toolbar in _db.ProcGetToolbar(UID))
          //  {

          //      player.Response.Write(Toolbar.CADDIE);
          //      player.Response.Write(Toolbar.CHARACTER_ID);
          //      player.Response.Write(Toolbar.CLUB_ID);
          //      player.Response.Write(Toolbar.BALL_ID);
          //      player.Response.Write(Toolbar.ITEM_SLOT_1);
          //      player.Response.Write(Toolbar.ITEM_SLOT_2);
          //      player.Response.Write(Toolbar.ITEM_SLOT_3);
          //      player.Response.Write(Toolbar.ITEM_SLOT_4);
          //      player.Response.Write(Toolbar.ITEM_SLOT_5);
          //      player.Response.Write(Toolbar.ITEM_SLOT_6);
          //      player.Response.Write(Toolbar.ITEM_SLOT_7);
          //      player.Response.Write(Toolbar.ITEM_SLOT_8);
          //      player.Response.Write(Toolbar.ITEM_SLOT_9);
          //      player.Response.Write(Toolbar.ITEM_SLOT_10);
          //      player.Response.Write(0);
          //      player.Response.Write(0);
          //      player.Response.Write(0);
          //      player.Response.Write(0);
          //      player.Response.Write(0);
          //      player.Response.Write(0);
          //      player.Response.Write(Toolbar.Skin_1);
          //      player.Response.Write(Toolbar.Skin_2);
          //      player.Response.Write(Toolbar.Skin_3);
          //      player.Response.Write(Toolbar.Skin_4);
          //      player.Response.Write(Toolbar.Skin_5);
          //      player.Response.Write(Toolbar.Skin_6);
          //      player.Response.Write(Toolbar.MASCOT_ID);
          //      player.Response.Write(Toolbar.POSTER_1);
          //      player.Response.Write(Toolbar.POSTER_2);
          //  }
          //  #endregion

          //  #region PlayerStatisticsInfo
          //  player.Response.Write(new byte[] { 0x58, 0x01, Session });
          //  player.Response.Write(UID);
          //  foreach (var User_Statistics in _db.ProcGetStatistic(UID))
          //  {
          //      player.Response.Write((uint)User_Statistics.Drive);
          //      player.Response.Write((uint)User_Statistics.Putt);
          //      player.Response.Write((uint)User_Statistics.Playtime);
          //      player.Response.Write((uint)User_Statistics.ShotTime);
          //      player.Response.Write(User_Statistics.Longest);
          //      player.Response.Write((uint)User_Statistics.Pangya);
          //      player.Response.Write((uint)User_Statistics.Timeout);
          //      player.Response.Write((uint)User_Statistics.OB);
          //      player.Response.Write((uint)User_Statistics.Distance);
          //      player.Response.Write((uint)User_Statistics.Hole);
          //      player.Response.Write((uint)User_Statistics.TeamHole);
          //      player.Response.Write((uint)User_Statistics.Holeinone);
          //      player.Response.Write((ushort)User_Statistics.Bunker);
          //      player.Response.Write((uint)User_Statistics.Fairway);
          //      player.Response.Write((uint)User_Statistics.Albatross);
          //      player.Response.Write((uint)User_Statistics.Holein);
          //      player.Response.Write((uint)User_Statistics.PuttIn);
          //      player.Response.Write(User_Statistics.LongestPuttin);
          //      player.Response.Write(User_Statistics.LongestChipIn);
          //      player.Response.Write((uint)User_Statistics.Game_Point);
          //      player.Response.Write((byte)User_Statistics.Game_Level);
          //      player.Response.Write((long)User_Statistics.Pang);//pangs inicias
          //      player.Response.Write((uint)User_Statistics.TotalScore);
          //      player.Response.Write((byte)User_Statistics.BestScore0);
          //      player.Response.Write((byte)User_Statistics.BestScore1);
          //      player.Response.Write((byte)User_Statistics.BestScore2);
          //      player.Response.Write((byte)User_Statistics.BestScore3);
          //      player.Response.Write((byte)User_Statistics.BESTSCORE4);
          //      player.Response.Write((byte)0x00);//unknow
          //      player.Response.Write((ulong)User_Statistics.MaxPang0);
          //      player.Response.Write((ulong)User_Statistics.MaxPang1);
          //      player.Response.Write((ulong)User_Statistics.MaxPang2);
          //      player.Response.Write((ulong)User_Statistics.MaxPang3);
          //      player.Response.Write((ulong)User_Statistics.MaxPang4);
          //      player.Response.Write((ulong)User_Statistics.SumPang);
          //      player.Response.Write((uint)User_Statistics.GameCount);
          //      player.Response.Write((uint)User_Statistics.DisconnectGames);
          //      player.Response.Write((uint)User_Statistics.wTeamWin);
          //      player.Response.Write((uint)User_Statistics.wTeamGames);
          //      player.Response.Write((uint)User_Statistics.LadderPoint);//165
          //      player.Response.Write((uint)User_Statistics.LadderWin);
          //      player.Response.Write((uint)User_Statistics.LadderLose);
          //      player.Response.Write((uint)User_Statistics.LadderDraw);
          //      player.Response.Write((uint)User_Statistics.LadderHole);
          //      player.Response.Write((uint)User_Statistics.ComboCount);
          //      player.Response.Write((uint)User_Statistics.MaxComboCount);
          //      player.Response.Write((uint)User_Statistics.NoMannerGameCount);
          //      player.Response.Write((ulong)User_Statistics.SkinsPang);//201
          //      player.Response.Write((uint)User_Statistics.SkinsWin);
          //      player.Response.Write((uint)User_Statistics.SkinsLose);
          //      player.Response.Write((uint)User_Statistics.SkinsRunHoles);
          //      player.Response.Write((uint)User_Statistics.SkinsStrikePoint);
          //      player.Response.Write((uint)User_Statistics.SkinsAllinCount);
          //      player.Response.Write((byte)0); //Unknow3[0]
          //      player.Response.Write((byte)0); //Unknow3[1]
          //      player.Response.Write((byte)0); //Unknow3[2]
          //      player.Response.Write((byte)0); //Unknow3[3]
          //      player.Response.Write((byte)0); //Unknow3[4]
          //      player.Response.Write((byte)0); //Unknow3[5]
          //      player.Response.Write(User_Statistics.GameCountSeason);
          //      player.Response.Write((byte)0); //Unknow3[0]
          //      player.Response.Write((byte)0); //Unknow3[1]
          //      player.Response.Write((byte)0); //Unknow3[2]
          //      player.Response.Write((byte)0); //Unknow3[3]
          //      player.Response.Write((byte)0); //Unknow3[4]
          //      player.Response.Write((byte)0); //Unknow3[5]
          //      player.Response.Write((byte)0); //Unknow3[6]
          //      player.Response.Write((byte)0); //Unknow3[7]
          //  }
          //  player.SendResponse();
          //  #endregion

          //  #region PlayerGuildInfo
          //  player.Response.Write(new byte[] { 0x5D, 0x01 });
          //  player.Response.Write(UID);
          //  foreach (var GuildData in _db.ProcGuildGetPlayerData(UID, 0))
          //  {
          //      player.Response.Write(GuildData.GUILD_INDEX);//GUILD ID?
          //      player.Response.WriteStr(GuildData.GUILD_NAME, 17);//GUILD NAME(TH > 20, US 17?)
          //      player.Response.Write(0); // guild level point 
          //      player.Response.Write(0); //guild pangs   
          //      player.Response.Write(GuildData.GUILD_TOTAL_MEMBER); //guild total member ok
          //      player.Response.WriteStr(GuildData.GUILD_IMAGE, 12); //GuilD_IMAGE
          //      player.Response.WriteStr(GuildData.GUILD_NOTICE, 101);//GUILD_Notice
          //      player.Response.WriteStr(GuildData.GUILD_INTRODUCING, 101);//GUILD_INTRODUCING
          //      player.Response.Write(GuildData.GUILD_POSITION);// Guild Position
          //      player.Response.Write(GuildData.GUILD_LEADER_UID); //// Guild Leader UID?
          //      player.Response.WriteStr(GuildData.GUILD_LEADER_NICKNAME, 22); //guild leader nickname OK
          //      player.Response.Write(Tools.GetFixTime(GuildData.GUILD_CREATE_DATE));
          //  }
          //  player.SendResponse();
          //  #endregion

          //  #region PlayerRecordInfo(Natural)
          //  player.Response.Write(new byte[] { 0x5C, 0x01, 0x33, });
          //  player.Response.Write(UID);
          //  player.Response.Write(21);
          //  player.Response.Write(JunkMapStatistic2);
          //  player.SendResponse();
          //  #endregion

          //  #region PlayerRecordInfo(GP)
          //  player.Response.Write(new byte[] { 0x5C, 0x01, 0x34, });
          //  player.Response.Write(UID);
          //  player.Response.Write(21);//code
          //  player.Response.Write(JunkMapStatistic2);
          //  player.SendResponse();
          //  #endregion

          //  #region PlayerInfoUnknown
          //  player.Response.Write(new byte[] { 0x5B, 0x01, Session });
          //  player.Response.Write(UID);
          //  player.Response.WriteUInt16(0);
          //  player.SendResponse();
          //  #endregion

          //  #region PlayerTrophyInfo(Special)
          //  player.Response.Write(new byte[] { 0x5A, 0x01, Session });
          //  player.Response.Write(UID);
          //  player.Response.Write(0);
          //  player.Response.Write(0);
          //  player.Response.Write(0);
          //  player.Response.Write(0);
          //  player.SendResponse();
          //  #endregion

          //  #region PlayerTrophyInfo(N)
          //  player.Response.Write(new byte[] { 0x59, 0x01, Session });
          //  player.Response.Write(UID);
          //  player.Response.WriteZero(78);
          //  player.SendResponse();
          //  #endregion

          //  #region PlayerRecordInfo(N)
          //  player.Response.Write(new byte[] { 0x5C, 0x01, Session });
          //  player.Response.Write(UID);
          //  player.Response.Write(21);
          //  player.Response.Write(JunkMapStatistic2);
          //  player.SendResponse();
          //  #endregion

          //  #region PlayerTrophyInfo(GP)
          //  player.Response.Write(new byte[] { 0x57, 0x02, Session });
          //  player.Response.Write(UID);
          //  player.Response.Write(0);
          //  player.Response.Write(0);
          //  player.Response.Write(0);
          //  player.Response.Write(0);
          //  player.SendResponse();
          //  #endregion

          //  #region PlayerResultInfo
          //  player.Response.Write(new byte[] { 0x89, 0x00, 0x01, 0x00, 0x00, 0x00, Session });
          //  player.Response.Write(UID);
          //  player.SendResponse();
          //  #endregion
        }
    }
}
