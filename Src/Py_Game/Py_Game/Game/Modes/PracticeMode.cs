using System;
using System.Linq;
using PangyaAPI;
using PangyaAPI.BinaryModels;
using Py_Game.Defines;
using Py_Game.Client;
using Py_Game.Client.Data;
using Py_Game.Game.Data;
using static Py_Game.GameTools.TGeneric;
using static Py_Game.GameTools.PacketCreator;
using Py_Game.Game.Helpers;
using System.IO;
using PangyaAPI.PangyaPacket;
using PangyaAPI.Tools;
namespace Py_Game.Game.Modes
{
    public class PracticeMode : GameBase
    {
        public PracticeMode(GPlayer player, GameInformation GameInfo, GameEvent CreateEvent, GameEvent UpdateEvent, GameEvent DestroyEvent, PlayerEvent OnJoin, PlayerEvent OnLeave, ushort GameID) : base(player, GameInfo, CreateEvent, UpdateEvent, DestroyEvent, OnJoin, OnLeave, GameID)
        {
        }
        public override void AcquireData(GPlayer player)
        {
            player.GameInfo.GameReady = true;
            WriteConsole.WriteLine($"[PLAYER_REQUEST_ACQUIRE_DATA]: {player.GetNickname}");
            player.GameInfo.GameData.Reverse();
            player.GameInfo.ConnectionID = player.ConnectionID;
            player.GameInfo.UID = player.GetUID;
            player.GameInfo.GameCompleted = false;

            player.GameInfo.Versus.LoadHole = false;
            player.GameInfo.Versus.LoadComplete = false;
            player.GameInfo.Versus.ShotSync = false;

            var packet = new PangyaBinaryWriter();
            packet.Write(new byte[] { 0x76, 0x00 });
            packet.Write((byte)4);
            packet.Write(1); //meu id? 
            packet.Write(GameTools.Tools.GetFixTime(DateTime.Now));
            player.SendResponse(packet.GetBytes());

            packet = new PangyaBinaryWriter();
            packet.Write(new byte[] { 0x45, 0x00, });
            packet.Write(player.Statistic());
            packet.Write(player.Inventory.GetTrophyInfo());
            packet.Write(uint.MaxValue);//XP?
            packet.Write(uint.MaxValue);//XP?
            packet.Write(uint.MaxValue);//xp?
            player.SendResponse(packet);

            packet = new PangyaBinaryWriter();

            packet.Write(new byte[] { 0x52, 0x00 });
            packet.Write(fGameData.Map); //mapa
            packet.Write((byte)4); //type game
            packet.Write(fGameData.Mode);//mode game
            packet.Write(fGameData.HoleTotal); //hole total
            packet.Write(Trophy); //id do trofeu
            packet.Write(fGameData.VSTime);
            packet.Write(fGameData.GameTime);
            packet.Write(GetHoleBuild());
            player.SendResponse(packet.GetBytes());
            UpdatePlayer(player);
        }

        public override void DestroyRoom()
        {
            throw new NotImplementedException();
        }

        public override byte[] GameInformation()
        {
            var response = new PangyaBinaryWriter();

            response.WriteStr(fGameData.Name, 64); //ok
            response.Write(fGameData.Password.Length > 0 ? false : true);
            response.Write(Started == true ? (byte)0 : (byte)1);
            response.Write(Await);//Orange
            response.Write(fGameData.MaxPlayer);
            response.Write((byte)Players.Count);
            response.Write(GameKey, 17);//ultimo byte é zero
            response.Write(fGameData.Time30S);
            response.Write(fGameData.HoleTotal);
            response.Write((byte)4);//GameType
            response.Write((ushort)ID);
            response.Write(fGameData.Mode);
            response.Write(fGameData.Map);
            response.Write(fGameData.VSTime);
            response.Write(fGameData.GameTime);
            response.Write(0);
            response.Write(Idle);
            response.Write(fGameData.GMEvent); //GM Event 0(false), ON 1(true)
            response.WriteZero(66);//GUILD DATA
            response.Write(100);// rate pang 
            response.Write(100);// rate chuva 
            response.Write(Owner);
            response.Write((byte)0x13); //is practice
            response.Write(fGameData.Artifact);//artefato
            response.Write(fGameData.NaturalMode);//natural mode
            response.Write(fGameData.GPTypeID);//Grand Prix 1
            response.Write(fGameData.GPTypeIDA);//Grand Prix 2
            response.Write(fGameData.GPTime);//Grand Time
            response.Write(Iff<uint>(fGameData.GP, 1, 0));// grand prix active
            return response.GetBytes();
        }

