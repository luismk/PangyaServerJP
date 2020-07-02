using PangyaAPI.BinaryModels;
using Py_Game.IffManager.Data.Part;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.IffManager.Collection
{
    public class PartCollection : Dictionary<UInt32, IffPartData>
    {
        /// <summary>
        /// ID determining relation to other IFF files
        /// </summary>
        public ushort BindingID;

        /// <summary>
        /// Version of this IFF file
        /// </summary>
        public uint Version;

        public PartCollection()
        {
            MemoryStream Buffer;
            IffPartData Part;
            PangyaBinaryReader Reader = null;

            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "Part.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                    PangyaAPI.WriteConsole.WriteLine(" data\\Part.iff is not loaded");
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
                    Part = (IffPartData)Reader.Read(new IffPartData());

                    this.Add(Part.Base.TypeID, Part);
                }
            }
            finally
            {
                Reader.Dispose();
                Buffer.Dispose();
            }
        }

        //Destructor
        ~PartCollection()
        {
            this.Clear();
        }

        public IffPartData GetItem(UInt32 TypeID)
        {
            IffPartData Part = new IffPartData();
            if (!LoadPart(TypeID, ref Part))
            {
                return Part;
            }
            return Part;
        }
        public string GetItemName(UInt32 TypeID)
        {
            IffPartData Part = new IffPartData();
            if (!LoadPart(TypeID, ref Part))
            {
                return "";
            }
            return Part.Base.Name;
        }

        public UInt32 GetPrice(UInt32 TypeID)
        {
            IffPartData Part = new IffPartData();
            if (!LoadPart(TypeID, ref Part))
            {
                return 99999999;
            }
            return Part.Base.ItemPrice;
        }

        public UInt32 GetRentalPrice(UInt32 TypeId)
        {
            IffPartData Part = new IffPartData();
            if (!LoadPart(TypeId, ref Part))
            {
                return 0;
            }
            if ((Part.Base.Enabled == 1))
            {
                return Part.RentPang;
            }
            return 0;
        }

        public sbyte GetShopPriceType()
        {
            foreach (var Part in this.Values)
            {
                if (Part.Base.PriceType > 0)
                {
                    return (sbyte)Part.Base.PriceType;
                }
            }
            return 0;
        }

        public sbyte GetShopPriceType(UInt32 TypeId)
        {
            IffPartData Part = new IffPartData();
            if (!LoadPart(TypeId, ref Part))
            {
                return -1;
            }
            return (sbyte)Part.Base.PriceType;
        }

        public bool IsBuyable(UInt32 TypeId)
        {
            IffPartData Part = new IffPartData();
            if (!LoadPart(TypeId, ref Part))
            {
                return false;
            }
            if (Part.Base.Enabled == 1 && Part.Base.MoneyFlag == 0 || Part.Base.MoneyFlag == 1 || Part.Base.MoneyFlag == 2)
            {
                return true;
            }
            return false;
        }

        public bool IsExist(UInt32 TypeId)
        {
            IffPartData Part = new IffPartData();

            if (!LoadPart(TypeId, ref Part))
            {
                return false;
            }
            return Convert.ToBoolean(Part.Base.Enabled);
        }

        public bool LoadPart(UInt32 ID, ref IffPartData Part)
        {
            if (!this.TryGetValue(ID, out Part))
            {
                return false;
            }
            return true;
        }


        public void Save(string filePath)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create, FileAccess.Write)))
            {
                writer.Write((ushort)this.Count);
                writer.Write(this.BindingID);
                writer.Write(this.Version);
                foreach (var Part in this.Values)
                {
                    var entry = Part;

                    if (Part.Base.Icon != "" && entry.Base.PriceType == 0)
                    {
                        entry.Base.PriceType = 37;
                        entry.Base.MoneyFlag = 1;
                        if (entry.Base.ItemPrice >= 10000000)
                        {
                            entry.Base.ItemPrice = 1;
                        }
                        int size = Marshal.SizeOf(entry);
                        byte[] arr = new byte[size];

                        IntPtr ptr = Marshal.AllocHGlobal(size);
                        Marshal.StructureToPtr(entry, ptr, true);
                        Marshal.Copy(ptr, arr, 0, size);
                        Marshal.FreeHGlobal(ptr);

                        writer.Write(arr);
                    }
                    else
                    {
                        int size = Marshal.SizeOf(entry);
                        byte[] arr = new byte[size];

                        IntPtr ptr = Marshal.AllocHGlobal(size);
                        Marshal.StructureToPtr(entry, ptr, true);
                        Marshal.Copy(ptr, arr, 0, size);
                        Marshal.FreeHGlobal(ptr);

                        writer.Write(arr);
                    }
                }
            }
        }
    }
}
