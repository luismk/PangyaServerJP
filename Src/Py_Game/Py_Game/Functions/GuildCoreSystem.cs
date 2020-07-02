//using PangyaAPI;
//using Py_Game.Client;
//using System;
//using System.Linq;
//using Py_Connector.DataBase;
//using static Py_Game.GameTools.Tools;
//using static Py_Game.GameTools.ErrorCode;
//namespace Py_Game.Functions
//{
//    public class GuildCoreSystem
//    {
//        public void PlayerCallGuildList(GPlayer player, Packet packet, string findguild = "%")
//        {


//            if (!packet.ReadInt32(out int PageSelected))
//            {
//                player.SendResponse(new byte[] { 0xBC, 0x01, 0x00, 0x00, 0x00, 0x00 });
//                return;
//            }
//            var _db = new PangyaEntities();
//            var GuildTotal = _db.Database.SqlQuery<int>($"SELECT COUNT(*) AS GUILD_TOTAL FROM[dbo].Pangya_Guild_Info WHERE GUILD_NAME LIKE '%' + '{findguild}' + '%' AND GUILD_VALID = 1").First();
//            var GetGuildList = _db.ProcGuildGetList(PageSelected, 15, findguild).ToList();

//            player.Response.Write(new byte[] { 0xBC, 0x01 });
//            player.Response.WriteUInt32(1);
//            player.Response.WriteInt32(PageSelected); //page  
//            player.Response.Write(GuildTotal);
//            player.Response.Write((ushort)GetGuildList.Count);
//            foreach (var data in GetGuildList)
//            {
//                player.Response.Write(data.GUILD_INDEX);
//                player.Response.WriteStr(data.GUILD_NAME, 17);
//                player.Response.Write(data.GUILD_POINT ?? 0); // data level point ok
//                player.Response.Write(data.GUILD_PANG ?? 0); //data pangs member OK
//                player.Response.Write((uint)data.GUILD_TOTAL_MEMBER); //data total member OK
//                player.Response.Write(GetFixTime(data.GUILD_CREATE_DATE));
//                player.Response.WriteStr(data.GUILD_INTRODUCING, 105);//GUILD_INTRODUCING
//                player.Response.Write(data.GUILD_LEADER_UID); //// Guild Leader UID?
//                player.Response.WriteStr(data.GUILD_LEADER_NICKNAME, 22); //data leader nickname OK 
//                player.Response.WriteStr(data.GUILD_IMAGE, 12); //GuilD_IMAGE
//            }
//            player.SendResponse();
//        }

//        public void PlayerValidGuildName(GPlayer player, Packet packet)
//        {
//            var GUILDNAME = packet.ReadPStr();
//            var _db = new PangyaEntities();
//            var CODE = _db.ProcGuildNameAvailable(GUILDNAME).FirstOrDefault();

//            player.Response.Write(new byte[] { 0xB6, 0x01 });

//            if (CODE == 1)
//            {
//                player.Response.Write(54510);
//            }

//            //Create GUILD
//            if (CODE == 0)
//            {
//                player.Response.Write(1);
//                player.Response.WritePStr(GUILDNAME);
//            }

//            player.SendResponse();
//        }

//        public void PlayerSearchGuild(GPlayer player, Packet packet)
//        {

//            var PageSelected = packet.ReadInt32();
//            var GuildSearch = packet.ReadPStr();
//            var _db = new PangyaEntities();
//            var check = _db.ProcGuildGetList(PageSelected, 15, GuildSearch).ToList();

//            var GuildTotal = _db.Database.SqlQuery<int>($"SELECT COUNT(*) AS GUILD_TOTAL FROM[dbo].Pangya_Guild_Info WHERE GUILD_NAME LIKE '%' + '{GuildSearch}' + '%' AND GUILD_VALID = 1").First();


