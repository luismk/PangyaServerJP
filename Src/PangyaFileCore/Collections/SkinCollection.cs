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
    public class SkinCollection : Dictionary<uint, Skin> 
    {
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
            Skin Skin;

            PangyaBinaryReader Reader = null;

            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c=> c.Name == "Skin.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                     throw new Exception(" data\\Skin.iff is not loaded");
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
                    Skin = (Skin)Reader.Read(new Skin());

                    this.Add(Skin.Base.TypeID, Skin);
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
            Skin Skin = new Skin();
            if (!LoadSkin(TypeID, ref Skin))
            {
                return "";
            }
            return Skin.Base.Name;
        }

        public UInt32 GetPrice(UInt32 TypeID, uint Day)
        {
            Skin Skin = new Skin();
            if (!LoadSkin(TypeID, ref Skin))
            {
                return 0;
            }
            if (Skin.Base.Enabled == 1)
            {
                switch (Day)
                {
                    case 0:   return Skin.Base.ItemPrice;
                    case 7: return Skin.Price7;
                    case 30: return Skin.Price30;
                    default: return Skin.PriceUnk;
                }
            }
            return 0;
        }


        public sbyte GetShopPriceType(UInt32 TypeId)
        {
            Skin Skin = new Skin();
            if (!LoadSkin(TypeId, ref Skin))
            {
                return -1;
            }
            return (sbyte)Skin.Base.PriceType;
        }

        public byte GetSkinFlag(UInt32 TypeId)
        {
            Skin Items = new Skin();
            if (!LoadSkin(TypeId, ref Items))
            {
                return 0;
            }
            if ((Items.Base.TypeID == TypeId) && (Items.Base.Enabled == 1))
            {
                if ((Items.Price7 == 0) && (Items.Price30 == 0) && (Items.PriceUnk == 0))
                {
                    return 0;
                }
                else
                {
                    return 0x20;
                }
            }
            return 0;
        }

        public bool IsBuyable(UInt32 TypeId)
        {
            Skin Item = new Skin();
            if (!LoadSkin(TypeId, ref Item))
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
            Skin Skin = new Skin();

            if (!LoadSkin(TypeId, ref Skin))
            {
                return false;
            }
            return Convert.ToBoolean(Skin.Base.Enabled);
        }

        public bool LoadSkin(UInt32 ID, ref Skin Skin)
        {
            if (!this.TryGetValue(ID, out Skin))
            {
                return false;
            }
            return true;
        }

        internal object GetItem(uint typeID)
        {
            throw new NotImplementedException();
        }
    }
}
