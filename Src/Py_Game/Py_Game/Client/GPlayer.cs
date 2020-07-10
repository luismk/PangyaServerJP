using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using PangyaAPI;
using Py_Connector.DataBase;
using Py_Game.Client.Inventory;
using Py_Game.Defines;
using Py_Game.GameTools;
using Py_Game.Functions.Core;
using Py_Game.Functions;
using Py_Game.Functions.MiniGames;
using static Py_Game.Functions.MiniGames.PapelSystem;
using static Py_Game.Functions.MiniGames.MemorialSystem;
using static Py_Game.Functions.MiniGames.ScratchCardSystem;
using Py_Game.Lobby;
using Py_Game.Game;
using Py_Game.Client.Data;
using System.IO;
using PangyaAPI.BinaryModels;
using PangyaAPI.PangyaClient;
using PangyaAPI.Tools;
using PangyaAPI.PangyaPacket;
using Py_Game.MainServer;

namespace Py_Game.Client
{
    public partial class GPlayer : Player
    {
        public ushort GameID { get; set; }
        public PlayerInventory Inventory { get; set; }
        public GameBase Game { get; set; }
        public bool InLobby { get; set; }
        public bool InGame { get; set; }

        public bool TutorialCompleted { get; set; }
        public byte Visible { get; set; }
        public string LockerPWD { get; set; }
        public uint LockerPang { get; set; }
        public PangyaEntities Db { get; set; }
        public uint GetPang { get { return ((uint)UserStatistic.Pang); } }

        public uint GetCookie { get; set; }
        public uint GetExpPoint { get { return (UserStatistic.EXP); } }

        public new byte GetLevel { get { return (Convert.ToByte(UserStatistic.Level)); } }

       // public new GameServer Server { get { return Program._server; } }

        public string GetSubLogin { get { return GetLogin + "@NT"; } }
        public byte Assist { get; set; }
        public Dictionary<uint, TAchievementCounter> AchievemetCounters { get; set; }
        public List<TAchievement> Achievements { get; set; }
        public List<TAchievementQuest> AchievementQuests { get; set; }
        public uint SearchUID { get; set; }
        public uint IDState { get; set; }
        public GamePlay GameInfo { get; set; }
        public GuildData GuildInfo;
        public StatisticData UserStatistic;
        public TClubUpgradeTemporary ClubTemporary { get; set; }

        public Channel Lobby { get; set; }

        public byte Level
        {
            get
            {
                return GetLevel;
            }
            set
            {
                SetLevel(value);
            }
        }
        public uint Exp
        {
            get
            {
                return GetExpPoint;
            }
            set
            {
                SetExp(value);
            }
        }
        public GPlayer(TcpClient tcp) : base(tcp)
        {
            Db = new PangyaEntities();
            Achievements = new List<TAchievement>();
            AchievemetCounters = new Dictionary<uint, TAchievementCounter>();
            AchievementQuests = new List<TAchievementQuest>();
            InLobby = false;
            InGame = false;
            Visible = 0;
            LockerPWD = "0";
            GameID = 0xFFFF;
            LockerPang = 0;
            Lobby = null;
            Game = null;
            GameInfo = new GamePlay();
            GetSex = 0x0080;
            ClubTemporary = new TClubUpgradeTemporary();
        }