//            player.Response.Write(new byte[] { 0xBD, 0x01 });
//            player.Response.Write((uint)1);
//            player.Response.Write((uint)PageSelected);
//            player.Response.Write((uint)GuildTotal);
//            player.Response.Write((ushort)check.Count);
//            foreach (var data in check)
//            {
//                player.Response.Write((uint)data.GUILD_INDEX);
//                player.Response.WriteStr(data.GUILD_NAME, 21);
//                player.Response.Write(data.GUILD_POINT ?? 0); // data level point ok
//                player.Response.Write(data.GUILD_PANG ?? 0); //data pangs member OK
//                player.Response.Write((uint)data.GUILD_TOTAL_MEMBER); //data total member OK
//                player.Response.Write(GetFixTime(data.GUILD_CREATE_DATE));
//                player.Response.WriteStr(data.GUILD_INTRODUCING, 105);//GUILD_INTRODUCING
//                player.Response.Write(data.GUILD_LEADER_UID); //// Guild Leader UID?
//                player.Response.WriteStr(data.GUILD_LEADER_NICKNAME, 22); //data leader nickname OK 
//                player.Response.WriteStr(data.GUILD_IMAGE, 9); //GuilD_IMAGE
//                player.Response.WriteZero(3);
//            }
//            player.SendResponse();
//        }

//        public void PlayerCreateGuild(GPlayer player, Packet packet)
//        {
//            bool Check()
//            {
//                return player.GuildInfo.GuildID == 0;
//            }

//            var GUILDNAME = packet.ReadPStr();
//            var GuildIntro = packet.ReadPStr();

//            if (!player.Inventory.IsExist(436207919))//item que cria guild
//            {
//                player.SendResponse(new byte[] { 0xB5, 0x01, 0xf1, 0xd2, 0x00, 0x00 });
//                return;
//            }

//            if (!Check())
//            {
//                player.SendResponse(new byte[] { 0xB5, 0x01, 0xf1, 0xd2, 0x00, 0x00 });
//                return;
//            }
//            var _db = new PangyaEntities();
//            var CODE = _db.USP_GUILD_CREATE((int)player.GetUID, GUILDNAME, GuildIntro).First();

//            // Player is in guild
//            if (CODE == 10)
//            {
//                player.SendResponse(new byte[] { 0xB5, 0x01, 0xee, 0xd4, 0x00, 0x00 });
//                return;
//            }
//            if (CODE == 2)
//            {
//                player.SendResponse(new byte[] { 0xB5, 0x01, 0x00, 0x00, 0x00, 0x00 });
//                return;
//            }
//            // Player is in guild
//            if (CODE == 9)
//            {
//                player.SendResponse(new byte[] { 0xB5, 0x01, 0xf3, 0xd2, 0x00, 0x00 });
//                return;
//            }

//            //Create GUILD
//            if (CODE == 0)
//            {
//                // Delete Guild Creator
//                var RemoveItemData = player.Inventory.Remove(436207919, 1, false);
//                // Successfully Created
//                player.Response.Write(new byte[] { 0xC5, 0x00 });
//                player.Response.Write((byte)1);
//                player.Response.Write(RemoveItemData.ItemTypeID);
//                player.Response.Write(1);//count?
//                player.Response.Write(RemoveItemData.ItemIndex);
//                player.SendResponse();

//                player.Response.Write(new byte[] { 0xB5, 0x01 });
//                player.Response.Write(1);// Status Successfully
//                player.SendResponse();
//                //Reload player in guild
//                player.SendGuildData();
//            }
//        }

//        public void PlayerRequestGuildData(GPlayer player, Packet packet)
//        {
//            string guildname = "";

//            var GuildIndex = packet.ReadInt32();
//            var _db = new PangyaEntities();
//            var GuildGetPlayerData = _db.ProcGuildGetPlayerData((int)player.GetUID, GuildIndex).ToList();


