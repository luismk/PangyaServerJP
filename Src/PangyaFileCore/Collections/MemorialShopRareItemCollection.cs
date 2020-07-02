using PangyaFileCore.BinaryModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using PangyaFileCore.Data;

namespace PangyaFileCore.Collections
{
   public class MemorialShopRareItemCollection : List<MemorialRareItem> 
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
            MemorialRareItem Rare;
            PangyaBinaryReader Reader = null;

            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "MemorialShopRareItem.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                     throw new Exception(" data\\MemorialShopRareItem.iff is not loaded");
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
                long recordLength = (Reader.GetSize() - 8L) / recordCount;
                Reader.ReadUInt16(out BindingID);
                Reader.ReadUInt32(out Version);

                for (int i = 0; i < recordCount; i++)
                {
                    Rare = (MemorialRareItem)Reader.Read(new MemorialRareItem());
                    this.Add(Rare);
                }
            }
            finally
            {
                Reader.Dispose();
                Buffer.Dispose();
            }

        }
       
        public MemorialRareItem GetItem(UInt32 TypeID)
        {
            foreach (var item in this)
            {
                if (item.TypeID == TypeID)
                {
                    return item;
                }
            }
            return new MemorialRareItem();
        }
    }
}
