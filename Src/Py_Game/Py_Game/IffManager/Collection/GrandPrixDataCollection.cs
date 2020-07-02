using PangyaAPI.BinaryModels;
using Py_Game.IffManager.Data.GrandPrixData;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
namespace Py_Game.IffManager.Collection
{
   public class GrandPrixDataCollection
    {
        Dictionary<uint, IffGrandPrixData> FGPData = null;
        public Dictionary<uint, IffGrandPrixData> GP { get { return FGPData; } }
        /// <summary>
        /// ID determining relation to other IFF files
        /// </summary>
        public ushort BindingID;

        /// <summary>
        /// Version of this IFF file
        /// </summary>
        public uint Version;

        //Constructor 
        public GrandPrixDataCollection()
        {
            MemoryStream Buffer;
            IffGrandPrixData GrandPrix;
            FGPData = new Dictionary<uint, IffGrandPrixData>();

            PangyaBinaryReader Reader = null;

            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "GrandPrixData.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                    PangyaAPI.WriteConsole.WriteLine(" data\\GrandPrixData.iff is not loaded");
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
                    GrandPrix = new IffGrandPrixData();

                    GrandPrix = (IffGrandPrixData)Reader.Read(GrandPrix);


                    FGPData.Add(GrandPrix.TypeID, GrandPrix);
                }
            }
            finally
            {
                Reader.Dispose();
                Buffer.Dispose();
            }
        }
       

        public IffGrandPrixData GetGP(UInt32 TypeId)
        {
            IffGrandPrixData GP = new IffGrandPrixData();
            if (!LoadGP(TypeId, ref GP))
            {
                return GP;
            }
            return GP;
        }

        public bool IsGPExist(UInt32 TypeId)
        {
            IffGrandPrixData GP = new IffGrandPrixData();
            if (!LoadGP(TypeId, ref GP))
            {
                return false;
            }
            return true;
        }
        public bool LoadGP(UInt32 ID, ref IffGrandPrixData GrandPrix)
        {
            if (!FGPData.TryGetValue(ID, out GrandPrix))
            {
                return false;
            }
            return true;
        }
    }
}
