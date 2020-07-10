using PangyaAPI.BinaryModels;
using System;
using System.Collections.Generic;
using Py_Game.Defines;
using Py_Game.Client;
using Py_Game.Data;
using Py_Game.Client.Data;
using Py_Game.Client.Inventory.Data;
using static Py_Game.GameTools.Tools;
using static Py_Game.GameTools.TCompare;
namespace Py_Game.GameTools
{
    public static class PacketCreator
    {
        public static byte[] ShowGameStart(TGAME_PLAY result)
        {
            using (var response = new PangyaBinaryWriter())
            {
                response.Write(new byte[] { 0x7F, 0x00 });
                response.Write((uint)result);
                return response.GetBytes();
            }
        }

        public static byte[] ShowGMNotice(string Msg, string Nickname = "Staff")
        {
            using (var response = new PangyaBinaryWriter())
            {
                if (Nickname.Length <= 0 || Msg.Length <= 0)
                {
                    return response.GetBytes();
                }
                response.Write(new byte[] { 0x40, 0x00, 0x07 });
                response.WritePStr(Nickname);
                response.WritePStr(Msg);
                return response.GetBytes();
            }
        }
        public static byte[] ShowMessageGlobal(string message)
        {
            using (var response = new PangyaBinaryWriter())
            {
                response.Write(new byte[] { 0x42, 0x00 });
                response.WritePStr(message);
                return response.GetBytes();
            }
        }
        public static byte[] ShowUserGift(int UID, string Username, string Nickname)
        {
            using (var response = new PangyaBinaryWriter())
            {
                response.Write(new byte[] { 0xA1, 0x00, 0x00 });
                response.WriteInt32(UID);
                response.WriteStr(Username, 22);
                response.WriteStr(Nickname, 22);
                response.WriteZero(93);
                response.WriteStr(Username + "@NT", 19);
                response.WriteZero(109);
                response.WriteInt32(UID);
                return response.GetBytes();
            }
        }


        public static byte[] ShowActionGamePlayInfo(GPlayer player)
        {
            PangyaBinaryWriter Response;

            Response = new PangyaBinaryWriter();
            try
            {
                Response.Write(new byte[] { 0x48, 0x00, 0x03, 0xFF, 0xFF });
                Response.Write(player.GetGameInfomations(0));
                Response.Write(player.GetGameInfomations(1));
                Response.WriteByte(0);
                return Response.GetBytes();
            }
            finally
            {
                Response.Dispose();
            }
        }

        public static byte[] ShowGPTimeCounter(int ProgramMin, int ProgramSec)
        {
            PangyaBinaryWriter Response;

            Response = new PangyaBinaryWriter();
            try
            {
                ProgramMin = 0x0B000000;
                Response.Write(new byte[] { 0x40, 0x00 });
                Response.WriteInt32(ProgramMin);
                Response.WriteByte((byte)0x00);
                Response.WriteInt32(ProgramSec);
                return Response.GetBytes();
            }
            finally
            {
                Response.Dispose();
            }
        }
        public static byte[] ShowCancelPacket()
        {
            return new byte[] { 0x0E, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        }
        public static byte[] ShowEnterLobby(byte code)
        {
            return new byte[] { 0x4E, 0x00, code };
        }
        public static byte[] ShowFirstShotReady()
        {
            return new byte[] { 0x90, 0x00 };
        }
        public static byte[] LobbyInfo(string LobbyName, ushort MaxPlayers, ushort PlayersCount, byte LobbyID, uint LobbyFlag)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.WriteStr(LobbyName, 21);
                //Response.Write(new byte[] { 0x01, 0x00, 0x00, 0x01 });
                //Response.WriteZero(15);
                //Response.Write(new byte[] { 0x08, 0x10, 0x06, 0x07, 0x1a });
                //Response.WriteZero(7);
                //Response.Write(new byte[] { 0x01, 0x14, 0x00, 0x00 }); // Secound byte is variable
                //Response.Write(new byte[] { 0x64, 0x02, 0x00, 0x1a });
                //Response.WriteZero(4);
                Response.Write(new byte[] { 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x10, 0x06, 0x07, 0x1A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x14, 0x00, 0x00, 0x64, 0x02, 0x00, 0x1A, 0x00, 0x00, 0x00, 0x00, });
                Response.WriteUInt16(MaxPlayers);
                Response.WriteUInt16(PlayersCount);
                Response.Write(LobbyID); //Lobby ID
                Response.WriteUInt16(0); //ルーム制限あるね
                Response.WriteUInt16(LobbyFlag); //メンテナンス表記+ナチュラルマーク
                Response.WriteUInt16(0); //メンテナンス表記+Granplix
                Response.WriteUInt16(0); //メンテナンス表記+なんか
                Response.WriteUInt16(0); //メンテナンス表記+ナチュラルマーク
                Response.WriteUInt16(0); //ルーム制限あるね
                Response.WriteUInt16(0); //メンテナンス表記+Granplix
                Response.WriteUInt16(0); //メンテナンス表記+なんか
                return Response.GetBytes();
            }
        }

