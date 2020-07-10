using PangyaAPI.Auth;
using Py_Game.Client;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static Py_Game.Lobby.Collection.ChannelCollection;
using PangyaAPI.BinaryModels;
using Py_Game.Lobby.Collection;
using Py_Connector.DataBase;
using System.IO;
using Py_Game.Lobby;
using System.Runtime.CompilerServices;
using PangyaAPI.Server;
using PangyaAPI.Tools;
using PangyaAPI.PangyaClient;
using PangyaFileCore;
using static PangyaFileCore.IffBaseManager;
namespace Py_Game.MainServer
{
    public class GameServer : TcpServer
    {
        public bool Messenger_Active { get; set; }
        public IniFile Ini { get; set; }
        public GameServer()
        {
            try
            {
                Ini = new IniFile(ConfigurationManager.AppSettings["ServerConfig"]);
                Data = new ServerSettings
                {
                    Name = Ini.ReadString("Config", "Name", "Pippin"),
                    Version = Ini.ReadString("Config", "Version", "SV_GS_Release_2.0"),
                    UID = Ini.ReadUInt32("Config", "UID", 20201),
                    MaxPlayers = Ini.ReadUInt32("Config", "MaxPlayers", 3000),
                    Port = Ini.ReadUInt32("Config", "Port", 20201),
                    IP = Ini.ReadString("Config", "IP", "127.0.0.1"),
                    Property = Ini.ReadUInt32("Config", "Property", 2048),
                    BlockFunc = Ini.ReadInt64("Config", "BlockFuncSystem", 0),
                    EventFlag = Ini.ReadInt16("Config", "EventFlag", 0),
                    ImgNo = Ini.ReadInt16("Config", "Icon", 1),
                    GameVersion = "824.00",
                    Type = AuthClientTypeEnum.GameServer,
                    AuthServer_Ip = Ini.ReadString("Config", "AuthServer_IP", "127.0.0.1"),
                    AuthServer_Port = Ini.ReadInt32("Config", "AuthServer_Port", 7997),
                    Key = "3493ef7ca4d69f54de682bee58be4f93"
                };
                ShowLog = Ini.ReadBool("Config", "PacketLog", false);
                Messenger_Active = Ini.ReadBool("Config", "Messenger_Server", false);

                Console.Title = $"Pangya Fresh Up! GameServer - {Data.Name} - Players: {Players.Count} ";

                if (ConnectToAuthServer(AuthServerConstructor()) == false)
                {
                   new GameTools.ClearMemory().FlushMemory();
                    WriteConsole.WriteLine("[ERROR_START_AUTH]: Não foi possível se conectar ao AuthServer");
                    Console.ReadKey();
                    Environment.Exit(1);
                }

                _server = new TcpListener(IPAddress.Parse(Data.IP), (int)Data.Port);

            }
            catch (Exception erro)
            {
                new GameTools.ClearMemory().FlushMemory();
                WriteConsole.WriteLine($"[ERROR_START]: {erro.Message}");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
        
        public override void ServerStart()
        {
            try
            {
                Data.InsertServer();
                _isRunning = true;
                _server.Start((int)Data.MaxPlayers);
                WriteConsole.WriteLine($"[SERVER_START]: PORT {Data.Port}", ConsoleColor.Green);
                //Inicia os Lobby's
                Ini = new IniFile(ConfigurationManager.AppSettings["ChannelConfig"]);

                LobbyList = new ChannelCollection(Ini);
                //Inicia a leitura dos arquivos .iff
                new IffBaseManager();//is 100% work? test for iff
               //Inicia Thread para escuta de clientes
               var WaitConnectionsThread = new Thread(new ThreadStart(HandleWaitConnections));
                WaitConnectionsThread.Start();
            }
            catch (Exception erro)
            {
                new GameTools.ClearMemory().FlushMemory();
                WriteConsole.WriteLine($"[ERROR_START]: {erro.Message}");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }
        protected override Player OnConnectPlayer(TcpClient tcp)
        {
            var player = new GPlayer(tcp)
            {
                ConnectionID = NextConnectionId
            };

            NextConnectionId += 1;

            SendKey(player);

            WriteConsole.WriteLine($"[PLAYER_CONNECT]: {player.GetAddress}:{player.GetPort}", ConsoleColor.Green);
            
            Players.Add(player);
            UpdateServer();
            Console.Title = $"Pangya Fresh Up! GameServer - {Data.Name} - Players: {Players.Count} ";
            
            return player;
        }

        protected override AuthClient AuthServerConstructor()
        {
            return new AuthClient(Data);
        }

        protected override void SendKey(Player player)
        {
            var Player = (GPlayer)player;
            try
            {
                if (Player.Tcp.Connected && Player.Connected)
                {
                    Player.Response = new PangyaBinaryWriter();
                    //Gera Packet com chave de criptografia (posisão 8)
                    Player.Response.Write(new byte[] { 0x00, 0x06, 0x00, 0x00, 0x3f, 0x00, 0x01, 0x01 });
                    Player.Response.WriteByte(Player.GetKey);
                    Player.SendBytes(Player.Response.GetBytes());
                    Player.Response.Clear();
                }
            }
            catch
            {
                Player.Close();
            }
        }

        public override void DisconnectPlayer(Player Player)
        {
            var Client = (GPlayer)Player;
            if (Client != null && Client.Connected)
            {
                var PLobby = Client.Lobby;

                if (PLobby != null)
                {
                    PLobby.RemovePlayer(Client);
                }               
                Client.PlayerLeave(); //{ push player to offline }
             
                Players.Remove(Client); //{ remove from player lists }
                Player.Connected = false;
                Player.Dispose();
                Player.Tcp.Close();
            }
            WriteConsole.WriteLine(string.Format("[PLAYER_DISCONNECT]: {0} is disconnected", Client?.GetLogin), ConsoleColor.Red);

            UpdateServer();
            Console.Title = $"Pangya Fresh Up! GameServer - {Data.Name} - Players: {Players.Count} ";
            new GameTools.ClearMemory().FlushMemory();
        }

        protected override void ServerExpection(Player Client, Exception Ex)
        {
            var player = (GPlayer)Client;
            var _db = new PangyaEntities();
            try
            {

                var query = $" exec dbo.ProcSaveExceptionLog @UID = '{player.GetUID}', @USER =  '{player.GetLogin}', @EXCEPTIONMESSAGE= '{Ex.Message}', @SERVER = '{Data.Name}'";

                _db.Database.SqlQuery<PangyaEntities>(query);

                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                var FileWrite = new StreamWriter("GameLog.txt", true);
                FileWrite.WriteLine($"--------------------------- PLAYER_EXCEPTION ------------------------------------------");
                FileWrite.WriteLine($"Date: {DateTime.Now}");
                FileWrite.WriteLine($"Server_Info: NAME {Data.Name}, ID {Data.UID}, PORT {Data.Port}");
                FileWrite.WriteLine(trace.GetFrame(0).GetMethod().ReflectedType.FullName);
                FileWrite.WriteLine("Method: " + Ex.TargetSite);
                FileWrite.WriteLine("Line: " + trace.GetFrame(0).GetFileLineNumber());
                FileWrite.WriteLine("Column: " + trace.GetFrame(0).GetFileColumnNumber());
                FileWrite.WriteLine($"--------------------------- END ------------------------------------------");
                FileWrite.Dispose();
                if (player.Connected)
                {
                    DisconnectPlayer(player);
                }
            }
            finally
            {
                _db.Dispose();
            }
            new GameTools.ClearMemory().FlushMemory();
        }


        public override Player GetClientByConnectionId(uint ConnectionId)
        {
            var Client = (GPlayer)Players.Model.Where(c => c.ConnectionID == ConnectionId).FirstOrDefault();

            return Client;
        }

        public override Player GetPlayerByNickname(string Nickname)
        {
            var Client = (GPlayer)Players.Model.Where(c => c.GetNickname == Nickname).FirstOrDefault();

            return Client;
        }

        public override Player GetPlayerByUsername(string Username)
        {
            var Client = (GPlayer)Players.Model.Where(c => c.GetLogin == Username).FirstOrDefault();

            return Client;
        }

        public override Player GetPlayerByUID(uint UID)
        {
            var Client = (GPlayer)Players.Model.Where(c => c.GetUID == UID).FirstOrDefault();

            return Client;
        }

        public override bool GetPlayerDuplicate(uint UID)
        {
            throw new NotImplementedException();
        }

        public override bool PlayerDuplicateDisconnect(uint UID)
        {
            throw new NotImplementedException();
        }

        protected override void OnAuthServerPacketReceive(AuthClient client, AuthPacket packet)
        {
            if (packet.ID != AuthPacketEnum.SERVER_KEEPALIVE)
            {
                WriteConsole.WriteLine("[SYNC_RECEIVED_PACKET]:  " + packet.ID);
            }
            switch (packet.ID)
            {
                case AuthPacketEnum.SERVER_KEEPALIVE: //KeepAlive
                    {
                    }
                    break;
                case AuthPacketEnum.SERVER_CONNECT:
                    {
                    }
                    break;
                case AuthPacketEnum.SERVER_RELEASE_CHAT:
                    {
                        string GetNickName = packet.Message.PlayerNick;
                        string GetMessage = packet.Message.PlayerMessage;
                        GameTools.PacketCreator.ChatText(GetNickName, GetMessage, true);
                    }
                    break;
                case AuthPacketEnum.RECEIVES_USER_UID:
                    break;
                case AuthPacketEnum.SEND_DISCONNECT_PLAYER:
                    {
                        uint UID = packet.Message.ID;

                        var player = GetPlayerByUID(UID);

                        if (player != null)
                        {
                            DisconnectPlayer(player);
                        }
                    }
                    break;
                case AuthPacketEnum.SERVER_RELEASE_TICKET:
                    {
                        string GetNickName = packet.Message.GetNickName;
                        string GetMessage = packet.Message.GetMessage;
                        using (var result = new PangyaBinaryWriter())
                        {
                            result.Write(new byte[] { 0xC9, 0x00 });
                            result.WritePStr(GetNickName);
                            result.WritePStr(GetMessage);
                            SendToAll(result.GetBytes());
                        }
                    }
                    break;
                case AuthPacketEnum.SERVER_RELEASE_BOXRANDOM:
                    {
                        string GetMessage = packet.Message.GetMessage;
                        Notice(GetMessage);
                    }
                    break;
                case AuthPacketEnum.SERVER_RELEASE_NOTICE_GM:
                    {
                        string Nick = packet.Message.GetNick;
                        string message = packet.Message.mensagem;
                        HandleStaffSendNotice(Nick, message);
                    }
                    break;
                case AuthPacketEnum.SERVER_RELEASE_NOTICE:
                    {
                        string message = packet.Message.mensagem;
                        using (var result = new PangyaBinaryWriter())
                        {
                            result.Write(new byte[] { 0x42, 0x00 });
                            result.WritePStr("Aviso: " + message);
                            SendToAll(result.GetBytes());
                        }
                    }
                    break;
                case AuthPacketEnum.PLAYER_LOGIN_RESULT:
                    {
                        LoginResultEnum loginresult = packet.Message.Type;

                        if (loginresult == LoginResultEnum.Error || loginresult == LoginResultEnum.Exception)
                        {
                            WriteConsole.WriteLine("[CLIENT_ERROR]: Sorry", ConsoleColor.Red);
                            return;
                        }
                    }
                    break;
                case AuthPacketEnum.SERVER_COMMAND:
                    break;
                default:
                    WriteConsole.WriteLine("[AUTH_PACKET]:  " + packet.ID);
                    break;
            }
        }
        public void HandleStaffSendNotice(string Nickname, string Msg)
        {
            var response = new PangyaBinaryWriter();
            try
            {
                if (Nickname.Length <= 0 || Msg.Length <= 0)
                {
                    return;
                }

                response.Write(new byte[] { 0x40, 0x00, 0x07 });
                response.WritePStr(Nickname);
                response.WritePStr(Msg);
                this.SendToAll(response.GetBytes());
            }
            finally
            {
                response.Dispose();
            }
        }
        public void RunCommand(string[] Command)
        {
            string ReadCommand;
            GPlayer P;

            if (Command.Length > 1)
            {
                ReadCommand = Command[1];
            }
            else
            {
                ReadCommand = "";
            }
            switch (Command[0])
            { 
                case "cls":
                case "limpar":
                case "clear":
                    {
                        Console.Clear();
                    }
                    break;
                case "kickuid":
                    {
                        P = (GPlayer)GetPlayerByUID(uint.Parse(ReadCommand.ToString()));
                        if (P == null)
                        {
                            WriteConsole.WriteLine("[SYSTEM_COMMAND]: THIS UID IS NOT ONLINE!", ConsoleColor.Red);
                            break;
                        }
                        DisconnectPlayer(P);
                    }
                    break;
                case "kickname":
                    {
                        P = (GPlayer)GetPlayerByNickname(ReadCommand);
                        if (P == null)
                        {
                            WriteConsole.WriteLine("[SYSTEM_COMMAND]: THIS NICKNAME IS NOT ONLINE!", ConsoleColor.Red);
                            return;
                        }
                        DisconnectPlayer(P);
                    }
                    break;
                case "kickuser":
                    {
                        P = (GPlayer)GetPlayerByUsername(ReadCommand);
                        if (P == null)
                        {
                            WriteConsole.WriteLine("[SYSTEM_COMMAND]: THIS USERNAME IS NOT ONLINE!", ConsoleColor.Red);
                            return;
                        }
                        DisconnectPlayer(P);
                    }
                    break;
                case "topnotice":
                    {
                        if (ReadCommand.Length > 0)
                            Notice(ReadCommand);
                    }
                    break;
                case "lobby":
                case "listalobby":
                case "canais":
                case "listacanais":
                case "showlobby":
                case "showchannel":
                    {
                        LobbyList.ShowChannel();
                    }
                    break;
                case "comandos":
                case "commands":
                case "ajuda":
                case "help":
                    {
                        ShowHelp();
                    }
                    break;
                default:
                    {
                        WriteConsole.WriteLine("[SYSTEM_COMMAND]: Sorry Unknown Command, type 'help' to get the list of commands", ConsoleColor.Red);
                    }
                    break;
            }
        }

        public Channel GetLobbyByID(byte ID)
        {
            foreach (var lobby in LobbyList)
            {
                if (lobby.Id == ID)
                {
                    return lobby;
                }
            }
            return null;
        }
        public Channel GetLobbyByName(string Name)
        {
            foreach (var lobby in LobbyList)
            {
                if (lobby.Name == Name)
                {
                    return lobby;
                }
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void SendTarget(byte ID, byte[] Data)
        {
            foreach (GPlayer Client in Players.Model)
            {
                if (Client.Lobby.Id == ID)
                {
                    Client.SendResponse(Data);
                }
            }
        }

        internal void ShowHelp()
        {
            Console.WriteLine(Environment.NewLine);
            WriteConsole.WriteLine("Welcome To Py-Game!" + Environment.NewLine);

            WriteConsole.WriteLine("See available console commands:" + Environment.NewLine);

            WriteConsole.WriteLine("help      | Displays console commands");
            WriteConsole.WriteLine("topnotice | Displays message to users who are playing Game");
            WriteConsole.WriteLine("kickuser  | Disconnect by UserName");
            WriteConsole.WriteLine("kicknick  | Disconnect by Nick");
            WriteConsole.WriteLine("kickuid   | Disconnect by UID");
            WriteConsole.WriteLine("ListLobby | ShowLobby");

            WriteConsole.WriteLine("clear     | Clear Console");
            WriteConsole.WriteLine("cls       | Clear console");
            WriteConsole.WriteLine("quit      | Close By Server");

            Console.WriteLine(Environment.NewLine);
        }
    }
}
