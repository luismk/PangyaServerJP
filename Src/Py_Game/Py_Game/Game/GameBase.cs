using Py_Game.Defines;
using PangyaAPI;
using PangyaAPI.BinaryModels;
using Py_Game.Client.Data;
using Py_Game.Client;
using Py_Game.Game.Collection;
using Py_Game.Lobby;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static Py_Game.GameTools.PacketCreator;
using Py_Game.Game.Helpers;
using Py_Game.Game.Data;
using Py_Game.Data;
using Py_Game.Game.Modes;
using PangyaAPI.PangyaPacket;
using PangyaAPI.Tools;

namespace Py_Game.Game
{
    public abstract class GameBase
    {
        #region Public Fields
        public UInt32 ID { get; set; }

        public string Password { get { return fGameData.Password; } }

        public GameInformation GameData { get { return fGameData; } set { fGameData = value; } }

        public GAME_TYPE GameType { get { return fGameData.GameType; } }

        public DateTime GameStart { get; set; }
        public DateTime GameEnd { get; set; }

        public uint GetTrophy { get { { return Trophy; } } }
        public byte Count { get { { return (byte)Players.Count; } } }
        public bool HoleComplete { get; set; }
        // Terminating
        public bool Terminating { get; set; } = false;
        public DateTime TerminateTime { get; set; }

        protected GameInformation fGameData;
        public uint PlayConnectionID { get; set; } = uint.MaxValue;
        public Point3D CurrentHole;
        protected uint Owner { get; set; }
        protected bool Started { get; set; }
        protected bool Await { get; set; }
        protected uint Trophy { get; set; }
        protected byte Idle;
        // Trophy Showing
        protected uint Gold { get; set; }
        protected uint Silver1 { get; set; }
        protected uint Silver2 { get; set; }
        protected uint Bronze1 { get; set; }
        protected uint Bronze2 { get; set; }
        protected uint Bronze3 { get; set; }
        // Medal Showing
        protected uint BestRecovery { get; set; }
        protected uint BestChipIn { get; set; }
        protected uint BestDrive { get; set; }
        protected uint BestSpeeder { get; set; }
        protected uint LongestPutt { get; set; }
        protected uint LuckyAward { get; set; }
        protected bool FirstShot { get; set; }
        // Map
        protected byte Map { get; set; }
        public List<GPlayer> Players { get; set; }
        /// <summary>
        /// UID = playerUID
        /// ShopItemData = object
        /// </summary>
        public List<ShopItemData> GameShopData { get; set; }
        protected List<GamePlay> PlayerData { get; set; }
        // UID AND GAMEDATA
        protected Dictionary<uint, DataGame> Scores { get; set; }
        protected GameHoles Holes { get; set; }


        protected byte[] GameKey { get; set; }
        // Event
        protected GameEvent Create { get; set; }//cria sala
        protected GameEvent Update { get; set; }//update sala
        protected GameEvent Destroy { get; set; }//destroy sala
        // Player Event
        protected PlayerEvent PlayerJoin { get; set; }
        protected PlayerEvent PlayerLeave { get; set; }
     
        #endregion

        #region Public Delegate
        public delegate void LobbyEvent(Channel Lobby, GameBase Game);

        public delegate void GameEvent(GameBase Game);

        public delegate void PlayerEvent(GameBase Game, GPlayer Player);

        #endregion

        #region Public Abstract Method's
        //Disconnect Game
        public abstract void PlayerGameDisconnect(GPlayer player);
        //create player 
        public abstract void SendPlayerOnCreate(GPlayer player);
        //join player
        public abstract void SendPlayerOnJoin(GPlayer player);
        public abstract void SendHoleData(GPlayer player);
        public abstract void OnPlayerLeave();
        //check room
        public abstract bool Validate();
        //Gera experiencia(XP)
        public abstract void GenerateExperience();
        public abstract void PlayerLoading(GPlayer player, Packet CP);
        public abstract void PlayerShotInfo(GPlayer player, Packet CP);
        public abstract void PlayerShotData(GPlayer player, Packet CP);
        public abstract void PlayerLoadSuccess(GPlayer player);
        public abstract void PlayerLeavePractice();
        public abstract void PlayerStartGame();
        public abstract void PlayerSyncShot(GPlayer player, Packet CP);
        // Final Result       
        public abstract void PlayerSendFinalResult(GPlayer player, Packet CP);
        public abstract byte[] GameInformation();
        public abstract byte[] GetGameHeadData();
        public abstract void DestroyRoom();
        public abstract void AcquireData(GPlayer player);

        #endregion

        #region Construtor 
        public GameBase(GPlayer player,
            GameInformation GameInfo, GameEvent CreateEvent, GameEvent UpdateEvent, GameEvent DestroyEvent, PlayerEvent OnJoin, PlayerEvent OnLeave, ushort GameID)
        {
            player.InGame = true;
            //{ Create Game Data }
            Players = new List<GPlayer>();
            PlayerData = new List<GamePlay>();
            Scores = new Dictionary<uint, DataGame>();
            Holes = new GameHoles();
            GameShopData = new List<ShopItemData>();
            //{ Game Data }
            fGameData = GameInfo;
            ID = GameID;
            Create = CreateEvent;
            Update = UpdateEvent;
            Destroy = DestroyEvent;
            PlayerJoin = OnJoin;
            PlayerLeave = OnLeave;
            HoleComplete = false;
            CreateKey();
            Terminating = false;
            Started = false;
            Await = false;
            FirstShot = false;
            Trophy = 0;
            Idle = 0;
            if (Validate() == false)
            {
                player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CREATE_FAILED.ShowRoomError());
                return;
            }