//            if (GuildIndex <= 0)
//            {
//                player.Response.Write(new byte[] { 0xB8, 0x01 });
//                player.Response.Write(54500);
//                player.SendResponse();
//                return;
//            }
//            else
//            {
//                player.Response.Write(new byte[] { 0xB8, 0x01 });
//                player.Response.Write((uint)1);
//                foreach (var data in GuildGetPlayerData)
//                {
//                    guildname = data.GUILD_NAME;
//                    player.Response.Write((uint)data.GUILD_INDEX);
//                    player.Response.WriteStr(data.GUILD_NAME, 17);
//                    player.Response.Write(0); // data level point ok
//                    player.Response.Write(0); //data pangs member OK
//                    player.Response.Write(data.GUILD_TOTAL_MEMBER); //data total member OK
//                    player.Response.WriteStr(data.GUILD_IMAGE, 9); //GuilD_IMAGE
//                    player.Response.WriteZero(3);
//                    player.Response.WriteStr(data.GUILD_NOTICE, 101);//GUILD_Notice
//                    player.Response.WriteStr(data.GUILD_INTRODUCING, 101);//GUILD_INTRODUCING
//                    player.Response.WriteUInt32(data.GUILD_POSITION);
//                    player.Response.Write(data.GUILD_LEADER_UID); //// Guild Leader UID?
//                    player.Response.WriteStr(data.GUILD_LEADER_NICKNAME, 22); //data leader nickname OK          
//                    player.Response.Write(GetFixTime(data.GUILD_CREATE_DATE)); //GuilD_IMAGE
//                }
//                player.SendResponse();
//            }
//        }

//        public void PlayerGetGuildPlayer(GPlayer player, Packet packet)
//        {

//            var GuildID = packet.ReadInt32();
//            var Page = packet.ReadInt32();

//            var _db = new PangyaEntities();
//            var GuildGetData = _db.ProcGuildGetData(GuildID, Page, 15).ToList();
//            var GuildTotal = _db.Database.SqlQuery<int>($"SELECT COUNT(*) AS GUILD_TOTAL FROM[dbo].Pangya_Guild_Info WHERE GUILD_NAME LIKE '%' + '%' + '%' AND GUILD_VALID = 1").First();

//            player.Response.Write(new byte[] { 0xC6, 0x01 });
//            player.Response.WriteUInt32(1);
//            player.Response.WriteInt32(Page);
//            player.Response.WriteInt32(GuildTotal);
//            player.Response.Write((ushort)GuildGetData.Count);
//            foreach (var data in GuildGetData)
//            {
//                player.Response.WriteInt32(data.GUILD_ID);
//                player.Response.WriteInt32(data.GUILD_MEMBER_UID);
//                player.Response.WriteInt32((int)data.GUILD_POSITION);
//                player.Response.WriteStr(data.GUILD_MESSAGE, 25);
//                player.Response.WriteStr(data.GUILD_NAME, 17);
//                player.Response.WriteStr(data.PLAYER_NICKNAME, 22);
//                player.Response.Write((byte)data.Logon);
//            }
//            player.SendResponse();
//        }

//        public void PlayerGuildLog(GPlayer player, Packet packet)
//        {
//            var _db = new PangyaEntities();
//            var GuildGetLog = _db.ProcGuildGetLog((int)player.GetUID).ToList();

//            player.Response.Write(new byte[] { 0xBE, 0x01 });
//            player.Response.Write((uint)1);
//            player.Response.Write((ushort)GuildGetLog.Count);
//            foreach (var data in GuildGetLog)
//            {
//                player.Response.Write(uint.MaxValue);
//                player.Response.Write((UInt32)data.GUILD_ID);
//                player.Response.WriteStr(data.GUILD_NAME, 17);
//                player.Response.Write((uint)data.GUILD_ACTION); // data level point ok                         
//                player.Response.Write(GetFixTime(data.GUILD_ACTION_DATE)); //GuilD_IMAGE   
//            }
//            player.SendResponse();
//        }

