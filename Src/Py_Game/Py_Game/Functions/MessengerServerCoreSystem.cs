using Py_Connector.DataBase;
using Py_Game.Client;
using System.Linq;
using System;
using Py_Game.MainServer;

namespace Py_Game.Functions
{
    public class MessengerServerCoreSystem
    {
        public void PlayerCallMessengerServer(GPlayer PL)
        {
            if (Program._server.Messenger_Active)
            {
                PL.SendResponse(new byte[] { 0xF1, 0x00, 0x00 });
                PL.SendResponse(new byte[] { 0x35, 0x01 });
            }
        }

        public void PlayerConnectMessengerServer(GPlayer PL)
        {
            var db = new PangyaEntities();

            try
            {
                var server = db.ProcGetMessengerServer().ToList();
                PL.Response.Write(new byte[] { 0xFC, 0x00 });
                PL.Response.Write((byte)server.Count);
                foreach (var servidor in server)
                {
                    PL.Response.WriteStr(servidor.Name, 40);
                    PL.Response.Write(servidor.ServerID);
                    PL.Response.Write(servidor.MaxUser); //Max Users
                    PL.Response.Write(PL.Server.Players.Count); //total de players conectados
                    PL.Response.WriteStr(servidor.IP, 18);
                    PL.Response.Write(servidor.Port);
                    PL.Response.Write(4096);//propriedade
                    PL.Response.WriteZero(13);
                }
                PL.SendResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                db.Dispose();
            }
        }
    }
}