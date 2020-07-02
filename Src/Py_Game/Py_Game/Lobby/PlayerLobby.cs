using System;
using System.Linq;
using Py_Game.Game;
using Py_Game.Defines;
using PangyaAPI;
using PangyaAPI.BinaryModels;
using Py_Game.Client;
using static Py_Game.GameTools.PacketCreator;
using static Py_Game.GameTools.Tools;
using static PangyaFileCore.IffBaseManager;
using static Py_Game.Lobby.Collection.ChannelCollection;
using Py_Game.Game.Modes;
using Py_Game.Game.Collection;
using Py_Game.Game.Data;
using PangyaAPI.Tools;
using PangyaAPI.PangyaPacket;

namespace Py_Game.Lobby
{
    public class Channel
    {
        public GenericDisposableCollection<GPlayer> Players { get; set; }
        public GameList Games { get; set; }
        public Channel this[GPlayer player] => LobbyList.GetLobby(player.Lobby);
        public GameBase this[ushort GameID] => GetGameHandle(GameID);
        public bool IsFull { get { return Players.Count >= MaxPlayers; } }
        public byte PlayersCount { get { return (byte)PlayersInLobby(); } }
        public byte Id { get; set; }

        public string Name { get; set; }

        public ushort MaxPlayers { get; set; }

        public uint Flag { get; set; }

        public Channel(string name, ushort maxPlayers, byte id, uint flag)
        {
            Name = name;
            MaxPlayers = maxPlayers;
            Id = id;
            Flag = flag;
            Games = new GameList(this);
            Players = new GenericDisposableCollection<GPlayer>();
        }

        public bool AddPlayer(GPlayer player)
        {
            if (Players.Model.Any(c => c == player) == false)
            {
                Players.Add(player);

                player.Lobby = this; //Define no Player qual lobby ele está
                return true;
            }
            return false;
        }


        public byte[] Build()
        {
            return LobbyInfo(Name, MaxPlayers, Convert.ToUInt16(Players.Count), Id, Flag);
        }