            if (Players.Count > fGameData.MaxPlayer)
            {
                player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_RESULT_FULL.ShowRoomError());
                return;
            }
            if (Add(player))
            {
                SetOwner(player.GetUID);
                player.SetGameID((ushort)ID);
                player.GameInfo.SetDefault();
                SetRole(player, true);
                GenerateGameTrophy();
                GameUpdate();
                SendGameInfo();
                ComposePlayer();
                Create(this);
                PlayerJoin(this, player);
                if (GameType == GAME_TYPE.CHAT_ROOM)
                {
                    player.GameInfo.GameReady = true;
                }
                SendPlayerOnCreate(player);
            }
        }
        #endregion

        #region Destroy
        ~GameBase()
        {
            Scores.Clear();
            ClearPlayerData();
            Players.Clear();
        }
        #endregion

        #region Public Method's
        public void CommandReady()
        {
            foreach (var player in Players.Where(c => c.GameInfo.GameReady == false).ToList())
            {
                player.GameInfo.GameReady = true;
                Send(ShowGameReady(player.ConnectionID, 1));
            }
            PlayerStartGame();
        }

        public void Send(byte[] Data)
        {
            foreach (var p in Players)
            {
                p.SendResponse(Data);
            }
        }

        public void Send(PangyaBinaryWriter resp)
        {
            foreach (var p in Players)
            {
                p.SendResponse(resp);
            }
        }

        public void Write(byte[] Data)
        {
            foreach (var p in Players)
            {
                p.SendResponse(Data);
            }
        }

        public void ClearPlayerData()
        {
            PlayerData.Clear();
        }

        public void ClearScoresData()
        {
            Scores.Clear();
        }

        public void SendUnfinishedData()
        {
            foreach (var P in PlayerData)
                if (!P.GameCompleted)
                    Send(ShowHoleData(P.ConnectionID, P.HolePos, (byte)P.GameData.TotalShot, (uint)P.GameData.Score, P.GameData.Pang, P.GameData.BonusPang, false));
        }

        public bool _allFinished()
        {
            foreach (var P in Players)
            {
                if (!P.GameInfo.GameCompleted)
                    return false;
            }
            return true;
        }

        public void CopyScore()
        {
            GamePlay S;

            ClearPlayerData();
            ClearScoresData();
            foreach (var P in Players)
            {
                S = new GamePlay();

                S = P.GameInfo;
                PlayerData.Add(S);
                Scores.Add(P.GetUID, S.GameData);
            }
        }

        public void ClearHole()
        {
            Holes.Dispose();
        }

        public void AddPlayerInEvent(GPlayer player)
        {
            Add(player);

            player.SetGameID((ushort)ID);
            player.GameInfo.SetDefault();
            SetRole(player, true);

            GameUpdate();
            SendGameInfo();
            ComposePlayer();
            SendPlayerOnJoin(player);
        }

        public bool AddPlayer(GPlayer player)
        {
            if (Players.Count > fGameData.MaxPlayer)
            {
                player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_RESULT_FULL.ShowRoomError());
                return false;
            }
            if (Add(player))
            {
                if (Players.Count > 1 && player.GetCapability == 4 || player.GetCapability == 15)
                {
                    Send(ShowNewMaster(player.ConnectionID));
                    Send(ShowGameReady(player.ConnectionID, 0));
                    Owner = player.GetUID;
                }
                player.SetGameID((ushort)ID);
                player.GameInfo.SetDefault();
                SetRole(player, false);
                ComposePlayer();
                GenerateGameTrophy();
                GameUpdate();
                SendGameInfo();
                Update(this);
                PlayerJoin(this, player);
                SendPlayerOnJoin(player);
                player.Game = this;
                return true;
            }
            else
            {
                player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_RESULT_FULL.ShowRoomError());

                return false;
            }
        }

        public void SetGPTime(DateTime StartTime)
        {
            fGameData.GPStart = StartTime;
        }

        public void StartGPCounter()
        {
            var now = DateTime.Now;
            while (true)
            {
                //Aguarda tempo
                Thread.Sleep(TimeSpan.FromSeconds(30));
                try
                {
                    var Minutes = now.Minute - fGameData.GPStart.Minute;
                    var Seconds = fGameData.GPStart.Second - now.Second;

                    Send(ShowGPTimeCounter(Minutes, Seconds));
                }
                catch { }
            }
        }

        public bool RemovePlayer(GPlayer player)
        {
            if (player == null)
            {
                return false;
            }
            else
            {
                Players.Remove(player);
                PlayerGameDisconnect(player);
                return true;
            }
        }

        public void FindNewMaster()
        {
            foreach (var P in Players)
            {
                Write(ShowNewMaster(P.ConnectionID));
                Owner = P.GetUID;
                break;
            }
        }
        #endregion

        #region Protected Method's
        //Cria os dados do hole
        protected void BuildHole()
        {
            Holes.Init((TGAME_MODE)GameData.Mode, GameData.GameType, fGameData.HoleNumber > 0, fGameData.Map, fGameData.HoleTotal);
        }
        //Obtem dados do hole criado
        protected byte[] GetHoleBuild()
        {
            var rnd = new Random();
            using (var result = new PangyaBinaryWriter())
            {
                foreach (var H in Holes)
                {
                    result.Write(rnd.Next());
                    result.Write(H.Pos);
                    result.Write(H.Map);
                    result.Write(H.Hole);
                }
                //CoinData
                result.Write(new byte[]
                {
                0xFF, 0xFF, 0xFF, 0xFF,
                0x05, 0x01, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x41, 0x31, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00,
                0x04, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x68, 0x12,
                0xC4, 0x5A, 0x00, 0x00, 0x00,
                0x00, 0x14, 0x00, 0x00, 0x00, 0x01, 0x00, 0x41, 0x31, 0x5C, 0x5F, 0xBD, 0x43, 0x50, 0xCD, 0x8A, 0xC2, 0x2B, 0xBF, 0x04, 0x44, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xD7, 0xD5, 0xC4, 0x5A, 0x00, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x01, 0x00, 0x41, 0x31, 0xFE, 0xD4, 0xAE, 0x41, 0x93, 0xD8, 0xBA, 0xC2, 0x04, 0x4E, 0x0B, 0x44, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x56, 0x07, 0xC5, 0x5A, 0x00, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x01, 0x00, 0x41, 0x31, 0xC9, 0xB6, 0x96, 0x43, 0xDD, 0xA4, 0x8E, 0xC2, 0xD1, 0x82, 0x3D, 0xC3, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC4, 0x9E, 0xC5, 0x5A, 0x00, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x01, 0x00, 0x41, 0x31, 0xAE, 0xF7, 0xC5, 0x43, 0xEC, 0x11, 0x90, 0xC2, 0x02, 0x0B, 0xE0, 0x43, 0x03, 0x00, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xD5, 0x60, 0xEC, 0x38, 0x00, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x02, 0x01, 0x41, 0x31, 0xB0, 0xF2, 0x8C, 0x42, 0xCB, 0x61, 0x11, 0xC3, 0x3F, 0x75, 0x1A, 0x43, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32, 0x63, 0xEC, 0x38, 0x00, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x02, 0x01, 0x41, 0x31, 0x6D, 0xE7, 0x9D, 0x41, 0xF6, 0xC8, 0x02, 0xC3, 0x17, 0xD9, 0x0F, 0xC2, 0x03, 0x00, 0x00, 0x00, 0x05, 0x01, 0x00, 0x00, 0x00, 0x71, 0x41, 0xAA, 0xAB, 0x00, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x03, 0x02, 0x41, 0x31, 0x89, 0x41, 0xF8, 0x41, 0xD5, 0x98, 0x1C, 0x43, 0xC5, 0x30, 0x98, 0xC3, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x96, 0x99, 0xAA, 0xAB, 0x00, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x03, 0x02, 0x41, 0x31, 0x4C, 0xB7, 0xA2, 0xC2, 0x71, 0x3D, 0x8F, 0xC1, 0x56, 0x5E, 0xF0, 0x43, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE0, 0x13, 0xAB, 0xAB, 0x00, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x03, 0x02, 0x41, 0x31, 0x46, 0xB6, 0x99, 0xC1, 0x08, 0x6C, 0x90, 0x42, 0xA2, 0x05, 0x14, 0xC3, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA7, 0x93, 0xAB, 0xAB, 0x00, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x03, 0x02, 0x41, 0x31, 0x98, 0xAE, 0x88, 0xC2, 0x8F, 0xC2, 0x1B, 0xC1, 0xC7, 0x5B, 0xC4, 0x43, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x5F, 0xBD, 0xAB, 0xAB, 0x00, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x03, 0x02, 0x41, 0x31, 0xC5, 0x60, 0xD0, 0xC2, 0x9A, 0x19, 0x0B, 0x42, 0x0A, 0x97, 0xCA, 0x42, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                });
                var GetBytes = result.GetBytes();
                return GetBytes;
            }
        }
        protected bool CheckItemPractice(uint TypeID)
        {
            var player = Players.First();
            if (player.Inventory.IsExist(TypeID))
            {
                player.Inventory.Remove(TypeID, 1, true);
                return true;
            }
            return false;
        }
        protected void SendGameInfo()
        {
            PangyaBinaryWriter Response;
            Response = new PangyaBinaryWriter();
            try
            {
                Response.Write(new byte[] { 0x49, 0x00, 0x00, 0x00 }); // TODO
                Response.Write(GameInformation());
                Send(Response.GetBytes());
            }
            finally
            {
                Response.Dispose();
            }
        }
        protected bool Add(GPlayer player)
        {
            if (null == player)
            {
                if (GameType == GAME_TYPE.GM_EVENT)
                {
                    SetOwner(uint.MaxValue);
                    return false;
                }
                return false;
            }
            if (Players.Any(c => c.GetLogin == player.GetLogin) == false)
            {

                player.Game = this;
                Players.Add(player);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void SetRole(GPlayer player, bool IsAdmin)
        {
            switch (IsAdmin)
            {
                case true: { player.GameInfo.Role = 0x08; } break;
                case false: { player.GameInfo.Role = 0x01; } break;
            }
            if (GameType == GAME_TYPE.CHAT_ROOM)
            {
                player.GameInfo.GameReady = true;
            }
            if (Players.Count > 1 && player.GetCapability == 4 || player.GetCapability == 15)
            {
                foreach (var client in Players.Where(c => c.GameInfo.Role == 0x08))
                {
                    client.GameInfo.Role = 0x01;
                }
                player.GameInfo.Role = 0x08;
            }
        }

        protected void SetOwner(uint UID)
        {
            Owner = UID;
        }

        protected void ComposePlayer()
        {
            byte i = 0;
            foreach (var P in Players)
            {
                i += 1;
                P.GameInfo.GameSlot = i;
            }
        }

        protected void CreateKey()
        {
            var result = new byte[16];

            new Random().NextBytes(result);

            GameKey = result;
        }

        protected void GameUpdate()
        {
            Send(this.GetGameHeadData());
        }

        public void UpdatePlayer(GPlayer player)
        {
            GPlayer p = Players.Where(c => c.GetLogin == player.GetLogin).FirstOrDefault();
            if (p != null)
            {
                p = player;
            }
        }

        protected byte[] DecryptShot(byte[] data)
        {
            var decrypted = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                decrypted[i] = (byte)(data[i] ^ GameKey[i % 16]);
            }
            return decrypted;
        }
        //Cria o trofeu ID
        protected void GenerateGameTrophy()
        {
            UInt32 SumLevel, AvgLevel;
            SumLevel = 0;

            foreach (var P in Players)
            {
                SumLevel += P.GetLevel;
            }
            if (SumLevel <= 0)
            {
                AvgLevel = 0;
            }
            else
            {
                AvgLevel = (uint)(SumLevel / Players.Count);
            }
            switch (AvgLevel)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    {
                        Trophy = 738197504;
                    }
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    {
                        Trophy = 738263040;
                    }
                    break;
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    {
                        Trophy = 738328576;
                    }
                    break;
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                    {
                        Trophy = 738394112;
                    }
                    break;
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                    {
                        Trophy = 738459648;
                    }
                    break;
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                    {
                        Trophy = 738525184;
                    }
                    break;
                case 31:
                case 32:
                case 33:
                case 34:
                case 35:
                    {
                        Trophy = 738590720;
                    }
                    break;
                case 36:
                case 37:
                case 38:
                case 39:
                case 40:
                    {
                        Trophy = 738656256;
                    }
                    break;
                case 41:
                case 42:
                case 43:
                case 44:
                case 45:
                    {
                        Trophy = 738721792;
                    }
                    break;
                case 46:
                case 47:
                case 48:
                case 49:
                case 50:
                    {
                        Trophy = 738787328;
                    }
                    break;
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                    {
                        Trophy = 738852864;
                    }
                    break;
                case 56:
                case 57:
                case 58:
                case 59:
                case 60:
                    {
                        Trophy = 738918400;
                    }
                    break;
                case 61:
                case 62:
                case 63:
                case 64:
                case 65:
                    {
                        Trophy = 738983936;
                    }
                    break;
                case 66:
                case 67:
                case 68:
                case 69:
                case 70:
                    {
                        Trophy = 738983936;
                    }
                    break;
            }
            switch (GameType)
            {
                case GAME_TYPE.CHAT_ROOM:
                case GAME_TYPE.VERSUS_MATCH:
                case GAME_TYPE.VERSUS_STROKE:
                    Trophy = 0;
                    break;
            }
        }
        #endregion

        #region Game Send Packet's
        protected void PlayerRebuildHole()
        {
            Holes.RebuildGameHole(fGameData.Map);
        }

        public byte[] GetGameInfo()
        {
            using (var packet = new PangyaBinaryWriter())
            {
                packet.Write(new byte[] { 0x86, 0x00 });
                packet.Write(Started == true ? 0 : 1);
                packet.Write(fGameData.HoleTotal);
                if (fGameData.VSTime > 0)
                {
                    packet.Write(fGameData.VSTime);
                }
                else
                {
                    packet.Write(fGameData.GameTime);//fGameData.VSTime
                }
                packet.Write(fGameData.Map);
                packet.Write((byte)GameType);
                packet.WriteByte(fGameData.Mode);
                packet.Write(Trophy);
                packet.Write(0);
                packet.Write(0);
                packet.WriteZero(6);//unknown
                packet.WriteUInt32(1000);//1000
                return packet.GetBytes();
            }
        }

        protected void GoToNextHole()
        {
            if (Holes.GoToNext())
            {
                Send(new byte[] { 0x65, 0x00 });
            }
            else
            {
                SendGameResult();
            }
        }

        protected void SendGameResult()
        {
            var result = new PangyaBinaryWriter();

            result.Write(new byte[] { 0x65, 0x00 });
            result.Write(Count);
            foreach (var player in Players)
            {
                result.Write(player.ConnectionID);
                result.Write(player.GameInfo.GameSlot);
                result.Write((byte)2); // total point
                result.Write((byte)5); // course shot count
                result.Write((ushort)player.GameInfo.GameData.EXP); // player xp
                result.Write(0x67000000);//pang
                result.Write(0);//un
                result.Write(0xD2000000);//bonus pang
                result.Write(0);//un2
                result.Write(0);//un3
                result.Write(0);//un4
            }
            Send(result);
            Started = false;
        }

        protected void SendWind()
        {
            var Weather = Holes.CurrentHole;
            Send(ShowWind(Weather.WindPower, Weather.WindDirection));
        }

        protected void SendWeather()
        {
            var Weather = Holes.CurrentHole;
            Send(ShowWeather(Weather.Weather));
        }

        protected void SendMatchData(GPlayer player)
        {
            player.Response.Write(new byte[] { 0x79, 0x00 });
            player.Response.Write(player.GameInfo.GameData.EXP); //XP
            player.Response.Write(Trophy);
            player.Response.Write(new byte[] { 0x00, 0x02 });
            player.Response.Write(LuckyAward);//luck
            player.Response.Write(0);
            player.Response.Write(BestSpeeder);//bestspeed
            player.Response.Write(0);
            player.Response.Write(BestDrive);//bestdrive
            player.Response.Write(0);
            player.Response.Write(BestChipIn);//bestchipIn
            player.Response.Write(0);
            player.Response.Write(LongestPutt);//LongPutt
            player.Response.Write(0);
            player.Response.Write(BestRecovery);//BestRecovery
            player.Response.Write(0);
            player.Response.Write(Gold);//trophy-Gold
            player.Response.Write(0);
            player.Response.Write(Silver1);//trophy-silve1
            player.Response.Write(0);
            player.Response.Write(Silver2);//trophy-silve2
            player.Response.Write(0);
            player.Response.Write(Bronze1);//trophy-bronze1
            player.Response.Write(0);
            player.Response.Write(Bronze2);//trophy-bronze2
            player.Response.Write(0);
            player.Response.Write(Bronze3);//trophy-bronze3
            player.SendResponse();
        }

        protected void PlayerGamePause(GPlayer player, Packet packet)
        {
            var action = packet.ReadByte();

            if (GameType == GAME_TYPE.VERSUS_STROKE)
            {
                Send(ShowPlayerPauseGame(player.ConnectionID, action));
            }
            else
            {
                player.SendResponse(ShowPlayerPauseGame(player.ConnectionID, action));
            }
        }

        protected void PlayerGameRotate(GPlayer player, Packet packet)
        {
            var Angle = packet.ReadSingle();

            if (GameType == GAME_TYPE.VERSUS_STROKE)
            {
                Send(ShowPlayerRotate(player.ConnectionID, Angle));
            }
            else
            {
                player.SendResponse(ShowPlayerRotate(player.ConnectionID, Angle));
            }
        }

        protected void PlayerChangeClub(GPlayer player, Packet packet)
        {
            var ClubType = packet.ReadByte();

            if (GameType == GAME_TYPE.VERSUS_STROKE)
            {
                Send(ShowPlayerChangeClub(player.ConnectionID, ClubType));
            }
            else
            {
                player.SendResponse(ShowPlayerChangeClub(player.ConnectionID, ClubType));
            }
        }

        protected void PlayerDropBall(Packet packet)
        {
            var Pos = new Point3D();

            Pos = (Point3D)packet.Read(Pos);

            Send(ShowDropBall(Pos));
        }

        protected void PlayerUsePowerShot(GPlayer player, Packet packet)
        {
            var active = (TPOWER_SHOT)packet.ReadByte();

            Send(ShowPowerShot(player.ConnectionID, active));
        }

        protected void PlayerUseRingEffects(GPlayer player, Packet packet)
        {
            Send(ShowRingEffects(player.ConnectionID, packet.GetRemainingData));
        }

        protected void PlayerMatchData(GPlayer player, Packet packet)
        {
            var Response = new PangyaBinaryWriter();
            Response.Write(new byte[] { 0xF7, 0x01 });
            Response.Write(player.ConnectionID);
            Response.Write((byte)player.GameInfo.HolePos);
            Response.Write(packet.ReadBytes(87));
            Send(Response.GetBytes());
        }

        protected void PlayerPutt(GPlayer player, Packet packet)
        {
            uint TypeID = packet.ReadUInt32();

            if (!(TypeID == 0x1BE00016))
            {
                return;
            }
            player.SendResponse(ShowAssistPutting(TypeID, player.GetUID));
        }

        protected void PlayerSendResult(GPlayer player, Packet packet)
        {
            StatisticData Statistics;
            #region Read Data player.PlayerData.User_Statistics
            Statistics = new StatisticData()
            {
                Drive = packet.ReadUInt32(),
                Putt = packet.ReadUInt32(),
                PlayTime = packet.ReadUInt32(),
                ShotTime = packet.ReadUInt32(),
                LongestDistance = packet.ReadSingle(),
                Pangya = packet.ReadUInt32(),
                TimeOut = packet.ReadUInt32(),
                OB = packet.ReadUInt32(),
                DistanceTotal = packet.ReadUInt32(),
                Hole = packet.ReadUInt32(),
                TeamHole = packet.ReadUInt32(),
                HIO = packet.ReadUInt32(),
                Bunker = packet.ReadUInt16(),
                Fairway = packet.ReadUInt32(),
                Albratoss = packet.ReadUInt32(),
                Holein = packet.ReadUInt32(),
                Puttin = packet.ReadUInt32(),
                LongestPutt = packet.ReadSingle(),
                LongestChip = packet.ReadUInt32(),
                EXP = packet.ReadUInt32(),
                Level = packet.ReadByte(),
                Pang = packet.ReadUInt64(),
                TotalScore = packet.ReadUInt32(),
                Score = new byte[5] { packet.ReadByte(), packet.ReadByte(), packet.ReadByte(), packet.ReadByte(), packet.ReadByte() },
                Unknown = packet.ReadByte(),
                MaxPang0 = packet.ReadUInt64(),
                MaxPang1 = packet.ReadUInt64(),
                MaxPang2 = packet.ReadUInt64(),
                MaxPang3 = packet.ReadUInt64(),
                MaxPang4 = packet.ReadUInt64(),
                SumPang = packet.ReadUInt64(),
                GamePlayed = packet.ReadUInt32(),
                Disconnected = packet.ReadUInt32(),
                TeamWin = packet.ReadUInt32(),
                TeamGame = packet.ReadUInt32(),
                LadderPoint = packet.ReadUInt32(),
                LadderWin = packet.ReadUInt32(),
                LadderDraw = packet.ReadUInt32(),
                LadderHole = packet.ReadUInt32(),
                ComboCount = packet.ReadUInt32(),
                MaxCombo = packet.ReadUInt32(),
                NoMannerGameCount = packet.ReadUInt32(),
                SkinsPang = packet.ReadUInt64(),
                SkinsWin = packet.ReadUInt32(),
                SkinsLose = packet.ReadUInt32(),
                SkinsRunHole = packet.ReadUInt32(),
                SkinsStrikePoint = packet.ReadUInt32(),
                SKinsAllinCount = packet.ReadUInt32(),
                Unknown1 = new byte[6] { packet.ReadByte(), packet.ReadByte(), packet.ReadByte(), packet.ReadByte(), packet.ReadByte(), packet.ReadByte() },
                GameCountSeason = packet.ReadUInt32(),
                Unknown2 = new byte[8] { packet.ReadByte(), packet.ReadByte(), packet.ReadByte(), packet.ReadByte(), packet.ReadByte(), packet.ReadByte(), packet.ReadByte(), packet.ReadByte() },
            };
            packet.Save("PlayerStatistic");
            #endregion
            player.GameInfo.GameData.Statistic = Statistics;
            player.GameInfo.GameData.HoleComplete = true;
            HoleComplete = true;
        }

        protected void PlayerGameReady(GPlayer player, Packet packet)
        {
            var S = packet.ReadByte();

            player.GameInfo.GameReady = S > 0;

            Send(ShowGameReady(player.ConnectionID, S));
        }

        protected void PlayerUseItem(GPlayer PL, Packet packet)
        {
            var TypeID = packet.ReadUInt32();

            if (GameType != GAME_TYPE.TOURNEY)
            {
                Send(ShowPlayerUseItem(PL.ConnectionID, TypeID));
            }
            else
            {
                PL.SendResponse(ShowPlayerUseItem(PL.ConnectionID, TypeID));
            }
            PL.Inventory.Remove(TypeID, 1, true);
            PL.SendTransaction();

            if (GameTools.Tools.GetItemGroup(TypeID) == 2) { PL.Inventory.ItemSlot.Remove(); }
        }

        protected void PlayerUseBooster(GPlayer player)
        {
            if (GameType == GAME_TYPE.VERSUS_STROKE)
            {
                Send(ShowPlayerTimeBoost(player.ConnectionID));
            }
            else
            {
                player.SendResponse(ShowPlayerTimeBoost(player.ConnectionID));
            }
        }

        protected void PlayerChatIcon(GPlayer player, Packet packet)
        {
            var IconType = packet.ReadUInt16();

            Send(ShowGameIcon(player.ConnectionID, IconType));
        }

        protected void PlayerSleepIcon(GPlayer player, Packet packet)
        {
            var I = packet.ReadByte();

            Write(ShowSleep(player.ConnectionID, I));

            PlayerJoin(this, player);
        }

        protected void PlayerAnimalEffect(GPlayer player)
        {
            Send(ShowAnimalEffect(player.ConnectionID));
        }

        protected void PlayerHoleData(GPlayer PL, Packet packet)
        {
            var H = (HoleData)packet.Read(new HoleData());
            
            CurrentHole = new Point3D()
            {
                X = H.X,
                Z = H.Z
            };

            PL.GameInfo.HolePos3D.X = H.X;
            PL.GameInfo.HolePos3D.Z = H.Z;
            PL.GameInfo.HolePos = H.HolePosition;
            PL.GameInfo.GameData.ParCount = (sbyte)H.Par;
            if (!((fGameData.NaturalMode & 2) == 0))
            {
                switch (H.Par)
                {
                    case 4:
                        PL.GameInfo.GameData.ShotCount = 2;
                        break;
                    case 5:
                        PL.GameInfo.GameData.ShotCount = 3;
                        break;
                    default:
                        {
                            PL.GameInfo.GameData.ShotCount = 1;
                        }
                        break;
                }
            }
            else
            {
                PL.GameInfo.GameData.ShotCount = 1;
            }
            SendHoleData(PL);
        }

        protected void PlayerSendInvitation(GPlayer player, Packet packet)
        {
            GPlayer client;
            if (!packet.ReadPStr(out string nick))
            {
                return;
            }
            if (!packet.ReadUInt32(out uint PlayerUID))
            {
                return;
            }

            client = (GPlayer)player.Server.GetPlayerByUID(PlayerUID);

            if (client == null) { return; }

            var result = new PangyaBinaryWriter();

            result.Write(new byte[] { 0x48, 0x00, 0x01, 0xFF, 0xFF });
            result.Write(client.ConnectionID);
            result.WriteZero(39);
            result.WriteByte(2);
            result.WriteZero(57);
            result.Write(PlayerUID);
            result.WriteZero(228);
            result.WriteUInt64(1);
            result.WriteZero(1);
            Send(result.GetBytes());

            client.SendResponse(result.GetBytes());

            result = new PangyaBinaryWriter();

            result.Write(new byte[] { 0x83, 0x00 });
            result.Write(new byte[] { 0x00, 0x00 });
            result.Write(player.Server.Data.UID);
            result.Write(player.Lobby.Id);
            result.WriteUInt16(0);// 0= enter, 2= falied?
            result.Write(PlayerUID);
            result.WritePStr(nick);
            result.Write(PlayerUID);
            Send(result.GetBytes());

            player.SendResponse(result.GetBytes());
        }

        protected void PlayerMasterKickPlayer(GPlayer player, Packet packet)
        {
            if (!packet.ReadUInt32(out uint PlayerUID)) { return; }

            try
            {
                var Client = (GPlayer)player.Server.GetPlayerByUID(PlayerUID);

                if (Client != null)
                {
                    Players.Remove(Client);

                    Send(ShowGameLeave(Client.ConnectionID, 2));
                    Update(this);

                    PlayerJoin(this, Client);

                    Client.SendResponse(ShowLeaveGame());
                }
            }
            catch
            {
                player.Close();
            }
        }

        protected void PlayerGameSetting(Packet packet)
        {
            packet.Skip(2);
            packet.ReadByte(out byte count); //contagem 
            for (int i = 1; i <= count; ++i)
            {
                packet.ReadByte(out byte action);//action enum

                switch ((TGameShift)action)
                {
                    case TGameShift.SHIFT_NAME: //name
                        {
                            packet.ReadPStr(out string NAME);
                            if (NAME.Length > 0)
                                fGameData.Name = NAME;
                        }
                        break;
                    case TGameShift.SHIFT_PWD: //senha
                        {
                            packet.ReadPStr(out string PWD);
                            if (PWD.Length > 0)
                                fGameData.Password = PWD;
                        }
                        break;
                    case TGameShift.SHIFT_STROK: //game_type
                        {
                            packet.ReadByte(out byte Game_Type);

                            fGameData.GameType = (GAME_TYPE)Game_Type;
                        }
                        break;
                    case TGameShift.SHIFT_MAP: //MAP
                        {
                            packet.ReadByte(out fGameData.Map);
                        }
                        break;
                    case TGameShift.SHIFT_NUMHOLE: //hole total
                        {
                            packet.ReadByte(out fGameData.HoleTotal);
                        }
                        break;
                    case TGameShift.SHIFT_MODE: //mode
                        {
                            packet.ReadByte(out fGameData.Mode);
                        }
                        break;
                    case TGameShift.SHIFT_VSTIME: //vs_time = turntime
                        {
                            packet.ReadByte(out byte VSTIME);

                            fGameData.VSTime = (uint)VSTIME * 1000;
                        }
                        break;
                    case TGameShift.SHIFT_MAXPLAYER: //max_player
                        {
                            packet.ReadByte(out byte MaxPlayer);

                            if (MaxPlayer <= Players.Count)
                                fGameData.MaxPlayer = MaxPlayer;
                        }
                        break;
                    case TGameShift.SHIFT_MATCHTIME: //Match_time = game time
                        {
                            packet.ReadByte(out byte GameTime);

                            fGameData.GameTime = (uint)(60 * GameTime) * 1000;
                        }
                        break;
                    case TGameShift.SHIFT_IDLE: //Idle
                        {
                            packet.ReadByte(out Idle);
                        }
                        break;
                    case TGameShift.SHIFT_HOLENUM: //HoleNumber
                        {
                            packet.ReadByte(out byte HoleNumber);

                            fGameData.HoleNumber = HoleNumber;
                        }
                        break;
                    case TGameShift.SHIFT_HOLELOCK: //LockHole
                        {
                            packet.ReadUInt32(out uint LockHole);

                            fGameData.LockHole = LockHole;
                        }
                        break;
                    case TGameShift.SHIFT_NATURAL: //natural mode
                        {
                            packet.ReadUInt32(out uint NaturalMode);

                            fGameData.NaturalMode = NaturalMode;
                        }
                        break;
                    default:
                        WriteConsole.WriteLine($"[PlayerGameSetting]: Unknown Setting type: [{action}] Count > {count} ");
                        break;
                }
            }
            GameUpdate();
            Update(this);
        }

        protected void PlayerFirstShotReady()
        {
            Send(ShowFirstShotReady());
            FirstShot = true;
        }

        protected void PlayerEnterToRoom(GPlayer player,Packet packet)
        {
            var ConID = packet.ReadUInt32();

            Send(ShowRoomEntrance(player.ConnectionID));
        }

        protected void PlayerAction(GPlayer player, Packet packet)
        {
            try
            {
                player.GameInfo.Action.Message = packet.GetRemainingData;

                Send(ShowPlayerAction(player.ConnectionID, packet.GetRemainingData));
                if (!packet.ReadByte(out byte Action))
                {
                    return;
                }


                if (GameType == GAME_TYPE.CHAT_ROOM && Action == 4)
                {
                    //Send(ShowRoomEntrance(player.ConnectionID, 15));
                }
                switch ((TPLAYER_ACTION)Action)
                {
                    case TPLAYER_ACTION.PLAYER_ACTION_ROTATION:
                        {
                        }
                        break;
                    case TPLAYER_ACTION.PLAYER_ACTION_APPEAR:
                        {
                            player.GameInfo.Action.Vector = (Point3D)packet.Read(new Point3D());
                        }
                        break;
                    case TPLAYER_ACTION.PLAYER_ACTION_SUB:
                        {
                            if (!packet.ReadUInt32(out player.GameInfo.Action.Posture))
                            {
                                return;
                            }
                        }
                        break;
                    case TPLAYER_ACTION.PLAYER_ACTION_MOVE:
                        {
                            var move = new Point3D();
                            player.GameInfo.AddWalk((Point3D)packet.Read(move));
                            Console.WriteLine("[PLAYER_MOVE] => " + player.GameInfo.Action.Distance(move));
                        }
                        break;
                    case TPLAYER_ACTION.PLAYER_ACTION_ANIMATION:
                        {
                            player.GameInfo.Action.Message = packet.GetRemainingData;
                        }
                        break;
                    case TPLAYER_ACTION.PLAYER_ACTION_HEADER:
                        {
                            if (!packet.ReadUInt32(out player.GameInfo.Action.Animate))
                            {
                                return;
                            }
                        }
                        break;
                   
                    case TPLAYER_ACTION.PLAYER_ANIMATION_WITH_EFFECTS:
                        {
                        }
                        break;
                    case TPLAYER_ACTION.PLAYER_ACTION_NULL:
                    case TPLAYER_ACTION.PLAYER_ACTION_UNK:
                    default:
                        {
                            packet.Log();
                        }
                        break;
                }

            }
            finally
            {
            }
        }

        #endregion

        #region Handle_Game_Packet's
        public void HandlePacket(TGAMEPACKET ID, GPlayer player, Packet packet)
        {
            switch (ID)
            {
                case TGAMEPACKET.PLAYER_SEND_INVITE_CONFIRM:
                    {

                    }
                    break;
                case TGAMEPACKET.PLAYER_SEND_INVITE:
                    {
                        PlayerSendInvitation(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_USE_ITEM:
                    {
                        PlayerUseItem(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_PRESS_READY:
                    {
                        PlayerGameReady(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_START_GAME:
                    {
                        PlayerStartGame();
                    }
                    break;
                case TGAMEPACKET.PLAYER_LOAD_OK:
                    {
                        PlayerLoadSuccess(player);
                    }
                    break;
                case TGAMEPACKET.PLAYER_SHOT_DATA:
                    {
                        PlayerShotData(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_ENTER_TO_ROOM:
                    {
                        PlayerEnterToRoom(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_ACTION:
                    {
                        PlayerAction(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_CLOSE_SHOP:
                case TGAMEPACKET.PLAYER_OPEN_SHOP:
                case TGAMEPACKET.PLAYER_ENTER_SHOP:
                case TGAMEPACKET.PLAYER_EDIT_SHOP_NAME:
                case TGAMEPACKET.PLAYER_SHOP_ITEMS:
                case TGAMEPACKET.PLAYER_BUY_SHOP_ITEM:
                case TGAMEPACKET.PLAYER_SHOP_CREATE_VISITORS_COUNT:
                case TGAMEPACKET.PLAYER_SHOP_PANGS:
                case TGAMEPACKET.PLAYER_SHOP_VISITORS_COUNT:
                    {
                        ((ModeChatRoom)this).HandleModeChatRoomShop(ID, player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_MASTER_KICK_PLAYER:
                    {
                        PlayerMasterKickPlayer(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_CHANGE_GAME_OPTION:
                    {
                        PlayerGameSetting(packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_1ST_SHOT_READY:
                    {
                        if (FirstShot == false && player.GameInfo.Role == 0x08)
                        {
                            PlayerFirstShotReady();
                        }
                    }
                    break;
                case TGAMEPACKET.PLAYER_LOADING_INFO:
                    {
                        PlayerLoading(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_GAME_ROTATE:
                    {
                        PlayerGameRotate(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_CHANGE_CLUB:
                    {
                        PlayerChangeClub(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_GAME_MARK:
                    {
                    }
                    break;
                case TGAMEPACKET.PLAYER_ACTION_SHOT:
                    {
                        PlayerShotInfo(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_SHOT_SYNC:
                    {
                        PlayerSyncShot(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_HOLE_INFORMATIONS:
                    {
                        PlayerHoleData(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_REQUEST_ANIMALHAND_EFFECT:
                    {
                        PlayerAnimalEffect(player);
                    }
                    break;
                case TGAMEPACKET.PLAYER_MY_TURN:
                    {
                    }
                    break;
                case TGAMEPACKET.PLAYER_HOLE_COMPLETE:
                    {
                        PlayerSendResult(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_CHAT_ICON:
                    {
                        PlayerChatIcon(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_SLEEP_ICON:
                    {
                        PlayerSleepIcon(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_MATCH_DATA:
                    {
                        PlayerMatchData(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_MOVE_BAR:
                    {

                    }
                    break;
                case TGAMEPACKET.PLAYER_PAUSE_GAME:
                    {
                        PlayerGamePause(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_QUIT_SINGLE_PLAYER:
                    {
                        PlayerLeavePractice();
                    }
                    break;
                case TGAMEPACKET.PLAYER_CALL_ASSIST_PUTTING:
                    {
                        PlayerPutt(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_USE_TIMEBOOSTER:
                    {
                        PlayerUseBooster(player);
                    }
                    break;
                case TGAMEPACKET.PLAYER_DROP_BALL:
                    {
                        PlayerDropBall(packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_CHANGE_TEAM:
                    {
                    }
                    break;
                case TGAMEPACKET.PLAYER_VERSUS_TEAM_SCORE:
                    {
                    }
                    break;
                case TGAMEPACKET.PLAYER_POWER_SHOT:
                    {
                        PlayerUsePowerShot(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_WIND_CHANGE:
                    {
                        PlayerRebuildHole();
                    }
                    break;
                case TGAMEPACKET.PLAYER_SEND_GAMERESULT:
                    {
                        PlayerSendFinalResult(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_REQUEST_RING_EFFECTS:
                    {
                        PlayerUseRingEffects(player, packet);
                    }
                    break;
                default:
                    break;
            }
            UpdatePlayer(player);
        }
        #endregion
    }
}
