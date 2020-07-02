using PangyaFileCore.BinaryModels;
using PangyaFileCore.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
namespace PangyaFileCore.Collections
{
    public class CardCollection : Dictionary<uint, Card>
    {
        public List<Card> ListCard { get; set; }
        /// <summary>
        /// ID determining relation to other IFF files
        /// </summary>
        public ushort BindingID;

        /// <summary>
        /// Version of this IFF file
        /// </summary>
        public uint Version;

        public void Load()
        {
            MemoryStream Buffer;
            Card Card;
            PangyaBinaryReader Reader = null;
            ListCard = new List<Card>();
            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "Card.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                    throw new Exception("data\\Card.iff is not loaded");
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
                    Card = (Card)Reader.Read(new Card());

                    Add(Card.Base.TypeID, Card);
                }
            }
            finally
            {
                Reader.Dispose();
            }
        }

        public string GetItemName(UInt32 TypeID)
        {
            Card Card = new Card();
            if (!LoadCard(TypeID, ref Card))
            {
                return "Unknown Item Name";
            }
            return Card.Base.Name;
        }

        public UInt32 GetPrice(UInt32 TypeID)
        {
            Card Card = new Card();
            if (!LoadCard(TypeID, ref Card))
            {
                return 0;
            }
            return Card.Base.ItemPrice;
        }

        public sbyte GetShopPriceType(UInt32 TypeId)
        {
            Card Card = new Card();
            if (!LoadCard(TypeId, ref Card))
            {
                return -1;
            }
            return (sbyte)Card.Base.PriceType;
        }

        public Dictionary<UInt32, UInt32> GetSPCL(UInt32 TypeId)
        {
            Card C = new Card();
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
            Card Item = new Card();
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
            Card Card = new Card();

            if (!LoadCard(TypeId, ref Card))
            {
                return false;
            }
            return Convert.ToBoolean(Card.Base.Enabled);
        }

        public bool LoadCard(UInt32 ID, ref Card Card)
        {
            if (!this.TryGetValue(ID, out Card))
            {
                return false;
            }
            return true;
        }

        public object GetItem(uint TypeId)
        {
            Card Card = new Card();

            if (!LoadCard(TypeId, ref Card))
            {
                return Card;
            }
            return Card;
        }
    }
}
