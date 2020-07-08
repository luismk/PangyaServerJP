namespace PangyaAPI.Auth
{
    public enum AuthClientTypeEnum
    {
        LoginServer = 0,
        GameServer = 1,
        MessengerServer = 2,
        AuthServer = 3,
        Control = 4,
        All = 5
    }
    public enum AuthPacketEnum
    {
        SERVER_KEEPALIVE = 0x00,
        SERVER_CONNECT = 0x01,
        SERVER_COMMAND = 0x03,
        RECEIVES_USER_UID = 0x04,
        DISCONNECT_PLAYER_ALL_ON_SERVERS = 0x05,
        SERVER_RELEASE_CHAT = 0x15,
        SERVER_RELEASE_NOTICE_GM = 0X09,
        SERVER_RELEASE_TICKET = 0x10,
        SERVER_RELEASE_BOXRANDOM = 0x11,
        SERVER_RELEASE_NOTICE = 0x12,
        CHECK_PLAYER_DUPLICATE = 0x13,
        RESULT_PLAYER_DUPLICATE = 0x14,
        SEND_DISCONNECT_PLAYER = 0x16,
        LOGIN_RESULT = 0x17,
        PLAYER_LOGIN_RESULT = 0x18
    }

    public enum LoginResultEnum
    {
        Sucess,
        Error,
        Exception,
    }
    public enum AuthMessageEnum
    {
        SEND_GIFT_PLAYER = 0X00,
        SEND_KICK_PLAYER = 0x01,

    }
}
