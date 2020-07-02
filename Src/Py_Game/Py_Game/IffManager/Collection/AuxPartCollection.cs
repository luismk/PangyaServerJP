using PangyaAPI.BinaryModels;
using Py_Game.IffManager.Data.AuxPart;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Py_Game.IffManager.Collection
{
    public class AuxPartCollection : Dictionary<UInt32, IffAuxPartData>
    {
        /// <summary>
        /// ID determining relation to other IFF files
        /// </summary>
        public ushort BindingID;

        /// <summary>
        /// Version of this IFF file
        /// </summary>
        public uint Version;
        public AuxPartCollection()
        {
            MemoryStream Buffer;
            PangyaBinaryReader Reader = null;
            IffAuxPartData Aux;
            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "AuxPart.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                    PangyaAPI.WriteConsole.WriteLine(" data\\AuxPart.iff is not loaded");
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
                    Aux = (IffAuxPartData)Reader.Read(new IffAuxPartData());

                    this.Add(Aux.Base.TypeID, Aux);
                }
            }
            finally
            {
                Reader.Dispose();
                Buffer.Dispose();
            }
        }
        

        public string GetItemName(UInt32 ID)
        {
            IffAuxPartData Aux = new IffAuxPartData();
            if (!LoadAux(ID, ref Aux))
            {
                return "";
            }
            return Aux.Base.Name;
        }

        public bool IsExist(UInt32 ID)
        {
            IffAuxPartData Item = new IffAuxPartData();
            if (!LoadAux(ID, ref Item))
            {
                return false;
            }
            if (Item.Base.Enabled == 1)
            {
                return true;
            }
            return false;
        }

        public bool LoadAux(UInt32 ID, ref IffAuxPartData Aux)
        {
            if (!this.TryGetValue(ID, out Aux))
            {
                return false;
            }
            return true;
        }
    }
}