        public override byte[] GetGameHeadData()
        {
            var response = new PangyaBinaryWriter();
            response.Write(new byte[] { 0x4A, 0x00, 0xFF, 0xFF });
            response.Write((byte)4);//GameType
            response.Write(fGameData.Map);
            response.Write(fGameData.HoleTotal);
            response.Write(fGameData.Mode);
            if (fGameData.HoleNumber > 0)
            {
                response.Write(fGameData.HoleNumber);
                response.Write(fGameData.LockHole == 7 ? 7 : 0);
            }
            response.Write(fGameData.NaturalMode);
            response.Write(fGameData.MaxPlayer);
            response.Write(fGameData.Time30S);
            response.Write(Idle);  //Room Idle
            response.Write(fGameData.VSTime);
            response.Write(fGameData.GameTime);
            response.Write(0); // trophy typeid
            response.Write(fGameData.Password.Length > 0 ? false : true);
            if (fGameData.Password.Length > 0)
            {
                response.WritePStr(fGameData.Password);
            }
            response.WritePStr(fGameData.Name);
            return response.GetBytes();
        }

        public override void OnPlayerLeave()
        {
            this.Started = false;
        }

        public override void PlayerGameDisconnect(GPlayer player)
        {
            if (Count == 0)
            {
                player.SetGameID(0xFFFF);
                player.SendResponse(ShowLeaveGame());
                PlayerLeave(this, player);
                this.Destroy(this);
                player.Game = null;
                FirstShot = false;
            }
            else
            {
                player.SetGameID(0xFFFF);
                PlayerLeave(this, player);
                OnPlayerLeave();
                Send(ShowGameLeave(player.ConnectionID, 2));
                //{ Find New Master }
                if ((uint)player.GetUID == Owner && Players.Count >= 1)
                    FindNewMaster();

                //{ Room Update }
                Update(this);

                player.SendResponse(ShowLeaveGame());

                player.Game = null;

                FirstShot = false;
            }
        }

        public override void PlayerLeavePractice()
        {


            // { copy score }
            CopyScore();

            //AfterMatchDone();
            GenerateExperience();

            foreach (var P in Players)
                Write(ShowNameScore(P.GetNickname, P.GameInfo.GameData.Score, P.GameInfo.GameData.Pang));


            SendUnfinishedData();

            Write(new byte[] { 0x8C, 0x00 });

            foreach (var P in Players)
            {
                //{ CE 00 }
                SendMatchData(P);
                //{ 33 01 }

            }
            Started = false;
        }

        public new void SendUnfinishedData()
        {
            foreach (var P in PlayerData)
                if (!P.GameCompleted)
                    Write(ShowHoleData(P.ConnectionID, P.HolePos, (byte)P.GameData.TotalShot, (uint)P.GameData.Score, P.GameData.Pang, P.GameData.BonusPang, false));
        }

        public override void PlayerLoading(GPlayer player, Packet packet)
        {
            byte Process;

            Process = packet.ReadByte();

            Send(ShowGameLoading(player.ConnectionID, Process));
            player.GameInfo.Versus.LoadComplete = (Process * 10 >= 80);

            WriteConsole.WriteLine($"[PLAYER_LOADING]: {player.GetNickname}:{Process * 10}%");
        }

        public override void PlayerLoadSuccess(GPlayer client)
        {
            client.GameInfo.Versus.LoadComplete = true;
            client.GameInfo.GameData.HoleComplete = false;
            client.GameInfo.Versus.HoleDistance = 99999999;

            client.Write(ShowMatchTimeUsed(13000));
            client.Write(ShowWhoPlay(client.ConnectionID));
        }

