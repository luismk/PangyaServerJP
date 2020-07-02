using PangyaAPI;
using PangyaAPI.PangyaClient;
using PangyaAPI.PangyaPacket;
using PangyaAPI.Tools;
using Py_Messenger.Defines;
using System;

namespace Py_Messenger.Client
{
    public partial class MPlayer : Player
    {
        public void HandleRequestPacket(PangyaEnums PacketID, Packet packet)
        {
            switch (PacketID)
            {
                case PangyaEnums.PLAYER_LOGIN:
                    {
                        HandlePlayerLogin(packet);
                    }
                    break;
                case PangyaEnums.PLAYER_RECONNECT:
                    {
                        SendResponse(new byte[]
                           {
                           0x30, 0x00, 0x15, 0x01, 0xB4, 0x10, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x01, 0xFF, 0xFF, 0x60,
                           0x00, 0x00, 0x00, 0xEA, 0x4E, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                           });


                        //Packet2
                        SendResponse(new byte[]
                        {
                            0x30, 0x00, 0x02, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00
                        });
                    }
                    break;
                case PangyaEnums.PLAYER_DISCONNECT:
                    {
                        HandlePlayerDisconnect();
                    }
                    break;
                case PangyaEnums.PLAYER_FIND_FRIEND:
                    {
                        HandleFindFriend(packet);
                    }
                    break;
                case PangyaEnums.PLAYER_REQUEST_FIND_FRIEND:
                    {
                        HandleAddFriend(packet);
                    }
                    break;
                case PangyaEnums.Unknown_19:
                    break;
                case PangyaEnums.PLAYER_SERVER_DATA:
                    {
                        HandleServerData(packet);
                    }
                    break;
                case PangyaEnums.PLAYER_REQUEST_GUILD_CHAT:
                    break;
                case PangyaEnums.PLAYER_BLOCK_FRIEND:
                    break;
                case PangyaEnums.PLAYER_UNBLOCK_FRIEND:
                    break;
                case PangyaEnums.PLAYER_DELETE_FRIEND:
                    {
                        HandleDeleteFriend(packet);
                    }
                    break;
                case PangyaEnums.SERVER_CHECK_PLAYER_CONNECTED:
                    {
                        HandlePlayerConnected(packet);
                    }
                    break;
                case PangyaEnums.PLAYER_REQUEST_CHAT:
                    break;
                case PangyaEnums.PLAYER_CHANGE_FRIEND_NICKNAME:
                    break;
                case PangyaEnums.Unknown_42:
                    break;
                default:
                    break;
            }
            WriteConsole.WriteLine($"[PLAYER_CALL_PACKET]: [{PacketID},{GetLogin}]", ConsoleColor.Cyan);
        }
    }
}
