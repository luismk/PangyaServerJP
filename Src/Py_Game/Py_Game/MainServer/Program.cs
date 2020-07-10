using PangyaAPI.Auth;
using PangyaAPI.BinaryModels;
using PangyaAPI.PangyaClient;
using PangyaAPI.PangyaPacket;
using PangyaAPI.Tools;
using PangyaFileCore;
using static PangyaFileCore.IffBaseManager;
using Py_Game.Client;
using Py_Game.Defines;
using Py_Game.GameTools;
using System;
namespace Py_Game.MainServer
{
    class Program
    {
        #region Fields 
        public static GameServer _server;
        #endregion
        static void Main()
        {
            //Inicia servidor
            _server = new GameServer();           
            _server.ServerStart();
            _server.OnClientConnected += player =>
            {
                player.Server = _server;
            };
            _server.OnPacketReceived += Server_OnPacketReceived;
            //Escuta contínuamente entradas no console (Criar comandos para o Console)
            for (;;)
            {
                var comando = Console.ReadLine().Split(new char[] { ' ' }, 2);
                _server.RunCommand(comando);
            }
        }

        private static void Server_OnPacketReceived(Player player, Packet packet)
        {
            var Client = (GPlayer)player;
            if ((TGAMEPACKET)packet.Id != TGAMEPACKET.PLAYER_LOGIN)
            {
                WriteConsole.WriteLine($"[PLAYER_CALL_PACKET]: [{(TGAMEPACKET)packet.Id},{player.GetLogin}]", ConsoleColor.Cyan);
            }
            packet.Save();
            Client.HandleRequestPacket((TGAMEPACKET)packet.Id, packet);
        }

        public static bool SendAuth(AuthPacket packet)
        {
            if (_server.AuthServer != null)
            {
                _server.AuthServer.Send(packet);
                return true;
            }
            else
            {
                switch (packet.ID)
                {
                    case AuthPacketEnum.SERVER_RELEASE_CHAT:
                        {
                            bool Type = packet.Message.IsGM;
                            string GetNickName = packet.Message.GetNickName;
                            string GetMessage = packet.Message.GetMessage;
                            if (Type)
                            {
                                PacketCreator.ChatText(GetNickName, GetMessage, Type);
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
                                _server.SendToAll(result.GetBytes());
                            }
                        }
                        break;
                    case AuthPacketEnum.SERVER_RELEASE_BOXRANDOM:
                        {
                            string GetMessage = packet.Message.GetMessage;
                            _server.Notice(GetMessage);
                        }
                        break;
                    case AuthPacketEnum.SERVER_RELEASE_NOTICE:
                        {
                            string GetNickname = packet.Message.GetNickname;
                            string Messages = packet.Message.mensagem;

                            PacketCreator.ShowGMNotice(Messages, GetNickname);
                        }
                        break;
                }
                return false;
            }
        }

        internal static bool CheckVersion(string version)
        {
            switch (version)
            {
                case "JP.R7.983.00":
                    {
                        return true;
                    }
                default:
                    _server.Data.BlockFunc = 2;
                    return false;
            }
        }
    }   
}