//        public void PlayerJoinGuild(GPlayer player, Packet packet)
//        {
//            bool Check()
//            {
//                return player.GuildInfo.GuildID == 0;
//            }

//            var Guild_ID = packet.ReadInt32();
//            var GuildIntro = packet.ReadPStr();

//            if (!Check())
//            {
//                player.SendResponse(new byte[] { 0xC0, 0x01, 0xF5, 0xD2, 0x00, 0x00 });
//                return;
//            }
//            var _db = new PangyaEntities();
//            var code = _db.USP_GUILD_JOIN((int)player.GetUID, Guild_ID, GuildIntro).First();

//            if (code == 2)
//            {
//                player.SendResponse(new byte[] { 0xC0, 0x01, 0x00, 0x00, 0x00, 0x00 });
//                return;
//            }

//            if (code == 10)
//            {
//                player.SendResponse(new byte[] { 0xC0, 0x01, 0xF6, 0xD4, 0x00, 0x00 });
//                return;
//            }

//            if (code == 9)
//            {
//                player.SendResponse(new byte[] { 0xC0, 0x01, 0xFF, 0xD2, 0x00, 0x00 });
//                return;
//            }
//            if (code == 8)
//            {
//                player.SendResponse(new byte[] { 0xC0, 0x01, 0x00, 0x00, 0x00, 0x00 });
//                return;
//            }


//            if (code == 0)
//            {
//                player.SendResponse(new byte[] { 0xC0, 0x01, 0x01, 0x00, 0x00, 0x00 });
//                player.SendGuildData();
//            }
//        }

//        public void PlayerCancelJoinGuild(GPlayer player, Packet packet)
//        {
//            bool Check()
//            {
//                return player.GuildInfo.GuildID > 0;
//            }

//            void SendCode(byte[] Code)
//            {
//                player.Response.Write(new byte[] { 0xC1, 0x01 });
//                player.Response.Write(Code);
//                player.SendResponse();
//            }

//            var Guild_ID = packet.ReadInt32();

//            if (!Check())
//            {
//                SendCode(Zero);
//                return;
//            }
//            var _db = new PangyaEntities();
//            var code = _db.USP_GUILD_CANCELJOIN((int)player.GetUID, Guild_ID).First();

//            if (code == 2)
//            {
//                SendCode(Zero);
//                return;
//            }

//            else if (code == 10)
//            {
//                SendCode(GUILD_NOT_WAIT_FOR_ACCEPT);
//                return;
//            }

//            else if (code == 9)
//            {
//                SendCode(GUILD_NOT_FOUND);
//                return;
//            }
//            else if (code == 8)
//            {
//                SendCode(GUILD_NOT_WAIT_FOR_ACCEPT);
//                return;
//            }
//            else
//            {
//                SendCode(One);
//                player.SendGuildData();
//            }
//        }

//        public void PlayerGuildAccept(GPlayer player, Packet packet)
//        {
//            void SendCode(byte[] Code)
//            {
//                player.Response.Write(new byte[] { 0xC2, 0x01 });
//                player.Response.Write(Code);
//                player.SendResponse();
//            }

//            var Guild_ID = packet.ReadInt32();
//            var UID = packet.ReadInt32();

//            var _db = new PangyaEntities();
//            var data = _db.USP_GUILD_ACTION((int)player.GetUID, Guild_ID, 1, UID, 0, "").FirstOrDefault();

//            if (data.Value == 1)
//            {
//                SendCode(GUILD_NOT_WAIT_FOR_ACCEPT);
//                return;
//            }


//            if (data.Value == 2)
//            {
//                SendCode(GUILD_NOT_ADMIN);
//                return;
//            }

//            if (data.Value == 0)
//            {
//                SendCode(One);

//                player.Response.Write(new byte[] { 0xD1, 0x01 });
//                player.Response.Write(42);
//                player.Response.Write(UID);
//                player.SendResponse();
//            }
//        }

