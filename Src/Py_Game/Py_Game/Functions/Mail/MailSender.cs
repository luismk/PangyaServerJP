using Py_Connector.DataBase;
using Py_Game.Defines;
using static PangyaFileCore.IffBaseManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PangyaFileCore.Data;

namespace Py_Game.Functions.Mail
{
    public class MailSender : List<MailItem>, IDisposable
    {
        private StringBuilder Message { get; set; }
        public string Sender { get; set; }

        public uint GetItemGroup(uint TypeID)
        {
            var result = (uint)Math.Round((TypeID & 4227858432) / Math.Pow(2.0, 26.0));

            return result;
        }

        public void AddItem(MailItem ItemData, bool WithName = false)
        {
            MailItem PlayerItemDataData = new MailItem
            {
                TypeID = ItemData.TypeID,
                SetID = ItemData.SetID,
                Quantity = ItemData.Quantity,
                Day = ItemData.Day
            };
            Add(PlayerItemDataData);
        }

        public void AddItem(UInt32 rTypeID, UInt32 rQuantity, bool WithName = false)
        {
            MailItem MailItem;

            switch ((TITEMGROUP)(GetItemGroup(rTypeID)))
            {
                case TITEMGROUP.ITEM_TYPE_SETITEM:
                    {
                        AddSetItem(rTypeID, WithName);
                    }
                    break;
                case TITEMGROUP.ITEM_TYPE_MASCOT:
                    {
                        MailItem = new MailItem()
                        {
                            TypeID = rTypeID,
                            SetID = 0,
                            Quantity = rQuantity,
                            Day = 7
                        };
                        AddItem(MailItem, WithName);
                    }
                    break;
                case TITEMGROUP.ITEM_TYPE_CADDIE:
                    {
                        MailItem = new MailItem()
                        {
                            TypeID = rTypeID,
                            SetID = 0,
                            Quantity = rQuantity,
                            Day = 0
                        };
                        AddItem(MailItem, WithName);
                    }
                    break;
                case TITEMGROUP.ITEM_TYPE_CADDIE_ITEM:
                case TITEMGROUP.ITEM_TYPE_SKIN:
                default:
                    {
                        MailItem = new MailItem()
                        {
                            TypeID = rTypeID,
                            SetID = 0,
                            Quantity = rQuantity,
                            Day = 0
                        };
                        AddItem(MailItem, WithName);
                    }
                    break;
            }
        }

