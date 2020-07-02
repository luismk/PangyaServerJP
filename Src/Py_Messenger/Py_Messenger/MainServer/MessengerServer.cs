using PangyaAPI;
using PangyaAPI.Auth;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using PangyaAPI.BinaryModels;
using Py_Connector.DataBase;
using System.IO;
using System.Configuration;
using Py_Messenger.Client;
using PangyaAPI.Server;
using PangyaAPI.Tools;
using PangyaAPI.PangyaClient;

namespace Py_Messenger.MainServer
{
    public class MessengerServer : TcpServer
    {
        public IniFile Ini { get; set; }
        public MessengerServer()
        {
            try
            {
                Ini = new IniFile(ConfigurationManager.AppSettings["Config"]);
                Data = new ServerSettings
                {
                    Name = Ini.ReadString("Config", "Name", "Message Server"),
                    Version = Ini.ReadString("Config", "Version", "SV_MS_Release_1.0"),
                    UID = Ini.ReadUInt32("Config", "UID", 30303),
                    MaxPlayers = Ini.ReadUInt32("Config", "MaxPlayers", 3000),
                    Port = Ini.ReadUInt32("Config", "Port", 30303),
                    IP = Ini.ReadString("Config", "IP", "127.0.0.1"),
                    Property = Ini.ReadUInt32("Config", "Property", 4096),                  
                    Type = AuthClientTypeEnum.MessengerServer,
                    AuthServer_Ip = Ini.ReadString("Config", "AuthServer_IP", "127.0.0.1"),
                    AuthServer_Port = Ini.ReadInt32("Config", "AuthServer_Port", 7997),
                    Key = "3493ef7ca4d69f54de682bee58be4f93"
                };

                Console.Title = $"Pangya Fresh Up! MessengerServer - Players: {Players.Count} ";

                if (ConnectToAuthServer(AuthServerConstructor()) == false)
                {
                    WriteConsole.WriteLine("[ERROR_START_AUTH]: Não foi possível se conectar ao AuthServer");
                    Console.ReadKey();
                    Environment.Exit(1);
                }

                _server = new TcpListener(IPAddress.Parse(Data.IP), (int)Data.Port);

            }
            catch (Exception erro)
            {
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
                this._isRunning = true;
                _server.Start((int)Data.MaxPlayers);
                WriteConsole.WriteLine($"[SERVER_START]: PORT {Data.Port}", ConsoleColor.Green);
               
                //Inicia Thread para escuta de clientes
                var WaitConnectionsThread = new Thread(new ThreadStart(HandleWaitConnections));
                WaitConnectionsThread.Start();
            }
            catch (Exception erro)
            {
                WriteConsole.WriteLine($"[ERROR_START]: {erro.Message}");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }
        protected override Player OnConnectPlayer(TcpClient tcp)
        {
            var player = new MPlayer(tcp)
            {
                Server = this,
                ConnectionID = NextConnectionId
            };

            NextConnectionId += 1;

            SendKey(player);

            WriteConsole.WriteLine($"[PLAYER_CONNECT]: {player.GetAddress}:{player.GetPort}", ConsoleColor.Green);

            Players.Add(player);
            UpdateServer();
            Console.Title = $"Pangya Fresh Up! MessengerServer - Players: {Players.Count} ";

            return player;
        }

        protected override AuthClient AuthServerConstructor()
        {
            return new AuthClient(Data);
        }

        protected override void SendKey(Player player)
        {
            var Player = (MPlayer)player;
            try
            {
                if (Player.Tcp.Connected && Player.Connected)
                {
                    Player.Response = new PangyaBinaryWriter();
                    //Gera Packet com chave de criptografia (posisão 9)
                    Player.Response.Write(new byte[] { 0x00, 0x09, 0x00, 0x00, 0x2e, 0x00, 0x01, 0x01 });
                    Player.Response.WriteUInt32(Player.GetKey);
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
            var Client = (MPlayer)Player;
            if (Client != null && Client.Connected)
            {
                Players.Remove(Client); //{ remove from player lists }
                Player.Connected = false;
                Player.Dispose();
                Player.Tcp.Close();
            }
            WriteConsole.WriteLine(string.Format("[PLAYER_DISCONNECT]: {0} is disconnected", Client?.GetLogin), ConsoleColor.Red);

            UpdateServer();
            Console.Title = $"Pangya Fresh Up! MessengerServer - Players: {Players.Count} ";
        }

        protected override void ServerExpection(Player Client, Exception Ex)
        {
            var player = (MPlayer)Client;
            var _db = new PangyaEntities();
            try
            {
                var query = $" exec dbo.ProcSaveExceptionLog @UID = '{player.GetUID}', @USER =  '{player.GetLogin}', @EXCEPTIONMESSAGE= '{Ex.Message}', @SERVER = '{Data.Name}'";

                _db.Database.SqlQuery<PangyaEntities>(query);

                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                var FileWrite = new StreamWriter("MsgLog.txt", true);
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
        }


        public override Player GetClientByConnectionId(uint ConnectionId)
        {
            var Client = (MPlayer)Players.Model.Where(c => c.ConnectionID == ConnectionId).FirstOrDefault();

            return Client;
        }

        public override Player GetPlayerByNickname(string Nickname)
        {
            var Client = (MPlayer)Players.Model.Where(c => c.GetNickname == Nickname).FirstOrDefault();

            return Client;
        }

        public override Player GetPlayerByUsername(string Username)
        {
            var Client = (MPlayer)Players.Model.Where(c => c.GetLogin == Username).FirstOrDefault();

            return Client;
        }

        public override Player GetPlayerByUID(uint UID)
        {
            var Client = (MPlayer)Players.Model.Where(c => c.GetUID == UID).FirstOrDefault();

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
            MPlayer P;

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
                        P = (MPlayer)GetPlayerByUID(uint.Parse(ReadCommand.ToString()));
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
                        P = (MPlayer)GetPlayerByNickname(ReadCommand);
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
                        P = (MPlayer)GetPlayerByUsername(ReadCommand);
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

        internal void ShowHelp()
        {
            Console.WriteLine(Environment.NewLine);
            WriteConsole.WriteLine("Welcome To Py-Messenger!" + Environment.NewLine);

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
