using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Messenger.Defines
{
    public enum PangyaEnums
    {
        /// <summary>
        /// LOGIN PLAYER IN SERVER MS
        /// </summary>
        PLAYER_LOGIN = 0x0012,
        /// <summary>
        /// RECONNECT TO PLAYER IN SERVER
        /// </summary>
        PLAYER_RECONNECT = 0x0014,
        /// <summary>
        /// DISCONNECT TO PLAYER IN SERVER
        /// </summary>
        PLAYER_DISCONNECT = 0x0016,
        /// <summary>
        /// requesita procurar um amigo, usando messanger_server
        /// </summary>
        PLAYER_FIND_FRIEND = 0x0017,

        PLAYER_REQUEST_FIND_FRIEND = 0x0018,

        Unknown_19 = 0x0019,

        /// <summary>
        /// Envia informacoes do servidor + lobby selecionado pelo player
        /// </summary>
        PLAYER_SERVER_DATA = 0x0023,
        /// <summary>
        /// chat para guild no messenger server
        /// </summary>
        PLAYER_REQUEST_GUILD_CHAT = 0x0025,
        /// <summary>
        /// jogador bloqueia amigo
        /// </summary>
        PLAYER_BLOCK_FRIEND = 0x001A,
        /// <summary>
        /// jogador desbloqueia amigo
        /// </summary>
        PLAYER_UNBLOCK_FRIEND = 0x001B,
        /// <summary>
        /// jogaodr deleta o amigo da lista de amigos
        /// </summary>
        PLAYER_DELETE_FRIEND = 0x001C,
        /// <summary>
        /// PLAYER ONLINE
        /// </summary>
        SERVER_CHECK_PLAYER_CONNECTED = 0x001D,
        /// <summary>
        /// chat do messenger server
        /// </summary>
        PLAYER_REQUEST_CHAT = 0x001E,

        /// <summary>
        /// MUDA O APELIDO DO JOGADOR 
        /// </summary>
        PLAYER_CHANGE_FRIEND_NICKNAME = 0x001F,

        Unknown_42 = 0x002A,
    }

    public enum USER_STATUS
    {
        IS_PLAYING = 0,
        IS_RECONNECT = 1,
        IS_ONLINE = 4,
        IS_IDLE = 3,
    }
}
