using PangyaAPI.BinaryModels;
using Py_Game.IffManager.Data.SetItem;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Py_Game.IffManager.Collection
{
   public class SetItemCollection : Dictionary<UInt32, IffSetItemData>
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
        public SetItemCollection()
        {
            MemoryStream Buffer;
            IffSetItemData Item;

            PangyaBinaryReader Reader = null;

            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "SetItem.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                    PangyaAPI.WriteConsole.WriteLine(" data\\SetItem.iff is not loaded");
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
                    Item = (IffSetItemData)Reader.Read(new IffSetItemData());
                    this.Add(Item.Base.TypeID, Item);
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
            IffSetItemData Item = new IffSetItemData();
            if (!LoadSetItem(TypeID, ref Item))
            {
                return "";
            }
            return Item.Base.Name;
        }

        public UInt32 GetPrice(UInt32 TypeID)
        {
            IffSetItemData Item = new IffSetItemData();
            if (!LoadSetItem(TypeID, ref Item))
            {
                return 0;
            }
            return Item.Base.ItemPrice;
        }

        public string GetSetItemStr(UInt32 TypeId)
        {
            string result = "";
            IffSetItemData Items = new IffSetItemData();
            UInt32 Count;
            if (!LoadSetItem(TypeId, ref Items))
            {
                return result;
            }
            if ((Items.Base.Enabled == 1))
            {
                for (Count = 0; Count <= 9; Count++)
                {
                    if (!(Items.TypeIff[Count] > 0))
                    {
                        break;
                    }
                    result += string.Format("{0}, {1}", new object[] { Items.TypeIff[Count], Items.QtyIff[Count] });
                }
                return result;
            }
            return result;
        }

        public sbyte GetShopPriceType(UInt32 TypeId)
        {
            IffSetItemData Item = new IffSetItemData();
            if (!LoadSetItem(TypeId, ref Item))
            {
                return -1;
            }
            return (sbyte)Item.Base.PriceType;
        }

        public bool IsBuyable(UInt32 TypeId)
        {
            IffSetItemData Item = new IffSetItemData();
            if (!LoadSetItem(TypeId, ref Item))
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
            IffSetItemData Item = new IffSetItemData();

            if (!LoadSetItem(TypeId, ref Item))
            {
                return false;
            }
            if (Item.Base.Enabled == 1)
            {
                return true;
            }
            return false;
        }

        public bool LoadSetItem(UInt32 ID, ref IffSetItemData SetItem)
        {
            if (!this.TryGetValue(ID, out SetItem))
            {
                return false;
            }
            return true;
        }

        public bool Load(UInt32 ID, out IffSetItemData SetItem)
        {
            if (!this.TryGetValue(ID, out SetItem))
            {
                return false;
            }
            return true;
        }

        public List<Dictionary<UInt32, UInt32>> SetList(UInt32 TypeID)
        {
            List<Dictionary<UInt32, UInt32>> result;
            IffSetItemData Items = new IffSetItemData();
            byte Count;
            result = new List<Dictionary<UInt32, UInt32>>();
            if (!LoadSetItem(TypeID, ref Items))
            {
                return result;
            }
            for (Count = 0; Count <= Items.TypeIff.Length - 1; Count++)
            {
                if (Items.TypeIff[Count] > 0)
                {
                    result.Add(new Dictionary<uint, uint>() { { Items.TypeIff[Count], Items.QtyIff[Count] } });
                }
            }
            return result;
        }
    }
}
