using PangyaAPI;
using PangyaAPI.BinaryModels;
using PangyaAPI.Tools;
using Py_Connector.DataBase;
using Py_Game.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Py_Game.Lobby.Collection
{
    public class ChannelCollection : List<Channel>
    {
        public static ChannelCollection LobbyList { get; set; }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public ChannelCollection(IniFile Ini)
        {
            //GameInformation gameInfo;
            byte i;
            try
            {
                var LobbyCount = Ini.ReadByte("Channel", "ChannelCount", 0);
                for (i = 1; i <= LobbyCount; i++)
                {
                    string name = Ini.ReadString("Channel", $"ChannelName_{i}", $"#Lobby {i}");
                    ushort maxuser = Ini.ReadUInt16("Channel", $"ChannelMaxUser_{i}", 100);
                    byte Id = Ini.ReadByte("Channel", $"ChannelID_{i}", i);
                    uint flag = Ini.ReadUInt32("Channel", $"ChannelFlag_{i}", 2048);
                    var lobby = new Channel(name, maxuser, Id, flag);    
                    Add(lobby);
                }
            }
            catch
            { }
            finally
            {
                if (Ini != null)
                    Ini.Dispose();
            }

            WriteConsole.WriteLine("[SERVER_SYSTEM_CHANNEL]: Canais foram carregados !", ConsoleColor.Green);
        }
        public byte[] Build(bool IsBuild = false)
        {
            using (PangyaBinaryWriter Packet = new PangyaBinaryWriter())
            {
                if (IsBuild)
                {
                    Packet.Write(new byte[] { 0x4D, 0x00 });
                }
                Packet.WriteByte(Count);
                for (int i = 0; i < Count; i++)
                {
                    Packet.Write(this[i].Build());
                }
                return Packet.GetBytes();
            }
        }

        public byte[] GetBuildServerInfo()
        {
            var _db = new PangyaEntities();
            var GameServer = _db.ProcGetGameServer().ToList();
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[] { 0x9F, 0x00, Convert.ToByte(GameServer.Count) });
                foreach (var server in GameServer)
                {
                    Response.WriteStr(server.Name, 40);
                    Response.WriteUInt32((uint)server.ServerID);//server UID
                    Response.WriteUInt32((uint)server.MaxUser); //suporte maximo de jogadores no servidor simultaneamente
                    Response.WriteUInt32((uint)server.UsersOnline); //Total de jogadores no servidor atualmente ou simultaneamente(limitador)
                    Response.WriteStr(server.IP, 18);
                    Response.WriteUInt32((uint)server.Port);
                    Response.WriteUInt32((uint)server.Property); //imagem do grand prix 2048, manto 16               
                    Response.WriteUInt32(0); //Angelic Number Count
                    Response.WriteUInt16((ushort)server.ImgEvent);
                    Response.WriteUInt16(126);
                    Response.WriteUInt16(160);
                    Response.WriteUInt16(100);//rate pang
                    Response.WriteUInt16(server.ImgNo);
                }
                Response.Write(Build(false));               
                return Response.GetBytes();
            }
        }

        public void HandleRemoveLobbyPlayer(GPlayer player)
        {
            this.FirstOrDefault(l => l.Players.ToList().Any(p => p?.GetUID == player.GetUID))?.Players?.ToList().Remove(player);
        }
        public void DestroyLobbies()
        {
            this.Clear();
        }

        public void ShowChannel()
        {
            foreach (var lobby in this)
            {
                WriteConsole.WriteLine($"[SHOW_LOBBY_INFO]: LobbyPlayers [{lobby.Players.Count}/{lobby.MaxPlayers}] LobbyID [{lobby.Id}] LobbyName [{lobby.Name}]", ConsoleColor.Green);
            }
        }

        public Channel GetLobby(Channel lobby)
        {
            foreach (var result in this)
            {
                if (result.Id == lobby.Id)
                {
                    return result;
                }
            }
            return null;
        }
        public Channel GetLobby(byte LobbyID)
        {
            foreach (var result in this)
            {
                if (result.Id == LobbyID)
                {
                    return result;
                }
            }
            return null;
        }
    }
}