        public override void PlayerSendFinalResult(GPlayer player, Packet packet)
        {
        }

        public override void PlayerShotData(GPlayer player, Packet packet)
        {
            TShotData S;
            player.GameInfo.Versus.ShotSync = false;

            var decrypted = DecryptShot(packet.ReadBytes((int)(packet.GetSize - packet.GetPos)));

            packet.SetReader(new PangyaBinaryReader(new MemoryStream(decrypted)));

            S = (TShotData)packet.Read(new TShotData());
         
            switch (S.ShotType)
            {
                case TShotType.Unknown:
                case TShotType.Normal:
                    player.GameInfo.GameData.Pang += S.Pang;
                    player.GameInfo.GameData.BonusPang += S.BonusPang;
                    player.GameInfo.GameData.ShotCount += 1;
                    player.GameInfo.GameData.TotalShot += 1;
                    break;
                case TShotType.OB:
                    {
                        player.GameInfo.GameData.Pang += S.Pang;
                        player.GameInfo.GameData.BonusPang += S.BonusPang;
                        player.GameInfo.GameData.ShotCount += 2;
                        player.GameInfo.GameData.TotalShot += 2;
                    }
                    break;
                case TShotType.Success:
                    {
                        if (player.GameInfo.GameData.Pang - S.Pang > 4000 || player.GameInfo.GameData.BonusPang - S.BonusPang > 4000)
                        {
                            player.Close();
                            return;
                        }
                        player.GameInfo.GameData.HoleComplete = true;
                        player.GameInfo.GameData.HoleCompletedCount += 1;
                        player.GameInfo.UpdateScore(player.GameInfo.GameData.HoleComplete);
                        if (player.GameInfo.GameData.HoleCompletedCount >= fGameData.HoleTotal)
                        {
                            player.GameInfo.GameCompleted = true;
                        }
                        WriteConsole.WriteLine(player.GameInfo.GameData.Score.ToString());
                    }
                    break;
            }

            packet.Clear();
            packet.Write(new byte[] { 0x6E, 0x00 });
            packet.WriteUInt32(player.ConnectionID);
            packet.WriteByte((byte)player.GameInfo.HolePos);
            packet.WriteSingle(S.Pos.X);
            packet.WriteSingle(S.Pos.Z);
            packet.Write(S.MatchData);
            Send(packet.GetBytes());

            player.GameInfo.Versus.HoleDistance = S.Pos.HoleDistance(player.GameInfo.HolePos3D);
            Console.WriteLine("[PLAYER_HOLE_DISTANCE]: " + player.GameInfo.Versus.HoleDistance);
        }

        public override void PlayerShotInfo(GPlayer player, Packet packet)
        {

            var ShotType = packet.ReadUInt16();
            byte[] UN;
            var resp = new PangyaBinaryWriter();
            resp.Write(new byte[] { 0x55, 0x00 });
            resp.Write(player.ConnectionID);
            switch (ShotType)
            {
                case 1:
                    {
                        packet.Skip(9);
                        UN = packet.ReadBytes(61);
                        resp.Write(UN);
                        packet.Save($"PlayerShotInfo-{(TShotType)ShotType}-{player.GetLogin}");
                    }
                    break;
                default:
                    {
                        UN = packet.ReadBytes(61);
                        resp.Write(UN);
                        packet.Save($"PlayerShotInfo-{(TShotType)ShotType}-{player.GetLogin}");
                    }
                    break;
            }
            Send(resp);
        }

