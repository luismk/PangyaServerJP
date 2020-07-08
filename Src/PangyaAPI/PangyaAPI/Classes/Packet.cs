using PangyaAPI.BinaryModels;
using Py_Connector.DataBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static PangyaAPI.Crypts.Cryptor;
namespace PangyaAPI
{
    public class Packet : IDisposable
    {
        #region Private Fields
        private readonly MemoryStream _stream;
        /// <summary>
        /// Leitor do packet
        /// </summary>
        private PangyaBinaryReader Reader;

        private PangyaBinaryWriter Reply = new PangyaBinaryWriter();

        /// <summary>
        /// Mensagem do Packet
        /// </summary>
        public byte[] Message { get; set; }

        private byte[] MessageCrypted { get; set; }
        #endregion

        #region Public Fields
        /// <summary>
        /// Id do Packet
        /// </summary>
        public short Id { get; set; }
        #endregion

        #region Constructor

        public Packet(byte[] message, byte key)
        {
            Id = BitConverter.ToInt16(new byte[] { message[5], message[6] }, 0);

            MessageCrypted = new byte[message.Length];
            Buffer.BlockCopy(message, 0, MessageCrypted, 0, message.Length); //Copia mensagem recebida criptografada

            Message = message.ClientDecrypt(key);

            _stream = new MemoryStream(Message);

            _stream.Seek(2, SeekOrigin.Current); //Seek Inicial
            Reader = new PangyaBinaryReader(_stream);
        }


        #region Methods Get
        public uint GetSize
        {
            get => Reader.GetSize();
        }
        public uint GetPos
        {
            get => Reader.GetPosition();
        }

        public double ReadDouble()
        {
            return Reader.ReadDouble();
        }

        public byte ReadByte()
        {
            return Reader.ReadByte();
        }
        public short ReadInt16()
        {
            return Reader.ReadInt16();
        }
        public ushort ReadUInt16()
        {
            return Reader.ReadUInt16();
        }

        

        public uint ReadUInt32()
        {
            return Reader.ReadUInt32();
        }
        public int ReadInt32()
        {
            return Reader.ReadInt32();
        }

        public ulong ReadUInt64()
        {
            return Reader.ReadUInt64();
        }

        public long ReadInt64()
        {
            return Reader.ReadInt64();
        }

        public float ReadSingle()
        {
            return Reader.ReadSingle();
        }

        public string ReadPStr()
        {
            return Reader.ReadPStr();
        }
        public void Skip(int count)
        {
            Reader.Skip(count);
        }


        public void Seek(int offset, int origin)
        {
            Reader.Seek(offset, origin);
        }

        public T Read<T>() where T : struct
        {
            return Reader.Read<T>();
        }
        public IEnumerable<uint> Read(uint count)
        {
            return Reader.Read(count);
        }
        public object Read(object value, int Count)
        {
            return Reader.Read(value, Count);
        }

        public object Read(object value)
        {
            return Reader.Read(value);
        }

       

        public string ReadPStr(uint Count)
        {
            var data = new byte[Count];
            //ler os dados
            Reader.BaseStream.Read(data, 0, (int)Count);
            var value = Encoding.ASCII.GetString(data);
            return value;
        }

        public bool ReadPStr(out string value, uint Count)
        {
            return Reader.ReadPStr(out value, Count);
        }
        public bool ReadPStr(out string value)
        {
            return Reader.ReadPStr(out value);
        }
        public bool ReadDouble(out Double value)
        {
            return Reader.ReadDouble(out value);
        }
        public bool ReadBytes(out byte[] value)
        {
            return Reader.ReadBytes(out value);
        }
        public bool ReadByte(out byte value)
        {
            return Reader.ReadByte(out value);
        }
        public bool ReadInt16(out short value)
        {
            return Reader.ReadInt16(out value);
        }
        public bool ReadUInt16(out ushort value)
        {
            return Reader.ReadUInt16(out value);
        }

        public bool ReadUInt32(out uint value)
        {
            return Reader.ReadUInt32(out value);
        }

        public bool ReadInt32(out int value)
        {
            return Reader.ReadInt32(out value);
        }

