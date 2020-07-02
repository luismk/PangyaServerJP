using PangyaAPI.BinaryModels;
using Py_Game.IffManager.Data.Character;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.IffManager.Collection
{
    public class CharacterCollection : Dictionary<uint, IffCharacterData>
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
        public CharacterCollection()
        {
            MemoryStream Buffer;
            IffCharacterData Character;

            PangyaBinaryReader Reader = null;

            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "Character.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                    PangyaAPI.WriteConsole.WriteLine(" data\\Character.iff is not loaded");
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
                    Character = (IffCharacterData)Reader.Read(new IffCharacterData());

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
            IffCharacterData Character = new IffCharacterData();
            if (!LoadCharacter(TypeID, ref Character))
            {
                return "";
            }
            return Character.Base.Name;
        }

        public UInt32 GetPrice(UInt32 TypeID)
        {
            IffCharacterData Character = new IffCharacterData();
            if (!LoadCharacter(TypeID, ref Character))
            {
                return 99999999;
            }
            return Character.Base.ItemPrice;
        }


        public sbyte GetShopPriceType(UInt32 TypeId)
        {
            IffCharacterData Character = new IffCharacterData();
            if (!LoadCharacter(TypeId, ref Character))
            {
                return -1;
            }
            return (sbyte)Character.Base.PriceType;
        }

        public bool IsBuyable(UInt32 TypeId)
        {
            IffCharacterData Character = new IffCharacterData();
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
            IffCharacterData Character = new IffCharacterData();

            if (!LoadCharacter(TypeId, ref Character))
            {
                return false;
            }
            return Convert.ToBoolean(Character.Base.Enabled);
        }

        public bool LoadCharacter(UInt32 ID, ref IffCharacterData Character)
        {
            if (!TryGetValue(ID, out Character))
            {
                return false;
            }
            return true;
        }

    }
}
