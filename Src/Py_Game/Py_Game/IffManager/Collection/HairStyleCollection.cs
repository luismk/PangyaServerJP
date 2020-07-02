using PangyaAPI.BinaryModels;
using Py_Game.Defines;
using Py_Game.IffManager.Data.HairStyle;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.IffManager.Collection
{
    public class HairStyleCollection : Dictionary<uint, IffHairStyleData>
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
        public HairStyleCollection()
        {
            MemoryStream Buffer;
            PangyaBinaryReader Reader = null;
            IffHairStyleData HairStyle;

            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "HairStyle.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                    PangyaAPI.WriteConsole.WriteLine(" data\\HairStyle.iff is not loaded");
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
                    HairStyle = (IffHairStyleData)Reader.Read(new IffHairStyleData());

                    this.Add(HairStyle.Base.TypeID, HairStyle);
                }
            }
            finally
            {
                Reader.Dispose();
                Buffer.Dispose();
            }
        }
        //Destructor
        ~HairStyleCollection()
        {
            this.Clear();
        }
        public string GetItemName(UInt32 TypeID)
        {
            IffHairStyleData HairStyle = new IffHairStyleData();
            if (!LoadHairStyle(TypeID, ref HairStyle))
            {
                return "";
            }
            return HairStyle.Base.Name;
        }
        public UInt32 GetHairColor(UInt32 TypeID, CharacterTypeIdEnum CharType)
        {
            foreach (var HairStyle in this)
            {
                if (HairStyle.Key == TypeID)
                {
                    switch (CharType)
                    {
                        case CharacterTypeIdEnum.ARIN:
                            {
                                if (HairStyle.Value.CharType == CharTypeByHairColor.Arin)
                                {
                                    return HairStyle.Value.HairColor;
                                }
                            }
                            break;
                        case CharacterTypeIdEnum.NURI:
                            {
                                if (HairStyle.Value.CharType == CharTypeByHairColor.Nuri)
                                {
                                    return HairStyle.Value.HairColor;
                                }
                            }
                            break;
                        case CharacterTypeIdEnum.AZER:
                            {
                                if (HairStyle.Value.CharType == CharTypeByHairColor.Azer)
                                {
                                    return HairStyle.Value.HairColor;
                                }
                            }
                            break;
                        case CharacterTypeIdEnum.MAX:
                            {
                                if (HairStyle.Value.CharType == CharTypeByHairColor.Max)
                                {
                                    return HairStyle.Value.HairColor;
                                }
                            }
                            break;
                        case CharacterTypeIdEnum.HANA:
                            {
                                if (HairStyle.Value.CharType == CharTypeByHairColor.Hana)
                                {
                                    return HairStyle.Value.HairColor;
                                }
                            }
                            break;
                        case CharacterTypeIdEnum.CESILLIA:
                            {
                                if (HairStyle.Value.CharType == CharTypeByHairColor.Cecilia)
                                {
                                    return HairStyle.Value.HairColor;
                                }
                            }
                            break;
                        case CharacterTypeIdEnum.SPIKA:
                            {
                                if (HairStyle.Value.CharType == CharTypeByHairColor.Spika)
                                {
                                    return HairStyle.Value.HairColor;
                                }
                            }
                            break;
                        case CharacterTypeIdEnum.LUCIA:
                            {
                                if (HairStyle.Value.CharType == CharTypeByHairColor.Lucia)
                                {
                                    return HairStyle.Value.HairColor;
                                }
                            }
                            break;
                        case CharacterTypeIdEnum.NELL:
                            {
                                if (HairStyle.Value.CharType == CharTypeByHairColor.Nell)
                                {
                                    return HairStyle.Value.HairColor;
                                }
                            }
                            break;
                        case CharacterTypeIdEnum.KAZ:
                            {
                                if (HairStyle.Value.CharType == CharTypeByHairColor.Kaz)
                                {
                                    return HairStyle.Value.HairColor;
                                }
                            }
                            break;
                        case CharacterTypeIdEnum.KOOH:
                            {
                                if (HairStyle.Value.CharType == CharTypeByHairColor.Kooh)
                                {
                                    return HairStyle.Value.HairColor;
                                }
                            }
                            break;
                        case CharacterTypeIdEnum.HANA_R:
                            {
                                if (HairStyle.Value.CharType == CharTypeByHairColor.HR)
                                {
                                    return HairStyle.Value.HairColor;
                                }
                            }
                            break;
                        case CharacterTypeIdEnum.CESILLIA_R:
                            {
                                if (HairStyle.Value.CharType == CharTypeByHairColor.CR)
                                {
                                    return HairStyle.Value.HairColor;
                                }
                            }
                            break;
                        case CharacterTypeIdEnum.NURI_R:
                            {
                                if (HairStyle.Value.CharType == CharTypeByHairColor.NR)
                                {
                                    return HairStyle.Value.HairColor;
                                }
                            }
                            break;
                    }
                }
            }
            return uint.MaxValue;
        }
        public IffHairStyleData GetItem(UInt32 TypeID)
        {
            IffHairStyleData HairStyle = new IffHairStyleData();
            if (!LoadHairStyle(TypeID, ref HairStyle))
            {
                return HairStyle;
            }
            return HairStyle;
        }
        public UInt32 GetPrice(UInt32 TypeID)
        {
            IffHairStyleData HairStyle = new IffHairStyleData();
            if (!LoadHairStyle(TypeID, ref HairStyle))
            {
                return 0;
            }
            return HairStyle.Base.ItemPrice;
        }

        public sbyte GetShopPriceType(UInt32 TypeId)
        {
            IffHairStyleData HairStyle = new IffHairStyleData();
            if (!LoadHairStyle(TypeId, ref HairStyle))
            {
                return -1;
            }
            return (sbyte)HairStyle.Base.PriceType;
        }

        public bool IsBuyable(UInt32 TypeId)
        {
            IffHairStyleData Item = new IffHairStyleData();
            if (!LoadHairStyle(TypeId, ref Item))
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
            IffHairStyleData HairStyle = new IffHairStyleData();

            if (!LoadHairStyle(TypeId, ref HairStyle))
            {
                return false;
            }
            return Convert.ToBoolean(HairStyle.Base.Enabled);
        }

        public bool LoadHairStyle(UInt32 ID, ref IffHairStyleData HairStyle)
        {
            if (!this.TryGetValue(ID, out HairStyle))
            {
                return false;
            }
            return true;
        }
    }
}