        public void AddItemTutorial(uint Value, uint Option)
        {
            AddText("Oba voce acabou de completar uma missao do tutorial !");
            if (Option == 0)
            {
                switch (Value)
                {
                    case 1:
                        {
                            AddItem(0x1A00000F, 3);
                        }
                        break;
                    case 2:
                        {
                            AddItem(0x18000007, 3);
                        }
                        break;
                    case 4:
                        {
                            AddItem(0x18000005, 3);
                        }
                        break;
                    case 8:
                        {
                            AddItem(0x18000008, 3);
                        }
                        break;
                    case 16:
                        {
                            AddItem(0x1A000010, 500);
                        }
                        break;
                    case 32:
                        {
                            AddItem(0x18000004, 3);
                        }
                        break;
                    case 64:
                        {
                            AddItem(0x1A000010, 500);
                        }
                        break;
                    case 128:
                        {
                            AddItem(0x1A000010, 1000);

                            
                        }
                        break;
                    case 256:
                        {
                            AddItem(0x1A00000F, 3);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (Value)
                {
                    case 256:
                        {
                            AddItem(0x1A00000F, 3);
                        }
                        break;

                    case 512:
                        {
                            AddItem(0x18000028, 1);
                        }
                        break;
                    case 1024:
                        {
                            AddItem(0x18000006, 1);
                        }
                        break;
                    case 2048:
                        {
                            AddItem(0x18000007, 5);
                        }
                        break;
                    case 4096:
                        {
                            AddItem(0x18000000, 4);
                        }
                        break;
                    case 8192:
                        {
                            AddItem(0x18000001, 4);
                        }
                        break;
                }
            }
        }

        public void AddItemTutorialEvent(uint Option)
        {
            switch (Option)
            {
                case 1:
                    {
                        AddText("Opa voce acabou de terminar o tutorial Rookie !");
                        //Caddie Papel
                        AddItem(0x1C000000, 1);
                        //Club Set Air Knight Lucky
                        AddItem(0x10000012, 1);
                    }
                    break;
                case 2:
                    {
                        AddText("Opa voce acabou de terminar o tutorial Beginner !");
                        //Item Power Potion
                        AddItem(0x18000027, 10);
                        //Pang Pouch 5000 - fresh up 10000
                        AddItem(0x1A000010, 10000);
                    }
                    break;
            }
        }
        public void AddItemLevel(TLEVEL Level)
        {
            AddText($"Congratulations Level Up {Level} !");
            switch (Level)
            {
                case TLEVEL.ROOKIE_F:
                case TLEVEL.ROOKIE_E:
                    {
                        this.AddItem(402653192, 10);
                        this.AddItem(402653191, 10);
                    }
                    break;
                case TLEVEL.ROOKIE_D:
                    {
                        AddItem(402653189, 10);
                        AddItem(402653188, 5);
                    }
                    break;
                case TLEVEL.ROOKIE_C:
                    {
                        AddItem(436207633, 50);
                    }
                    break;
                case TLEVEL.ROOKIE_B:
                    AddItem(402653188, 10);
                    AddItem(436207633, 50);
                    break;
                case TLEVEL.ROOKIE_A:
                    AddItem(0x1A00000F, 5);
                    AddItem(436207633, 50);
                    break;
                case TLEVEL.BEGINNER_E:
                    AddItem(0x1A000010, 10000);
                    AddItem(436207656, 5);
                    break;
                case TLEVEL.BEGINNER_D:
                    AddItem(402653200, 5);
                    AddItem(402653190, 5);
                    break;
                case TLEVEL.BEGINNER_C:
                    AddItem(402653202, 5);
                    AddItem(402653198, 5);
                    break;
                case TLEVEL.BEGINNER_B:
                    AddItem(402653201, 5);
                    AddItem(402653223, 5);
                    break;
                case TLEVEL.BEGINNER_A:
                    AddItem(436207618, 20);
                    AddItem(402653224, 5);
                    break;
                case TLEVEL.JUNIOR_E:
                    AddItem(0x1A000010, 30000);
                    break;
                case TLEVEL.JUNIOR_D:
                    AddItem(436207667, 1);
                    break;
                case TLEVEL.JUNIOR_C:
                    AddItem(1073741827, 1);
                    AddItem(335544321, 18);
                    break;
                case TLEVEL.JUNIOR_B:
                    AddItem(1073741826, 1);
                    AddItem(335544322, 36);
                    break;
                case TLEVEL.JUNIOR_A:
                    AddItem(1073741825, 1);
                    AddItem(436207877, 1);
                    break;
                case TLEVEL.SENIOR_E:
                    {
                        AddItem(0x1A000010, 50000);
                    }
                    break;
                case TLEVEL.SENIOR_D:
                    AddItem(2092957699, 3);

                    AddItem(436207680, 50);
                    break;
                case TLEVEL.SENIOR_C:
                    AddItem(2092957698, 2);

                    AddItem(436207680, 50);
                    break;
                case TLEVEL.SENIOR_B:

                    AddItem(2092957697, 1);

                    AddItem(436207680, 50);
                    break;
                case TLEVEL.SENIOR_A:
                    AddItem(2092957696, 1);

                    AddItem(2092957700, 1);
                    break;
                case TLEVEL.AMATEUR_E:
                    AddItem(0x1A000010, 70000);
                    break;
                case TLEVEL.AMATEUR_D:
                    AddItem(436207710, 1);
                    break;
                case TLEVEL.AMATEUR_C:
                    AddItem(436207709, 1);
                    break;
                case TLEVEL.AMATEUR_B:
                    AddItem(436207708, 1);
                    break;
                case TLEVEL.AMATEUR_A:
                    AddItem(436207707, 1);
                    break;
                case TLEVEL.SEMI_PRO_E:
                    AddItem(0x1A000010, 90000);
                    break;
                case TLEVEL.SEMI_PRO_D:
                    AddItem(0x1A0000F7, 1);
                    break;
                case TLEVEL.SEMI_PRO_C:
                    AddItem(0x1A0000F7, 1);
                    break;
                case TLEVEL.SEMI_PRO_B:
                    AddItem(0x1A0000F7, 1);
                    break;
                case TLEVEL.SEMI_PRO_A:
                    AddItem(0x1A0000F7, 1);
                    break;
                case TLEVEL.PRO_E:
                    AddItem(0x1A000010, 110000);
                    break;
                case TLEVEL.PRO_D:
                    AddItem(2092957699, 4);
                    break;
                case TLEVEL.PRO_C:
                    AddItem(2092957698, 3);
                    break;
                case TLEVEL.PRO_B:
                    AddItem(2092957697, 2);
                    break;
                case TLEVEL.PRO_A:
                    AddItem(0x1A0000F7, 2);
                    break;
                case TLEVEL.NATIONAL_PRO_E:
                    AddItem(0x1A000010, 130000);
                    break;
                case TLEVEL.NATIONAL_PRO_D:
                    AddItem(1073741826, 3);
                    break;
                case TLEVEL.NATIONAL_PRO_C:
                    AddItem(2092957698, 5);
                    break;
                case TLEVEL.NATIONAL_PRO_B:
                    AddItem(0x1A0000F7, 2);
                    break;
                case TLEVEL.NATIONAL_PRO_A:
                    AddItem(1073741825, 3);
                    break;
                case TLEVEL.WORLD_PRO_E:
                    AddItem(0x1A000010, 150000);
                    break;
                case TLEVEL.WORLD_PRO_D:
                    AddItem(1073741826, 5);
                    break;
                case TLEVEL.WORLD_PRO_C:
                    AddItem(2092957697, 3);
                    break;
                case TLEVEL.WORLD_PRO_B:
                    AddItem(0x1A0000F7, 2);
                    break;
                case TLEVEL.WORLD_PRO_A:
                    AddItem(1073741825, 5);
                    break;
                case TLEVEL.MASTER_E:
                    AddItem(0x1A000010, 170000);
                    break;
                case TLEVEL.MASTER_D:
                    AddItem(436207667, 3);
                    break;
                case TLEVEL.MASTER_C:
                    AddItem(436207667, 5);
                    break;
                case TLEVEL.MASTER_B:
                    AddItem(436207667, 7);
                    break;
                case TLEVEL.MASTER_A:
                    AddItem(436207667, 10);
                    break;
                case TLEVEL.TOP_MASTER_E:
                    AddItem(0x1A000010, 190000);
                    break;
                case TLEVEL.TOP_MASTER_D:
                    AddItem(0x1A0000F7, 2);
                    break;
                case TLEVEL.TOP_MASTER_C:
                    AddItem(0x1A0000F7, 3);
                    break;
                case TLEVEL.TOP_MASTER_B:
                    AddItem(0x1A0000F7, 4);
                    break;
                case TLEVEL.TOP_MASTER_A:
                    AddItem(1073741826, 7);
                    break;
                case TLEVEL.WORLD_MASTER_E:
                    AddItem(0x1A000010, 210000);
                    break;
                case TLEVEL.WORLD_MASTER_D:
                    AddItem(436207667, 5);
                    break;
                case TLEVEL.WORLD_MASTER_C:
                    AddItem(436207667, 10);
                    break;
                case TLEVEL.WORLD_MASTER_B:
                    AddItem(436207667, 15);
                    break;
                case TLEVEL.WORLD_MASTER_A:
                    AddItem(1073741825, 15);
                    break;
                case TLEVEL.LEGEND_E:
                    AddItem(0x1A000010, 230000);
                    break;
                case TLEVEL.LEGEND_D:
                    AddItem(0x1A000010, 250000);
                    break;
                case TLEVEL.LEGEND_C:
                    AddItem(0x1A000010, 300000);
                    break;
                case TLEVEL.LEGEND_B:
                    AddItem(0x1A000010, 350000);
                    break;
                case TLEVEL.LEGEND_A:
                    AddItem(0x1A000010, 400000);
                    break;
                case TLEVEL.INFINITY_LEGEND_E:
                    AddItem(0x1A000010, 6000000);
                    break;
                case TLEVEL.INFINITY_LEGEND_D:
                    AddItem(0x1A000010, 700000);
                    break;
                case TLEVEL.INFINITY_LEGEND_C:
                    AddItem(0x1A000010, 800000);
                    break;
                case TLEVEL.INFINITY_LEGEND_B:
                    AddItem(0x1A000010, 900000);
                    break;
                case TLEVEL.INFINITY_LEGEND_A:
                    AddItem(0x1A000010, 1000000);
                    break;
            }
        }

        public void AddSetItem(UInt32 SetTypeID, bool WithName = false)
        {
            UInt32 Index;
            MailItem MailItem;
            if (!(GetItemGroup(SetTypeID) == 9))
            {
                return;
            }
            if (!IffEntry.SetItem.Load(SetTypeID, out SetItem SetItem))
            {
                return;
            }
            if (SetItem.Base.Enabled == 1)
            {
                for (Index = 0; Index <= SetItem.Total; Index++)
                {
                    if (!(SetItem.Type[Index] > 0))
                    {
                        break;
                    }
                    MailItem = new MailItem()
                    {
                        TypeID = SetItem.Type[Index],
                        SetID = SetTypeID,
                        Quantity = SetItem.Qty[Index],
                        Day = 0
                    };
                    AddItem(MailItem);
                }
            }
        }

        public void AddText(string Text)
        {
            this.Message.Append(Text);
        }

        //Constructor
        public MailSender()
        {
            Message = new StringBuilder();
        }
        //Destructor
        ~MailSender()
        {
            Dispose(false);
        }
        public string GetSqlUpdateMailString()
        {
            StringBuilder stringBuilder;

            stringBuilder = new StringBuilder();
            try
            {
                for (int i = 0; i < this.Count; i++)
                {
                    stringBuilder.Append('^');
                    stringBuilder.Append(this[i].TypeID);
                    stringBuilder.Append('^');
                    stringBuilder.Append(this[i].SetID);
                    stringBuilder.Append('^');
                    stringBuilder.Append(this[i].Quantity);
                    stringBuilder.Append('^');
                    stringBuilder.Append(this[i].Day);
                    stringBuilder.Append('^');
                    stringBuilder.Append(GetItemGroup(this[i].TypeID));
                    stringBuilder.Append(',');
                }
                return stringBuilder.ToString();
            }
            finally
            {
                stringBuilder.Clear();
            }
        }
        public void Send(uint UID)
        {
            var _db = new PangyaEntities();
            var Query = $"EXEC [dbo].[ProcMailInsert] @UID = '{UID}',  @SENDER_MAIL = '{Sender}',  @SENDER_MSG_MAIL = '{Message}', @JSONData = '{GetSqlUpdateMailString()}'";
            _db.Database.SqlQuery<PangyaEntities>(Query).FirstOrDefault();
        }

        #region Dispose
        private bool disposedValue = false; // Para detectar chamadas redundantes

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Clear();
                    this.Message.Clear();
                }
                disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class MailItem
    {
        public UInt32 TypeID;
        public UInt32 SetID;
        public UInt32 Quantity;
        public UInt16 Day;
    }
}
