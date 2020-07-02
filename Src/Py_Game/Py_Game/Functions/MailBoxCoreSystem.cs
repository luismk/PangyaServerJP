using Py_Game.Client;
using System;
using System.Linq;
using Py_Game.Client.Inventory;
using PangyaAPI.BinaryModels;
using PangyaAPI;
using static Py_Game.GameTools.Tools;
using static Py_Game.GameTools.PacketCreator;
using Py_Connector.DataBase;
using Py_Game.Client.Inventory.Data;
using System.Text;
using PangyaAPI.PangyaPacket;

namespace Py_Game.Functions
{
    public class MailBoxSystem
    {
        public void PlayerGetMailList(GPlayer PL, Packet packet, bool IsDel = false)
        {
            var _db = new PangyaEntities();
            PangyaBinaryWriter Reply;


            if (!packet.ReadInt32(out int PageSelect))
            {
                return;
            }
            Reply = new PangyaBinaryWriter();


            var QueryList = _db.ProcGetMail((int)PL.GetUID, PageSelect, 20, 2).ToList();

            try
            {
                if (IsDel)
                {
                    Reply.Write(new byte[] { 0x15, 0x02 });
                }
                else
                {
                    Reply.Write(new byte[] { 0x11, 0x02 });
                }
                Reply.Write(0);
                Reply.Write(PageSelect);//count
                if (QueryList.Count > 1)
                {
                    Reply.Write(QueryList.FirstOrDefault().PAGE_TOTAL ?? 0);
                }
                else
                {
                    Reply.Write(1);
                }
                Reply.Write(QueryList.Count);
                foreach (var mail in QueryList)
                {
                    var type = Convert.ToUInt32(mail.SETTYPEID > 0 ? mail.SETTYPEID : mail.TYPEID);
                    Reply.Write(mail.Mail_Index);//Mail_Index
                    Reply.WriteStr(mail.Sender, 16);//Sender
                    Reply.WriteZero(116);
                    Reply.Write((byte)mail.IsRead);//)
                    Reply.Write(mail.Mail_Item_Count ?? 0);//Mail_Total COunt
                    Reply.Write(4294967295);
                    Reply.Write(type);//typeID
                    Reply.Write(Convert.ToByte(mail.IsTimer ?? 0));//time
                    Reply.Write(mail.QTY ?? 0);
                    Reply.Write(Convert.ToUInt32(mail.DAY ?? 0));
                    Reply.WriteZero(16);
                    Reply.Write(4294967295);
                    Reply.Write(0);
                    Reply.WriteStr(IsUCCNull(mail.UCC_UNIQUE), 14);
                }
                PL.SendResponse(Reply.GetBytes());
            }
            catch
            {
                PL.Close();
            }
        }

        public void PlayerDeleteMail(GPlayer PL, Packet packet)
        {
            var _db = new PangyaEntities();
            int Ret = 0;
            if (!packet.ReadUInt32(out uint Count))
            {
                return;
            }

            try
            {
                for (var I = 0; I <= Count - 1; I++)
                {
                    if (!packet.ReadUInt32(out uint MailIndex))
                    {
                        return;
                    }
                    Ret = (int)_db.ProcDelMail((int)PL.GetUID, (int)MailIndex).FirstOrDefault();
                }

                if (Ret == 0)
                {
                    PL.SendResponse(new byte[] { 0x14, 0x02, 0xFA, 0x16, 0x2D, 0x00 });//still have an item in email so cannot be deleted
                    return;
                }

                PlayerGetMailList(PL, packet, true);
            }
            catch
            {
                PL.Close();
            }
        }

