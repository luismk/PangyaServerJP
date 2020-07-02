using Newtonsoft.Json;
using PangyaAPI;
using PangyaAPI.Auth;
using PangyaAPI.Server;
using PangyaAPI.Tools;
using Py_Connector.DataBase;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Py_AuthServer
{
    public class AuthServer
    {
        #region Delegates
        public delegate void ConnectedEvent(AuthClient client);
        public delegate void PacketReceivedEvent(AuthClient client, AuthPacket packet);
        #endregion

        #region Events

        /// <summary>
        /// Este evento ocorre quando um Servidor se conecta com o Authserver
        /// </summary>
        public event ConnectedEvent OnClientConnected;

        /// <summary>
        /// Este evento ocorre quando o AuthServer Recebe um Packet de um Servidor
        /// </summary>
        public event PacketReceivedEvent OnPacketReceived;

        #endregion

        #region Fields

        /// <summary>
        /// Lista de Clientes conectados
        /// </summary>
        public GenericDisposableCollection<AuthClient> Clients = new GenericDisposableCollection<AuthClient>();

        public GenericDisposableCollection<APlayer> Players = new GenericDisposableCollection<APlayer>();

        private TcpListener _server;

        public ServerSettings Data { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="ip">IP do servidor (Local ou Global)</param>
        /// <param name="port">Porta</param>
        /// <param name="maxConnections">
        /// Número máximo de conexões 
        /// Quando o Player se conecta ao Game-server, automaticamente ele é desconectado do LoginServer pois não necessita mais desta comunicação
        /// </param>
        public AuthServer()
        {
            try
            {
                var Ini = new IniFile(ConfigurationManager.AppSettings["Config"]);

                Data = new ServerSettings()
                {
                    Name = Ini.ReadString("Config", "Name", "AuthServer"),
                    Version = Ini.ReadString("Config", "Version", "SV_AT_Release_2.0"),
                    UID = Ini.ReadUInt32("Config", "UID", 7997),
                    MaxPlayers = Ini.ReadUInt32("Config", "MaxPlayers", 3000),
                    Port = Ini.ReadUInt32("Config", "Port", 7997),
                    IP = Ini.ReadString("Config", "IP", "127.0.0.1"),
                    Type = AuthClientTypeEnum.AuthServer
                };
                _server = new TcpListener(IPAddress.Parse(Data.IP), (int)Data.Port);
            }
            catch (Exception erro)
            {
                WriteConsole.WriteLine($"ERRO_START: {erro.Message}");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        public void Start()
        {
            try
            {
                Data.InsertServer();

                _server.Start((int)Data.MaxPlayers);
                WriteConsole.WriteLine($"[SERVER_START]: PORT {Data.Port}", ConsoleColor.Green);

                //Inicia Thread para escuta de clientes
                var WaitConnectionsThread = new Thread(new ThreadStart(HandleWaitConnections));
                WaitConnectionsThread.Start();

            }
            catch (Exception erro)
            {
                WriteConsole.WriteLine($"ERRO_START: {erro.Message}");
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Aguarda Conexões
        /// </summary>
        private void HandleWaitConnections()
        {
            while (true)
            {
                // Inicia Escuta de novas conexões.
                TcpClient newClient = _server.AcceptTcpClient();

                // Cliente conectado
                // Cria uma Thread para manusear a comunicação
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                t.Start(newClient);
            }
        }

        /// <summary>
        /// Manuseia Comunicação do Cliente
        /// </summary>
        private void HandleClient(object obj)
        {
            //Recebe cliente a partir do parâmetro
            TcpClient tcpClient = (TcpClient)obj;

            NetworkStream clientStream = tcpClient.GetStream();

            #region READ ON CONNECT INICIAL

            AuthPacket packet = ReceivePacket(clientStream);

            var client = new AuthClient(tcpClient, packet);

            Clients.Add(client);
            Console.Title = string.Format("Pangya Fresh Up! AuthServer - LOGIN: {0}, GAMES: {1}, MESSENGER: {2}", Clients.Model.Where(c => c.Data.Type == AuthClientTypeEnum.LoginServer).ToList().Count, Clients.Model.Where(c => c.Data.Type == AuthClientTypeEnum.GameServer).ToList().Count, Clients.Model.Where(c => c.Data.Type == AuthClientTypeEnum.MessengerServer).ToList().Count);
            ClientConnected(client);

            OnPacketReceived?.Invoke(client, packet);

            #endregion

            //Escuta contínuamente as mensagens dos clientes (Servidores) enquanto estiver conectado
            while (tcpClient.Connected)
            {
                try
                {
                    packet = ReceivePacket(clientStream);

                    OnPacketReceived?.Invoke(client, packet);
                }
                catch
                {
                    //Desconecta client
                    DisconnectClient(client);
                }
            }
            //Caso o Client não estiver mais conectado
            if (tcpClient.Connected)
                DisconnectClient(client);
        }

        private static AuthPacket ReceivePacket(NetworkStream clientStream)
        {
            var messageBufferRead = new byte[500000]; //Tamanho do BUFFER á ler

            //Lê mensagem do cliente
            var bytesRead = clientStream.Read(messageBufferRead, 0, 500000);

            //variável para armazenar a mensagem recebida
            var message = new byte[bytesRead];

            //Copia mensagem recebida
            Buffer.BlockCopy(messageBufferRead, 0, message, 0, bytesRead);

            var json = System.Text.Encoding.Default.GetString(message);

            var packet = JsonConvert.DeserializeObject<AuthPacket>(json);
            return packet;
        }

        private void ClientConnected(AuthClient Server)
        {
            //Chama evento OnClientConnected
            OnClientConnected?.Invoke(Server);

            WriteConsole.WriteLine($"[CLIENT_CONNECTED]: {Server.Data.Name} | Type: {Server.Data.Type}", ConsoleColor.Green);
            UpdateServer();
        }
        #endregion

        #region Public Methods

        public virtual void DisconnectClient(AuthClient Server)
        {
            Clients.Model.Remove(Server);

            UpdateClient(Server);

            Server.Dispose();

            WriteConsole.WriteLine($"[CLIENT_DISCONNECTED]: <{Server?.Data.Name}>", ConsoleColor.Red);

            Console.Title = string.Format("Pangya Fresh Up! AuthServer - LOGIN: {0}, GAMES: {1}, MESSENGER: {2}", Clients.Count, Clients.Model.Where(c => c.Data.Type == AuthClientTypeEnum.LoginServer).ToList().Count, Clients.Model.Where(c => c.Data.Type == AuthClientTypeEnum.GameServer).ToList().Count, Clients.Model.Where(c => c.Data.Type == AuthClientTypeEnum.MessengerServer).ToList().Count);
            UpdateServer();
        }

        public void SendToAll(AuthPacket packet)
        {
            for (int i = 0; i < this.Clients.ToList().Count; i++)
            {
                Send(Clients[i], packet);
            }
        }

        public void Send(AuthClient client, AuthPacket packet)
        {
            var _stream = client.Tcp.GetStream();

            var json = JsonConvert.SerializeObject(packet);

            var result = json.Select(Convert.ToByte).ToArray();
            _stream.Write(result, 0, result.Length);
        }

        public void Send(AuthClientTypeEnum Types,AuthPacket packet)
        {
            var clients = Clients.Model.Where(c => c.Data.Type == Types).ToList();
            for (int i = 0; i < clients.ToList().Count; i++)
            {
                Send(clients[i], packet);
            }
        }



        public void UpdateClient(AuthClient Client)
        {
            var db = new PangyaEntities();

            var query = $"UPDATE [dbo].[Pangya_Server] Set UsersOnline = '{0}', Active = '{0}' where ServerID = '{Client.Data.UID}'";

            db.Database.SqlQuery<PangyaEntities>(query).FirstOrDefault();
        }

        public void UpdateServer()
        {
            var db = new PangyaEntities();

            var query = $"UPDATE [dbo].[Pangya_Server] Set UsersOnline = '{Clients.Count}' where ServerID = '{Data.UID}'";

            db.Database.SqlQuery<PangyaEntities>(query).FirstOrDefault();
        }
        #endregion
    }
}
