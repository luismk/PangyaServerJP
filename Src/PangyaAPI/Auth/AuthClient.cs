using Newtonsoft.Json;
using PangyaAPI.Server;
using Py_Connector.DataBase;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace PangyaAPI.Auth
{
    public class AuthClient : IDisposeable
    {
        #region Delegates
        public delegate void DisconnectedEvent();
        public delegate void PacketReceivedEvent(AuthClient authClient, AuthPacket packet);
        #endregion

        #region Events

        /// <summary>
        /// Este evento ocorre quando o ProjectG se conecta ao Servidor
        /// </summary>
        public event DisconnectedEvent OnDisconnect;

        /// <summary>
        /// Este evento ocorre quando o client recebe um Packet do AuthServer
        /// </summary>
        public event PacketReceivedEvent OnPacketReceived;
        #endregion

        #region Public Fields

        public ServerSettings Data { get; set; } = new ServerSettings();
        //Conexão
        public TcpClient Tcp;

        public AuthPacket CurrentPacket { get; set; }

        #endregion

        #region Constructor

        public AuthClient(ServerSettings _client)
        {
            Tcp = new TcpClient();
            Data = _client;
        }

        public AuthClient(TcpClient client, AuthPacket packet)
        {
            Tcp = client;
            ServerSettings ClientData;

            ClientData = new ServerSettings()
            {
                UID = packet.Message._data.UID,
                Type = packet.Message._data.Type,
                AuthServer_Ip = packet.Message._data.AuthServer_Ip,
                AuthServer_Port = packet.Message._data.AuthServer_Port,
                Port = packet.Message._data.Port,
                MaxPlayers = packet.Message._data.MaxPlayers,
                IP = packet.Message._data.IP,
                Key = packet.Message._data.Key,
                Name = packet.Message._data.Name,
                BlockFunc = packet.Message._data.BlockFunc,
                EventFlag = packet.Message._data.EventFlag,
                GameVersion = packet.Message._data.GameVersion,
                ImgNo = packet.Message._data.ImgNo,
                Property = packet.Message._data.Property,
                Version = packet.Message._data.Version,
              
            };               
            Data = ClientData;

            using (var _db = new PangyaEntities())
            {
                if (_db.Pangya_Server.Where(c => c.ServerID == Data.UID).Any())
                {
                    var query = $"UPDATE [dbo].[Pangya_Server] Set Active = '{1}' where ServerID = '{Data.UID}'";
                    _db.Database.SqlQuery<PangyaEntities>(query).FirstOrDefault();
                }
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Verifica de tempo em tempo se o AuthServer ainda está conectado.
        /// </summary>
        private void KeepAlive()
        {
            while (true)
            {
                //Aguarda tempo
                Thread.Sleep(TimeSpan.FromSeconds(5));

                try
                {
                    //Send KeepAlive
                    Send(new AuthPacket()
                    {
                        ID = AuthPacketEnum.SERVER_KEEPALIVE
                    });
                }
                catch
                {
                    //Dispara evento quando não há conexão
                    OnDisconnect?.Invoke();
                }
            }
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Conecta-se ao AuthServer
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            Tcp.Connect(Data.AuthServer_Ip, Data.AuthServer_Port);

            var packet = new AuthPacket()
            {
                ID = AuthPacketEnum.SERVER_CONNECT,
                Message = new
                {
                    _data = Data
                }
            };

            var response = SendAndReceive(packet);

            if (response.Message.Success == true)
            {
                //Inicia Thread KeepAlive
                var authClienthread = new Thread(new ThreadStart(HandleAuthClient));
                authClienthread.Start();               

                return true;
            }
            else
            {
                Console.WriteLine(response.Message.Exception);
                return false;
            }
        }

        private void HandleAuthClient()
        {
            //Inicia Thread KeepAlive
            var keepAliveThread = new Thread(new ThreadStart(KeepAlive));
            keepAliveThread.Start();

         

            while (Tcp.Connected)
            {
                try
                {
                    var messageBufferRead = new byte[500000]; //Tamanho do BUFFER á ler

                    //Lê mensagem do cliente
                    var bytesRead = Tcp.GetStream().Read(messageBufferRead, 0, 500000);

                    //variável para armazenar a mensagem recebida
                    var message = new byte[bytesRead];

                    //Copia mensagem recebida
                    Buffer.BlockCopy(messageBufferRead, 0, message, 0, bytesRead);

                    var json = System.Text.Encoding.Default.GetString(message);

                    var response = JsonConvert.DeserializeObject<AuthPacket>(json);

                    //Dispara evento OnPacketReceived
                    OnPacketReceived?.Invoke(this, response);
                }
                catch
                {
                    OnDisconnect?.Invoke();
                }
            }
        }

        /// <summary>
        /// Envia Packet sem aguardar uma Resposta
        /// </summary>
        public void Send(AuthPacket packet)
        {
            var _stream = Tcp.GetStream();

            var json = JsonConvert.SerializeObject(packet);

            var result = json.Select(Convert.ToByte).ToArray();
            _stream.Write(result, 0, result.Length);
        }

        /// <summary>
        /// Envia Packet aguardando uma resposta
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public AuthPacket SendAndReceive(AuthPacket packet)
        {
            Send(packet);

            var messageBufferRead = new byte[500000]; //Tamanho do BUFFER á ler

            //Lê mensagem do cliente
            var bytesRead = Tcp.GetStream().Read(messageBufferRead, 0, 500000);

            //variável para armazenar a mensagem recebida
            var message = new byte[bytesRead];

            //Copia mensagem recebida
            Buffer.BlockCopy(messageBufferRead, 0, message, 0, bytesRead);

            var json = System.Text.Encoding.Default.GetString(message);

            var response = JsonConvert.DeserializeObject<AuthPacket>(json);

            return response;
        }

        #endregion

        #region IDisposable
        // booleano para controlar se
        // o método Dispose já foi chamado
        public bool Disposed { get; set; }

        // método privado para controle
        // da liberação dos recursos
        private void Dispose(bool disposing)
        {
            // Verifique se Dispose já foi chamado.
            if (!this.Disposed)
            {
                if (disposing)
                {
                    // Liberando recursos gerenciados
                    Tcp.Dispose();
                }

                // Seta a variável booleana para true,
                // indicando que os recursos já foram liberados
                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // C#
        ~AuthClient()
        {
            Dispose(false);
        }
        #endregion
    }
}
