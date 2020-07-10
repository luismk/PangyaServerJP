using System;
using System.Linq;
using PangyaAPI;
using PangyaAPI.BinaryModels;
using Py_Game.Defines;
using Py_Game.Client;
using Py_Game.Client.Data;
using Py_Game.Game.Data;
using static Py_Game.GameTools.TGeneric;
using static Py_Game.GameTools.PacketCreator;
using PangyaAPI.PangyaPacket;

namespace Py_Game.Game.Modes
{
    public class ModeChatRoom : GameBase
    {
        public ModeChatRoom(GPlayer player, GameInformation GameInfo, GameEvent CreateEvent, GameEvent UpdateEvent, GameEvent DestroyEvent, PlayerEvent OnJoin, PlayerEvent OnLeave, ushort GameID) : base(player, GameInfo, CreateEvent, UpdateEvent, DestroyEvent, OnJoin, OnLeave, GameID)
        {
        }

        public void HandleModeChatRoomShop(TGAMEPACKET ID, GPlayer player, Packet packet)
        {
            switch (ID)
            {
                case TGAMEPACKET.PLAYER_CLOSE_SHOP:
                    {
                        PlayerCloseShop(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_OPEN_SHOP:
                    {
                        PlayerOpenShop(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_ENTER_SHOP:
                    {
                        PlayerEnterShop(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_EDIT_SHOP_NAME:
                    {
                        PlayerEditShopName(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_SHOP_ITEMS:
                    {
                        PlayerItemShop(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_SHOP_VISITORS_COUNT:
                    {
                        PlayerShopVisitCount(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_SHOP_PANGS:
                    {
                        PlayerShopPangsInfo(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_BUY_SHOP_ITEM:
                    {
                        PlayerShopBuyItem(player, packet);
                    }
                    break;
                case TGAMEPACKET.PLAYER_SHOP_CREATE_VISITORS_COUNT:
                    {
                        PlayerCreateVisitorsCount(player, packet);
                    }
                    break;
                default:
                    break;
            }            
        }

        protected void PlayerShopBuyItem(GPlayer player, Packet packet)
        {
            if (!packet.ReadUInt32(out uint ShopID))
            {
                return;
            }

            var itens = (ShopItem)packet.Read(new ShopItem());
            foreach (var Data in GameShopData.Where(c => c.ShopOwnerID == ShopID && c.Items.Any(b=> b.TypeID == itens.TypeID)))
            {               
                if (Data.Remove(itens))
                {
                    player.Send(ShowShopItemBuy(1, itens));
                    player.Send(ShowShopItemBuyResult(Data.NickName, Data.ShopOwnerID, itens));
                }
            }
        }

        protected void PlayerShopPangsInfo(GPlayer player, Packet packet)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[] { 0xEA, 0x00 });
                Response.Write(1);// return code
                Response.WriteUInt64(player.GameInfo.GameShop.Pangs); // income
                player.SendResponse(Response.GetBytes());
            }
        }

        protected void PlayerCreateVisitorsCount(GPlayer player, Packet packet)
        {
            if (!packet.ReadUInt32(out uint ShopID))
            {
                return;
            }
            
            if (GameShopData.Any(c => c.ShopOwnerID == ShopID) == false)
            {
                player.Response.Write(new byte[] { 0xE7, 0x00 });
                player.Response.Write(0);//UNKNOWN
                player.SendResponse();
                return;
            }
            
            foreach (var Data in GameShopData.Where(c => c.ShopOwnerID == ShopID))
            {
                Data.CountVisit++;
                player.Response.Write(new byte[] { 0xE7, 0x00 });
                player.Response.Write(player.ConnectionID);//UNKNOWN
                player.SendResponse();
            }
        }

        protected void PlayerEnterShop(GPlayer player, Packet packet)
        {
            if (!packet.ReadUInt32(out UInt32 PlayerUID))
            {
                return;
            }

            var PL = (GPlayer)player.Server.GetPlayerByUID(PlayerUID);


            if (PL == null)
            {
                player.Response.Write(new byte[] { 0xE6, 0x00 });
                player.Response.WriteUInt32(1);
                player.SendResponse();
                return;
            }

            player.SendResponse(ShowShopEnter(PL));
        }

        protected void PlayerItemShop(GPlayer player, Packet packet)
        {
            var ItemCount = packet.ReadInt32();
            for (int i = 0; i < ItemCount; i++)
            {
                var itens = (ShopItem)packet.Read(new ShopItem());
                foreach (var Data in GameShopData.Where(c => c.UID == player.GetUID))
                {
                    Data.Items.Add(itens);
                }
                player.GameInfo.GameShop.Add(itens);
            }
            player.SendResponse(ShowShopItems(player, ItemCount, GameShopData.Where(c => c.UID == player.GetUID).ToList()));
        }

        protected void PlayerEditShopName(GPlayer player, Packet packet)
        {
            if (!packet.ReadPStr(out string ShopName))
            {
                player.Response.Write(new byte[] { 0xE8, 0x00 });
                player.Response.Write(1);
                player.SendResponse();
                return;
            }
            player.GameInfo.GameShop.Name = ShopName;
            Send(ShowEditShopName(ShopName, player.GetUID, player.GetNickname));
            foreach (var item in GameShopData.Where(c => c.UID == player.GetUID))
            {
                item.Name = ShopName;
            }
        }
        protected void PlayerCloseShop(GPlayer player, Packet packet)
        {
            foreach (var data in GameShopData)
            {
                if (data.UID == player.GetUID)
                {
                    GameShopData.ToList().Remove(data);
                    break;
                }
            }
            Send(ShowShopClose(player.GetNickname, player.GetUID));
            player.GameInfo.GameShop.Clear();
        }
        protected void PlayerOpenShop(GPlayer player, Packet packet)
        {
            Send(ShowOpenShop(player.GetNickname, player.GetUID));
            player.GameInfo.GameShop.ShopOwnerID = (uint)new Random().Next();
            GameShopData.Add(new ShopItemData() { UID = player.GetUID, Name = "", ShopOwnerID = player.GameInfo.GameShop.ShopOwnerID, NickName = player.GetNickname });
        }

        protected void PlayerShopVisitCount(GPlayer player, Packet packet)
        {
            uint CountVisit = 0;
            foreach (var data in GameShopData)
            {
                if (data.UID == player.GetUID)
                {
                    CountVisit = data.CountVisit;
                }
            }
            player.SendResponse(ShowShopCountVisit(CountVisit));
        }




        public override void AcquireData(GPlayer player)
        {
            throw new NotImplementedException();
        }

        public override void DestroyRoom()
        {
            throw new NotImplementedException();
        }

        public override byte[] GameInformation()
        {
            var response = new PangyaBinaryWriter();

            response.WriteStr(fGameData.Name, 64); //ok
            response.Write(fGameData.Password.Length > 0 ? false : true);
            response.Write(Started == true ? (byte)0 : (byte)1);
            response.Write(Await);//Orange
            response.Write(fGameData.MaxPlayer);
            response.Write((byte)Players.Count);
            response.Write(GameKey, 17);//ultimo byte é zero
            response.Write(fGameData.Time30S);
            response.Write(fGameData.HoleTotal);
            response.Write((byte)GameType);//GameType
            response.Write((ushort)ID);
            response.Write(fGameData.Mode);
            response.Write(fGameData.Map);
            response.Write(fGameData.VSTime);
            response.Write(fGameData.GameTime);
            response.Write(0);
            response.Write(Idle);
            response.Write(fGameData.GMEvent); //GM Event 0(false), ON 1(true)
            response.WriteZero(76);//GUILD DATA
            response.Write(100);// rate pang 
            response.Write(100);// rate chuva 
            response.Write(Owner);
            response.Write((byte)0xFF); //is practice
            response.Write(fGameData.Artifact);//artefato
            response.Write(fGameData.NaturalMode);//natural mode
            response.Write(fGameData.GPTypeID);//Grand Prix 1
            response.Write(fGameData.GPTypeIDA);//Grand Prix 2
            response.Write(fGameData.GPTime);//Grand Time
            response.Write(Iff<uint>(fGameData.GP, 1, 0));// grand prix active
            return response.GetBytes();
        }

        public override byte[] GetGameHeadData()
        {
            var response = new PangyaBinaryWriter();
            response.Write(new byte[] { 0x4A, 0x00, 0xFF, 0xFF });
            response.Write((byte)GameType);//GameType
            response.Write(fGameData.Map);
            response.Write(fGameData.HoleTotal);
            response.Write(fGameData.Mode);
            response.Write(fGameData.NaturalMode);
            response.Write(fGameData.MaxPlayer);
            response.Write(fGameData.Time30S);
            response.Write(Idle);  //Room Idle
            response.Write(fGameData.VSTime);
            response.Write(fGameData.GameTime);
            response.Write(0); // trophy typeid
            response.Write(fGameData.Password.Length > 0 ? false : true);
            if (fGameData.Password.Length > 0)
            {
                response.WritePStr(fGameData.Password);
            }
            response.WritePStr(fGameData.Name);
            return response.GetBytes();
        }


        public override void GenerateExperience()
        {
            throw new NotImplementedException();
        }


        public override void OnPlayerLeave()
        {
            this.Started = false;
        }

        public override void PlayerGameDisconnect(GPlayer player)
        {
            if (Count == 0)
            {
                player.SetGameID(0xFFFF);
                player.SendResponse(ShowLeaveGame());
                PlayerLeave(this, player);
                this.Destroy(this);
                player.Game = null;
            }
            else
            {
                player.SetGameID(0xFFFF);
                PlayerLeave(this, player);
                OnPlayerLeave();
                Send(ShowGameLeave(player.ConnectionID, 2));
                //{ Find New Master }
                if (player.GetUID == Owner && Players.Count >= 1)
                    FindNewMaster();

                //{ Room Update }
                Update(this);

                player.SendResponse(ShowLeaveGame());

                player.Game = null;
            }
        }

        public override void PlayerLeavePractice()
        {
            throw new NotImplementedException();
        }

        public override void PlayerLoading(GPlayer player, Packet CP)
        {
            throw new NotImplementedException();
        }

        public override void PlayerLoadSuccess(GPlayer player)
        {
            throw new NotImplementedException();
        }

        public override void PlayerSendFinalResult(GPlayer player, Packet CP)
        {
            throw new NotImplementedException();
        }

        public override void PlayerShotData(GPlayer player, Packet CP)
        {
            throw new NotImplementedException();
        }

        public override void PlayerShotInfo(GPlayer player, Packet CP)
        {
            throw new NotImplementedException();
        }

        public override void PlayerStartGame()
        {
            throw new NotImplementedException();
        }

        public override void PlayerSyncShot(GPlayer player, Packet CP)
        {
            throw new NotImplementedException();
        }

        public override void SendHoleData(GPlayer player)
        {
            throw new NotImplementedException();
        }

        public override void SendPlayerOnCreate(GPlayer player)
        {
            var packet = new PangyaBinaryWriter();
            try
            {
                packet.Write(new byte[] { 0x48, 0x00, 0x00 });
                packet.Write(new byte[] { 0xFF, 0xFF });
                packet.Write((byte)1);
                packet.Write(player.GetGameInfomations(2));
                packet.Write((byte)0);
                var GetBytes = packet.GetBytes();
                Send(GetBytes);
            }
            finally
            {
                packet.Dispose();
            }
        }

        public override void SendPlayerOnJoin(GPlayer player)
        {
            PangyaBinaryWriter packet;
            packet = new PangyaBinaryWriter();
            try
            {
                player.GameInfo.GameReady = true;
                foreach (GPlayer P in Players)
                {
                    packet.Clear();
                    packet.Write(new byte[] { 0x48, 0x00 });
                    packet.Write((byte)7);//List
                    packet.Write(new byte[] { 0xFF, 0xFF });
                    packet.Write(Count);
                    packet.Write(P.GetGameInfomations(2));
                    packet.Write((byte)0);
                    player.SendResponse(packet.GetBytes());
                }

                packet.Clear();
                packet.Write(new byte[] { 0x48, 0x00 });
                packet.Write((byte)1);
                packet.Write(new byte[] { 0xFF, 0xFF });
                packet.Write(player.GetGameInfomations(2));
                packet.Write((byte)0);
                Send(packet.GetBytes());
            }
            finally
            {
                packet.Dispose();
            }
        }

        public override bool Validate()
        {
            return true;
        }
    }
}
