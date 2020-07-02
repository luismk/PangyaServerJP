using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
namespace PangyaAPI.Tools
{
    public class IniFile : IDisposable
    {
        [DllImport("kernel32")]
        static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// Local do arquivo
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="_filename">nome do arquivo</param>
        public IniFile(string _filename)
        {
            try
            {
                //caso o arquivo não existir, é lançado uma exceção
                if (File.Exists(_filename) == false)
                {
                    throw new Exception($"File no Exist: {_filename}");
                }
                else
                {
                    //caso o arquivo existir, é adicionado um local + nome do arquivo
                    FilePath = AppDomain.CurrentDomain.BaseDirectory + _filename;
                }

            }
            //caso caia no exception
            catch (Exception ex)
            {
                WriteConsole.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// Destrutor
        /// </summary>
        ~IniFile()
        {
            Dispose(false);
        }
        /// <summary>
        /// Cria o arquivo .ini
        /// </summary>
        /// <param name="section">Seção = cabeçario [Config]</param>
        /// <param name="key">Local = nomedealgo</param>
        /// <param name="value">valor</param>
        /// <returns>string</returns>
        public void Write(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value.ToLower(), FilePath);
        }
        /// <summary>
        /// Ler o arquivo .ini e retorna string
        /// </summary>
        /// <param name="section">Seção = cabeçario [Config]</param>
        /// <param name="key">Local = nomedealgo = 0 </param>
        /// <param name="def">padrao = caso não encontre o valor no key, retorna o def</param>
        /// <returns>string</returns>
        public string Read(string section, string key, object def)
        {
            StringBuilder BuildStr = new StringBuilder(255);
            GetPrivateProfileString(section, key, def.ToString(), BuildStr, 255, FilePath);
            return BuildStr.ToString();
        }
        /// <summary>
        /// Ler o arquivo .ini e retorna string
        /// </summary>
        /// <param name="section">Seção = cabeçario [Config]</param>
        /// <param name="key">Local = nomedealgo = 0 </param>
        /// <param name="def">padrao = caso não encontre o valor no key, retorna o def</param>
        /// <returns>string</returns>
        public string ReadString(string section, string key, string def)
        {
            StringBuilder SB = new StringBuilder(255);
            GetPrivateProfileString(section, key, def, SB, 255, this.FilePath);
            return Convert.ToString(SB.ToString());
        }
        /// <summary>
        /// Ler o arquivo .ini e retorna Char
        /// </summary>
        /// <param name="section">Seção = cabeçario [Config]</param>
        /// <param name="key">Local = nomedealgo = 0 </param>
        /// <param name="def">padrao = caso não encontre o valor no key, retorna o def</param>
        /// <returns>string</returns>
        public Char ReadChar(string section, string key, char def)
        {
            StringBuilder SB = new StringBuilder(255);
            GetPrivateProfileString(section, key, def.ToString(), SB, 255, this.FilePath);
            return Convert.ToChar(SB.ToString());
        }
        /// <summary>
        /// Ler o arquivo .ini e retorna Int32
        /// </summary>
        /// <param name="section">Seção = cabeçario [Config]</param>
        /// <param name="key">Local = nomedealgo = 0 </param>
        /// <param name="def">padrao = caso não encontre o valor no key, retorna o def</param>
        /// <returns>string</returns>
        public Int32 ReadInt32(string section, string key, int def)
        {
            StringBuilder SB = new StringBuilder(255);
            GetPrivateProfileString(section, key, def.ToString(), SB, 255, this.FilePath);
            return Convert.ToInt32(SB.ToString());
        }
        /// <summary>
        /// Ler o arquivo .ini e retorna UInt32
        /// </summary>
        /// <param name="section">Seção = cabeçario [Config]</param>
        /// <param name="key">Local = nomedealgo = 0 </param>
        /// <param name="def">padrao = caso não encontre o valor no key, retorna o def</param>
        /// <returns>string</returns>
        public UInt32 ReadUInt32(string section, string key, uint def)
        {
            StringBuilder SB = new StringBuilder(255);
            GetPrivateProfileString(section, key, def.ToString(), SB, 255, this.FilePath);
            return Convert.ToUInt32(SB.ToString());
        }
        /// <summary>
        /// Ler o arquivo .ini e retorna Int64
        /// </summary>
        /// <param name="section">Seção = cabeçario [Config]</param>
        /// <param name="key">Local = nomedealgo = 0 </param>
        /// <param name="def">padrao = caso não encontre o valor no key, retorna o def</param>
        /// <returns>string</returns>
        public Int64 ReadInt64(string section, string key, long def)
        {
            StringBuilder SB = new StringBuilder(255);
            GetPrivateProfileString(section, key, def.ToString(), SB, 255, this.FilePath);
            return Convert.ToInt64(SB.ToString());
        }
        /// <summary>
        /// Ler o arquivo .ini e retorna UInt64
        /// </summary>
        /// <param name="section">Seção = cabeçario [Config]</param>
        /// <param name="key">Local = nomedealgo = 0 </param>
        /// <param name="def">padrao = caso não encontre o valor no key, retorna o def</param>
        /// <returns>string</returns>
        public UInt64 ReadUInt64(string section, string key, ulong def)
        {
            StringBuilder SB = new StringBuilder(255);
            GetPrivateProfileString(section, key, def.ToString(), SB, 255, this.FilePath);
            return Convert.ToUInt64(SB.ToString());
        }
        /// <summary>
        /// Ler o arquivo .ini e retorna Int16
        /// </summary>
        /// <param name="section">Seção = cabeçario [Config]</param>
        /// <param name="key">Local = nomedealgo = 0 </param>
        /// <param name="def">padrao = caso não encontre o valor no key, retorna o def</param>
        /// <returns>string</returns>
        public Int16 ReadInt16(string section, string key, short def)
        {
            StringBuilder SB = new StringBuilder(255);
            GetPrivateProfileString(section, key, def.ToString(), SB, 255, this.FilePath);
            return Convert.ToInt16(SB.ToString());
        }
        /// <summary>
        /// Ler o arquivo .ini e retorna UInt16
        /// </summary>
        /// <param name="section">Seção = cabeçario [Config]</param>
        /// <param name="key">Local = nomedealgo = 0 </param>
        /// <param name="def">padrao = caso não encontre o valor no key, retorna o def</param>
        /// <returns>string</returns>
        public UInt16 ReadUInt16(string section, string key, ushort def)
        {
            StringBuilder SB = new StringBuilder(255);
            GetPrivateProfileString(section, key, def.ToString(), SB, 255, this.FilePath);
            return Convert.ToUInt16(SB.ToString());
        }
        /// <summary>
        /// Ler o arquivo .ini e retorna Byte
        /// </summary>
        /// <param name="section">Seção = cabeçario [Config]</param>
        /// <param name="key">Local = nomedealgo = 0 </param>
        /// <param name="def">padrao = caso não encontre o valor no key, retorna o def</param>
        /// <returns>string</returns>
        public Byte ReadByte(string section, string key, Byte def)
        {
            StringBuilder SB = new StringBuilder(255);
            GetPrivateProfileString(section, key, def.ToString(), SB, 255, this.FilePath);
            return Convert.ToByte(SB.ToString());
        }
        /// <summary>
        /// Ler o arquivo .ini e retorna SByte
        /// </summary>
        /// <param name="section">Seção = cabeçario [Config]</param>
        /// <param name="key">Local = nomedealgo = 0 </param>
        /// <param name="def">padrao = caso não encontre o valor no key, retorna o def</param>
        /// <returns>string</returns>
        public SByte ReadSByte(string section, string key, sbyte def)
        {
            StringBuilder SB = new StringBuilder(255);
            GetPrivateProfileString(section, key, def.ToString(), SB, 255, this.FilePath);
            return Convert.ToSByte(SB.ToString());
        }

        /// <summary>
        /// Ler o arquivo .ini e retorna bool
        /// </summary>
        /// <param name="section">Seção = cabeçario [Config]</param>
        /// <param name="key">Local = nomedealgo = 0 </param>
        /// <param name="def">padrao = caso não encontre o valor no key, retorna o def</param>
        /// <returns>string</returns>
        public bool ReadBool(string section, string key, bool def)
        {
            StringBuilder SB = new StringBuilder(255);
            GetPrivateProfileString(section, key, def.ToString(), SB, 255, this.FilePath);
            return Convert.ToBoolean(SB.ToString());
        }

        #region IDisposable Support
        private bool disposedValue = false; // Para detectar chamadas redundantes

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}