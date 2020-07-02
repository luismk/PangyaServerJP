using PangyaFileCore.BinaryModels;
using PangyaFileCore.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace PangyaFileCore.Collections
{
    public class CadieMagicCollection : List<CadieMagicBox>
    {
        public Dictionary<uint, CadieMagicBox> ItemMagicBox;
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
            CadieMagicBox item;

            PangyaBinaryReader Reader = null;
            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "CadieMagicBox.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                     throw new Exception(" data\\CadieMagicBox.iff is not loaded");
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
                Reader.ReadUInt16(out BindingID);
                Reader.ReadUInt32(out Version);
                for (int i = 0; i < recordCount; i++)
                {
                    item = (CadieMagicBox)Reader.Read(new CadieMagicBox());

                    this.Add(item);
                }
                AddMagicBox();
            }
            finally
            {
                Buffer.Dispose();
            }
        }
        private void AddMagicBox()
        {
            ItemMagicBox = new Dictionary<uint, CadieMagicBox>();
            foreach (var Magic in this)
            {
                ItemMagicBox.Add(Magic.MagicID, Magic);
            }
        }

        public Dictionary<UInt32, UInt32> GetItem(UInt32 MagicID)
        {
            CadieMagicBox MGBox = new CadieMagicBox();
            if (!LoadBox(MagicID, ref MGBox))
            {
                return new Dictionary<UInt32, UInt32>() { { 0, 0 } };
            }
            return new Dictionary<UInt32, UInt32>() { { MGBox.TypeID, MGBox.Quatity } };
        }

        public List<Dictionary<UInt32, UInt32>> GetMagicTrade(UInt32 MagicID)
        {
            CadieMagicBox MGBox = new CadieMagicBox();
            List<Dictionary<UInt32, UInt32>> result = new List<Dictionary<UInt32, UInt32>>();

            if (!LoadBox(MagicID, ref MGBox))
            {
                return result;
            }

            for (var Count = 0; Count <= MGBox.TradeID.Length - 1; Count++)
            {
                if (MGBox.TradeID[Count] > 0)
                {
                    result.Add(new Dictionary<uint, uint>() { { MGBox.TradeID[Count], MGBox.TradeQuantity[Count] } });
                }
            }
            return result;
        }

        public bool LoadBox(UInt32 ID, ref CadieMagicBox Box)
        {
            if (!ItemMagicBox.TryGetValue(ID, out Box))
            {
                return false;
            }
            return true;
        }
    }
}
