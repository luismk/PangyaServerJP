using PangyaAPI.BinaryModels;
using PangyaAPI.Server;
using Py_Connector.DataBase;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using static PangyaAPI.Crypts.Cryptor;
namespace PangyaAPI.PangyaClient
{
    public abstract partial class Player : IDisposeable
    {
        #region Public Fields

        /// <summary>
        /// Cliente está conectado
        /// </summary>
        public bool Connected { get; set; }

        /// <summary>
        /// Servidor em que o cliente está conectado
        /// </summary>
        public TcpServer Server { get; set; }

        /// <summary>
        /// Conexão do cliente
        /// </summary>
        public TcpClient Tcp { get; set; }
        /// <summary>
        /// Chave de criptografia e decriptografia
        /// </summary>
        byte Key { get; set; }

        public PangyaBinaryWriter Response { get; set; }
        /// <summary>
        /// Identificador da conexão
        /// </summary>
        public uint ConnectionID { get; set; }
        public UInt32 GetUID { get; set; }
        public byte GetKey { get { return Key; } set { Key = value; } }
        public byte GetFirstLogin { get; set; }
        public string GetLogin { get; set; } = string.Empty;
        public string GetNickname { get; set; } = string.Empty;
        public byte GetSex { get; set; }
        public byte GetCapability { get; set; } = 0;
        public byte GetLevel { get; set; } = 0;
        public string GetAuth1 { get; set; } = String.Empty;
        public string GetAuth2 { get; set; } = String.Empty;

        public string GetAddress
        {
            get { return ((IPEndPoint)Tcp.Client.RemoteEndPoint).Address.ToString(); }
        }

        public string GetPort
        {
            get { return ((IPEndPoint)Tcp.Client.RemoteEndPoint).Port.ToString(); }
        }
        public PangyaEntities _db { get; set; }

        #endregion

        #region Constructor

        public Player(TcpClient tcp)
        {
            Tcp = tcp;

            //Gera uma chave dinâmica
            Key = Convert.ToByte(new Random().Next(1, 15));

            //Max Value hexadecimal value: FF (255)

            ////Chave Fixa
            Key = (byte)(new Random().Next(1, 14) + 1); //chave 10

            Response = new PangyaBinaryWriter(new MemoryStream());

            Connected = true;

            _db = new PangyaEntities();
        }

        public void Disconnect()
        {
            Server.DisconnectPlayer(this);
        }
        #endregion

        #region Player Send Packets 
        public void Send(PangyaBinaryWriter packet)
        {
            var buffer = packet.GetBytes().ServerEncrypt(GetKey);

            SendBytes(buffer);
        }

        public void Write(PangyaBinaryWriter Data)
        {
            Send(Data);
        }

        public void Write(byte[] Data)
        {
            Send(Data);
        }
        public void SendResponse(List<byte[]> Data)
        {
            Response = new PangyaBinaryWriter();
            Data.ForEach(item => Response.Write(item));
            SendResponse();
        }
        public void SendResponse(byte[] Data)
        {
            Send(Data);
        }
        public void SendToAll(byte[] Data)
        {
            Server.SendToAll(Data);
        }

        public void SendResponse(byte[] Header, byte[] Data)
        {
            Response.Write(Header);
            Response.Write(Data);
            Send(Response.GetBytes());
            Response.Clear();
        }
        public void SendResponse()
        {
            var buffer = Response.GetBytes().ServerEncrypt(GetKey);

            SendBytes(buffer);
            if (Response.GetSize > 0)
            {
                Response.Clear();
            }
        }
        public void SendResponse(PangyaBinaryWriter packet)
        {
            Send(packet);
            if (packet.GetSize > 0)
            {
                Response.Clear();
            }
        }
        public void Send(byte[] Data)
        {
            var buffer = Data.ServerEncrypt(GetKey);

            SendBytes(buffer);
        }

        public void SendBytes(byte[] buffer)
        {
            if (Tcp.Connected && Connected)
            {
                Tcp.GetStream().Write(buffer, 0, buffer.Length);
            }
            Task.Delay(5000);
        }
       
        /// </summary>
        public void RefreshData()
        {
            while (true)
            {
                //Aguarda tempo
                Thread.Sleep(TimeSpan.FromMinutes(20));

                try
                {
                    var context = ((IObjectContextAdapter)_db).ObjectContext;

                    var refreshableObjects = from entry in context.ObjectStateManager.GetObjectStateEntries(
                                                                EntityState.Deleted
                                                              | EntityState.Modified
                                                              | EntityState.Unchanged)
                                             where entry.EntityKey != null
                                             select entry.Entity;
                    context.Refresh(RefreshMode.StoreWins, refreshableObjects);
                }
                catch
                {
                    Disconnect();
                }
            }
        }
        #endregion

        #region Dispose

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
                    this.Connected = false;
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



        /// <summary>
        /// Destrutor
        /// </summary>
        ~Player()
        {
            Dispose(false);
        }


        #endregion
    }
}