        public void PlayerReadMail(GPlayer PL, Packet packet)
        {
            var _db = new PangyaEntities();
            PangyaBinaryWriter Reply;


            if (!packet.ReadUInt32(out uint MailIndex))
            {
                return;
            }

            Reply = new PangyaBinaryWriter();

            var QueryList = _db.ProcReadMail((int)PL.GetUID, (int)MailIndex).ToList();

            try
            {

                if (QueryList.Count > 0)
                {
                    Reply.Write(new byte[] { 0x12, 0x02 });
                    Reply.Write((uint)0);
                    Reply.Write(QueryList.First().Mail_Index);//Select Page               
                    Reply.WritePStr(QueryList.First().Sender);//Sender
                    Reply.WritePStr(QueryList.First().RegDate.ToString());//IsRead
                    Reply.WritePStr(QueryList.First().Msg);
                    Reply.Write((byte)1);
                    Reply.Write(QueryList.Count);//FF FF FF FF
                    foreach (var data in QueryList)
                    {
                        Reply.Write(uint.MaxValue);
                        Reply.Write(data.TYPEID);//typeID
                        Reply.Write(Convert.ToByte(data.IsTime));
                        Reply.Write(data.QTY ?? 0);//quantidade
                        Reply.Write(Convert.ToUInt32(data.DAY ?? 0));//dias
                        Reply.WriteZero(16);
                        Reply.Write(4294967295);//FF FF FF FF
                        Reply.Write(0);
                        Reply.WriteStr(IsUCCNull(data.UCC_UNIQUE), 14); //UCC_UNIQUE
                    }
                    PL.SendResponse(Reply.GetBytes());
                }
                else
                {
                    Reply.Write(new byte[] { 0x12, 0x02 });
                    Reply.Write((uint)2);
                    PL.SendResponse(Reply.GetBytes());
                }
            }
            catch
            {
                PL.Close();
            }
        }

        public void PlayerReleaseItem(GPlayer PL, Packet packet)
        {
            UInt32 FTypeID;
            UInt32 FQuantity;
            UInt32 ItemMailIndex;
            AddData ItemAddedData;
            AddItem ItemData;
            StringBuilder SQLString;

            if (!packet.ReadUInt32(out uint MailIndex))
            {
                return;
            }

            var _db = new PangyaEntities();

            var Query = _db.ProcMailItem((int)PL.GetUID, (int)MailIndex).ToList();

            if (!(Query.Count >= 1))
            {
                PL.SendResponse(new byte[] { 0x14, 0x02, 0x98, 0x26, 0x2D, 0x00 });
                return;
            }

            SQLString = new StringBuilder();
            for (int i = 0; i < Query.Count; i++)
            {
                FTypeID = (uint)Query[i].TYPEID;
                FQuantity = (uint)Query[i].QTY;
                // EXP POCKET
                if (FTypeID == 436207965)
                {
                }
                // PANG POCKET
                if (FTypeID == 436207632)
                {
                    if ((PL.GetPang + FQuantity) > uint.MaxValue)
                    {
                        PL.SendResponse(new byte[] { 0x14, 0x02, 0x98, 0x26, 0x2D, 0x00 });
                        break;
                    }
                }
                // OTHERS ITEM
                if (!PL.Inventory.Available(FTypeID, FQuantity))
                {
                    PL.SendResponse(new byte[] { 0x14, 0x02, 0x98, 0x26, 0x2D, 0x00 });//item invalido
                    break;
                }
            }

            for (int i = 0; i < Query.Count; i++)
            {
                FTypeID = (uint)Query[i].TYPEID;
                FQuantity = (uint)Query[i].QTY;
                ItemMailIndex = (uint)Query[i].Mail_Index;
                // EXP POCKET
                if (FTypeID == 436207965)
                {
                    PL.AddExp(FQuantity);
                    SQLString.Append('^');
                    SQLString.Append(ItemMailIndex);
                    SQLString.Append('^');
                    SQLString.Append(436207965);
                    SQLString.Append('^');
                    SQLString.Append(0);
                    SQLString.Append(',');
                }
                else if (FTypeID == 436207632)
                {
                    PL.AddPang(FQuantity);
                    PL.SendPang();
                    SQLString.Append('^');
                    SQLString.Append(ItemMailIndex);
                    SQLString.Append('^');
                    SQLString.Append(436207632);
                    SQLString.Append('^');
                    SQLString.Append(0);
                    SQLString.Append(',');
                }
                else
                {
                    // OTHER ITEM
                    ItemData = new AddItem
                    {
                        ItemIffId = FTypeID,
                        Quantity = FQuantity,
                        Transaction = true,
                        Day = 0
                    };
                    ItemAddedData = PL.AddItem(ItemData);

                    SQLString.Append('^');
                    SQLString.Append(ItemMailIndex);
                    SQLString.Append('^');
                    SQLString.Append(FTypeID);
                    SQLString.Append('^');
                    SQLString.Append(ItemAddedData.ItemIndex);
                    SQLString.Append(',');
                }
                PL.SendTransaction();
                var Table = $"Exec dbo.USP_MAIL_UPDATE @UID = '{(int)PL.GetUID}', @ITEMSTR = '{SQLString.ToString()}'";
                _db.Database.SqlQuery<PangyaEntities>(Table).FirstOrDefault();
            }
            // update mail items
            PL.SendResponse(new byte[] { 0x14, 0x02, 0x00, 0x00, 0x00, 0x00 });
        }

