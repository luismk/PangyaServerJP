using PangyaFileCore.BinaryModels;
using PangyaFileCore.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
namespace PangyaFileCore.Collections
{
    public class GrandPrixRankRewardCollection : Dictionary<uint, GrandPrixRankReward> 
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
            GrandPrixRankReward GP;

            PangyaBinaryReader Reader = null;

            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "GrandPrixRankReward.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                    throw new Exception(" data\\GrandPrixRankReward.iff is not loaded");
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
                    GP = (GrandPrixRankReward)Reader.Read(new GrandPrixRankReward());
                    if (this.Where(c => c.Key == GP.TypeID).Any() == false)
                        this.Add(GP.TypeID, GP);
                }
            }
            finally
            {
                Reader.Dispose();
            }
        }
    }
}
