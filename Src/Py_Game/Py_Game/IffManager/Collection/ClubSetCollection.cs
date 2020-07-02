using PangyaAPI.BinaryModels;
using Py_Game.Client.Data;
using Py_Game.Defines;
using Py_Game.IffManager.Data.ClubSet;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Py_Game.IffManager.Collection
{
   public class ClubSetCollection : Dictionary<uint, IffClubSetData>
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
        public ClubSetCollection()
        {
            MemoryStream Buffer;
            IffClubSetData Club;
            PangyaBinaryReader Reader = null;

            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "ClubSet.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                    PangyaAPI.WriteConsole.WriteLine(" data\\ClubSet.iff is not loaded");
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
                    Club = (IffClubSetData)Reader.Read(new IffClubSetData());

                    this.Add(Club.Base.TypeID, Club);
                }
            }
            finally
            {
                Reader.Dispose();
                Buffer.Dispose();
            }
        }

        internal IffClubSetData GetItem(uint ID)
        {
            IffClubSetData ClubInfo = new IffClubSetData();
            if (!LoadItem(ID, ref ClubInfo))
            {
                return ClubInfo;
            }
            return ClubInfo;
        }

        public ClubStatus GetClubStatus(UInt32 ID)
        {
            ClubStatus result = new ClubStatus();
            IffClubSetData ClubInfo = new IffClubSetData();
            if (!LoadItem(ID, ref ClubInfo))
            {
                return result;
            }
            result.Power = ClubInfo.MaxPow;
            result.Control = ClubInfo.MaxCon;
            result.Impact = ClubInfo.MaxImp;
            result.Spin = ClubInfo.MaxSpin;
            result.Curve = ClubInfo.MaxCurve;
            result.ClubType = (ECLUBTYPE)ClubInfo.ClubType;
            result.ClubSPoint = (byte)ClubInfo.ClubSPoint;
            return result;
        }
        public string GetItemName(UInt32 TypeID)
        {
            IffClubSetData Club = new IffClubSetData();
            if (!LoadItem(TypeID, ref Club))
            {
                return "";
            }
            return Club.Base.Name;
        }

        public UInt32 GetPrice(UInt32 TypeID)
        {
            IffClubSetData Club = new IffClubSetData();
            if (!LoadItem(TypeID, ref Club))
            {
                return 99999999;
            }
            return Club.Base.ItemPrice;
        }



        public sbyte GetShopPriceType(UInt32 TypeId)
        {
            IffClubSetData Club = new IffClubSetData();
            if (!LoadItem(TypeId, ref Club))
            {
                return -1;
            }
            return (sbyte)Club.Base.PriceType;
        }

        public bool IsBuyable(UInt32 TypeId)
        {
            IffClubSetData Item = new IffClubSetData();
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
            IffClubSetData Club = new IffClubSetData();

            if (!LoadItem(TypeId, ref Club))
            {
                return false;
            }
            return Convert.ToBoolean(Club.Base.Enabled);
        }

        public bool LoadItem(UInt32 ID, ref IffClubSetData Club)
        {
            if (!this.TryGetValue(ID, out Club))
            {
                return false;
            }
            return true;
        }
    }
}