        public byte[] BuildGameLists()
        {
            ushort Count = 0;
            var result = new PangyaBinaryWriter();
            var packet = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x47, 0x00 });
            foreach (var Game in Games)
            {
                if (Game.GameType == GAME_TYPE.HOLE_REPEAT || Game.Terminating)
                    continue;
                packet.Write(Game.GameInformation());
                Count++;
            }
            result.WriteUInt16(Count);
            result.WriteUInt16(0xFFFF);
            result.Write(packet.GetBytes());
            return result.GetBytes();
        }

        public byte[] BuildPlayerLists()
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x46, 0x00 });
            result.WriteByte(4);
            result.WriteByte(PlayersCount);
            foreach (GPlayer player in Players.Model)
            {
                if (player.InLobby)
                {
                    result.Write(player.GetLobbyInfo());
                }
            }
            return result.GetBytes();
        }


        public void DestroyGame(GameBase GameHandle)
        {
            DestroyGameEvent(GameHandle);

            GameHandle.Terminating = true;
            GameHandle.TerminateTime = DateTime.Now;
            //{ remove from game list }
            Games.GameRemove(GameHandle);
        }


        public GameBase GetGameHandle(GPlayer player)
        {
            return Games.GetByID(player.GameID);
        }

        public GameBase GetGameHandle(ushort GameID)
        {
            return Games.GetByID(GameID);
        }

        public GPlayer GetPlayerByConnectionId(UInt32 ConnectionId)
        {
            foreach (GPlayer Client in Players.Model)
            {
                if (Client.ConnectionID == ConnectionId)
                {
                    return Client;
                }
            }
            return null;
        }
        //02 : The Room is full
        //03 : The Room is not exist
        //04 : wrong password
        //05 : you cannot get in this room level
        //07 : can not create game
        //08 : game is in progress
        public void PlayerCreateGame(GPlayer player, Packet packet)
        {
            GameInformation GameData;

            GameData = new GameInformation
            {
                Unknown1 = packet.ReadByte(),//1
                VSTime = packet.ReadUInt32(),//5/
                GameTime = packet.ReadUInt32(),//9
                MaxPlayer = packet.ReadByte(),//10
                GameType = (GAME_TYPE)packet.ReadByte(),//11
                HoleTotal = packet.ReadByte(),//12
                Map = packet.ReadByte(),//13
                Mode = packet.ReadByte(),//14
                NaturalMode = packet.ReadUInt32(),//18
            };

            //Course = 63, hole repeted = 68, chip-in = 73
            if (GameData.GameType == GAME_TYPE.HOLE_REPEAT && packet.GetSize == 68)
            {
                packet.Skip(5);
                GameData.HoleNumber = 1;
                GameData.LockHole = 7;
                GameData.NaturalMode = 0;
                GameData.Mode = (byte)TGAME_MODE.GAME_MODE_REPEAT;
            }
            if (GameData.GameType == GAME_TYPE.HOLE_REPEAT && packet.GetSize == 63)
            {
                GameData.HoleNumber = 0;
                GameData.LockHole = 0;
            }
            packet.ReadPStr(out GameData.Name);
            packet.ReadPStr(out GameData.Password);
            packet.ReadUInt32(out GameData.Artifact);

            GameData.GP = false;
            GameData.GPTypeID = 0;
            GameData.GPTypeIDA = 0;
            GameData.GPTime = 0;
            // { GM Event } && { Chat Room }
            if (player.GetCapability == 4 && GameData.MaxPlayer >= 100 || GameData.GameType == GAME_TYPE.CHAT_ROOM && player.GetCapability == 4)
            {
                GameData.GMEvent = true;
            }

            var GameHandle = CreateGame(player, GameData);
            if (GameHandle != null)
            {
                WriteConsole.WriteLine($"[CREATE ROOM]: GAMERESULT = Sucess, Type: {GameData.GameType}", ConsoleColor.Green);
            }
            else
            {
                WriteConsole.WriteLine($"[CREATE ROOM]: GAMERESULT = Failed, Type: {GameData.GameType} ", ConsoleColor.Red);
            }
        }

        public void PlayerJoinGrandPrix(GPlayer player, Packet packet)
        {
            DateTime ReDate(DateTime GPDate)
            {
                if (GPDate < DateTime.Now)
                {
                    return GPDate.AddDays(1);
                }
                return GPDate;
            }

            Boolean Check(ushort Hour, ushort Min, ushort HourEnd, ushort MinEnd)
            {
                DateTime OpenDatetime;
                DateTime EndDatetime;

                if (Hour == 0 && Min == 0 && HourEnd == 0 && MinEnd == 0)
                {
                    return true;
                }

                OpenDatetime = CreateGPDateTime(Hour, Min);
                EndDatetime = ReDate(CreateGPDateTime(HourEnd, MinEnd));
                if (DateTime.Now >= OpenDatetime && DateTime.Now < EndDatetime)
                {
                    return true;
                }
                return false;
            }

            Boolean GPProgram(ushort Hour, ushort Min)
            {
                return (Hour > 0 || Min > 0);
            }

            packet.ReadUInt32(out uint GPTypeID);

            if (IffEntry.GrandPrix.IsGPExist(GPTypeID) == false)
            {
                player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CREATE_FAILED.ShowRoomError());
                WriteConsole.WriteLine("PlayerJoinGrandPrix: GrandPrix typeid is not existed.");
                return;
            }


            var GP = IffEntry.GrandPrix.GetGP(GPTypeID);

            var GPGame = GetGameHandle((ushort)GPTypeID);

            if (GPGame == null || GP.IsNovice)
            {
                if (!Check(GP.Hour_Open, GP.Min_Open, GP.Hour_End, GP.Min_End))
                {
                    player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CREATE_FAILED.ShowRoomError());
                    return;
                }
                var GameInfo = new GameInformation()
                {
                    GameTime = 0,
                    VSTime = 0,
                    MaxPlayer = 30,
                    GameType = GAME_TYPE.GRANDPRIX,
                    HoleTotal = GP.TotalHole,
                    Map = (byte)GP.Map,
                    Mode = (byte)GP.Mode,
                    NaturalMode = GP.Natural,
                    GP = true,
                    GPTypeID = GP.TypeID,
                    GPTypeIDA = GP.TypeGP == 0 ? 256 : GP.TypeGP,
                    GPTime = (uint)(GP.TimeHole * 1000),
                    GMEvent = false,
                    Name = GP.Name,
                    Password = "",
                    Artifact = GP.Artifact
                };
                GPGame = CreateGame(player, GameInfo);

                if (GPProgram(GP.Hour_Program, GP.Min_Program))
                {
                    GPGame.SetGPTime(ReDate(CreateGPDateTime(GP.Hour_Program, GP.Min_Program)));
                }
            }

            if (GPGame != null && GPGame.GameData.GPTime > 0)
            {
                GPGame.SendPlayerOnJoin(player);
            }
            player.SendResponse(ShowWhoPlay(player.ConnectionID));
            ////40 00 B0000000 BB 78 00 00 BB
            if (GPGame != null && GPGame.GameData.GPTime > 0)
            {

            }
        }

        public void PlayerLeaveGame(GPlayer player)
        {
            var GameHandle = player.Game;

            if (GameHandle == null)
            {
                return;
            }
            GameHandle.RemovePlayer(player);
        }

        public void PlayerLeaveGP(GPlayer player)
        {
            var GameHandle = player.Game;

            if (GameHandle == null)
            {
                return;
            }

            GameHandle.RemovePlayer(player);

            player.SendResponse(new byte[] { 0xBA, 0x00 }, GameTime());
            player.SendResponse(new byte[] { 0x54, 0x02, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF });
        }

        public void PlayerJoinGame(GPlayer player, Packet packet)
        {
            packet.ReadUInt16(out ushort GameID);
            packet.ReadPStr(out string Pass);

            GameBase GameHandle = this[GameID];

            if (GameHandle == null)
            {
                player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_ROOM_DONT_EXISTS.ShowRoomError());
                return;
            }

            if (GameHandle.Password.Length > 0 && player.GetCapability < 4)
            {
                if (GameHandle.Password != Pass)
                {
                    player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_INCORRECT_PASSWORD.ShowRoomError());
                    return;
                }
            }

            if (GameHandle.Count == 0 && GameHandle.GameType == GAME_TYPE.GM_EVENT)
            {
                GameHandle.AddPlayerInEvent(player);
            }
            else
            {
                GameHandle.AddPlayer(player);
            }

        }

        public void PlayerRequestGameInfo(GPlayer player, Packet packet)
        {
            packet.ReadUInt16(out ushort GameID);

            GameBase GameHandle = this[GameID];
            if (GameHandle == null)
                return;


            player.SendResponse(GameHandle.GetGameInfo()); // : TODO ---------------------
        }

        public void JoinMultiplayerGamesList(GPlayer player)
        {
            WriteConsole.WriteLine($"TOTAL GAMES IN LOBBY: {Games.Count}", ConsoleColor.Green);

            if (player.InLobby)
                return;
            // Set Current User To Lobby
            player.InLobby = true;
            //Send List Players in Lobby
            Send(BuildPlayerLists());

            player.SendResponse(BuildGameLists());

            if (Players.Count > 1)
                Send(ShowPlayerAction(player, TLOBBY_ACTION.CREATE));
        }

        public void LeaveMultiplayerGamesList(GPlayer player)
        {
            if (player.InLobby)
            {
                player.InLobby = false; // Set Current User To Lobby
                // Send to All player
                Send(ShowPlayerAction(player, TLOBBY_ACTION.DESTROY));
            }
        }

        internal GameBase CreateGame(GPlayer player, GameInformation GameData)
        {
            if (Games.Count >= 10)
            {
                player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CREATE_FAILED.ShowRoomError());
                return null;
            }

            GameBase result = null;

            switch (GameData.GameType)
            {
                case GAME_TYPE.VERSUS_STROKE:
                case GAME_TYPE.VERSUS_MATCH:
                    {
                        result = new ModeVersus(player, GameData, CreateGameEvent, UpdateGameEvent, DestroyGame, PlayerJoinGameEvent, PlayerLeaveGameEvent, Games.GetID);
                    }
                    break;
                case GAME_TYPE.CHAT_ROOM:
                    {
                        result = new ModeChatRoom(player, GameData, CreateGameEvent, UpdateGameEvent, DestroyGame, PlayerJoinGameEvent, PlayerLeaveGameEvent, Games.GetID);
                    }
                    break;
                case GAME_TYPE.GAME_TYPE_03:
                    {
                        player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CANT_CREATE.ShowRoomError());
                    }
                    break;
                case GAME_TYPE.TOURNEY:
                    {
                     //   result = new Torney(player, GameData, CreateGameEvent, UpdateGameEvent, DestroyGame, PlayerJoinGameEvent, PlayerLeaveGameEvent, Games.GetID);
                    }
                    break;
                case GAME_TYPE.TOURNEY_TEAM:
                    {
                        player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CANT_CREATE.ShowRoomError());
                    }
                    break;
                case GAME_TYPE.TOURNEY_GUILD:
                    {
                        player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CANT_CREATE.ShowRoomError());
                    }
                    break;
                case GAME_TYPE.PANG_BATTLE:
                    {
                        player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CANT_CREATE.ShowRoomError());
                    }
                    break;
                case GAME_TYPE.GAME_TYPE_08:
                    {
                        player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CANT_CREATE.ShowRoomError());
                    }
                    break;
                case GAME_TYPE.GAME_TYPE_09:
                    {
                        player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CANT_CREATE.ShowRoomError());
                    }
                    break;
                case GAME_TYPE.GAME_APROACH:
                    {
                        player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CANT_CREATE.ShowRoomError());
                    }
                    break;
                case GAME_TYPE.GM_EVENT:
                    {
                      ///  result = new GMEvent(player, GameData, CreateGameEvent, UpdateGameEvent, DestroyGame, PlayerJoinGameEvent, PlayerLeaveGameEvent, Games.GetID);
                    }
                    break;
                case GAME_TYPE.GAME_TYPE_0C:
                    {
                        player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CANT_CREATE.ShowRoomError());
                    }
                    break;
                case GAME_TYPE.GAME_ZOD_OFF:
                    {
                        player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CANT_CREATE.ShowRoomError());
                    }
                    break;
                case GAME_TYPE.CHIP_IN_PRACTICE:
                    {
                      //  result = new PracticeChip(player, GameData, CreateGameEvent, UpdateGameEvent, DestroyGame, PlayerJoinGameEvent, PlayerLeaveGameEvent, Games.GetID);
                    }
                    break;
                case GAME_TYPE.GAME_TYPE_0F:
                    {
                        player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CANT_CREATE.ShowRoomError());
                    }
                    break;
                case GAME_TYPE.GAME_TYPE_10:
                    {
                        player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CANT_CREATE.ShowRoomError());
                    }
                    break;
                case GAME_TYPE.GAME_TYPE_11:
                    {
                        player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CANT_CREATE.ShowRoomError());
                    }
                    break;
                case GAME_TYPE.SSC:
                    {
                        player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CANT_CREATE.ShowRoomError());
                    }
                    break;
                case GAME_TYPE.HOLE_REPEAT:
                    {
                      result = new PracticeMode(player, GameData, CreateGameEvent, UpdateGameEvent, DestroyGame, PlayerJoinGameEvent, PlayerLeaveGameEvent, Games.GetID);
                    }
                    break;
                case GAME_TYPE.GRANDPRIX:
                    {
                       // result = new GrandPrix(player, GameData, CreateGameEvent, UpdateGameEvent, DestroyGame, PlayerJoinGameEvent, PlayerLeaveGameEvent, Games.GetID);
                    }
                    break;
            }

            if (result == null)
            {
                player.SendResponse(TGAME_CREATE_RESULT.CREATE_GAME_CANT_CREATE.ShowRoomError());
            }
            else
            {
                Games.Create(result);
            }
            return result;
        }

        public void CreateGameEvent(GameBase GameHandle)
        {
            if (GameHandle == null || GameHandle.GameType == GAME_TYPE.HOLE_REPEAT)
            {
                return;
            }
            Send(ShowGameAction(GameHandle.GameInformation(), TGAME_ACTION.CREATE));
        }

        public void DestroyGameEvent(GameBase GameHandle)
        {
            if (GameHandle == null || GameHandle.GameType == GAME_TYPE.HOLE_REPEAT)
            {
                return;
            }
            Send(ShowGameAction(GameHandle.GameInformation(), TGAME_ACTION.DESTROY));
        }


        public void PlayerJoinGameEvent(GameBase GameHandle, GPlayer player)
        {
            if (GameHandle == null || GameHandle.GameType == GAME_TYPE.HOLE_REPEAT)
            {
                return;
            }
            Send(ShowPlayerAction(player, TLOBBY_ACTION.UPDATE));
        }

        public void PlayerLeaveGameEvent(GameBase GameHandle, GPlayer player)
        {
            if (GameHandle == null || GameHandle.GameType == GAME_TYPE.HOLE_REPEAT)
            {
                return;
            }
            Send(ShowPlayerAction(player, TLOBBY_ACTION.UPDATE));
        }

        public void UpdateGameEvent(GameBase GameHandle)
        {
            if (GameHandle == null || GameHandle.GameType == GAME_TYPE.HOLE_REPEAT)
            {
                return;
            }
            Send(ShowGameAction(GameHandle.GameInformation(), TGAME_ACTION.UPDATE));
        }

        public void PlayerSendChat(GPlayer player, string Messages)
        {
            if (player.Game != null)
            {
                var GameHandle = player.Game;
                GameHandle.Write(ChatText(player.GetNickname, Messages, player.GetCapability == 4 || player.GetCapability == 15));
               
                DebugCommand(player, Messages, player.GetCapability == 4 || player.GetCapability == 15);
            }
            else
            {
                foreach (GPlayer Client in this.Players.Model)
                {
                    if (Client.InLobby && Client.GameID == 0xFFFF)
                    {
                        Client.SendResponse(ChatText(player.GetNickname, Messages, player.GetCapability == 4 || player.GetCapability == 15));
                    }
                }
                DebugCommand(player, Messages, player.GetCapability == 4 || player.GetCapability == 15);
            }
        }

        protected void DebugCommand(GPlayer client, string message, bool IsAdmin)
        {
            GameBase Game;
            string[] Command;
            string ReadCommand = "";

            Command = message.Split(new char[] { ' ' });

            Game = client.Game;
            if (Command.Length > 1)
            {
                ReadCommand = Command[1].ToUpper();
            }
            if (Command[0].ToUpper() == "COMMAND" && IsAdmin)
            {
                switch (ReadCommand)
                {
                    case "@PLAY":
                        {
                            if (Game != null)
                            {
                                if (Game.GameType != GAME_TYPE.CHAT_ROOM)
                                {
                                    Game.CommandReady();
                                }
                            }
                        }
                        break;
                    case "SENDPANGS":
                        {
                            if (Command.Length >= 3)
                            {
                                var pang = uint.Parse(Command[2]);

                                if (pang <= 0) { return; }

                                foreach (GPlayer player in Players.Model)
                                {
                                    if (player.InLobby)
                                    {
                                        player.AddPang(pang);
                                    }
                                    player.SendPang();
                                }
                                WriteConsole.WriteLine($"CHAT COMMAND SUCESS {Command[1]}, VALUE: {Command[2]} !", ConsoleColor.Green);
                            }
                        }
                        break;
                    case "SENDCOOKIES":
                        {
                            if (Command.Length >= 3)
                            {
                                var cookies = uint.Parse(Command[2]);

                                if (cookies <= 0) { return; }

                                foreach (GPlayer player in Players.Model)
                                {
                                    if (player.InLobby)
                                    {
                                        player.AddCookie(cookies);
                                    }
                                    player.SendCookies();
                                }
                                WriteConsole.WriteLine($"CHAT COMMAND SUCESS {Command[1]}, VALUE: {Command[2]} !", ConsoleColor.Green);
                            }
                        }
                        break;
                    default:
                        {
                            WriteConsole.WriteLine("CHAT COMMAND Unknown !");
                        }
                        break;
                }
            }
        }

        public void PlayerSendWhisper(GPlayer player, string Nickname, string Messages)
        {
            PangyaBinaryWriter Response;
            GPlayer Client;


            Response = new PangyaBinaryWriter();

            try
            {
                Client =(GPlayer) player.Server.GetPlayerByNickname(Nickname);


                if (Client == null)
                {
                    Response.Write(new byte[] { 0x40, 0x00 });
                    Response.WriteByte(5); //Status          
                    Response.WritePStr(Nickname);
                    player.SendResponse(Response);
                    return;
                }
                if (!Client.InLobby)
                {
                    Response.Write(new byte[] { 0x40, 0x00 });
                    Response.WriteByte(4); //Status          
                    Response.WritePStr(Nickname);
                    player.SendResponse(Response);
                    return;
                }

                Response = new PangyaBinaryWriter();
                Response.Write(new byte[] { 0x84, 0x00 });
                Response.WriteByte(0); //atingir player       
                Response.WritePStr(player.GetNickname);
                Response.WritePStr(Messages);
                Client.SendResponse(Response);

                Response = new PangyaBinaryWriter();
                Response.Write(new byte[] { 0x84, 0x00 });
                Response.WriteByte(1);//atingir player             
                Response.WritePStr(Nickname);
                Response.WritePStr(Messages);
                player.SendResponse(Response);
            }
            finally
            {
                if (Response != null)
                {
                    Response.Dispose();
                }
            }
        }
        public void RemovePlayer(GPlayer player)
        {
            LobbyList.HandleRemoveLobbyPlayer(player);

            GameBase GameHandle = player.Game;

            if (GameHandle != null)
            {
                GameHandle.RemovePlayer(player);
            }

            if (player.InLobby)
            {
                LeaveMultiplayerGamesList(player);
            }
        }

        public void UpdatePlayerLobbyInfo(GPlayer player)
        {
            if (player.InLobby)
            {
                Send(ShowPlayerAction(player, TLOBBY_ACTION.UPDATE));
                player.SendResponse(new byte[] { 0x0F, 0x00 });
            }
        }
        public int PlayersInLobby()
        {
            return Players.Model.Where(c => c.InLobby == true).ToList().Count;
        }

        public void Send(byte[] data)
        {
            foreach (GPlayer client in Players.Model)
                client.SendResponse(data);
        }
    }
}
