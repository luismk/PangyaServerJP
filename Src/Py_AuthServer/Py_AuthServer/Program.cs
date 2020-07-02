using PangyaAPI;
using PangyaAPI.Auth;
using PangyaAPI.Tools;
using Py_Connector.DataBase;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Py_AuthServer
{
    class Program
    {
        public static AuthServer Server;
        static PangyaEntities db;

        public static string AuthKey { get; set; }

        static void Main()
        {
            db = new PangyaEntities();
            Console.Title = string.Format("Pangya Fresh Up! AuthServer - LOGIN: {0}, GAMES: {1}, MESSENGER: {2}", 0, 0, 0);

            AuthKey = "3493ef7ca4d69f54de682bee58be4f93"; //Unogames em MD5

            //Inicia servidor
            Server = new AuthServer();
            Server.Start();
            Server.OnPacketReceived += TcpServer_OnPacketReceived;

            var servers = db.Pangya_Server.Where(c => c.Active == true && c.Port != 7997).ToList();
            foreach (var _server in servers)
            {
                db.Database.SqlQuery<PangyaEntities>($"UPDATE [dbo].[Pangya_Server] Set Active = '{0}' where ServerID = '{_server.ServerID}'").FirstOrDefault();
            }
            for (; ; )
            {
                var comando = Console.ReadLine().Split(new char[] { ' ' }, 2);
                switch (comando[0].ToLower())
                {
                    case "": break;
                    case "notice":
                        {
                            var message = comando[1];

                            Server.Send(AuthClientTypeEnum.GameServer, new AuthPacket() { ID = AuthPacketEnum.SERVER_RELEASE_NOTICE, Message = new { mensagem = message } });
                        }
                        break;
                    case "ticket":
                        {
                            var message = comando[1];

                            Server.Send(AuthClientTypeEnum.GameServer, new AuthPacket() { ID = AuthPacketEnum.SERVER_RELEASE_TICKET, Message = new { GetNickName = "ADMIN", GetMessage = message } });
                        }
                        break;
                    case "quit":
                        Console.WriteLine("The server was stopped!");
                        Environment.Exit(1);
                        break;
                    case "limpar":
                    case "cls":
                    case "clear":
                        Console.Clear();
                        break;
                    default:
                        Console.WriteLine("Comando inexistente");
                        break;
                }
            }
        }

        static void TcpServer_OnPacketReceived(AuthClient client, AuthPacket packet)
        {
            WriteConsole.WriteLine($"[SYNC_CALL_PACKET]: [{packet.ID}, {client.Data.Name}]", ConsoleColor.Cyan);
            switch (packet.ID)
            {
                case AuthPacketEnum.SERVER_KEEPALIVE:
                    {
                        client.Send(new AuthPacket() { ID = AuthPacketEnum.SERVER_KEEPALIVE });
                    }
                    break;
                case AuthPacketEnum.SERVER_CONNECT:
                    {
                        var response = new AuthPacket();

                        if (client.Data.Key != AuthKey)
                        {
                            response.Message = new
                            {
                                Success = false,
                                Exception = "Chave de autenticação inválida"
                            };

                            Server.Send(client, response);
                            Server.DisconnectClient(client);
                        }
                        else
                        {
                            response.Message = new
                            {
                                Success = true,
                            };

                            Server.Send(client, response);
                        }
                    }
                    break;
                case AuthPacketEnum.RECEIVES_USER_UID:
                    {
                        HandlePlayerUID(packet);
                    }
                    break;
                case AuthPacketEnum.DISCONNECT_PLAYER_ALL_ON_SERVERS:
                    {
                        HandlePlayerDisconnect(packet);
                    }
                    break;
                case AuthPacketEnum.SERVER_RELEASE_CHAT:
                    {
                        byte Type = packet.Message.IsGM;
                        if (Type == 15 || Type == 4)
                        {
                            var response = new AuthPacket
                            {
                                ID = AuthPacketEnum.SERVER_RELEASE_CHAT,
                                Message = new { PlayerNick = packet.Message.GetNickName, PlayerMessage = packet.Message.GetMessage }
                            };
                            Server.Send(AuthClientTypeEnum.GameServer, response);
                        }
                    }
                    break;
                case AuthPacketEnum.SERVER_RELEASE_TICKET:
                    {
                        HandleTicket(packet);
                    }
                    break;
                case AuthPacketEnum.SERVER_RELEASE_BOXRANDOM:
                    {
                        HandleWinBoxRandomNotice(packet);
                    }
                    break;
                case AuthPacketEnum.SERVER_RELEASE_NOTICE_GM:
                    {
                        HandleNotice(packet);
                    }
                    break;
                case AuthPacketEnum.SERVER_RELEASE_NOTICE:
                    {
                        HandleNotice(packet);
                    }
                    break;
                case AuthPacketEnum.LOGIN_RESULT:
                    {
                        HandleLogin(client, packet);
                    }
                    break;
                case AuthPacketEnum.SERVER_COMMAND:
                    break;
                default:
                    WriteConsole.WriteLine("[SYNC_REQUEST_PACKET_UNK]: " + packet.ID);
                    break;
            }
        }

        protected static void HandleLogin(AuthClient client,AuthPacket packet)
        {
            int UID = packet.Message.ID;

            var check = Server.Players.Model.Where(c => c.GetUID == UID);

            if (check.Any())
            {
                var result = new AuthPacket() { ID = AuthPacketEnum.PLAYER_LOGIN_RESULT, Message = new { Type = LoginResultEnum.Sucess } };
                Server.Send(client, result);
            }
            else
            {
                var result = new AuthPacket() { ID = AuthPacketEnum.PLAYER_LOGIN_RESULT, Message = new { Type = LoginResultEnum.Error } };
                Server.Send(client, result);

                HandlePlayerDisconnect(packet);
            }
        }

        protected static void HandlePlayerUID(AuthPacket packet)
        {
            int UID = packet.Message.ID;

            var check = Server.Players.Model.Where(c => c.GetUID == UID);

            if (check.Any())
            {
                Server.Players.Remove(check.First());

                var member = db.Pangya_Member.FirstOrDefault(c => c.UID == UID);
                if (member != null)
                {
                    var Player = new APlayer(null) { GetUID = (uint)UID, GetLogin = member.Username, GetNickname = member.Nickname };

                    Server.Players.Add(Player);
                }
            }
            else
            {
                var member = db.Pangya_Member.FirstOrDefault(c => c.UID == UID);
                if (member != null)
                {
                    var Player = new APlayer(null) { GetUID = (uint)UID, GetLogin = member.Username, GetNickname = member.Nickname };

                    Server.Players.Add(Player);
                }
            }
        }

        protected static void HandlePlayerDisconnect(AuthPacket packet)
        {
            int UID = packet.Message.ID;

            var check = Server.Players.Model.Where(c => c.GetUID == UID);
            var member = db.Pangya_Member.FirstOrDefault(c => c.UID == UID);

            if (check.Any())
            {
                Server.Players.Remove(check.First());                
            }
            if (member != null)
            {
                member.Logon = 0;

                db.SaveChanges();

                packet.ID = AuthPacketEnum.SEND_DISCONNECT_PLAYER;
                Server.SendToAll(packet);
            }
        }

        protected static void HandleTicket(AuthPacket packet)
        {
            Server.Send(AuthClientTypeEnum.GameServer, packet);
        }

        protected static void HandleNotice(AuthPacket packet)
        {
            Server.Send(AuthClientTypeEnum.GameServer, packet);
        }

        protected static void HandleWinBoxRandomNotice(AuthPacket packet)
        {
            Server.Send(AuthClientTypeEnum.GameServer, packet);
        }
    }
}
