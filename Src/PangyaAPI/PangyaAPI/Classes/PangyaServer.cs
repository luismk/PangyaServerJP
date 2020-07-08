using PangyaAPI.Auth;
using Py_Connector.DataBase;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PangyaAPI
{
    public class ServerSettings
    {
        public string Key { get; set; } = "AuthConnectionKey";
        public AuthClientTypeEnum Type { get; set; }
        public uint UID { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string IP { get; set; }
        public uint Port { get; set; }
        public uint MaxPlayers { get; set; }
        public uint Property { get; set; }
        public short EventFlag { get; set; }
        public short ImgNo { get; set; }
        public long BlockFunc { get; set; }
        public string GameVersion { get; set; } = "824.00";
        public string AuthServer_Ip { get; set; } = "127.0.0.1";
        public int AuthServer_Port { get; set; } = 7997;
        public ServerSettings()
        {

        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void InsertServer()
        {
            using (var _db = new PangyaEntities())
            {
                if (_db.Pangya_Server.Where(c => c.ServerID == UID).Any())
                {
                    var query = $"UPDATE [dbo].[Pangya_Server] Set Name = '{Name}', IP = '{IP}', Port =  '{Port}',MaxUser = '{MaxPlayers}', Property = '{Property}', BlockFunc = '{BlockFunc}', ImgNo = '{ImgNo}',ImgEvent = '{EventFlag}', UsersOnline =  '{0}', Active = '{1}' where ServerID = '{UID}'";
                    _db.Database.SqlQuery<PangyaEntities>(query).FirstOrDefault();
                }
                else
                {
                    var query = $"INSERT INTO [dbo].[Pangya_Server](ServerID, Name ,IP ,Port ,MaxUser, UsersOnline ,Property ,BlockFunc ,ImgNo ,ImgEvent ,ServerType ,Active) VALUES( '{UID}','{Name}', '{IP}', '{Port}','{MaxPlayers}', '{0}','{Property}', '{BlockFunc}','{ImgNo}', '{EventFlag}', '{Convert.ToInt32(Type)}', '{1}')";
                    _db.Database.SqlQuery<PangyaEntities>(query).FirstOrDefault();
                }
            }
        }
    }
}
