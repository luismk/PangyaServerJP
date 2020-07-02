using PangyaFileCore.BinaryModels;
using PangyaFileCore.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
namespace PangyaFileCore.Collections
{
    public class BallCollection : Dictionary<uint, Ball>
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
            PangyaBinaryReader Reader = null;
            Ball Ball;
            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "Ball.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                    throw new Exception(" data\\Caddie.iff is not loaded");
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
                    Ball = (Ball)Reader.Read(new Ball());

                    this.Add(Ball.Base.TypeID, Ball);
                }
            }
            finally
            {
                Reader.Dispose();
                Buffer.Dispose();
            }
        }
        //Destructor
        ~BallCollection()
        {
            this.Clear();
        }
        public string GetItemName(UInt32 TypeID)
        {
            Ball Ball = new Ball();
            if (!LoadBall(TypeID, ref Ball))
            {
                return "";
            }
            return Ball.Base.Name;
        }

        public UInt32 GetPrice(UInt32 TypeID)
        {
            Ball Ball = new Ball();
            if (!LoadBall(TypeID, ref Ball))
            {
                return 99999999;
            }
            return Ball.Base.ItemPrice;
        }

        public UInt32 GetRealQuantity(UInt32 TypeId, UInt32 Qty)
        {
            Ball Ball = new Ball();
            if (!LoadBall(TypeId, ref Ball))
            {
                return 0;
            }
            if ((Ball.Base.Enabled == 1) && (Ball.Power > 0))
            {
                return Ball.Power;
            }
            return Qty;
        }

        public sbyte GetShopPriceType(UInt32 TypeId)
        {
            Ball Ball = new Ball();
            if (!LoadBall(TypeId, ref Ball))
            {
                return -1;
            }
            return (sbyte)Ball.Base.PriceType;
        }

        public bool IsBuyable(UInt32 TypeId)
        {
            Ball Item = new Ball();
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
            Ball Item = new Ball();

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

        public bool LoadBall(UInt32 ID, ref Ball Ball)
        {
            if (!this.TryGetValue(ID, out Ball))
            {
                return false;
            }
            return true;
        }


        internal Ball GetItem(uint itemTypeID)
        {
            foreach (var item in this.Values)
            {
                if (item.Base.TypeID == itemTypeID)
                {
                    return item;
                }
            }
            return new Ball();
        }
    }
}