        public void HandleRequestPacket(TGAMEPACKET PacketID, Packet packet)
        {

            //// Remove o PACKET do erro 2955000 ao Selecionar Servidor
            //if (packet.Id == 139)
            //{
            //    return;
            //}

            //// Remove o PACKET do erro 2955000 ao Tacar durante partida
            //if (packet.Id == 66)
            //{
            //    return;
            //}

            //// Remove o PACKET do erro 2955000 ao Comprar COOKIES
            //if (packet.Id == 162)
            //{
            //    return;
            //}

            //// Remove o PACKET do erro 2955000 ao Comprar COOKIES
            //if (packet.Id == 405)
            //{
            //    return;
            //}

            //// Remove o PACKET do erro 2955000 ao Comprar COOKIES
            //if (packet.Id == 61)
            //{
            //    return;
            //}

            //// Remove o PACKET do erro 2955000 ao ver Info de Caddie
            //if (packet.Id == 107)
            //{
            //    return;
            //}

            //// Remove o PACKET do erro 2955000 ao Salvar Replay durante partida
            //if (packet.Id == 74)
            //{
            //    return;
            //}

            //// Remove o PACKET de erro ao Usar Asa de Safety
            //if (packet.Id == 312)
            //{
            //    return;
            //}



            //if (packet.Id == 157)
            //{
            //    this.Send(new byte[] { 0x0E, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            //    return;
            //}


            //// Remove o PACKET
            //if (packet.Id == 85)
            //{
            //    return;
            //}

            //// Remove o PACKET  ao enviar denuncia na partida
            //if (packet.Id == 58)
            //{
            //    return;
            //}

            //// Remove o PACKET de erro ao terminar tutorial
            //if (packet.Id == 174)
            //{
            //    return;
            //}

            //// Remove o PACKET de erro ao usar Trocar nome da Guild
            //if (packet.Id == 259)
            //{
            //    this.Send(new byte[] { 0x0E, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            //    return;
            //}

            //// Remove o PACKET de erro ao usar Tiki Point Shop
            //if (packet.Id == 397)
            //{
            //    this.Send(new byte[] { 0x0E, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            //    return;
            //}

            //// Remove o PACKET de erro ao usar Reciclagem de Card
            //if (packet.Id == 341)
            //{
            //    this.Send(new byte[] { 0x0E, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            //    return;
            //}

            switch (PacketID)
            {
                #region LoginCore System
                case TGAMEPACKET.PLAYER_LOGIN:
                    {
                        new LoginCoreSystem().PlayerLogin(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_KEEPLIVE:
                    {
                        this.Send(new byte[] { 0x0E, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                    }
                    break;
                case TGAMEPACKET.PLAYER_EXCEPTION:
                    {
                        var Code = packet.ReadByte();
                        var msg = packet.ReadPStr();
                        using (var FileWrite = new StreamWriter("PlayerException.txt", true))
                        {
                            FileWrite.WriteLine($"--------------------------- PLAYER_EXCEPTION ------------------------------------------");
                            FileWrite.WriteLine($"Date: {DateTime.Now}");
                            FileWrite.WriteLine($"Player_Info: {GetLogin}, ID {GetUID}");
                            FileWrite.WriteLine("ID_ERROR: " + Code);
                            FileWrite.WriteLine("Message: " + msg);
                            FileWrite.WriteLine($"------------------------------- END ---------------------------------------------------");
                        }

                        Response = new PangyaBinaryWriter();
                        //Gera Packet com chave de criptografia (posisão 8)
                        Response.Write(new byte[] { 0x00, 0x06, 0x00, 0x00, 0x3f, 0x00, 0x01, 0x01 });
                        Response.WriteByte(GetKey);
                        SendBytes(Response.GetBytes());
                        Response.Clear();

                        this.Server.DisconnectPlayer(this);
                    }
                    break;
                case TGAMEPACKET.PLAYER_MATCH_HISTORY:
                    {
                        new GameCore().PlayerGetMatchHistory(this);
                    }
                    break;
                #endregion
                #region Lobby System
                case TGAMEPACKET.PLAYER_CHAT:
                    {
                        new LobbyCoreSystem().PlayerChat(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_WHISPER:
                    {
                        new LobbyCoreSystem().PlayerWhisper(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_SELECT_LOBBY:
                    {
                        new LobbyCoreSystem().PlayerSelectLobby(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_CHANGE_NICKNAME:
                    {
                        new LobbyCoreSystem().PlayerChangeNickname(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_JOIN_MULTIGAME_LIST:
                    {
                        new LobbyCoreSystem().PlayerJoinMultiGameList(this);
                    }
                    break;
                case TGAMEPACKET.PLAYER_LEAVE_MULTIGAME_LIST:
                    {
                        new LobbyCoreSystem().PlayerLeaveMultiGamesList(this);
                    }
                    break;
                case TGAMEPACKET.PLAYER_JOIN_MULTIGAME_GRANDPRIX:
                    {
                        new LobbyCoreSystem().PlayerJoinMultiGameList(this, true);
                    }
                    break;
                case TGAMEPACKET.PLAYER_LEAVE_MULTIGAME_GRANDPRIX:
                    {
                        new LobbyCoreSystem().PlayerLeaveMultiGamesList(this, true);
                    }
                    break;
                case TGAMEPACKET.PLAYER_SAVE_MACRO:
                    {
                        new GameCore().PlayerSaveMacro(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_REQUEST_TIME:
                    {
                        new LobbyCoreSystem().PlayerGetTime(this);
                    }
                    break;
                case TGAMEPACKET.PLAYER_REQUEST_LOBBY_INFO:
                    {
                        new LobbyCoreSystem().PlayerGetLobbyInfo(this);
                    }
                    break;
                case TGAMEPACKET.PLAYER_CHANGE_SERVER:
                    {
                        new GameCore().PlayerChangeServer(this);
                    }
                    break;
                case TGAMEPACKET.PLAYER_SELECT_LOBBY_WITH_ENTER_TLobby:
                    {
                        new LobbyCoreSystem().PlayerSelectLobby(this, packet, true);
                    }
                    break;
                case TGAMEPACKET.PLAYER_REQUEST_PLAYERINFO:
                    {
                        new LoginInfoCoreSystem().HandleUserInfo(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_TUTORIAL_MISSION:
                    {
                        TutorialCoreSystem.PlayerTutorialMission(this, packet);
                    }
                    break;
                #endregion
                #region Papel Shop System
                case TGAMEPACKET.PLAYER_OPEN_PAPEL:
                    {
                        OpenRareShop(this);
                    }
                    break;
                case TGAMEPACKET.PLAYER_OPEN_NORMAL_BONGDARI:
                    {
                        PlayNormalPapel(this);
                    }
                    break;
                case TGAMEPACKET.PLAYER_OPEN_BIG_BONGDARI:
                    {
                        PlayBigPapel(this);
                    }
                    break;
                case TGAMEPACKET.PLAYER_MEMORIAL:
                    {
                        new MemorialSystem().PlayMemorialGacha(this, packet);
                    }
                    break;
                #endregion
                #region MailBox System
                case TGAMEPACKET.PLAYER_OPEN_MAILBOX:
                    {
                        new MailBoxSystem().PlayerGetMailList(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_READ_MAIL:
                    {
                        new MailBoxSystem().PlayerReadMail(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_RELEASE_MAILITEM:
                    {
                        new MailBoxSystem().PlayerReleaseItem(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_DELETE_MAIL:
                    {
                        new MailBoxSystem().PlayerDeleteMail(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_CHECK_USER_FOR_GIFT:
                    {
                        new MailBoxSystem().CheckUserForGift(this, packet);
                    }
                    break;
                #endregion
                #region GameMaster System
                case TGAMEPACKET.PLAYER_GM_COMMAND:
                    {
                        new GameMasterCoreSystem().PlayerGMCommand(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_GM_DESTROY_ROOM:
                    {
                        new GameMasterCoreSystem().PlayerGMDestroyRoom(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_GM_KICK_USER:
                    {
                        new GameMasterCoreSystem().PlayerGMDisconnectUserByConnectID(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_GM_SEND_NOTICE:
                    {
                        new GameMasterCoreSystem().PlayerGMSendNotice(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_GM_IDENTITY:
                    {
                        new GameMasterCoreSystem().PlayerGMChangeIdentity(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_GM_ENTER_ROOM:
                    {
                        new GameMasterCoreSystem().PlayerGMJoinGame(this, packet);
                    }
                    break;
                #endregion
                #region GameShop System
                case TGAMEPACKET.PLAYER_BUY_ITEM_GAME:
                    {
                        new GameShopCoreSystem().PlayerBuyItemGameShop(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_ENTER_TO_SHOP:
                    {
                        new GameShopCoreSystem().PlayerEnterGameShop(this);
                    }
                    break;
                #endregion
                #region MessengeServer System
                case TGAMEPACKET.PLAYER_REQUEST_MESSENGER_LIST:
                    {
                        new MessengerServerCoreSystem().PlayerConnectMessengerServer(this);
                    }
                    break;
                #endregion
                #region Handle Change Itens
                case TGAMEPACKET.PLAYER_SAVE_BAR:
                case TGAMEPACKET.PLAYER_CHANGE_EQUIPMENT:
                    {
                        new GameCore().PlayerSaveBar(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_CHANGE_EQUIPMENTS:
                    {
                        new GameCore().PlayerChangeEquipment(this, packet);
                    }
                    break;
                #endregion
                #region SelfDesign System
                case TGAMEPACKET.PLAYER_AFTER_UPLOAD_UCC:
                    {
                        new SelfDesignCoreSystem().PlayerAfterUploaded(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_REQUEST_UPLOAD_KEY:
                    {
                        new SelfDesignCoreSystem().PlayerRequestUploadKey(this, packet);
                    }
                    break;
                #endregion
                #region BoxRandom System
                case TGAMEPACKET.PLAYER_OPEN_BOX:
                    {
                        new BoxItemCoreSystem().PlayerOpenBox(this, packet);
                    }
                    break;
                #endregion                
                #region MyRoom System
                case TGAMEPACKET.PLAYER_ENTER_ROOM:
                    {
                        new MyRoomCoreSystem().PlayerEnterPersonalRoom(this);
                    }
                    break;
                case TGAMEPACKET.PLAYER_ENTER_ROOM_GETINFO:
                    {
                        new MyRoomCoreSystem().PlayerEnterPersonalRoomGetCharData(this);
                    }
                    break;
                #endregion
                #region ScracthCard System
                case TGAMEPACKET.PLAYER_OPENUP_SCRATCHCARD:
                    {
                        new ScratchCardSystem(this);
                    }
                    break;
                case TGAMEPACKET.PLAYER_ENTER_SCRATCHY_SERIAL:
                    {
                        PlayerScratchCardSerial(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_PLAY_SCRATCHCARD:
                    {
                        PlayerPlayScratchCard(this);
                    }
                    break;
                #endregion 
                #region Dolfine Locker System 
                case TGAMEPACKET.PLAYER_FIRST_SET_LOCKER:
                    {
                        new DolfineLockerSystem().PlayerSetLocker(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_ENTER_TO_LOCKER:
                    {
                        new DolfineLockerSystem().HandleEnterRoom(this);
                    }
                    break;
                case TGAMEPACKET.PLAYER_OPEN_LOCKER:
                    {
                        new DolfineLockerSystem().PlayerOpenLocker(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_CHANGE_LOCKERPWD:
                    {
                        new DolfineLockerSystem().PlayerChangeLockerPwd(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_GET_LOCKERPANG:
                    {
                        new DolfineLockerSystem().PlayerGetPangLocker(this);
                    }
                    break;
                case TGAMEPACKET.PLAYER_LOCKERPANG_CONTROL:
                    {
                        new DolfineLockerSystem().PlayerPangControlLocker(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_CALL_LOCKERITEMLIST:
                    {
                        new DolfineLockerSystem().PlayerGetLockerItem(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_PUT_ITEMLOCKER:
                    {
                        new DolfineLockerSystem().PlayerPutItemLocker(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_TAKE_ITEMLOCKER:
                    {
                        new DolfineLockerSystem().PlayerTalkItemLocker(this, packet);
                    }
                    break;
                #endregion
                #region ClubSet System
                case TGAMEPACKET.PLAYER_UPGRADE_CLUB:
                    {
                        new ClubSystem().PlayerClubUpgrade(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_UPGRADE_ACCEPT:
                    {
                        new ClubSystem().PlayerUpgradeClubAccept(this);
                    }
                    break;
                case TGAMEPACKET.PLAYER_UPGRADE_CALCEL:
                    {
                        new ClubSystem().PlayerUpgradeClubCancel(this);
                    }
                    break;
                case TGAMEPACKET.PLAYER_UPGRADE_RANK:
                    {
                        new ClubSystem().PlayerUpgradeRank(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_TRASAFER_CLUBPOINT:
                    {
                        new ClubSystem().PlayerTransferClubPoint(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_CLUBSET_ABBOT:
                    {
                        new ClubSystem().PlayerUseAbbot(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_CLUBSET_POWER:
                    {
                        new ClubSystem().PlayerUseClubPowder(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_UPGRADE_CLUB_SLOT:
                    {
                        new ClubSystem().PlayerUpgradeClubSlot(this, packet);
                    }
                    break;
                #endregion
                #region Guild System
                //case TGAMEPACKET.PLAYER_CHANGE_INTRO:
                //    break;
                //case TGAMEPACKET.PLAYER_CHANGE_NOTICE:
                //    break;
                //case TGAMEPACKET.PLAYER_CHANGE_SELFINTRO:
                //    break;
                //case TGAMEPACKET.PLAYER_LEAVE_GUILD:
                //    break;
                //case TGAMEPACKET.PLAYER_CALL_GUILD_LIST:
                //    break;
                //case TGAMEPACKET.PLAYER_SEARCH_GUILD:
                //    break;
                //case TGAMEPACKET.PLAYER_GUILD_AVAIABLE:
                //    break;
                //case TGAMEPACKET.PLAYER_CREATE_GUILD:
                //    break;
                //case TGAMEPACKET.PLAYER_REQUEST_GUILDDATA:
                //    break;
                //case TGAMEPACKET.PLAYER_GUILD_GET_PLAYER:
                //    break;
                //case TGAMEPACKET.PLAYER_GUILD_LOG:
                //    break;
                //case TGAMEPACKET.PLAYER_JOIN_GUILD:
                //    break;
                //case TGAMEPACKET.PLAYER_CANCEL_JOIN_GUILD:
                //    break;
                //case TGAMEPACKET.PLAYER_GUILD_ACCEPT:
                //    break;
                //case TGAMEPACKET.PLAYER_GUILD_KICK:
                //    break;
                //case TGAMEPACKET.PLAYER_GUILD_PROMOTE:
                //    break;
                //case TGAMEPACKET.PLAYER_GUILD_DESTROY:
                //    break;
                //case TGAMEPACKET.PLAYER_GUILD_CALL_UPLOAD:
                //    break;
                //case TGAMEPACKET.PLAYER_GUILD_CALL_AFTER_UPLOAD:
                //    break;
                #endregion
                #region ItemCore System
                case TGAMEPACKET.PLAYER_CHANGE_MASCOT_MESSAGE:
                    {
                        new GameCore().PlayerChangeMascotMessage(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_REQUEST_CHECK_DAILY_ITEM:
                    {
                        new LoginDailyRewardSystem().PlayerDailyLoginCheck(this, 1);
                    }
                    break;
                case TGAMEPACKET.PLAYER_REQUEST_ITEM_DAILY:
                    {
                        new LoginDailyRewardSystem().PlayerDailyLoginItem(this);
                    }
                    break;
                case TGAMEPACKET.PLAYER_RENEW_RENT:
                    {
                        new RentalCoreSystem().PlayerRenewRent(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_DELETE_RENT:
                    {
                        new RentalCoreSystem().PlayerDeleteRent(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_CALL_CUTIN:
                    {
                        new GameCore().PlayerGetCutinInfo(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_REMOVE_ITEM:
                    {
                        new RentalCoreSystem().PlayerRemoveItem(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_PLAY_AZTEC_BOX:
                    {
                        new CometRefillCoreSystem().PlayerOpenAzectBox(this, packet);
                    }
                    break;
                #endregion
                #region WebPangya
                case TGAMEPACKET.PLAYER_REQUEST_WEB_COOKIES:
                    break;
                #endregion
                #region MagicBox System
                case TGAMEPACKET.PLAYER_DO_MAGICBOX:
                    {
                        new CaddieMagicBoxSystem().PlayerMagicBox(this, packet);
                    }
                    break;
                #endregion               
                #region Quest System
                case TGAMEPACKET.PLAYER_LOAD_QUEST:
                    //  SendResponse(new byte[] { 0x0E, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                    break;
                case TGAMEPACKET.PLAYER_ACCEPT_QUEST:
                    break;
                #endregion
                #region Card System
                case TGAMEPACKET.PLAYER_OPEN_CARD:
                    {
                        new CardSystem().PlayerOpenCardPack(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_CARD_SPECIAL:
                    {
                        new CardSystem().PlayerCardSpecial(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_PUT_CARD:
                    {
                        new CardSystem().PlayerPutCard(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_PUT_BONUS_CARD:
                    {
                        new CardSystem().PlayerPutBonusCard(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_REMOVE_CARD:
                    {
                        new CardSystem().PlayerCardRemove(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_LOLO_CARD_DECK:
                    {
                        new CardSystem().PlayerLoloCardDeck(this, packet);
                    }
                    break;
                #endregion
                #region Achievement System
                case TGAMEPACKET.PLAYER_CALL_ACHIEVEMENT:
                    {
                        string fileDir = @"C:\SendAchievements\";
                        var PacketAchievements = File.ReadAllBytes(fileDir + "CompressAchievements.hex");

                        if (PacketAchievements.Count() == 0)
                        {
                            return;
                        }

                        SendBytes(PacketAchievements);
                        // new AchievementCoreSystem().PlayerGetAchievement(this, packet);
                    }
                    break;
                #endregion
                #region Ticket System
                case TGAMEPACKET.PLAYER_SEND_TOP_NOTICE:
                    {
                        new TicketCoreSystem().PlayerNoticeTicker(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_CHECK_NOTICE_COOKIE:
                    {
                        new TicketCoreSystem().PlayerCheckTickerCookies(this);
                    }
                    break;
                #endregion
                #region Character System
                case TGAMEPACKET.PLAYER_UPGRADE_STATUS:
                    {
                        new CharacterCoreSystem().PlayerUpgradeCharacter(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_DOWNGRADE_STATUS:
                    {
                        new CharacterCoreSystem().PlayerUpgradeCharacter(this, packet);
                    }
                    break;
                #endregion
                #region GameBase System
                case TGAMEPACKET.PLAYER_LEAVE_GAME:
                    {
                        new LobbyCoreSystem().PlayerLeaveGame(this);
                    }
                    break;
                case TGAMEPACKET.PLAYER_OPEN_TIKIREPORT:
                    break;
                case TGAMEPACKET.PLAYER_CREATE_GAME:
                    {
                        new LobbyCoreSystem().PlayerCreateGame(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_JOIN_GAME:
                    {
                        new LobbyCoreSystem().PlayerJoinGame(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_ENTER_GRANDPRIX:
                    {
                        new LobbyCoreSystem().PlayerEnterGP(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_ASSIST_CONTROL:
                    {
                        new GameCore().PlayerControlAssist(this);
                    }
                    break;
                case TGAMEPACKET.PLAYER_REQUEST_GAMEINFO:
                    {
                        new LobbyCoreSystem().PlayerGetGameInfo(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_LEAVE_GRANDPRIX:
                    {
                        new LobbyCoreSystem().PlayerLeaveGP(this);
                    }
                    break;
                // MAY BE USE FOR CHAT ROOM ONLY
                case TGAMEPACKET.PLAYER_SHOP_CREATE_VISITORS_COUNT:
                case TGAMEPACKET.PLAYER_CLOSE_SHOP:
                case TGAMEPACKET.PLAYER_ENTER_SHOP:
                case TGAMEPACKET.PLAYER_BUY_SHOP_ITEM:
                case TGAMEPACKET.PLAYER_OPEN_SHOP:
                case TGAMEPACKET.PLAYER_EDIT_SHOP_NAME:
                case TGAMEPACKET.PLAYER_SHOP_ITEMS:
                case TGAMEPACKET.PLAYER_SHOP_VISITORS_COUNT:
                case TGAMEPACKET.PLAYER_SHOP_PANGS:
                case TGAMEPACKET.PLAYER_ENTER_TO_ROOM:
                //
                case TGAMEPACKET.PLAYER_USE_ITEM:
                case TGAMEPACKET.PLAYER_SEND_INVITE:
                case TGAMEPACKET.PLAYER_SEND_INVITE_CONFIRM:
                case TGAMEPACKET.PLAYER_PRESS_READY:
                case TGAMEPACKET.PLAYER_START_GAME:
                case TGAMEPACKET.PLAYER_LOAD_OK:
                case TGAMEPACKET.PLAYER_SHOT_DATA:
                case TGAMEPACKET.PLAYER_ACTION:
                case TGAMEPACKET.PLAYER_MASTER_KICK_PLAYER:
                case TGAMEPACKET.PLAYER_CHANGE_GAME_OPTION:
                case TGAMEPACKET.PLAYER_1ST_SHOT_READY:
                case TGAMEPACKET.PLAYER_LOADING_INFO:
                case TGAMEPACKET.PLAYER_GAME_ROTATE:
                case TGAMEPACKET.PLAYER_CHANGE_CLUB:
                case TGAMEPACKET.PLAYER_GAME_MARK:
                case TGAMEPACKET.PLAYER_ACTION_SHOT:
                case TGAMEPACKET.PLAYER_SHOT_SYNC:
                case TGAMEPACKET.PLAYER_HOLE_INFORMATIONS:
                case TGAMEPACKET.PLAYER_REQUEST_ANIMALHAND_EFFECT:
                case TGAMEPACKET.PLAYER_MY_TURN:
                case TGAMEPACKET.PLAYER_HOLE_COMPLETE:
                case TGAMEPACKET.PLAYER_CHAT_ICON:
                case TGAMEPACKET.PLAYER_SLEEP_ICON:
                case TGAMEPACKET.PLAYER_MATCH_DATA:
                case TGAMEPACKET.PLAYER_MOVE_BAR:
                case TGAMEPACKET.PLAYER_PAUSE_GAME:
                case TGAMEPACKET.PLAYER_QUIT_SINGLE_PLAYER:
                case TGAMEPACKET.PLAYER_CALL_ASSIST_PUTTING:
                case TGAMEPACKET.PLAYER_USE_TIMEBOOSTER:
                case TGAMEPACKET.PLAYER_DROP_BALL:
                case TGAMEPACKET.PLAYER_CHANGE_TEAM:
                case TGAMEPACKET.PLAYER_VERSUS_TEAM_SCORE:
                case TGAMEPACKET.PLAYER_POWER_SHOT:
                case TGAMEPACKET.PLAYER_WIND_CHANGE:
                case TGAMEPACKET.PLAYER_REQUEST_RING_EFFECTS:
                case TGAMEPACKET.PLAYER_SEND_GAMERESULT:
                    {
                        var PLobby = Lobby;
                        if (PLobby == null) { Send(PacketCreator.ShowEnterLobby(2)); return; }

                        var PlayerGame = PLobby[GameID];

                        if (PlayerGame == null) { Send(PacketCreator.ShowRoomError(TGAME_CREATE_RESULT.CREATE_GAME_CREATE_FAILED2)); return; }

                        PlayerGame.HandlePacket(PacketID, this, packet);
                    }
                    break;
                #endregion
                case TGAMEPACKET.PLAYER_RECYCLE_ITEM:
                    {
                        new ItemRecycleCoreSystem().PlayerRecycleItem(this, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_REQUEST_CHAT_OFFLINE:
                    {
                        new ChatOffineCoreSystem().PlayerResponseChatOffline(this, packet);
                    }
                    break;
                #region PacketID no Found
                default:
                    {
                        WriteConsole.WriteLine($"[PLAYER_CALL_PACKET_UNKNOWN]: [{PacketID},{GetLogin}]", ConsoleColor.Red);
                        //anula qualquer pacote id não mencionado ou não identificado
                        //Send(PacketCreator.ShowCancelPacket());
                        packet.Save();
                    }
                    break;
                    #endregion
            }
        }

        public void Close()
        {
            Server.DisconnectPlayer(this);
        }

        public void PlayerLeave()
        {
            if (Tcp.Connected && Connected && Db != null)
            {
                Db.Database.SqlQuery<PangyaEntities>($"Exec [dbo].[USP_GAME_LOGOUT] @UID = '{GetUID}'").FirstOrDefault();

                if (Inventory != null)
                    Inventory.Save(Db);// SAVE ITEM
            }
        }
    }
}
