using PangyaFileCore.BinaryModels;
using PangyaFileCore.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
namespace PangyaFileCore.Collections
{
   public class CaddieItemCollection : List<CaddieItem>
    {

        /// <summary>
        /// ID determining relation to other IFF files
        /// </summary>
        public ushort BindingID;

        /// <summary>
        /// Version of this IFF file
        /// </summary>
        public uint Version;

        //Constructor 
        public void Load()
        {
            MemoryStream Buffer;
            CaddieItem CaddieItem;
            PangyaBinaryReader Reader = null;
            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "CaddieItem.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                     throw new Exception(" data\\CaddieItem.iff is not loaded");
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
                    CaddieItem = (CaddieItem)Reader.Read(new CaddieItem());

                    this.Add(CaddieItem);
                }
            }
            finally
            {
                Reader.Dispose();
                Buffer.Dispose();
            }
        }
        
        public string GetItemName(UInt32 TypeID)
        {
            foreach (var Item in this)
            {
                if (Item.Base.TypeID == TypeID && Item.Base.Enabled == 1)
                {
                    return Item.Base.Name;
                }
            }
            return "";
        }

        public UInt32 GetPrice(UInt32 TypeID, uint Day)
        {
            foreach (var Item in this)
            {
                if (Item.Base.Enabled == 1 && Item.Base.TypeID == TypeID)
                {
                    switch (Day)
                    {
                        case 1:
                            return Item.Price15;
                        case 15:
                            return Item.Price15;
                        case 30:
                            return Item.Price30;
                    }
                }

                if (Item.Price1 == 0 && Item.Price15 == 0 && Item.Price30 == 0 && Item.Base.TypeID == TypeID)
                {
                    return Item.Base.PriceType;
                }
            }

            return 0;
        }



        public sbyte GetShopPriceType(UInt32 TypeId)
        {
            foreach (var Item in this)
            {
                if (Item.Base.TypeID == TypeId && Item.Base.Enabled == 1)
                {
                    return (sbyte)Item.Base.PriceType;
                }
            }
            return -1;
        }

        public bool IsBuyable(UInt32 TypeId)
        {
            foreach (var Item in this)
            {
                if (Item.Base.TypeID == TypeId && Item.Base.Enabled == 1 && Item.Base.MoneyFlag == 0 || Item.Base.MoneyFlag == 1)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsExist(UInt32 TypeId)
        {
            foreach (var Item in this)
            {
                if (Item.Base.TypeID == TypeId && Item.Base.Enabled == 1)
                {
                    return true;
                }
            }
            return false;
        }

        internal object GetItem(uint typeID)
        {
            throw new NotImplementedException();
        }
    }
}