        public bool ReadUInt64(out ulong value)
        {
            return Reader.ReadUInt64(out value);
        }

        public bool ReadInt64(out long value)
        {
            return Reader.ReadInt64(out value);
        }

        public bool ReadSingle(out float value)
        {
            return Reader.ReadSingle(out value);
        }


        public byte[] GetRemainingData
        {
            get => Reader.GetRemainingData();
        }
        public byte[] ReadBytes(int count)
        {
            return Reader.ReadBytes(count);
        }

        public string GetLog()
        {
            StringBuilder builder = new StringBuilder(Message.Length * 2);
            foreach (byte num2 in Message)
            {
                builder.AppendFormat("{0:X2} ", num2);
            }
            return $"[PACKET_LOG]: {builder}";
        }

        public void Log()
        {
            StringBuilder builder = new StringBuilder(Message.Length * 2);
            foreach (byte num2 in Message)
            {
                builder.AppendFormat("{0:X2} ", num2);
            }
            WriteConsole.WriteLine($"[PACKET_LOG]: {builder}", ConsoleColor.Cyan);
        }
        public void Save()
        {
            File.WriteAllBytes($"savepacket\\Packet{new Random().Next(9999)}.Hex", Message);
        }
        public void Save(string name)
        {
            File.WriteAllBytes($"savepacket\\{name}{new Random().Next(9999)}.Hex", Message);
        }

        public void SetReader(PangyaBinaryReader read)
        {
            Reader = read;
        }

        #endregion

        #region Methods Writer

        public void Write(byte[] data)
        {
            try
            {
                Reply.Write(data);
            }
            catch
            {
            }
            return;
        }

        public void WriteStruct(object data)
        {
            try
            {
                Reply.WriteStruct(data);
            }
            catch
            {
            }
            return;
        }


        public void WriteStr(string message, int length)
        {

            try
            {
                if (message == null)
                {
                    message = string.Empty;
                }

                message = message.PadRight(length, (char)0x00);
                Reply.Write(message.Select(Convert.ToByte).ToArray());
            }
            catch
            {
            }
            return;
        }

        public bool WriteStr(string message)
        {
            try
            {
                WriteStr(message, message.Length);
            }
            catch
            {
                return false;
            }
            return true;

        }

        public void WritePStr(string value)
        {

            try
            {
                Reply.WritePStr(value);

            }
            catch
            {
                return;
            }
        }

        public void WriteZero(int count)
        {
            try
            {
                Reply.WriteZero(count);
            }
            catch
            {

            }

        }
        public void WriteUInt16(ushort value)
        {
            try
            {
                Reply.Write(value);
            }
            catch
            {

            }

        }

        public void WriteInt16(short value)
        {
            try
            {
                Reply.Write(value);
            }
            catch
            {

            }

        }
        public void WriteByte(byte value)
        {
            try
            {
                Reply.Write(value);
            }
            catch
            {

            }

        }

        public void WriteSingle(float value)
        {
            try
            {
                Reply.Write(value);
            }
            catch
            {

            }

        }

        public void WriteUInt32(uint value)
        {
            try
            {
                Reply.Write(value);
            }
            catch
            {

            }

        }

        public void WriteInt32(int value)
        {
            try
            {
                Reply.Write(value);
            }
            catch
            {

            }

        }

        public void WriteUInt64(ulong value)
        {
            try
            {
                Reply.Write(value);
            }
            catch
            {

            }

        }

        public void WriteInt64(long value)
        {
            try
            {
                Reply.Write(value);
            }
            catch
            {

            }

        }

        public void WriteDouble(double value)
        {
            try
            {
                Reply.Write(value);
            }
            catch
            {

            }

        }
        public byte[] GetBytes()
        {
            return Reply.GetBytes();
        }

        public void Clear()
        {
            Reply = new PangyaBinaryWriter();
        }

        #region IDisposable Support
        private bool disposedValue = false; // Para detectar chamadas redundantes

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Reader != null)
                    {
                        Reader.Dispose();
                    }
                    else if (Reply != null)
                    {
                        Reply.Dispose();
                    }
                }
                disposedValue = true;
            }
        }

        ~Packet()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
        #endregion
        #endregion
    }
}