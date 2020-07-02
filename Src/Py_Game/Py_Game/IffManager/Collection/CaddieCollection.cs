using PangyaAPI.BinaryModels;
using Py_Game.IffManager.Data.Caddie;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.IffManager.Collection
{
    public class CaddieCollection : List<IffCaddieData>
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
        public CaddieCollection()
        {
            MemoryStream Buffer;
            IffCaddieData Caddie;
            PangyaBinaryReader Reader = null;
            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "Caddie.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                    PangyaAPI.WriteConsole.WriteLine(" data\\Caddie.iff is not loaded");
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
                    Caddie = (IffCaddieData)Reader.Read(new IffCaddieData());

                    this.Add(Caddie);
                }
            }
            finally
            {

                Buffer.Dispose();
                Reader.Dispose();
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

        public UInt32 GetPrice(UInt32 TypeID)
        {
            foreach (var Item in this)
            {
                if (Item.Base.TypeID == TypeID && Item.Base.Enabled == 1)
                {
                    return Item.Base.ItemPrice;
                }
            }
            return 0;
        }

        public UInt32 GetSalary(UInt32 TypeId)
        {
            foreach (var Item in this)
            {
                if (Item.Base.TypeID == TypeId && Item.Base.Enabled == 1)
                {
                    return Item.Salary;
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
                if (Item.Base.TypeID == TypeId && (Item.Base.Enabled == 1))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