//        public void PlayerGuildKick(GPlayer player, Packet packet)
//        {
//            void SendCode(byte[] Code)
//            {
//                player.Response.Write(new byte[] { 0xC8, 0x01 });
//                player.Response.Write(Code);
//                player.SendResponse();
//            }

//            var Guild_ID = packet.ReadInt32();
//            var UID = packet.ReadInt32();

//            var _db = new PangyaEntities();
//            var data = _db.USP_GUILD_ACTION((int)player.GetUID, Guild_ID, 2, UID, 0, "").FirstOrDefault();

//            if (data.Value == 1)
//            {
//                SendCode(Zero);
//                return;
//            }


//            if (data.Value == 2)
//            {
//                SendCode(GUILD_NOT_ADMIN);
//                return;
//            }

//            if (data.Value == 0)
//            {
//                SendCode(One);

//                player.Response.Write(new byte[] { 0xD1, 0x01 });
//                player.Response.Write(43);
//                player.Response.Write(UID);
//                player.SendResponse();

//            }
//        }

//        public void PlayerGuildPromote(GPlayer player, Packet packet)
//        {
//            void SendCode(byte[] Code, uint Code2 = 0, bool check = false)
//            {
//                player.Response.Write(new byte[] { 0xC4, 0x01 });
//                player.Response.Write(Code);
//                if (check)
//                {
//                    player.Response.Write(Code2);
//                }
//                player.SendResponse();
//            }


//            var Guild_ID = packet.ReadInt32();
//            var UID = packet.ReadInt32();
//            var Position = packet.ReadInt32();
//            var _db = new PangyaEntities();
//            var data = _db.USP_GUILD_ACTION((int)player.GetUID, Guild_ID, 3, UID, Position, "").FirstOrDefault();

//            if (data.Value == 8 || data.Value == 10)
//            {
//                SendCode(Zero);
//                return;
//            }


//            if (data.Value == 9)
//            {
//                SendCode(GUILD_NOT_ADMIN);
//                return;
//            }

//            if (data.Value == 0)
//            {
//                SendCode(One, (uint)Position, true);
//            }
//        }

//        public void PlayerGuildCallUpload(GPlayer player, Packet packet)
//        {

//            var Guild_ID = packet.ReadInt32();

//            var _db = new PangyaEntities();
//            var data = _db.USP_GUILD_EMBLEM((int)player.GetUID, Guild_ID).FirstOrDefault();


//            if (!player.Inventory.IsExist(436207920))
//            {
//                player.SendResponse(new byte[] { 0xC9, 0x01, 0xe6, 0xd4, 0x00, 0x00 });
//                return;
//            }
//            if (data.CODE == 2)
//            {
//                player.SendResponse(new byte[] { 0xC9, 0x01, 0x00, 0x00, 0x00, 0x00 });
//                return;
//            }

//            //Create GUILD EMBLEM
//            if (data.CODE == 1)
//            {
//                player.Response.Write(new byte[] { 0xC9, 0x01 });
//                player.Response.Write(1);
//                player.Response.Write(data.EMBLEM_IDX);
//                player.Response.WritePStr(data.GUILD_MARK_IMG);
//                player.SendResponse();
//            }
//        }

//        public void PlayerGuildAfterUpload(GPlayer player)
//        {
//            var RemoveItem = player.Inventory.Remove(436207920, 1, false);

//            if (!RemoveItem.Status)
//            {
//                WriteConsole.WriteLine("GuildAfterUpload: Player has requested for image guild upload but their item cannot be deleted.", ConsoleColor.Red);
//                player.SendResponse(new byte[] { 0xCa, 0x01, 0xe6, 0xd4, 0x00, 0x00 });
//                return;
//            }
//            player.SendResponse(new byte[] { 0xCA, 0x01, 0x01, 0x00, 0x00, 0x00 });