        public void PlayerShowMailPopUp(GPlayer PL)
        {
            var _db = new PangyaEntities();
            PangyaBinaryWriter Reply;
            Reply = new PangyaBinaryWriter();

            var QueryList = _db.ProcGetMail((int)PL.GetUID, 1, 5, 1).ToList();

            Reply.Write(new byte[] { 0x10, 0x02 });
            Reply.Write(0);
            Reply.Write(QueryList.Count);//count
            foreach (var mail in QueryList)
            {
                var type = Convert.ToUInt32(mail.SETTYPEID > 0 ? mail.SETTYPEID : mail.TYPEID);

                Reply.WriteInt32(mail.Mail_Index);//Mail_Index
                Reply.WriteStr(mail.Sender, 10);//Sender
                Reply.WriteZero(123);
                Reply.Write(mail.Mail_Item_Count ?? 0);//Mail_Total COunt
                Reply.Write(uint.MaxValue);//FF FF FF FF
                Reply.WriteUInt32(type);//typeID
                Reply.Write((byte)0);//time
                Reply.WriteInt32(mail.QTY ?? 0);//quantidade
                Reply.WriteZero(20);
                Reply.WriteUInt32(uint.MaxValue);//FF FF FF FF
                Reply.WriteUInt32(0);
                Reply.WriteStr(IsUCCNull(mail.UCC_UNIQUE), 14);
            }
            PL.SendResponse(Reply.GetBytes());
        }

        public void CheckUserForGift(GPlayer player, Packet packet)
        {
            var _db = new PangyaEntities();

            if (!packet.ReadByte(out byte Type))
            {
                player.SendResponse(new byte[] { 0xA1, 0x00, 0x02 });
                return;
            }
            try
            {
                switch (Type)
                {
                    case 1://friend
                        {
                            if (!packet.ReadPStr(out string UserName)) { player.SendResponse(new byte[] { 0xA1, 0x00, 0x02 }); return; }

                            var Query = _db.ProcCheckUsername(UserName).ToList();
                            if (Query.Count <= 0)
                            {
                                player.SendResponse(new byte[] { 0xA1, 0x00, 0x02 });
                            }
                            else
                            {
                                var data = Query.First();

                                player.SendResponse(ShowUserGift(data.UID, data.Username, data.Nickname));
                            }
                        }
                        break;
                    case 0://no friend
                        {
                            packet.ReadPStr(out string UserName);

                            var Query = _db.ProcCheckUsername(UserName).ToList();
                            if (Query.Count <= 0)
                            {
                                player.SendResponse(new byte[] { 0xA1, 0x00, 0x02 });
                                return;
                            }
                            else
                            {
                                var data = Query.First();

                                player.SendResponse(ShowUserGift(data.UID, data.Username, data.Nickname));
                            }
                        }
                        break;
                    default:
                        {
                            player.Response.Write(new byte[] { 0xA1, 0x00 });
                            player.Response.WriteByte(2);
                            player.SendResponse();
                        }
                        break;
                }
            }
            catch
            {
                player.Close();
            }
        }
    }
}
