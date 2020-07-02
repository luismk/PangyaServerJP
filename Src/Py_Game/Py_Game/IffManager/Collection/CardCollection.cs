using PangyaAPI.BinaryModels;
using Py_Game.Defines;
using Py_Game.IffManager.Data.Card;
using Py_Game.ItemList;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
namespace Py_Game.IffManager.Collection
{
    public class CardCollection : Dictionary<uint, IffCardData>
    {
        public ItemRandomClass Cards { get; set; }
        Dictionary<uint, CardPack> PackData { get; set; }

        public List<IffCardData> ListCard { get; set; }
        /// <summary>
        /// ID determining relation to other IFF files
        /// </summary>
        public ushort BindingID;

        /// <summary>
        /// Version of this IFF file
        /// </summary>
        public uint Version;

        public CardCollection()
        {
            MemoryStream Buffer;
            IffCardData Card;
            PangyaBinaryReader Reader = null;
            ListCard = new List<IffCardData>();
            Cards = new ItemRandomClass();
            PackData = new Dictionary<uint, CardPack>();
            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "Card.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                    PangyaAPI.WriteConsole.WriteLine(" data\\Card.iff is not loaded");
                    return;
                }

                Buffer = new MemoryStream();
                FileZip.Open().CopyTo(Buffer);
            }