//            player.Response.Write(new byte[] { 0xC5, 0x01, 0x01 });
//            player.Response.Write(RemoveItem.ItemTypeID);
//            player.Response.Write(1);
//            player.Response.Write(RemoveItem.ItemIndex);
//            player.SendResponse();
//        }

//        public void PlayerChangeGuildIntro(GPlayer player, Packet packet)
//        {

//            var Guild_ID = packet.ReadInt32();
//            var UID = packet.ReadInt32();
//            var IntroMSG = packet.ReadPStr();

//            var _db = new PangyaEntities();
//            var data = _db.USP_GUILD_ACTION((int)player.GetUID, Guild_ID, 4, 0, 0, IntroMSG).FirstOrDefault();


//            if (data.Value == 8)
//            {
//                player.SendResponse(new byte[] { 0xba, 0x01, 0x00, 0x00, 0x00, 0x00 });
//                return;
//            }


//            if (data.Value == 9)
//            {
//                player.SendResponse(new byte[] { 0xba, 0x01, 0xE9, 0xD4, 0x00, 0x00 });
//                return;
//            }
//            if (data.Value == 0)
//            {
//                player.SendResponse(new byte[] { 0xba, 0x01, 0x01, 0x00, 0x00, 0x00 });
//            }
//        }

//        public void PlayerChangeGuildNotice(GPlayer player, Packet packet)
//        {

//            var Guild_ID = packet.ReadInt32();
//            var UID = packet.ReadInt32();
//            var IntroMSG = packet.ReadPStr();

//            var _db = new PangyaEntities();
//            var data = _db.USP_GUILD_ACTION((int)player.GetUID, Guild_ID, 5, 0, 0, IntroMSG).FirstOrDefault();


//            if (data.Value == 8)
//            {
//                player.SendResponse(new byte[] { 0xb9, 0x01, 0x00, 0x00, 0x00, 0x00 });
//                return;
//            }
//            if (data.Value == 9)
//            {
//                player.SendResponse(new byte[] { 0xb9, 0x01, 0xE9, 0xD4, 0x00, 0x00 });
//                return;
//            }
//            if (data.Value == 0)
//            {
//                player.SendResponse(new byte[] { 0xb9, 0x01, 0x01, 0x00, 0x00, 0x00 });
//            }
//        }

//        public void PlayerChangeGuildSelfIntro(GPlayer player, Packet packet)
//        {

//            var Guild_ID = packet.ReadInt32();
//            var UID = packet.ReadInt32();
//            var IntroMSG = packet.ReadPStr();

//            var _db = new PangyaEntities();
//            var data = _db.USP_GUILD_ACTION((int)player.GetUID, Guild_ID, 6, UID, 0, IntroMSG).FirstOrDefault();

//            if (data.Value == 8)
//            {
//                player.SendResponse(new byte[] { 0xc5, 0x01, 0x00, 0x00, 0x00, 0x00 });
//                return;
//            }
//            if (data.Value == 0)
//            {
//                player.SendResponse(new byte[] { 0xc5, 0x01, 0x01, 0x00, 0x00, 0x00 });
//            }
//        }

//        public void PlayerLeaveGuild(GPlayer player, Packet packet)
//        {

//            var Guild_ID = packet.ReadInt32();

//            var _db = new PangyaEntities();
//            var data = _db.USP_GUILD_ACTION((int)player.GetUID, Guild_ID, 7, 0, 0, "").FirstOrDefault();

//            if (data.Value == 8)
//            {
//                player.SendResponse(new byte[] { 0xc7, 0x01, 0x00, 0x00, 0x00, 0x00 });
//                return;
//            }
//            if (data.Value == 0)
//            {
//                player.SendResponse(new byte[] { 0xc5, 0x01, 0x01, 0x00, 0x00, 0x00 });

//                player.Response.Write(new byte[] { 0xD1, 0x01 });
//                player.Response.Write(0x2B);
//                player.Response.Write((int)player.GetUID);
//                player.SendResponse();

//                player.SendGuildData();
//            }
//        }
//    }
//}
