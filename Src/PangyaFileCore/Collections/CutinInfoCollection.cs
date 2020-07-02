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
    public class CutinInfoCollection : Dictionary<uint, CutinInfo> 
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
            CutinInfo CutinInfo;
            PangyaBinaryReader Reader = null;

            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "CutinInfomation.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                     throw new Exception(" data\\CutinInfomation.iff is not loaded");
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
                    CutinInfo = (CutinInfo)Reader.Read(new CutinInfo());

                    Add(CutinInfo.TypeID, CutinInfo);
                }
            }
            finally
            {
                Reader.Dispose();
                Buffer.Dispose();
            }

        }

        public byte[] GetCutinString(UInt32 TypeID)
        {
            CutinInfo Cutin = new CutinInfo();
            PangyaBinaryWriter Result;
            if (!LoadCutin(TypeID, ref Cutin))
            {
                return new byte[] { 0x8D, 0x01, 0x00 };
            }
            Result = new PangyaBinaryWriter();
            try
            {
                Result.Write(new byte[] { 0x8D, 0x01 });
                Result.WriteByte(1);
                Result.WriteUInt32(Cutin.TypeID);
                Result.WriteUInt32(Cutin.Num1);
                Result.WriteUInt32(Cutin.Num2);
                Result.WriteUInt32(Cutin.NumImg2);
                Result.WriteUInt32(Cutin.NumImg3);
                Result.WriteUInt32(Cutin.Time);
                Result.WriteUInt32(0);
                Result.WriteUInt32(Cutin.Num4);
                Result.WriteStr(Cutin.IMG1, 40);
                Result.WriteStr(Cutin.IMG2, 40);
                Result.WriteStr(Cutin.IMG3, 40);
                Result.Write(Cutin.UN, 40);
                return Result.GetBytes();
            }
            finally
            {
                Result.Dispose();
            }
        }

        public bool LoadCutin(UInt32 ID, ref CutinInfo CutinInfo)
        {
            if (!TryGetValue(ID, out CutinInfo))
            {
                return false;
            }
            return true;
        }
    }
}
