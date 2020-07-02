using PangyaFileCore.BinaryModels;
using PangyaFileCore.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;

namespace PangyaFileCore.Collections
{
    public class DescCollection : List<Desc>
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
            Desc Desc;

            PangyaBinaryReader Reader = null;

            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "Desc.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                    Console.WriteLine(" data\\Desc.iff is not loaded");
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
                    throw new NotSupportedException("The given IFF file is a ZIP file, please unpack it before attempting to parse it");
                }
                Reader.Seek(0, 0);

                Reader.ReadUInt16(out ushort recordCount);
                long recordLength = ((Reader.GetSize() - 8L) / (recordCount));
                Reader.ReadUInt16(out BindingID);
                Reader.ReadUInt32(out Version);

                for (int i = 0; i < recordCount; i++)
                {
                    Desc = new Desc();

                    Desc = (Desc)Reader.Read(new Desc());

                    Add(Desc);
                }
            }
            finally
            {
                Reader.Dispose();
                Buffer.Dispose();
            }
        }
        public Desc GetItem(uint TypeID)
        {
            return this.First(c => c.TypeID == TypeID);
        }
    }
}