        public static byte[] ShowChangeNickName(int Code = 0, string nickname = "")
        {
            var response = new PangyaBinaryWriter();
            try
            {
                response.Write(new byte[] { 0x50, 0x00 });
                response.WriteInt32(Code);
                if (Code == 0)
                {
                    response.WritePStr(nickname);
                }
                return response.GetBytes();
            }
            finally
            {
                response.Dispose();
            }
        }

        public static byte[] ChatText(string nickname, string Chat, bool GM)
        {
            var response = new PangyaBinaryWriter();
            response.Write(new byte[] { 0x40, 0x00 });
            response.WriteByte(IfCompare<byte>(GM, 0x80, 0)); //Tipo de chat           
            response.WritePStr(nickname);
            response.WritePStr(Chat);
            return response.GetBytes();
        }

        public static byte[] ShowRoomError(this TGAME_CREATE_RESULT error)
        {
            var Response = new PangyaBinaryWriter();
            Response.Write(new byte[] { 0x49, 0x00 });
            Response.WriteByte((byte)error);
            return Response.GetBytes();
        }
        public static byte[] ShowTutorialPlayer(ushort Code, uint MissionID, bool IsLogin = true)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[] { 0x1F, 0x01 });
                if (IsLogin)
                {
                    Response.WriteUInt16(Code);
                    Response.Write(MissionID);
                    Response.WriteByte(0);
                    Response.WriteUInt32(Code);
                    Response.WriteZero(8);
                }
                else
                {
                    Response.WriteByte(Code);
                    Response.Write((byte)1);
                    Response.Write(MissionID);
                }
                return Response.GetBytes();
            }
        }
        public static byte[] ShowLoadServer(byte Value)
        {
            return new byte[] { 0x44, 0x00, 0xD2, Value, 0x00, 0x00, 0x00 };
        }

        public static byte[] ShowLoadMap()
        {
            return new byte[] { 0x31, 0x01, 0x01, 0x15, 0x00, 0xE8, 0x03, 0x00, 0x00, 0x01, 0xE8, 0x03, 0x00, 0x00, 0x02, 0xE8, 0x03, 0x00, 0x00, 0x03, 0xE8, 0x03, 0x00, 0x00, 0x04, 0xE8, 0x03, 0x00, 0x00, 0x05, 0xE8, 0x03, 0x00, 0x00, 0x06, 0xE8, 0x03, 0x00, 0x00, 0x07, 0xE8, 0x03, 0x00, 0x00, 0x08, 0xE8, 0x03, 0x00, 0x00, 0x09, 0xE8, 0x03, 0x00, 0x00, 0x0A, 0xE8, 0x03, 0x00, 0x00, 0x0B, 0xE8, 0x03, 0x00, 0x00, 0x0C, 0xE8, 0x03, 0x00, 0x00, 0x0D, 0xE8, 0x03, 0x00, 0x00, 0x0E, 0xE8, 0x03, 0x00, 0x00, 0x0F, 0xE8, 0x03, 0x00, 0x00, 0x10, 0xE8, 0x03, 0x00, 0x00, 0x11, 0xE8, 0x03, 0x00, 0x00, 0x12, 0xE8, 0x03, 0x00, 0x00, 0x13, 0xE8, 0x03, 0x00, 0x00, 0x14, 0xE8, 0x03, 0x00, 0x00 };
        }
        public static byte[] ShowKeepLive()
        {
            return new byte[] { 0xA9, 0x01, 0x01, 0x10, 0x27, 0x00, 0x00 };
        }

        public static byte[] ShowLeaveGame()
        {
            var Response = new PangyaBinaryWriter();
            Response.Write(new byte[] { 0x4C, 0x00 });
            Response.WriteUInt16(0xFFFF);
            return Response.GetBytes();
        }
        public static byte[] ShowGameAction(byte[] GameInformation, TGAME_ACTION action)
        {
            var result = new PangyaBinaryWriter();

            result.Write(new byte[] {
                0x47, 0x00,
                0x01,//show action One
                (byte)action,
                0xFF, 0xFF,
            });
            result.Write(GameInformation);
            return result.GetBytes();
        }
        public static byte[] ShowPlayerAction(GPlayer lobbyPlayer, TLOBBY_ACTION action)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x46, 0x00 });
            result.WriteByte((byte)action); //ACTION PLAYER
            result.WriteByte(1); //TOTAL PLAYERS IN ACTION
            result.Write(lobbyPlayer.GetLobbyInfo());

            return result.GetBytes();
        }
        /// <summary>
        /// Ref Packet : A7 00
        /// </summary>
        /// <param name="Lists"></param>
        /// <returns></returns>
        public static byte[] ShowBoxItem(List<object> Lists)
        {
            var result = new PangyaBinaryWriter();
            try
            {
                result.Write(new byte[] { 0xA7, 0x00 });
                result.WriteByte((byte)Lists.Count);
                foreach (var APoint in Lists)
                {
                    result.WriteUInt32(((TItemData)APoint).TypeID);
                    result.WriteUInt32(((TItemData)APoint).ItemIndex);
                    result.WriteUInt16((UInt16)((TItemData)APoint).ItemQuantity);
                }
                return result.GetBytes();
            }
            finally
            {
                result.Dispose();
            }
        }
        public static byte[] ShowBoxItem(uint BoxTypeID, uint TypeID, uint Quantity)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x9D, 0x01 });
            result.WriteUInt32(0);
            result.WriteUInt32(BoxTypeID);
            result.WriteUInt32(TypeID);
            result.WriteUInt32(Quantity);
            return result.GetBytes();
        }
        /// <summary>
        /// Ref Packet AA 00
        /// </summary>
        /// <param name="Lists"></param>
        /// <param name="Pangs"></param>
        /// <param name="Cookies"></param>
        /// <returns></returns>
        public static byte[] ShowBoxNewItem(List<object> Lists, uint Pangs, uint Cookies)
        {
            var result = new PangyaBinaryWriter();
            try
            {
                result.Write(new byte[] { 0xAA, 0x00 });
                result.WriteUInt16((ushort)Lists.Count);
                foreach (var APoint in Lists)
                {
                    result.WriteUInt32(((TItemData)APoint).TypeID);
                    result.WriteUInt32(((TItemData)APoint).ItemIndex);
                    result.WriteZero(3);
                    result.WriteUInt16((UInt16)((TItemData)APoint).ItemQuantity);
                    result.WriteZero(25);

                }
                result.WriteUInt64(Pangs);
                result.WriteUInt64(Cookies);
                return result.GetBytes();
            }
            finally
            {
                result.Dispose();
            }
        }
        public static byte[] ShowReceiveMileage(uint Code = 0, uint Milage = 0, uint Count = 0)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x74, 0x02 });
            result.WriteUInt32(Code);
            if (Code == 0)
            {
                result.WriteUInt32(Milage);
                result.WriteUInt32(Count);
            }
            return result.GetBytes();
        }
        public static byte[] ShowClubStatus(TCLUB_ACTION Action, TCLUB_ACTION code, TCLUB_STATUS slot, uint ClubIndex, uint PangConsume = 0)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0xA5, 0x00 });
            result.WriteByte((byte)Action);
            result.WriteByte((byte)code);
            result.WriteByte((byte)slot);
            result.WriteUInt32(ClubIndex);
            result.WriteUInt64(PangConsume);
            return result.GetBytes();
        }
        public static byte[] ShowBuyItem(AddData Item, TBuyItem BuyData, uint Pang, uint Cookies)
        {
            var result = new PangyaBinaryWriter();

            result.Write(new byte[] { 0xAA, 0x00 });
            result.WriteUInt16(1);//count Sucess item(coloca sempre 1 eu acho)
            result.WriteUInt32(Item.ItemTypeID);
            result.WriteUInt32(Item.ItemIndex);
            result.WriteUInt16(BuyData.DayTotal);
            result.WriteByte(BuyData.Flag);
            result.WriteUInt16((ushort)Item.ItemNewQty);
            result.Write(GetFixTime(BuyData.EndDate));
            result.WriteStr(Item.ItemUCCKey, 9);
            result.WriteUInt64(Pang);
            result.WriteUInt64(Cookies);
            return result.GetBytes();
        }
        public static byte[] ShowBuyItemSucceed(TGAME_SHOP Code = TGAME_SHOP.BUY_SUCCESS, uint Pang = 0, uint Cookies = 0)
        {
            var result = new PangyaBinaryWriter();

            result.Write(new byte[] { 0x68, 0x00 });
            result.WriteUInt32((uint)Code);
            if (Code == TGAME_SHOP.BUY_SUCCESS)
            {
                result.WriteUInt64(Pang);
                result.WriteUInt64(Cookies);
            }
            return result.GetBytes();
        }
        public static byte[] ShowWeather(ushort type = 0)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x9E, 0x00 });
            result.WriteUInt16(type);
            result.WriteByte(0);
            return result.GetBytes();
        }
        public static byte[] ShowWind(ushort WP = 0, ushort WD = 0, byte active = 1)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x5B, 0x00 });
            result.WriteUInt16(WP);
            result.WriteUInt16(WD);
            result.WriteByte(active);
            return result.GetBytes();
        }
        public static byte[] ShowNameScore(string Nick, int Score, uint Pang)
        {
            var result = new PangyaBinaryWriter();

            result.Write(new byte[] { 0x40, 0x00 });
            result.WriteByte(0x11);
            result.WritePStr(Nick);
            result.WriteUInt16(0);
            result.WriteInt32(Score);//Score
            result.WriteUInt64(Pang);//Pangs
            result.WriteByte(0);

            return result.GetBytes();
        }
        public static byte[] ShowHoleData(uint ConID, uint CurrHole, byte TotalShot, uint Score, uint Pang, uint BonusPang, bool finish = true)
        {
            var result = new PangyaBinaryWriter();

            result.Write(new byte[] { 0x6D, 0x00 });
            result.WriteUInt32(ConID);
            result.WriteByte((byte)CurrHole);
            result.WriteByte(TotalShot);
            result.WriteUInt32(Score);
            result.WriteUInt64(Pang);
            result.WriteUInt64(BonusPang);
            result.WriteByte(IfCompare<byte>(finish, 1, 0));
            return result.GetBytes();
        }
        public static byte[] ShowGameLoading(uint ConID, byte Process)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0xA3, 0x00 });
            result.WriteUInt32(ConID);
            result.WriteByte(Process);
            return result.GetBytes();
        }
        public static byte[] ShowWhoPlay(uint ConID)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x53, 0x00 });
            result.WriteUInt32(ConID);
            return result.GetBytes();
        }
        public static byte[] ShowGameIcon(uint ConID, ushort IconType)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x5D, 0x00 });
            result.WriteUInt32(ConID);
            result.WriteUInt16(IconType);
            return result.GetBytes();
        }
        public static byte[] ShowSleep(uint ConID, byte SleepType)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x8E, 0x00 });
            result.WriteUInt32(ConID);
            result.WriteUInt16(SleepType);
            return result.GetBytes();
        }
        public static byte[] ShowPlayerUseItem(uint ConID, uint ID)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x5A, 0x00 });
            result.WriteUInt32(ID);
            result.WriteInt32(new Random().Next());
            result.WriteUInt32(ConID);
            return result.GetBytes();
        }
        public static byte[] ShowPlayerChangeClub(uint ConID, byte ClubType)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x59, 0x00 });
            result.WriteUInt32(ConID);
            result.WriteByte(ClubType);
            return result.GetBytes();
        }
        public static byte[] ShowPlayerRotate(uint ConID, Single Angle)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x56, 0x00 });
            result.WriteUInt32(ConID);
            result.WriteSingle(Angle);
            return result.GetBytes();
        }
        public static byte[] ShowAssistPutting(uint AssistTypeID, uint UID)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x6B, 0x02 });
            result.WriteUInt32(0);
            result.WriteUInt32(AssistTypeID);
            result.WriteUInt32(UID);
            return result.GetBytes();
        }

        public static byte[] ShowRoomEntrance(uint ConID, uint UN = 1065353216)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x96, 0x01 });
            result.WriteUInt32(ConID);
            result.Write(new byte[]
            {
                0x00, 0x00, 0x80, 0x3F,
                0x00, 0x00, 0x80, 0x3F,
                0x00, 0x00, 0x80, 0x3F,
                0x00, 0x00, 0x80, 0x3F
            });
            result.Write(UN);
            return result.GetBytes();
        }
        public static byte[] ShowGameMarking(Point3D Pos)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0xF8, 0x01 });
            result.WriteStruct(Pos);
            return result.GetBytes();
        }
        public static byte[] ShowGameReady(uint ConID, byte ReadyType)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x78, 0x00 });
            result.WriteUInt32(ConID);
            result.WriteByte(ReadyType);
            return result.GetBytes();
        }
        public static byte[] ShowPlayerPauseGame(uint ConID, byte PauseType)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x8B, 0x00 });
            result.WriteUInt32(ConID);
            result.WriteByte(PauseType);
            return result.GetBytes();
        }
        public static byte[] ShowAnimalEffect(uint UID)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x36, 0x02 });
            result.WriteUInt32(UID);
            return result.GetBytes();
        }
        public static byte[] ShowPlayerTimeBoost(uint ConID)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0xC7, 0x00 });
            result.Write(new byte[] { 0x00, 0x00, 0x40, 0x40 });
            result.WriteUInt32(ConID);
            return result.GetBytes();
        }
        public static byte[] ShowMatchTimeUsed(uint SecondUsed)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x8D, 0x00 });
            result.Write(SecondUsed);
            return result.GetBytes();
        }
        public static byte[] SendPlayerPlay(uint ConID)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x63, 0x00 });
            result.WriteUInt32(ConID);
            return result.GetBytes();
        }
        public static byte[] ShowDropItem(uint ConID)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0xCC, 0x00 });
            result.WriteUInt32(ConID);
            result.WriteByte((byte)0);
            return result.GetBytes();
        }
        public static byte[] ShowTreasureGuage(uint gauge = 255)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x32, 0x01 });
            result.WriteUInt32(gauge);
            return result.GetBytes();
        }
        public static byte[] ShowTeam(uint ConID, TTEAM_VERSUS Team)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x32, 0x01 });
            result.WriteUInt32(ConID);
            result.WriteByte((byte)Team);
            return result.GetBytes();
        }
        public static byte[] ShowShotData(TShotData ShotData)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x64, 0x00 });
            result.WriteUInt32(ShotData.ConnectionId);
            result.WriteDouble(ShotData.Pos.X);
            result.WriteDouble(ShotData.Pos.Y);
            result.WriteDouble(ShotData.Pos.Z);
            result.WriteByte((byte)ShotData.ShotType);
            //result.Write(ShotData.Un);
            result.WriteUInt32(ShotData.Pang);
            result.WriteUInt32(ShotData.BonusPang);
            //result.Write(ShotData.Un2);
            //result.Write(ShotData.MatchData);
            //result.Write(ShotData.ShotDecrypt);
            return result.GetBytes();
        }

        public static byte[] ShowPangRate()
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x77, 0x00 });
            result.WriteUInt32(100);
            return result.GetBytes();
        }
        public static byte[] ShowLeaveMatch(uint ConID, byte Type)
        {
            var Response = new PangyaBinaryWriter();
            Response.Write(new byte[] { 0x6C, 0x00 });
            Response.WriteUInt32(ConID);
            Response.WriteByte(Type);//tipos
            return Response.GetBytes();
        }
        public static byte[] ShowDropBall(Point3D Pos)
        {
            var result = new PangyaBinaryWriter();
            try
            {
                result.Write(new byte[] { 0x60, 0x00 });
                result.WriteStruct(Pos);
                return result.GetBytes();
            }
            finally
            {
                result.Dispose();
            }
        }
        public static byte[] ShowPowerShot(uint ConID, TPOWER_SHOT PowerShot)
        {
            var result = new PangyaBinaryWriter();

            result.Write(new byte[] { 0x58, 0x00 });
            result.WriteUInt32(ConID);
            result.WriteByte((byte)PowerShot);
            return result.GetBytes();
        }
        public static byte[] ShowRingEffects(uint ConID, byte[] un)
        {
            var result = new PangyaBinaryWriter();

            result.Write(new byte[] { 0x37, 0x02 });
            result.WriteUInt32(ConID);
            result.Write(un);
            return result.GetBytes();
        }
        public static byte[] ShowGameLeave(uint ConnectionId, byte opt = 1)
        {
            var resp = new PangyaBinaryWriter();
            switch (opt)
            {
                case 1:
                    {
                        resp.Write(new byte[] { 0x61, 0x00 });
                        resp.WriteUInt32(ConnectionId);
                    }
                    break;
                case 2:
                    {
                        resp.Write(new byte[] { 0x48, 0x00 });
                        resp.WriteByte((byte)2);  // ## Leave Game
                        resp.WriteUInt16(65535);
                        resp.WriteUInt32(ConnectionId);
                    }
                    break;
                case 3:
                    {
                        resp.Write(new byte[] { 0x6C, 0x00 });
                        resp.WriteUInt32(ConnectionId);
                        resp.WriteByte((byte)3);  // ## 2: leave from Game to lobby Game, 3: leave from room
                    }
                    break;
            }
            return resp.GetBytes();
        }
        public static byte[] ShowNewMaster(uint ConID)
        {
            var resp = new PangyaBinaryWriter();

            resp.Write(new byte[] { 0x7C, 0x00 });
            resp.WriteUInt32(ConID);
            resp.WriteUInt16(65535);
            return resp.GetBytes();
        }

        public static byte[] ShowOpenShop(string Nickname, uint UID)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[] { 0xE5, 0x00 });
                Response.WriteUInt32(1);
                Response.WritePStr(Nickname);
                Response.Write(UID);
                return Response.GetBytes();
            }
        }

        public static byte[] ShowEditShopName(string ShopName, uint UID, string Nickname)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[] { 0xE8, 0x00 });
                Response.Write(1);
                Response.WritePStr(ShopName);
                Response.Write(UID);//meu uid
                Response.WritePStr(Nickname);
                return Response.GetBytes();
            }
        }

        public static byte[] ShowShopCountVisit(uint Count)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[] { 0xE9, 0x00 });
                Response.Write(1);
                Response.Write(Count);
                return Response.GetBytes();
            }
        }

        public static byte[] ShowShopClose(string Nickname, uint UID)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[] { 0xE4, 0x00 });
                Response.WriteUInt32(1);
                Response.WritePStr(Nickname);
                Response.Write(UID);
                return Response.GetBytes();
            }
        }

        public static byte[] ShowShopEnter(GPlayer PL)
        {
            using (var res = new PangyaBinaryWriter())
            {
                res.Write(new byte[] { 0xE6, 0x00 });
                res.WriteUInt32(1);
                res.WriteStr(PL.GetNickname, 22);
                res.WritePStr(PL.GameInfo.GameShop.Name);
                res.WriteUInt32(PL.GameInfo.GameShop.ShopOwnerID); // shop owner id
                res.WriteUInt32(PL.GameInfo.GameShop.ItemsCount); // Number of items
                res.Write(PL.GameInfo.GameShop.GetData());
                return res.GetBytes();
            }
        }

        public static byte[] ShowShopItems(GPlayer PL, int ItemCount, List<ShopItemData> Data)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[] { 0xEB, 0x00 });
                Response.Write(1);//code
                Response.WriteStr(PL.GetNickname, 22);
                Response.Write(PL.GetUID);
                Response.Write(ItemCount);
                foreach (var data in Data)
                {
                    Response.Write(data.GetData());
                }
                return Response.GetBytes();
            }
        }

        public static byte[] ShowShopItemBuy(ulong Pangs, ShopItem itens)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[] { 0xEC, 0x00 });
                Response.Write(1);//code
                Response.WriteByte(0);
                Response.WriteUInt64(Pangs);//
                Response.WriteByte(0);
                Response.WriteStruct(itens);
                return Response.GetBytes();
            }
        }

        public static byte[] ShowShopItemBuyResult(string NickName, uint ShopID, ShopItem itens)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[] { 0xED, 0x00 });
                Response.WritePStr(NickName);
                Response.Write(ShopID);
                Response.WriteStruct(itens);
                return Response.GetBytes();
            }
        }
        public static byte[] ShowNewTrophy(uint TypeID)
        {
            var resp = new PangyaBinaryWriter();

            resp.Write(new byte[] { 0x97, 0x00 });
            resp.WriteUInt32(TypeID);
            return resp.GetBytes();
        }

        public static byte[] ShowPlayerAction(uint ConID, byte[] Message)
        {
            using (var response = new PangyaBinaryWriter())
            {
                response.Write(new byte[] { 0xC4, 0x00 });
                response.WriteUInt32(ConID);
                response.Write(Message);
                return response.GetBytes();
            }
        }
    }
}
