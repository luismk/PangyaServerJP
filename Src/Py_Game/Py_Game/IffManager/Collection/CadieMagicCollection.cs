using PangyaAPI.BinaryModels;
using Py_Game.IffManager.Data.CadieMagicBox;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.IffManager.Collection
{
    public class CadieMagicCollection : List<IffCadieMagicBoxData>
    {
        public Dictionary<uint, IffCadieMagicBoxData> ItemMagicBox;
        /// <summary>
        /// ID determining relation to other IFF files
        /// </summary>
        public ushort BindingID;

        /// <summary>
        /// Version of this IFF file
        /// </summary>
        public uint Version;

        //Constructor 
        public CadieMagicCollection()
        {
            MemoryStream Buffer;
            IffCadieMagicBoxData item;

            PangyaBinaryReader Reader = null;
            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "CadieMagicBox.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                    PangyaAPI.WriteConsole.WriteLine(" data\\CadieMagicBox.iff is not loaded");
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
                Reader.ReadUInt16(out BindingID);
                Reader.ReadUInt32(out Version);
                for (int i = 0; i < recordCount; i++)
                {
                    item = (IffCadieMagicBoxData)Reader.Read(new IffCadieMagicBoxData());

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
            ItemMagicBox = new Dictionary<uint, IffCadieMagicBoxData>();
            foreach (var Magic in this)
            {
                ItemMagicBox.Add(Magic.MagicID, Magic);
            }
        }

        public Dictionary<UInt32, UInt32> GetItem(UInt32 MagicID)
        {
            IffCadieMagicBoxData MGBox = new IffCadieMagicBoxData();
            if (!LoadBox(MagicID, ref MGBox))
            {
                return new Dictionary<UInt32, UInt32>() { { 0, 0 } };
            }
            return new Dictionary<UInt32, UInt32>() { { MGBox.TypeID, MGBox.Quatity } };
        }

        public List<Dictionary<UInt32, UInt32>> GetMagicTrade(UInt32 MagicID)
        {
            IffCadieMagicBoxData MGBox = new IffCadieMagicBoxData();
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

        public bool LoadBox(UInt32 ID, ref IffCadieMagicBoxData Box)
        {
            if (!ItemMagicBox.TryGetValue(ID, out Box))
            {
                return false;
            }
            return true;
        }
    }
}
