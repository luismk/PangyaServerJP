namespace Py_Game.Defines
{
    public enum TutorialType : ushort
    {
        Rookie = 0,
        NewRookie = 1,//este code aparece quando finaliza o Rookie normal
        Beginner = 256,
        Advancer = 16128
    }
    /// <summary>
    /// This flag is handling shop display related values
    /// </summary>
    public enum MoneyFlag : byte
    {
        /// <summary>
        /// Unknown value
        /// </summary>
        Unknown1 = 128,

        /// <summary>
        /// Displays a "Special" banner on a shop item
        /// </summary>
        BannerSpecial = 64,

        /// <summary>
        /// Displays a "Hot" banner on a shop item
        /// </summary>
        BannerHot = 32,

        /// <summary>
        /// Displays a "New" banner on a shop item
        /// </summary>
        BannerNew = 16,

        /// <summary>
        /// Unknown value
        /// </summary>
        Unknown2 = 8,

        /// <summary>
        /// Item is for display only
        /// </summary>
        DisplayOnly = 4,

        /// <summary>
        /// TODO: Figure out what this value is again
        /// </summary>
        Type = 2,

        /// <summary>
        /// This shop item is active
        /// </summary>
        Active = 1,

        /// <summary>
        /// No special shop display condition
        /// </summary>
        None = 0
    }

    /// <summary>
    /// This flag is handling buying conditions
    /// </summary>
    public enum ShopFlag : byte
    {
        /// <summary>
        /// Unknown value
        /// </summary>
        Unknown1 = 128,

        /// <summary>
        /// Unknown value
        /// </summary>
        Unknown2 = 64,

        /// <summary>
        /// Unknown value
        /// </summary>
        Unknown3 = 32,

        /// <summary>
        /// Unknown value
        /// </summary>
        Unknown4 = 16,

        /// <summary>
        /// Unknown value
        /// </summary>
        Unknown5 = 8,

        /// <summary>
        /// This shop item is a coupon
        /// </summary>
        Coupon = 4,

        /// <summary>
        /// This shop item is not giftable
        /// </summary>
        NonGiftable = 2,

        /// <summary>
        /// This shop item is giftable
        /// </summary>
        Giftable = 1,

        /// <summary>
        /// No special buying conditions
        /// </summary>
        None = 0
    }

    /// <summary>
    /// This flag is handling different card effects
    /// </summary>
    public enum CardEffectFlag : ushort
    {
        /// <summary>
        /// No card effect
        /// </summary>
        None = 0,

        /// <summary>
        /// This card grants an experience bonus
        /// </summary>
        Experience = 1,

        /// <summary>
        /// This card grants a percentual pang increase
        /// </summary>
        PercentPang = 2,

        /// <summary>
        /// This card grants a percentual experience increase
        /// </summary>
        PercentExperience = 3,

        /// <summary>
        /// This card adds a fixed pang bonus
        /// </summary>
        Pang = 4,

        /// <summary>
        /// This card increases the Power statistic
        /// </summary>
        Power = 5,

        /// <summary>
        /// This card increases the Control statistic
        /// </summary>
        Control = 6,

        /// <summary>
        /// This card increases the Accuracy statistic
        /// </summary>
        Accuracy = 7,

        /// <summary>
        /// This card increases the Spin statistic
        /// </summary>
        Spin = 8,

        /// <summary>
        /// This card increases the Curve statistic
        /// </summary>
        Curve = 9,

        /// <summary>
        /// This card increases the Power shot gauge at the beginning of a match
        /// </summary>
        StartingGauge = 10,

        /// <summary>
        /// TODO: Figure out what this effect does again
        /// </summary>
        Inventory = 11
    }
    public enum GM_COMMAND
    {
        GM_Visibility = 3,
        Player_Whisper = 4,
        Player_Lobby = 5,
        Player_Open = 8,
        Player_Close = 9,
        Player_Kick = 10,
        Player_Disconnect_By_UID = 11,
        Player_Change_GameWind = 14,
        Player_Change_GameWeather = 15,
        Player_GiveItem = 18,
        Player_GoldenBell = 19,
        Notice_Prize = 25,
        HioHoleCupScale = 26,
        SetMission = 28,
        MatchMap = 31,
       
    }

    public enum TLEVEL : byte
    {
        ROOKIE_F = 0x00,
        ROOKIE_E = 0x01,
        ROOKIE_D = 0x02,
        ROOKIE_C = 0x03,
        ROOKIE_B = 0x04,
        ROOKIE_A = 0x05,
        BEGINNER_E = 0x06,
        BEGINNER_D = 0x07,
        BEGINNER_C = 0x08,
        BEGINNER_B = 0x09,
        BEGINNER_A = 0x0A,
        JUNIOR_E = 0x0B,
        JUNIOR_D = 0x0C,
        JUNIOR_C = 0x0D,
        JUNIOR_B = 0x0E,
        JUNIOR_A = 0x0F,
        SENIOR_E = 0x10,
        SENIOR_D = 0x11,
        SENIOR_C = 0x12,
        SENIOR_B = 0x13,
        SENIOR_A = 0x14,
        AMATEUR_E = 0x15,
        AMATEUR_D = 0x16,
        AMATEUR_C = 0x17,
        AMATEUR_B = 0x18,
        AMATEUR_A = 0x19,
        SEMI_PRO_E = 0x1A,
        SEMI_PRO_D = 0x1B,
        SEMI_PRO_C = 0x1C,
        SEMI_PRO_B = 0x1D,
        SEMI_PRO_A = 0x1E,
        PRO_E = 0x1F,
        PRO_D = 0x20,
        PRO_C = 0x21,
        PRO_B = 0x22,
        PRO_A = 0x23,
        NATIONAL_PRO_E = 0x24,
        NATIONAL_PRO_D = 0x25,
        NATIONAL_PRO_C = 0x26,
        NATIONAL_PRO_B = 0x27,
        NATIONAL_PRO_A = 0x28,
        WORLD_PRO_E = 0x29,
        WORLD_PRO_D = 0x2A,
        WORLD_PRO_C = 0x2B,
        WORLD_PRO_B = 0x2C,
        WORLD_PRO_A = 0x2D,
        MASTER_E = 0x2E,
        MASTER_D = 0x2F,
        MASTER_C = 0x30,
        MASTER_B = 0x31,
        MASTER_A = 0x32,
        TOP_MASTER_E = 0x33,
        TOP_MASTER_D = 0x34,
        TOP_MASTER_C = 0x35,
        TOP_MASTER_B = 0x36,
        TOP_MASTER_A = 0x37,
        WORLD_MASTER_E = 0x38,
        WORLD_MASTER_D = 0x39,
        WORLD_MASTER_C = 0x3A,
        WORLD_MASTER_B = 0x3B,
        WORLD_MASTER_A = 0x3C,
        LEGEND_E = 0x3D,
        LEGEND_D = 0x3E,
        LEGEND_C = 0x3F,
        LEGEND_B = 0x40,
        LEGEND_A = 0x41,
        INFINITY_LEGEND_E = 0x42,
        INFINITY_LEGEND_D = 0x43,
        INFINITY_LEGEND_C = 0x44,
        INFINITY_LEGEND_B = 0x45,
        INFINITY_LEGEND_A = 0x46
    }

    public enum TWHISPER_ACTION
    {
        Enable = 2,
        Disable = 0
    }
    public enum TVISIBLE_ACTION
    {
        Enable = 0,
        Disable = 1
    }
    public enum TLOBBY_TYPE
    {
        Enable = 0,
        Disable = 1
    }
    public enum TCLUB_STATUS
    {
        csPower = 0x0,
        csControl,
        csImpact,
        csSpin,
        csCurve
    } // end TCLUB_STATUS

    public enum TCLUB_TYPE
    {
        CLUB_TYPE_1W = 0,
        CLUB_TYPE_2W,
        CLUB_TYPE_3W,
        CLUB_TYPE_2L,
        CLUB_TYPE_3L,
        CLUB_TYPE_4L,
        CLUB_TYPE_5L,
        CLUB_TYPE_6L,
        CLUB_TYPE_7L,
        CLUB_TYPE_8L,
        CLUB_TYPE_9L,
        CLUB_TYPE_PW,
        CLUB_TYPE_SW,
        CLUB_TYPE_PT1,
        CLUB_TYPE_PT2
    } // end TCLUB_TYPE

    public enum TGET_CLUB
    {
        gcTypeID,
        gcIndex
    } // end TGET_CLUB

    public enum TGET_ITEM
    {
        gcTypeID,
        gcIndex
    } // end TGET_ITEM

    public enum TTEAM_VERSUS
    {
        tvRed = 0x0,
        tvBlue = 0x1
    } // end TTEAM_VERSUS

    public enum TPOWER_SHOT
    {
        NO_POWERSHOT = 0x0,
        POWERSHOT = 0x1,
        DOUBLE_P_POWERSHOT = 0x2,
        DOUBLE_POWERSHOT = 0x3
    } // end TPOWER_SHOT

    public enum TPLAYER_CHATROOM_ACTION
    {
        PLAYER_WALK = 0x6,
        PLAYER_APPEAR = 0x4
    } // end TPLAYER_CHATROOM_ACTION

    public enum TPACKCARD
    {
        Pack1,
        Pack2,
        Pack3,
        Pack4,
        Rare,
        All
    } // end TPACKCARD

    public enum TPLAYER_ACTION
    {
        PLAYER_ACTION_ROTATION = 0x00,
        PLAYER_ACTION_UNK = 0x01,
        PLAYER_ACTION_APPEAR = 0x04,
        PLAYER_ACTION_SUB = 0x05,
        PLAYER_ACTION_MOVE = 0x06,
        PLAYER_ACTION_ANIMATION = 0x07,
        PLAYER_ACTION_HEADER = 0x08,
        PLAYER_ACTION_NULL = 0x9,
        PLAYER_ANIMATION_WITH_EFFECTS = 0x0A
    } // end TPLAYER_ACTION

    public enum TITEMGROUP
    {
        ITEM_TYPE_CHARACTER = 0x1,
        ITEM_TYPE_PART = 0x2,
        ITEM_TYPE_CLUB = 0x4,
        ITEM_TYPE_BALL = 0x5,
        ITEM_TYPE_USE = 0x6,
        ITEM_TYPE_CADDIE = 0x7,
        ITEM_TYPE_CADDIE_ITEM = 0x8,
        ITEM_TYPE_SETITEM = 0x9,
        ITEM_TYPE_SKIN = 0xE,
        ITEM_TYPE_MASCOT = 0x10,
        ITEM_TYPE_CARD = 0x1F,
        ITEM_TYPE_AUX = 0x1C,
        ITEM_TYPE_HAIR_STYLE = 0xF
    } // end TITEMGROUP

    public enum CharTypeByHairColor : byte
    {
        Nuri =0,
        Hana =1,
        Azer = 2,
        Cecilia = 3,
        Max = 4,
        Kooh = 5,
        Arin = 6,
        Kaz = 7,
        Lucia = 8,
        Nell = 9,
        Spika = 10,
        NR = 11,
        HR = 12,
        CR = 14
    }

    public enum CharacterTypeIdEnum
    {
        NURI = 67108864,
        HANA = 67108865,
        AZER = 67108866,
        CESILLIA = 67108867,
        MAX = 67108868,
        KOOH = 67108869,
        ARIN = 67108870,
        KAZ = 67108871,
        LUCIA = 67108872,
        NELL = 67108873,
        SPIKA = 67108874,
        NURI_R = 67108875,
        HANA_R = 67108876,
        CESILLIA_R = 67108878
    }

    public enum TPLAYER_ACTION_SUB
    {
        PLAYER_ACTION_SUB_STAND = 0x00,
        PLAYER_ACTION_SUB_SIT = 0x01,
        PLAYER_ACTION_SUB_SLEEP = 0x02
    } // end TPLAYER_ACTION_SUB

    public enum TPLAYER_ACTION_SUB_HEADER
    {
        PLAYER_ACTION_HEADER_SCRATCHCARD = 0x0A
    } // end TPLAYER_ACTION_SUB_HEADER


    public enum TGAME_PLAY : uint
    {
        //CODE      || MENSAGEM
        //972821518 || chip in practice is needed
        //01        || not enough players to start the game
        //02        || there are not enough players to start the game
        //07        || failed game already started
        //08        || you need to update pangya to the latest version
        ITEM_NO_EXIST = 972821518,
        NOT_ENOUGH_PLAYERS = 1,
        ENOUGH_PLAYERS = 2,
        START_GAME_FALIED = 07,
        UPDATE_GAME = 08
    }
    public enum GAME_TYPE : byte
    {
        VERSUS_STROKE = 0x00,
        VERSUS_MATCH = 0x01,
        CHAT_ROOM = 0x02,
        GAME_TYPE_03 = 0x03,
        TOURNEY = 0x04,
        TOURNEY_TEAM = 0x05,
        TOURNEY_GUILD = 0x06,
        PANG_BATTLE = 0x07,
        GAME_TYPE_08 = 0x08,
        GAME_TYPE_09 = 0x09,
        GAME_APROACH = 0x0A,
        GM_EVENT = 0x0B,
        GAME_TYPE_0C = 0x0C,
        GAME_ZOD_OFF = 0x0D,
        CHIP_IN_PRACTICE = 0x0E,
        GAME_TYPE_0F = 0x0F,
        GAME_TYPE_10 = 0x10,
        GAME_TYPE_11 = 0x11,
        SSC = 0x12,
        HOLE_REPEAT = 0x13,
        GRANDPRIX = 0x14
    }

    public enum TGAME_MODE
    {
        GAME_MODE_FRONT = 0x00,
        GAME_MODE_BACK = 0x01,
        GAME_MODE_RANDOM = 0x02,
        GAME_MODE_SHUFFLE = 0x03,
        GAME_MODE_REPEAT = 0x04,
        GAME_MODE_SSC = 0x05
    } // end TGAME_MODE
    public enum TGAME_ACTION
    {
        LIST = 0,
        CREATE = 1,
        DESTROY = 2,
        UPDATE = 3
    }

    public enum TLOBBY_ACTION
    {
        CREATE = 1, //criar novos players
        DESTROY = 2, //Remover player
        UPDATE = 3, //update player
        LIST = 4, //list players
    }
    public enum TGAME_MAP
    {
        Blue_Lagoon = 0x00,
        Blue_Water = 0x01,
        Sepia_Wind = 0x02,
        Wind_Hill = 0x03,
        Wiz_Wiz = 0x04,
        West_Wiz = 0x05,
        Blue_Moon = 0x06,
        Silvia_Cannon = 0x07,
        Ice_Cannon = 0x08,
        White_Wiz = 0x09,
        Shinning_Sand = 0x0A,
        Pink_Wind = 0x0B,
        Deep_Inferno = 0x0D,
        Ice_Spa = 0x0E,
        Lost_Seaway = 0x0F,
        Eastern_Valley = 0x10,
        Special_Flag = 0x11,
        Ice_Inferno = 0x12,
        Wiz_City = 0x13,
        Abbot_Mine = 0x14,
        Grand_Zodiac = 0x40, //mapa especial 
        Unknown = 0x7F
    }

    public enum TGAME_CREATE_RESULT : byte
    {
        CREATE_GAME_RESULT_SUCCESS = 0x00,
        CREATE_GAME_RESULT_FULL = 0x02,
        CREATE_GAME_ROOM_DONT_EXISTS = 0x03,
        CREATE_GAME_INCORRECT_PASSWORD = 0x04,
        CREATE_GAME_INVALID_LEVEL = 0x05,
        CREATE_GAME_CREATE_FAILED = 0x07,
        CREATE_GAME_ALREADY_STARTED = 0x08,
        CREATE_GAME_CREATE_FAILED2 = 0x09,
        CREATE_GAME_NEED_REGISTER_WITH_GUILD = 0x0D,
        CREATE_GAME_PANG_BATTLE_INSSUFICENT_PANGS = 0x0F,
        CREATE_GAME_APPROACH_INSSUFICENT_PANGS = 0x11,
        CREATE_GAME_CANT_CREATE = 0x12
    }

    public enum TGAMEPACKET
    {
        PLAYER_LOGIN = 0x0002,
        PLAYER_CHAT = 0x0003,
        PLAYER_SELECT_LOBBY = 0x0004,

        PLAYER_CREATE_GAME = 0x0008,
        PLAYER_JOIN_GAME = 0x0009,

        PLAYER_CHANGE_NICKNAME = 0x0038,
        PLAYER_EXCEPTION = 0x33,
        PLAYER_JOIN_MULTIGAME_LIST = 0x0081,
        PLAYER_LEAVE_MULTIGAME_LIST = 0x0082,
        PLAYER_REQUEST_MESSENGER_LIST = 0x008B,
        PLAYER_JOIN_MULTIGAME_GRANDPRIX = 0x0176,
        PLAYER_LEAVE_MULTIGAME_GRANDPRIX = 0x0177,
        PLAYER_ENTER_GRANDPRIX = 0x0179,
        PLAYER_OPEN_PAPEL = 0x0098,
        PLAYER_OPEN_NORMAL_BONGDARI = 0x014B,
        PLAYER_OPEN_BIG_BONGDARI = 0x0186,
        PLAYER_SAVE_MACRO = 0x0069,
        PLAYER_OPEN_MAILBOX = 0x0143,
        PLAYER_READ_MAIL = 0x0144,
        PLAYER_RELEASE_MAILITEM = 0x0146,
        PLAYER_DELETE_MAIL = 0x0147,
        PLAYER_GM_COMMAND = 0x008F,
        //{GAME PROCESS}
        PLAYER_USE_ITEM = 0x0017,
        PLAYER_SEND_INVITE_CONFIRM = 0x0029,
        PLAYER_SEND_INVITE = 0x00BA,
        PLAYER_PRESS_READY = 0x000D,
        PLAYER_START_GAME = 0x000E,
        PLAYER_LEAVE_GAME = 0x000F,
        PLAYER_KEEPLIVE = 0xF4,
        PLAYER_LOAD_OK = 0x0011,
        PLAYER_SHOT_DATA = 0x001B,
        PLAYER_ACTION = 0x0063,
        //{MAY BE USE FOR CHAT ROOM ONLY}
        PLAYER_ENTER_TO_ROOM = 0x00EB,        
        PLAYER_CLOSE_SHOP = 0x0075,
        PLAYER_OPEN_SHOP = 0x0076,
        PLAYER_ENTER_SHOP = 0x0077,
        PLAYER_SHOP_CREATE_VISITORS_COUNT = 0X0078,
        PLAYER_EDIT_SHOP_NAME = 0X0079,
        PLAYER_SHOP_VISITORS_COUNT = 0X007A,
        PLAYER_SHOP_ITEMS = 0X007C,
        PLAYER_BUY_SHOP_ITEM = 0X007D,
        PLAYER_SHOP_PANGS = 0X007B,
        //
        PLAYER_REQUEST_CHAT_OFFLINE = 0x003C,
        PLAYER_MASTER_KICK_PLAYER = 0x0026,
        PLAYER_CHANGE_GAME_OPTION = 0x000A,
        PLAYER_LEAVE_GRANDPRIX = 0x017A,
        PLAYER_AFTER_UPLOAD_UCC = 0x00B9,
        PLAYER_REQUEST_UPLOAD_KEY = 0x00C9,
        PLAYER_1ST_SHOT_READY = 0x0034,
        PLAYER_LOADING_INFO = 0x0048,
        PLAYER_GAME_ROTATE = 0x0013,
        PLAYER_CHANGE_CLUB = 0x0016,
        PLAYER_GAME_MARK = 0x012E,
        PLAYER_ACTION_SHOT = 0x0012,
        PLAYER_SHOT_SYNC = 0x001C,
        PLAYER_HOLE_INFORMATIONS = 0x001A,
        PLAYER_TUTORIAL_MISSION = 0X00AE,
        PLAYER_MY_TURN = 0x0022,
        PLAYER_HOLE_COMPLETE = 0x0031,
        PLAYER_CHAT_ICON = 0x0018,
        PLAYER_SLEEP_ICON = 0x0032,
        PLAYER_MATCH_DATA = 0x012F,
        PLAYER_MOVE_BAR = 0x0014,
        PLAYER_PAUSE_GAME = 0x0030,
        PLAYER_QUIT_SINGLE_PLAYER = 0x0130,
        PLAYER_CALL_ASSIST_PUTTING = 0x0185,
        PLAYER_USE_TIMEBOOSTER = 0x0065,
        PLAYER_DROP_BALL = 0x0019,
        PLAYER_CHANGE_TEAM = 0x0010,
        PLAYER_VERSUS_TEAM_SCORE = 0x0035,
        PLAYER_POWER_SHOT = 0x0015,
        PLAYER_WIND_CHANGE = 0x0141,
        PLAYER_SEND_GAMERESULT = 0x0006,

        //{ITEM SPECIAL}
        PLAYER_REQUEST_ANIMALHAND_EFFECT = 0x015C,
        PLAYER_REQUEST_RING_EFFECTS = 0x015D,
        teste = 0x0181,
        PLAYER_BUY_ITEM_GAME = 0x001D,
        PLAYER_ENTER_TO_SHOP = 0x0140,
        PLAYER_CHECK_USER_FOR_GIFT = 0x0007,

        PLAYER_SAVE_BAR = 0x000B,
        PLAYER_CHANGE_EQUIPMENT = 0x000C,
        PLAYER_CHANGE_EQUIPMENTS = 0x0020,
        PLAYER_WHISPER = 0x002A,
        PLAYER_REQUEST_TIME = 0x005C,
        PLAYER_GM_DESTROY_ROOM = 0x0060,
        PLAYER_GM_KICK_USER = 0x0061,
        PLAYER_GM_ENTER_ROOM = 0x003E,
        PLAYER_GM_IDENTITY = 0x0041,
        PLAYER_REQUEST_LOBBY_INFO = 0x0043,
        PLAYER_REMOVE_ITEM = 0x0064,
        PLAYER_PLAY_AZTEC_BOX = 0x00EC,
        PLAYER_OPEN_BOX = 0x00EF,
        PLAYER_CHANGE_SERVER = 0x0119,
        PLAYER_ASSIST_CONTROL = 0x0184,
        // PLAYER ITEM RECYCLE
        PLAYER_RECYCLE_ITEM = 0x018D,
        PLAYER_SELECT_LOBBY_WITH_ENTER_TLobby = 0x0083,
        PLAYER_REQUEST_GAMEINFO = 0x002D,
        PLAYER_GM_SEND_NOTICE = 0x0057,
        PLAYER_REQUEST_PLAYERINFO = 0x002F,
        PLAYER_CHANGE_MASCOT_MESSAGE = 0x0073,
        PLAYER_ENTER_ROOM = 0x00B5,
        PLAYER_ENTER_ROOM_GETINFO = 0x00B7,

        //SCRATCHCARD SYSTEM
        PLAYER_OPENUP_SCRATCHCARD = 0x012A,
        PLAYER_PLAY_SCRATCHCARD = 0x0070,
        PLAYER_ENTER_SCRATCHY_SERIAL = 0x0071,

        //DOLFINI LOCKER
        PLAYER_FIRST_SET_LOCKER = 0x00D0,
        PLAYER_ENTER_TO_LOCKER = 0x00D3,
        PLAYER_OPEN_LOCKER = 0x00CC,
        PLAYER_CHANGE_LOCKERPWD = 0x00D1,
        PLAYER_GET_LOCKERPANG = 0x00D5,
        PLAYER_LOCKERPANG_CONTROL = 0x00D4,
        PLAYER_CALL_LOCKERITEMLIST = 0x00CD,
        PLAYER_PUT_ITEMLOCKER = 0x00CE,
        PLAYER_TAKE_ITEMLOCKER = 0x00CF,

        // CLUB
        PLAYER_UPGRADE_CLUB = 0x0164,
        PLAYER_UPGRADE_ACCEPT = 0x0165,
        PLAYER_UPGRADE_CALCEL = 0x0166,
        PLAYER_UPGRADE_RANK = 0x0167,
        PLAYER_TRASAFER_CLUBPOINT = 0x016C,
        PLAYER_CLUBSET_ABBOT = 0x016B,
        PLAYER_CLUBSET_POWER = 0x016D,
        PLAYER_CHANGE_INTRO = 0x0106,
        PLAYER_CHANGE_NOTICE = 0x0105,
        PLAYER_CHANGE_SELFINTRO = 0x0111,
        PLAYER_LEAVE_GUILD = 0x0113,
        PLAYER_UPGRADE_CLUB_SLOT = 0x004B,

        // GUILD SYSTEM
        PLAYER_CALL_GUILD_LIST = 0x0108,
        PLAYER_SEARCH_GUILD = 0x0109,
        PLAYER_GUILD_AVAIABLE = 0x0102,
        PLAYER_CREATE_GUILD = 0x0101,
        PLAYER_REQUEST_GUILDDATA = 0x0104,
        PLAYER_GUILD_GET_PLAYER = 0x0112,
        PLAYER_GUILD_LOG = 0x010A,
        PLAYER_JOIN_GUILD = 0x010C,
        PLAYER_CANCEL_JOIN_GUILD = 0x010D,
        PLAYER_GUILD_ACCEPT = 0x010E,
        PLAYER_GUILD_KICK = 0x0114,
        PLAYER_GUILD_PROMOTE = 0x0110,
        PLAYER_GUILD_DESTROY = 0x0107,
        PLAYER_GUILD_CALL_UPLOAD = 0x0115,
        PLAYER_GUILD_CALL_AFTER_UPLOAD = 0x0116,

        // DIALY LOGIN
        PLAYER_REQUEST_CHECK_DAILY_ITEM = 0x016E,
        PLAYER_REQUEST_ITEM_DAILY = 0x016F,

        // ACHIEVEMENT
        PLAYER_CALL_ACHIEVEMENT = 0x0157,

        // Tiki Report
        PLAYER_OPEN_TIKIREPORT = 0x00AB,


        PLAYER_REQUEST_WEB_COOKIES = 0x00C1,

        // Memorial
        PLAYER_MEMORIAL = 0x017F,

        // PLAYER CARD SYSTEM
        PLAYER_OPEN_CARD = 0x00CA,
        PLAYER_CARD_SPECIAL = 0x00BD,       
        PLAYER_PUT_CARD = 0x018A,
        PLAYER_PUT_BONUS_CARD = 0x018B,
        PLAYER_REMOVE_CARD = 0x018C,
        PLAYER_LOLO_CARD_DECK = 0x0155,

        PLAYER_CALL_CUTIN = 0x00E5,

        // Magic Box
        PLAYER_DO_MAGICBOX = 0x0158,

        // RENT ITEM
        PLAYER_RENEW_RENT = 0x00E6,
        PLAYER_DELETE_RENT = 0x00E7,

        // QUEST
        PLAYER_LOAD_QUEST = 0x0151,
        PLAYER_ACCEPT_QUEST = 0x0152,
        
        PLAYER_MATCH_HISTORY = 0x009C,

        // TOP NOTICE
        PLAYER_SEND_TOP_NOTICE = 0x0066,
        PLAYER_CHECK_NOTICE_COOKIE = 0x0067,

        //CADDIE NOTICE EXPIRATION
        PLAYER_REQUEST_CADDIE_RENEW = 0x006B,

        PLAYER_UPGRADE_STATUS = 0x0188,
        PLAYER_DOWNGRADE_STATUS = 0x0189,

    } // end TGAMEPACKET
    public enum ItemTypeEnum : int
    {
        Active = 0,
        All = -1,
        Passive = 128,
        GM = 252,
    }
    public enum ECLUBTYPE : uint
    {
        TYPE_BALANCE = 0x0,
        TYPE_POWER = 0x1,
        TYPE_CONTROL = 0x2,
        TYPE_SPIN = 0x3,
        TYPE_SPECIAL = 0x4
    } // end ECLUBTYPE

    public enum CARDTYPE
    {
        tNormal = 0x0,
        tCaddie = 0x40,
        tNPC = 0x41,
        tSpecial = 0x80
    } // end CARDTYPE

    public enum CharType
    {
        bTypeID,
        bIndex
    } // end CharType

    public enum TGameShift
    {
        SHIFT_NAME = 0x0,
        SHIFT_PWD = 0x1,
        SHIFT_STROK = 0x2,
        SHIFT_MAP = 0x3,
        SHIFT_NUMHOLE = 0x4,
        SHIFT_MODE = 0x5,
        SHIFT_VSTIME = 0x6,
        SHIFT_MAXPLAYER = 0x7,
        SHIFT_MATCHTIME = 0x8,
        SHIFT_IDLE = 0x9,
        SHIFT_NATURAL = 0xE,
        SHIFT_HOLENUM = 0xB,
        SHIFT_HOLELOCK = 0xC
    } // end TGameShift

    public enum TShotType : byte
    {
        Unknown = 0x1,
        Normal = 0x2,
        OB = 0x3,
        Success = 0x4
    } // end TShotType
    public enum TCLUB_ACTION
    {
        Upgrade = 1,
        Decrement = 2,
        Downgrade = 3,
    }

    public enum TGAME_SHOP_ACTION : byte
    {
        /// <summary>
        /// Itens normais
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 
        /// </summary>
        Rental = 1
    }

    public enum TGAME_SHOP : int
    {
        BUY_SUCCESS = 0,
        BUY_FAIL = 1,
        PANG_NOTENOUGHT = 2,
        PASSWORD_WRONG = 3,
        ALREADY_HAVEITEM = 4,
        OUT_OF_TIME = 11,
        CANNOT_BUY_ITEM1 = 18,
        CANNOT_BUY_ITEM = 19,
        TOO_MUCH_ITEM = 21,
        COOKIE_NOTENOUGHT = 23,
        ITEM_EXPIRED = 35,
        ITEM_CANNOT_PURCHASE = 36,
        LEVEL_NOTENOUGHT = 44
    }
}

