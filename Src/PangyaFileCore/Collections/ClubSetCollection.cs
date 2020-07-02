using PangyaFileCore.BinaryModels;
using PangyaFileCore.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PangyaFileCore.Collections
{
   public class ClubSetCollection : Dictionary<uint, ClubSet>
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
            ClubSet Club;
            PangyaBinaryReader Reader = null;

            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "ClubSet.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                     throw new Exception(" data\\ClubSet.iff is not loaded");
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
                    Club = (ClubSet)Reader.Read(new ClubSet());

                    this.Add(Club.Base.TypeID, Club);
                }
            }
            finally
            {
                Reader.Dispose();
                Buffer.Dispose();
            }
        }

        public ClubSet GetItem(uint ID)
        {
            ClubSet ClubInfo = new ClubSet();
            if (!LoadItem(ID, ref ClubInfo))
            {
                return ClubInfo;
            }
            return ClubInfo;
        }
              
        public string GetItemName(UInt32 TypeID)
        {
            ClubSet Club = new ClubSet();
            if (!LoadItem(TypeID, ref Club))
            {
                return "";
            }
            return Club.Base.Name;
        }

        public UInt32 GetPrice(UInt32 TypeID)
        {
            ClubSet Club = new ClubSet();
            if (!LoadItem(TypeID, ref Club))
            {
                return 99999999;
            }
            return Club.Base.ItemPrice;
        }



        public sbyte GetShopPriceType(UInt32 TypeId)
        {
            ClubSet Club = new ClubSet();
            if (!LoadItem(TypeId, ref Club))
            {
                return -1;
            }
            return (sbyte)Club.Base.PriceType;
        }

        public bool IsBuyable(UInt32 TypeId)
        {
            ClubSet Item = new ClubSet();
            if (!LoadItem(TypeId, ref Item))
            {
                return false;
            }
            if (Item.Base.Enabled == 1 && Item.Base.MoneyFlag == 0 || Item.Base.MoneyFlag == 1)
            {
                return true;
            }
            return false;
        }

        public bool IsExist(UInt32 TypeId)
        {
            ClubSet Club = new ClubSet();

            if (!LoadItem(TypeId, ref Club))
            {
                return false;
            }
            return Convert.ToBoolean(Club.Base.Enabled);
        }

        public bool LoadItem(UInt32 ID, ref ClubSet Club)
        {
            if (!this.TryGetValue(ID, out Club))
            {
                return false;
            }
            return true;
        }
    }
}
