using PangyaFileCore.BinaryModels;
using PangyaFileCore.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
namespace PangyaFileCore.Collections
{
    public class CharacterCollection : Dictionary<uint, Character>
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
            Character Character;

            PangyaBinaryReader Reader = null;

            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "Character.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                     throw new Exception(" data\\Character.iff is not loaded");
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
                    Character = (Character)Reader.Read(new Character());

                    Add(Character.Base.TypeID, Character);
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
            Character Character = new Character();
            if (!LoadCharacter(TypeID, ref Character))
            {
                return "";
            }
            return Character.Base.Name;
        }

        public UInt32 GetPrice(UInt32 TypeID)
        {
            Character Character = new Character();
            if (!LoadCharacter(TypeID, ref Character))
            {
                return 99999999;
            }
            return Character.Base.ItemPrice;
        }


        public sbyte GetShopPriceType(UInt32 TypeId)
        {
            Character Character = new Character();
            if (!LoadCharacter(TypeId, ref Character))
            {
                return -1;
            }
            return (sbyte)Character.Base.PriceType;
        }

        public bool IsBuyable(UInt32 TypeId)
        {
            Character Character = new Character();
            if (!LoadCharacter(TypeId, ref Character))
            {
                return false;
            }
            if (Character.Base.Enabled == 1 && Character.Base.MoneyFlag == 0 || Character.Base.MoneyFlag == 1)
            {
                return true;
            }
            return false;
        }

        public bool IsExist(UInt32 TypeId)
        {
            Character Character = new Character();

            if (!LoadCharacter(TypeId, ref Character))
            {
                return false;
            }
            return Convert.ToBoolean(Character.Base.Enabled);
        }

        public bool LoadCharacter(UInt32 ID, ref Character Character)
        {
            if (!TryGetValue(ID, out Character))
            {
                return false;
            }
            return true;
        }

        internal object GetItem(object id)
        {
            throw new NotImplementedException();
        }
    }
}
