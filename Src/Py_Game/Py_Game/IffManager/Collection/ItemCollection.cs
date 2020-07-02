using System;
using System.Collections.Generic;
using System.Linq;
using Py_Game.IffManager.Data.Item;
using PangyaAPI.BinaryModels;
using System.IO;
using System.IO.Compression;

namespace Py_Game.IffManager.Collection
{
    public class ItemCollection : Dictionary<uint, IffItemData>
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
        public ItemCollection()
        {
            MemoryStream Buffer;
            IffItemData Item;
            PangyaBinaryReader Reader = null;

            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "Item.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                    PangyaAPI.WriteConsole.WriteLine(" data\\Item.iff is not loaded");
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
                long recordLength = (Reader.GetSize() - 8L) / recordCount;
                Reader.ReadUInt16(out BindingID);
                Reader.ReadUInt32(out Version);

                for (int i = 0; i < recordCount; i++)
                {
                    Item = (IffItemData)Reader.Read(new IffItemData());

                    this.Add(Item.Base.TypeID, Item);
                }
            }
            finally
            {
                Reader.Dispose();
                Buffer.Dispose();
            }
        }
        //Destructor
        ~ItemCollection()
        {
            this.Clear();
        }

        internal IffItemData GetItem(UInt32 TypeID)
        {
            IffItemData Item = new IffItemData();
            if (!LoadBall(TypeID, ref Item))
            {
                return Item;
            }
            return Item;
        }

        public string GetItemName(UInt32 TypeID)
        {
            IffItemData Item = new IffItemData();
            if (!LoadBall(TypeID, ref Item))
            {
                return "";
            }
            return Item.Base.Name;
        }

        public UInt32 GetPrice(UInt32 TypeID)
        {
            IffItemData Item = new IffItemData();
            if (!LoadBall(TypeID, ref Item))
            {
                return 99999999;
            }
            return Item.Base.ItemPrice;
        }

        public UInt32 GetRealQuantity(UInt32 TypeId, UInt32 Qty)
        {
            IffItemData Item = new IffItemData();
            if (!LoadBall(TypeId, ref Item))
            {
                return 0;
            }
            if ((Item.Base.Enabled == 1) && (Item.Power > 0))
            {
                return Item.Power;
            }
            return Qty;
        }

        public sbyte GetShopPriceType(UInt32 TypeId)
        {
            IffItemData Item = new IffItemData();
            if (!LoadBall(TypeId, ref Item))
            {
                return -1;
            }
            return (sbyte)Item.Base.PriceType;
        }

        public bool IsBuyable(UInt32 TypeId)
        {
            IffItemData Item = new IffItemData();
            if (!LoadBall(TypeId, ref Item))
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
            IffItemData Item = new IffItemData();

            if (!LoadBall(TypeId, ref Item))
            {
                return false;
            }
            if (Item.Base.Enabled == 1)
            {
                return true;
            }
            return false;
        }

        public bool LoadBall(UInt32 ID, ref IffItemData Item)
        {
            if (!this.TryGetValue(ID, out Item))
            {
                return false;
            }
            return true;
        }
    }
}