        public override void PlayerStartGame()
        {
            if (Started)
            {
                WriteConsole.WriteLine("[PLAYER_START_GAME]: failed game already started");
                return;
            }

            // { Clear Player Score Data }
            ClearPlayerData();

            //{ Trophy }
            Gold = 0xFFFFFFFF;
            Silver1 = 0xFFFFFFFF;
            Silver2 = 0xFFFFFFFF;
            Bronze1 = 0xFFFFFFFF;
            Bronze2 = 0xFFFFFFFF;
            Bronze3 = 0xFFFFFFFF;

            //{ Medal }
            BestRecovery = 0xFFFFFFFF;
            BestChipIn = 0xFFFFFFFF;
            BestDrive = 0xFFFFFFFF;
            BestSpeeder = 0xFFFFFFFF;
            LongestPutt = 0xFFFFFFFF;
            LuckyAward = 0xFFFFFFFF;

            Started = true;
            Await = true;
            BuildHole();


            Send(new byte[] { 0x30, 0x02 });
            Send(new byte[] { 0x31, 0x02 });

            Send(ShowPangRate());

            Update(this);

            GameStart = DateTime.Now;
        }

        public override void PlayerSyncShot(GPlayer client, Packet packet)
        {
            Send(ShowDropItem(client.ConnectionID));
            var Succeed = client.GameInfo.GameCompleted;

            if (Succeed)
            {
                client.SendResponse(new byte[] { 0x99, 0x01 });
            }
            // { Show Treasure Gauge }
            client.SendResponse(ShowTreasureGuage());
            //{ Show Name,Score,Pang when client finish their game }
            if (Succeed)
            {
                Send(ShowNameScore(client.GetNickname, client.GameInfo.GameData.Score, client.GameInfo.GameData.Pang));

                Send(ShowHoleData(client.ConnectionID, client.GameInfo.HolePos, (byte)client.GameInfo.GameData.TotalShot, (uint)client.GameInfo.GameData.Score, (uint)client.GameInfo.GameData.Pang, (uint)client.GameInfo.GameData.BonusPang));
            }
            else if (client.GameInfo.GameData.HoleComplete)
            {
                Send(ShowHoleData(client.ConnectionID, client.GameInfo.HolePos, (byte)client.GameInfo.GameData.TotalShot, (uint)client.GameInfo.GameData.Score, (uint)client.GameInfo.GameData.Pang, (uint)client.GameInfo.GameData.BonusPang));
            }
            //Send(ShowHoleData(client.ConnectionID, client.GameInfo.HolePos, (byte)client.GameInfo.GameData.TotalShot, (uint)client.GameInfo.GameData.Score, (uint)client.GameInfo.GameData.Pang, (uint)client.GameInfo.GameData.BonusPang));

            if (Succeed)
                Send(ShowLeaveMatch(client.ConnectionID, 2));

            if (_allFinished())
                _onAllPlayerFinished();
        }

        public override void SendHoleData(GPlayer player)
        {
            var H = Holes.CurrentHole;

            if (H == null) return;

            var Data = Holes.CurrentHole;
            player.SendResponse(ShowWind(Data.WindPower, Data.WindDirection));
            player.SendResponse(ShowWeather(Data.Weather));
        }

        public override void SendPlayerOnCreate(GPlayer player)
        {
            var packet = new PangyaBinaryWriter();
            packet.Write(new byte[] { 0x48, 0x00 });
            packet.Write((byte)0);
            packet.Write(new byte[] { 0xFF, 0xFF });
            packet.Write((byte)1);
            packet.Write(player.GetGameInfomations(1));
            packet.Write((byte)0);
            player.SendResponse(packet.GetBytes());
        }

        public override void SendPlayerOnJoin(GPlayer player)
        {
        }

        public override bool Validate()
        {
            if (fGameData.MaxPlayer > 4) { return false; }

            return true;
        }
        private void _onAllPlayerFinished()
        {
            // { copy score }
            CopyScore();

            //AfterMatchDone;
            GenerateExperience();

            //SendUnfinishedData();

            foreach (var P in Players)
            {
                SendMatchData(P);
            }
            Started = false;
        }

        public override void GenerateExperience()
        {
            foreach (var P in Players)
            {
                P.GameInfo.GameData.EXP = new GameExpTable().GetEXP(GAME_TYPE.HOLE_REPEAT, GameData.Map, 0, (byte)Players.Count, P.GameInfo.GameData.HoleCompletedCount);
            }
        }
    }
}
