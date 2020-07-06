using System;
using System.Linq;
using Py_Game.Client;
using PangyaAPI;
using PangyaAPI.BinaryModels;
using Py_Game.Client.Inventory;
using Py_Connector.DataBase;
using static Py_Game.GameTools.PacketCreator;
using static PangyaFileCore.IffBaseManager;
using Py_Game.Lobby;
using Py_Game.Game;
using Py_Game.Client.Inventory.Data;
using Py_Game.Client.Inventory.Data.Slot;
using Py_Game.Client.Inventory.Data.ItemDecoration;
using Py_Game.Data;
using Py_Game.Defines;
using PangyaAPI.PangyaPacket;
using PangyaAPI.Tools;

namespace Py_Game.Functions.Core
{
    public class GameCore
    {
        #region Public Methods
        public void PlayerGetMatchHistory(GPlayer player)
        {
            var _db = new PangyaEntities();
            var check = _db.ProcGetMatchHistory((int)player.GetUID).ToList();

            player.Response.Write(new byte[] { 0x0E, 0x01 });
            if (check.Count == 0)
            {
                player.Response.WriteZero(260); //260 bytes
            }
            else
            {
                foreach (var data in check)
                {
                    player.Response.Write(data.SEX.Value);
                    player.Response.WriteStr(data.NICKNAME, 22);
                    player.Response.WriteStr(data.USERID, 22);
                    player.Response.Write(data.UID.Value);
                }
            }
            player.SendResponse();

            if (player.GameID == ushort.MaxValue)
            {
                player.SendResponse(new byte[] { 0x2E, 0x02, 0x00, 0x00, 0x00, 0x00 });
                player.SendResponse(new byte[] { 0x20, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            }
        }

        public void PlayerSaveMacro(GPlayer player, Packet Reader)
        {
            var Macro = new string[8];

            for (int i = 0; i < 8; i++)
            {
                Reader.ReadPStr(out Macro[i], 64);
            }        

            var _db = new PangyaEntities();
            var query = $"exec ProcSaveMacro @UID = {player.GetUID}, @Macro1 = {Macro[0]}, @Macro2 = {Macro[1]}, @Macro3 = {Macro[2]}, @Macro4 = {Macro[3]}, @Macro5 = {Macro[4]}, @Macro6 = {Macro[5]}, @Macro7 = {Macro[6]}, @Macro8 = {Macro[7]}";
            _db.Database.SqlQuery<PangyaEntities>(query);
        }

        public void PlayerChangeServer(GPlayer player)
        {
            var _db = new PangyaEntities();
            var key = _db.ProcUpdateAuth((int)player.GetUID).First();
            try
            {
                player.Response.Write(new byte[] { 0xD4, 0x01 });
                player.Response.WriteUInt32(0);
                player.Response.WritePStr(key);//GameAuthKey
                player.SendResponse();
            }
            catch
            {
                //no enter in out server
                player.Response.Write(new byte[] { 0xD4, 0x1 });
                player.Response.WriteUInt32(1);
                player.SendResponse();
            }
        }

        public void PlayerControlAssist(GPlayer PL)
        {
            uint AssistItem;
            AddItem Item;

            AssistItem = 467664918;

            switch (PL.Inventory.GetQuantity(AssistItem))
            {
                case 1: // TO CLOSE {plus item 1]
                    {
                        Item = new AddItem()
                        {
                            ItemIffId = AssistItem,
                            Quantity = 1,
                            Transaction = true,
                            Day = 0
                        };
                        PL.AddItem(Item);
                        PL.Assist = 0x00;
                    }
                    break;
                case 2: // TO OPEN {minus item 1}
                    {
                        PL.Inventory.Remove(AssistItem, 1, true);
                        PL.Assist = 0x01;
                    }
                    break;
                default:
                    {
                        return;
                    }
            }

            PL.SendTransaction();

            PL.SendResponse(new byte[] { 0x6A, 0x02, 0x00, 0x00, 0x00, 0x00 });
        }

        public void PlayerSaveBar(GPlayer player, Packet packet)
        {
            Channel Lobby;
            GameBase GameHandle;
            PlayerInventory Inventory;


            Lobby = player.Lobby;

            GameHandle = Lobby.GetGameHandle(player);
            Inventory = player.Inventory;

            packet.ReadByte(out byte action);
            packet.ReadUInt32(out uint id);
            try
            {
                var Response = new PangyaBinaryWriter();

                Response.Write(new byte[] { 0x4B, 0x00 });
                Response.WriteUInt32(0);
                Response.WriteByte(action);
                Response.WriteUInt32(player.ConnectionID);
                switch (action)
                {
                    case 1: // ## caddie
                        {
                            if (!Inventory.SetCaddieIndex(id))
                            {
                                player.Close();
                                return;
                            }
                            Response.Write(Inventory.GetCaddieData());
                        }
                        break;
                    case 2: // ## ball
                        {
                            if (!Inventory.SetBallTypeID(id))
                            {
                                player.Close();
                                return;
                            }
                            Response.Write(id);
                        }
                        break;
                    case 3: // ## club
                        {
                            if (!Inventory.SetClubSetIndex(id))
                            {
                                player.Close();
                                return;
                            }
                            Response.Write(Inventory.GetClubData());//clubdata temp
                        }
                        break;
                    case 4: // ## char
                        {
                            if (!Inventory.SetCharIndex(id))
                            {
                                player.Close();
                                return;
                            }
                            Response.Write(Inventory.GetCharData());
                        }
                        break;
                    case 5: // ## mascot
                        {
                            if (!Inventory.SetMascotIndex(id))
                            {
                                player.Close();
                                return;
                            }
                            Response.Write(Inventory.GetMascotData());
                        }
                        break;
                    case 7: // ## start game
                        {
                            if (GameHandle == null) return;

                            GameHandle.AcquireData(player);
                        }
                        break;
                }
                if (action == 4 && GameHandle != null)
                {
                    GameHandle.Send(Response);
                    //Atualizar
                    GameHandle.Send(ShowActionGamePlayInfo(player));
                    if (GameHandle.GameType == GAME_TYPE.CHAT_ROOM)
                    {
                        //GameHandle.Send(ShowRoomEntrance(player.ConnectionID, 15));
                    }
                }
                else
                {
                    player.SendResponse(Response.GetBytes());
                }
            }
            catch
            {
                player.Close();
            }
        }

        public void PlayerChangeEquipment(GPlayer player, Packet Reader)
        {
            PangyaBinaryWriter Reply;
            bool Status;

            if (!Reader.ReadByte(out byte action)) { return; }

            Status = false;
            Reply = new PangyaBinaryWriter();
            try
            {
                Reply.Write(new byte[] { 0x6B, 0x00, 0x04 });
                Reply.WriteByte(action);
                switch (action)
                {
                    case 0:  // ## save char equip
                        {
                            var invchar = (CharacterData)Reader.Read(new CharacterData());

                            var character = player.Inventory.GetCharacter(invchar.TypeID);
                            if (character == null)
                            {
                                WriteConsole.WriteLine("[PLAYER_CHANGE_EQUIPCHAR]: Error Ao Tentar Setar EquipChar", ConsoleColor.Red);
                                player.Close();
                                return;
                            }
                            character.EquipTypeID = invchar.EquipTypeID;
                            character.EquipIndex = invchar.EquipIndex;
                            character.AuxPart = invchar.AuxPart;
                            character.AuxPart2 = invchar.AuxPart2;
                            character.AuxPart3 = invchar.AuxPart3;
                            character.AuxPart4 = invchar.AuxPart4;
                            character.AuxPart5 = invchar.AuxPart5;
                            character.FCutinIndex = invchar.FCutinIndex;
                            character.Power = invchar.Power;
                            character.Control =invchar.Control;
                            character.Impact = invchar.Impact;
                            character.Spin = invchar.Spin;
                            character.Curve = invchar.Curve;
                            player.Inventory.ItemCharacter.UpdateCharacter(character);
                            Status = true;
                            Reply.Write(player.Inventory.GetCharData(invchar.Index));
                        }
                        break;
                    case 1:  // ## change caddie
                        {
                            Reader.ReadUInt32(out uint CaddiIndex);
                            if (!player.Inventory.SetCaddieIndex(CaddiIndex))
                            {
                                WriteConsole.WriteLine("[PLAYER_CHANGE_CADDIE]: Error Ao Tentar Setar CaddieIndex", ConsoleColor.Red);
                                player.Close();
                            }
                            Status = true;
                            Reply.WriteUInt32(CaddiIndex);
                        }
                        break;
                    case 2: // ## item for play
                        {
                            ItemSlotData ItemSlots;

                            ItemSlots = (ItemSlotData)Reader.Read(new ItemSlotData());
                            player.Inventory.ItemSlot.SetItemSlot(ItemSlots);
                            Status = true;
                            Reply.Write(player.Inventory.ItemSlot.GetItemSlot());
                        }
                        break;
                    case 3: // ## Change Ball And Club
                        {
                            Reader.ReadUInt32(out uint BallTypeID);
                            Reader.ReadUInt32(out uint ClubIndex);

                            if (!player.Inventory.SetGolfEQP(BallTypeID, ClubIndex))
                            {
                                WriteConsole.WriteLine("[PLAYER_CHANGE_EQUIP]: Error Ao Tentar Setar GolfEquip", ConsoleColor.Red);
                                player.Close();
                            }
                            Status = true;
                            Reply.Write(player.Inventory.GetGolfEQP());
                        }
                        break;
                    case 4: // ## Change Decoration
                        {
                            var Decoration = (ItemDecorationData)Reader.Read(new ItemDecorationData());

                            if (!player.Inventory.SetDecoration(Decoration.BackGroundTypeID, Decoration.FrameTypeID, Decoration.StickerTypeID, Decoration.SlotTypeID, Decoration.UnknownTypeID, Decoration.TitleTypeID))
                            {
                                WriteConsole.WriteLine("[PLAYER_CHANGE_DEC]: Error Ao Tentar Setar Decoration", ConsoleColor.Red);
                                player.Close();
                                return;
                            }
                            Status = true;
                            Reply.Write(player.Inventory.GetDecorationData());
                        }
                        break;
                    case 5:  // ## change char
                        {
                            Reader.ReadUInt32(out uint CharacterIndex);

                            if (!player.Inventory.SetCharIndex(CharacterIndex))
                            {
                                WriteConsole.WriteLine("[PLAYER_CHANGE_CHAR]: Error Ao Tentar Setar CharIndex", ConsoleColor.Red);
                                player.Close();
                            }
                            Status = true;
                            Reply.WriteUInt32(CharacterIndex);
                        }
                        break;
                    case 8: // ## change mascot
                        {
                            Reader.ReadUInt32(out uint MascotIndex);

                            if (!player.Inventory.SetMascotIndex(MascotIndex))
                            {
                                WriteConsole.WriteLine("[PLAYER_CHANGE_MASCOT]: Error Ao Tentar Setar MascotIndex");
                                player.Close();
                                return;
                            }
                            Status = true;

                            Reply.Write(player.Inventory.GetMascotData());
                        }
                        break;
                    case 9:// #Cutin 
                        {
                            var CharacterIndex = Reader.ReadUInt32();
                            var CutinIndex = Reader.ReadUInt32();
                            if (!player.Inventory.SetCutInIndex(CharacterIndex, CutinIndex))
                            {
                                WriteConsole.WriteLine("[PLAYER_CHANGE_CHARCUTIN]: Error Ao Tentar Setar Cutin", ConsoleColor.Red);
                                player.Close();
                            }
                            Status = true;
                            Reply.Write(CharacterIndex);
                            Reply.Write(CutinIndex);
                            Reply.WriteZero(12);//12 byte vazios
                        }
                        break;
                    default:
                        WriteConsole.WriteLine("Action_Unkown: {0}, Array: {1}", new object[] { action, BitConverter.ToString(Reader.GetRemainingData) });
                        break;
                }
                if (Status)
                {
                    player.SendResponse(Reply.GetBytes());
                }
            }
            catch
            {
            }
        }

        public void PlayerGPTime(GPlayer player)
        {
            using (var Response = new PangyaBinaryWriter())
            {
                Response.Write(new byte[] { 0xbA, 0x00 });
                Response.Write(GameTools.Tools.GetFixTime(DateTime.Now));
                player.SendResponse(Response);
            }
        }

        public void PlayerChangeMascotMessage(GPlayer player, Packet packet)
        {
            packet.ReadUInt32(out uint MASCOT_IDX);
            packet.ReadPStr(out string MASCOT_MSG);

            if (!player.Inventory.SetMascotText(MASCOT_IDX, MASCOT_MSG))
            {
                player.SendResponse(new byte[] { 0xE2, 0x00, 0x01 });
                return;
            }
            player.Response.Write(new byte[] { 0xE2, 0x00, 0x04 });
            player.Response.WriteUInt32(MASCOT_IDX);
            player.Response.WritePStr(MASCOT_MSG);
            player.Response.WriteUInt64(player.GetPang);
            player.SendResponse();
        }

        public void PlayerGetCutinInfo(GPlayer player, Packet packet)
        {
            var cutin = (CutinInfoData)packet.Read(new CutinInfoData());

            switch (cutin.Type)
            {
                case 0:
                    {
                        player.SendResponse(IffEntry.CutinInfo.GetCutinString(cutin.TypeID));
                    }
                    break;
                case 1:
                    {
                        var Char = player.Inventory.GetCharacter(cutin.TypeID);
                        var Item = player.Inventory.ItemWarehouse.GetItem(Char.FCutinIndex);
                        if (Item == null)
                        {
                            return;
                        }
                        player.SendResponse(IffEntry.CutinInfo.GetCutinString(Item.ItemTypeID));
                    }
                    break;
            }
        }
        #endregion
    }
}
