using Py_Game.IffManager.Data.MemorialShopCoinItem;
using PangyaAPI.BinaryModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
namespace Py_Game.IffManager.Collection
{
   public class MemorialShopCoinItemCollection : Dictionary<uint, IffMemorialCoinItemData>
    {
        /// <summary>
        /// ID determining relation to other IFF files
        /// </summary>
        public ushort BindingID;

        /// <summary>
        /// Version of this IFF file
        /// </summary>
        public uint Version;

        public MemorialShopCoinItemCollection()
        {
            MemoryStream Buffer;
            IffMemorialCoinItemData Item;

            PangyaBinaryReader Reader = null;

            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "MemorialShopCoinItem.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                    PangyaAPI.WriteConsole.WriteLine(" data\\MemorialShopCoinItem.iff is not loaded");
                    return;
                }
                else
                {
                    Buffer = new MemoryStream();
                    FileZip.Open().CopyTo(Buffer);
                }
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
                    Item = (IffMemorialCoinItemData)Reader.Read(new IffMemorialCoinItemData());

                    this.Add(Item.TypeID, Item);
                }
            }
            finally
            {
                Reader.Dispose();
            }
        }
        
        public byte GetPool(UInt32 TypeID)
        {
            IffMemorialCoinItemData Coin = new IffMemorialCoinItemData();
            if (!LoadCoin(TypeID, ref Coin))
            {
                return 0;
            }
            return (byte)Coin.Pool;
        }

        public bool IsExist(UInt32 TypeID)
        {
            IffMemorialCoinItemData Coin = new IffMemorialCoinItemData();
            if (!LoadCoin(TypeID, ref Coin))
            {
                return false;
            }
            return true;
        }

        public bool LoadCoin(UInt32 TypeID, ref IffMemorialCoinItemData PCoin)
        {
            if (!this.TryGetValue(TypeID, out PCoin))
            {
                return false;
            }
            return true;
        }
    }
}