            try
            {
                Reader = new PangyaBinaryReader(Buffer);
                if (new string(Reader.ReadChars(2)) == "PK")
                {
                    throw new Exception("The given IFF file is a ZIP file, please unpack it before attempting to parse it");
                }

                Reader.Seek(0, 0);

                Reader.ReadUInt16(out ushort recordCount);
                long recordLength = ((Reader.GetSize() - 8L) / (recordCount));
                Reader.ReadUInt16(out BindingID);
                Reader.ReadUInt32(out Version);

                for (int i = 0; i < recordCount; i++)
                {
                    Card = (IffCardData)Reader.Read(new IffCardData());

                    Add(Card.Base.TypeID, Card);
                }

                CreateCardPack();
                CreateCardItem();
            }
            finally
            {
                Reader.Dispose();
            }
        }

        public string GetItemName(UInt32 TypeID)
        {
            IffCardData Card = new IffCardData();
            if (!LoadCard(TypeID, ref Card))
            {
                return "Unknown Item Name";
            }
            return Card.Base.Name;
        }

        public UInt32 GetPrice(UInt32 TypeID)
        {
            IffCardData Card = new IffCardData();
            if (!LoadCard(TypeID, ref Card))
            {
                return 0;
            }
            return Card.Base.ItemPrice;
        }

        public sbyte GetShopPriceType(UInt32 TypeId)
        {
            IffCardData Card = new IffCardData();
            if (!LoadCard(TypeId, ref Card))
            {
                return -1;
            }
            return (sbyte)Card.Base.PriceType;
        }

        public Dictionary<UInt32, UInt32> GetSPCL(UInt32 TypeId)
        {
            IffCardData C = new IffCardData();
            var result = new Dictionary<uint, uint>();
            if (!LoadCard(TypeId, ref C))
            {

                return result;
            }
            result.Add(C.Effect, C.EffectQty);
            return result;
        }

        public bool IsBuyable(UInt32 TypeId)
        {
            IffCardData Item = new IffCardData();
            if (!LoadCard(TypeId, ref Item))
            {
                return false;
            }
            if (Item.Base.Enabled == 1 && Item.Base.MoneyFlag == 0 || Item.Base.MoneyFlag == 1 || Item.Base.MoneyFlag == 2)
            {
                return true;
            }
            return false;
        }

        public bool IsExist(UInt32 TypeId)
        {
            IffCardData Card = new IffCardData();

            if (!LoadCard(TypeId, ref Card))
            {
                return false;
            }
            return Convert.ToBoolean(Card.Base.Enabled);
        }

        public bool LoadCard(UInt32 ID, ref IffCardData Card)
        {
            if (!this.TryGetValue(ID, out Card))
            {
                return false;
            }
            return true;
        }

        #region Get Create Result Card 
        public List<Dictionary<uint, byte>> GetCard(uint PackTypeID)
        {

            ushort GetProb(byte RareType)
            {
                switch (RareType)
                {
                    case 0:
                        return 100;
                    case 1:
                        return 6;
                    case 2:
                        return 2;
                    case 3:
                        return 1;
                    default:
                        return (ushort)GameTools.MathRand.Rand.Next(0, 120);
                }
            }

            ushort GetFreshUPProb(byte RareType)
            {
                switch (RareType)
                {
                    case 1:
                        return 100;
                    case 2:
                        return 10;
                    case 3:
                        return 4;
                    default:
                        return (ushort)GameTools.MathRand.Rand.Next(0, 120);
                }
            }

            List<Dictionary<uint, byte>> result;
            ItemRandomClass CRandom;
            byte CQty;
            ItemRandom CItem;
            result = new List<Dictionary<uint, byte>>();
            CRandom = new ItemRandomClass();
            try
            {
                PackData.TryGetValue(PackTypeID, out CardPack CPack);
                if (CPack == null)
                {
                    return result;
                }
                switch (CPack.CardTypePack)
                {
                    case TPACKCARD.Pack1:
                        foreach (var PZCard in ListCard)
                        {
                            if (PZCard.Volumn == 1)
                            {
                                CRandom.AddItems(PZCard.Base.TypeID, 1, PZCard.Rarity, GetProb(PZCard.Rarity));
                            }
                        }
                        break;
                    case TPACKCARD.Pack2:
                        foreach (var PZCard in ListCard)
                        {
                            if (PZCard.Volumn == 2)
                            {
                                CRandom.AddItems(PZCard.Base.TypeID, 1, PZCard.Rarity, GetProb(PZCard.Rarity));
                            }
                        }
                        break;
                    case TPACKCARD.Pack3:
                        foreach (var PZCard in ListCard)
                        {
                            if (PZCard.Volumn == 3)
                            {
                                CRandom.AddItems(PZCard.Base.TypeID, 1, PZCard.Rarity, GetProb(PZCard.Rarity));
                            }
                        }
                        break;
                    case TPACKCARD.Pack4:
                        foreach (var PZCard in ListCard)
                        {
                            if (PZCard.Volumn == 4)
                            {
                                CRandom.AddItems(PZCard.Base.TypeID, 1, PZCard.Rarity, GetProb(PZCard.Rarity));
                            }
                        }
                        break;
                    case TPACKCARD.Rare:
                        foreach (var PZCard in ListCard)
                        {
                            if (PZCard.Rarity >= 1)
                            {
                                CRandom.AddItems(PZCard.Base.TypeID, 1, PZCard.Rarity, GetFreshUPProb(PZCard.Rarity));
                            }
                        }
                        break;
                    case TPACKCARD.All:
                        foreach (var PZCard in ListCard)
                        {
                            CRandom.AddItems(PZCard.Base.TypeID, 1, PZCard.Rarity, GetProb(PZCard.Rarity));
                        }
                        break;
                }
                // ## set random class
                CRandom.SetCanDup(false);
                CRandom.Arrange();
                for (CQty = 1; CQty <= CPack.Quantity; CQty++)
                {
                    CItem = CRandom.GetItems();
                    result.Add(new Dictionary<uint, byte>
                    {
                        { CItem.TypeId, (byte)CItem.RareType }
                    });
                }
                return result;
            }
            finally
            {
                CRandom = null;
            }
        }

        protected void CreateCardPack()
        {
            foreach (var Card in this.Values)
            {
                switch (Card.Base.TypeID)
                {
                    case 2092957696: //{ Pangya Card Pack No.1 }
                    case 2092957697: //{ Golden Card Ticket }
                    case 2092957698: //{ Silver Card Ticket }
                    case 2092957699: //{ Bronze card ticket }
                    case 2092957700: //{ Pangya Card Pack No.2 }
                    case 2092957701:// { Card Pack No.3 }
                    case 2092957702: //{ Platinum Ticket }
                    case 2092957703: //{ Card Pack No.4 }
                    case 2092957704: //{ Grand Prix Card Pack }
                    case 2092957706: //{ Fresh Up! Card Pack }
                    case 2097152001: //{ Pangya Card Box No.2 }
                    case 2097152002: //{ Card Box No.3 }
                    case 2097152003: //{ Pangya Card Box #4 }
                    case 2084569125: //{ Unknown Name }
                    case 2084569128: //{ Unknown Name }          
                        continue;
                }
                ListCard.Add(Card);
            }

            // ## Pack 1
            PackData.Add(2092957696, new CardPack()
            {
                CardTypePack = TPACKCARD.Pack1,
                Quantity = 3
            });
            // ## Golden Card Ticket
            PackData.Add(2092957697, new CardPack()
            {
                CardTypePack = TPACKCARD.All,
                Quantity = 1
            });
            // ## Silver Card Ticket 
            PackData.Add(2092957698, new CardPack()
            {
                CardTypePack = TPACKCARD.All,
                Quantity = 1
            });
            // ## Bronze card ticket
            PackData.Add(2092957699, new CardPack()
            {
                CardTypePack = TPACKCARD.All,
                Quantity = 1
            });
            // ## Pack 2           
            PackData.Add(2092957700, new CardPack
            {
                CardTypePack = TPACKCARD.Pack2,
                Quantity = 3
            });
            // ## Pack 3
            PackData.Add(2092957701, new CardPack
            {
                CardTypePack = TPACKCARD.Pack3,
                Quantity = 3
            });
            // ## Platinum Card Ticket
            PackData.Add(2092957702, new CardPack()
            {
                CardTypePack = TPACKCARD.All,
                Quantity = 1
            });
            // ## Pack 4
            PackData.Add(2092957703, new CardPack
            {
                CardTypePack = TPACKCARD.Pack4,
                Quantity = 3
            });
            // ## FRESH UP!
            PackData.Add(2092957706, new CardPack
            {
                CardTypePack = TPACKCARD.Rare,
                Quantity = 3
            });
            // ## Grand Prix Card Pack
            PackData.Add(2092957704, new CardPack
            {
                CardTypePack = TPACKCARD.Rare,
                Quantity = 3
            });
        }

        protected void CreateCardItem()
        {
            ushort GetProb(byte RareType)
            {
                switch (RareType)
                {
                    case 0:
                        return 100;
                    case 1:
                        return 6;
                    case 2:
                        return 2;
                    case 3:
                        return 1;
                    default:
                        return (ushort)GameTools.MathRand.Rand.Next(0, 120);
                }
            }
            foreach (var PZCard in this.Values)
            {
                switch (PZCard.Base.TypeID)
                {
                    case 2092957696: //{ Pangya Card Pack No.1 }
                    case 2092957697: //{ Golden Card Ticket }
                    case 2092957698: //{ Silver Card Ticket }
                    case 2092957699: //{ Bronze card ticket }
                    case 2092957700: //{ Pangya Card Pack No.2 }
                    case 2092957701:// { Card Pack No.3 }
                    case 2092957702: //{ Platinum Ticket }
                    case 2092957703: //{ Card Pack No.4 }
                    case 2092957704: //{ Grand Prix Card Pack }
                    case 2092957706: //{ Fresh Up! Card Pack }
                    case 2097152001: //{ Pangya Card Box No.2 }
                    case 2097152002: //{ Card Box No.3 }
                    case 2097152003: //{ Pangya Card Box #4 }
                    case 2084569125: //{ Unknown Name }
                    case 2084569128: //{ Unknown Name }          
                        continue;
                }
                Cards.AddItems(PZCard.Base.TypeID, 1, PZCard.Rarity, GetProb(PZCard.Rarity));
            }
        }

        public CardData GetCard(uint TypeID, uint TypeID2, uint TypeID3)
        {
            CardData result;
            byte CQty;
            ItemRandom CItem;
            result = new CardData();
            IffCardData Card3 = new IffCardData();
            IffCardData Card2 = new IffCardData();
            IffCardData Card = new IffCardData();

            if (!LoadCard(TypeID, ref Card) || !LoadCard(TypeID2, ref Card2) || !LoadCard(TypeID3, ref Card3))
            {
                return result;
            }

            for (CQty = 1; CQty <= 1; CQty++)
            {
                CItem = Cards.GetItems(TypeID, TypeID2, TypeID3);
                if (CItem == null)
                {
                    CItem = Cards.GetItems();
                    result.TypeID = CItem.TypeId;
                    result.Quantity = 1;
                }
                else
                {
                    result.TypeID = CItem.TypeId;
                    result.Quantity = 1;
                }
            }
            return result;
        }

        public Dictionary<bool, IffCardData> GetCardSPCL(UInt32 TypeID)
        {
            IffCardData C = new IffCardData();
            var result = new Dictionary<bool, IffCardData>();
            if (!LoadCard(TypeID, ref C))
            {
                return new Dictionary<bool, IffCardData>();
            }
            result.Add(true, C);
            return result;
        }

        #endregion
    }
}
