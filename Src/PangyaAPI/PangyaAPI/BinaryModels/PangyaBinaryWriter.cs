using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
namespace PangyaAPI.BinaryModels
{
    public class PangyaBinaryWriter : BinaryWriter
    {
        public PangyaBinaryWriter(Stream output) { }
        public PangyaBinaryWriter(Stream output, Encoding encoding) : base(output, encoding)
        {
        }

        public PangyaBinaryWriter(Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen)
        {
        }
        public PangyaBinaryWriter()
        {
            this.OutStream = new MemoryStream();
        }

        public uint GetSize
        {
            get { return (uint)BaseStream.Length; }
        }
        /// <summary>
        /// GetBytes Written in Binary
        /// </summary>
        /// <returns>Array Of Bytes</returns>
        public byte[] GetBytes()
        {
            if (OutStream is MemoryStream)
                return ((MemoryStream)OutStream).ToArray();


            using (var memoryStream = new MemoryStream())
            {
                memoryStream.GetBuffer();
                OutStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public void Clear()
        {
            this.Flush();
            this.Close();
            this.OutStream = new MemoryStream();
        }


        public bool WriteStr(string message, int length)
        {

            try
            {
                if (message == null)
                {
                    message = string.Empty;
                }

                var ret = new byte[length];
                Encoding.GetEncoding("Shift_JIS").GetBytes(message).Take(length).ToArray().CopyTo(ret, 0);

                Write(ret);
            }
            catch
            {
                return false;
            }
            return true;
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

        public bool WritePStr(string data)
        {
            if (data == null) data = "";
            try
            {
                var encoded = Encoding.GetEncoding("Shift_JIS").GetBytes(data);
                var length = encoded.Length;
                if (length >= ushort.MaxValue)
                {
                    return false;
                }
                Write((short)length);
                Write(encoded);
            }
            catch
            {
                return false;
            }
            return true;
        }




        public bool Write(byte[] message, int length)
        {
            try
            {
                if (message == null)
                    message = new byte[length];

                var result = new byte[length];

                Buffer.BlockCopy(message, 0, result, 0, message.Length);

                Write(result);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool WriteZero(int Lenght)
        {
            try
            {
                Write(new byte[Lenght]);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool WriteUInt16(ushort value)
        {
            try
            {
                Write(value);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool WriteUInt16(int value)
        {
            try
            {
                Write((ushort)value);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool WriteUInt16(uint value)
        {
            try
            {
                Write((ushort)value);
            }
            catch
            {
                return false;
            }
            return true;
        }


        public bool WriteByte(byte value)
        {
            try
            {
                Write(value);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool WriteByte(int value)
        {
            try
            {
                Write(Convert.ToByte(value));
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool WriteSingle(float value)
        {
            try
            {
                Write(value);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool WriteUInt32(uint value)
        {
            try
            {
                Write(value);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool WriteInt32(int value)
        {
            try
            {
                Write(value);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool WriteUInt64(ulong value)
        {
            try
            {
                Write(value);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool WriteInt64(long value)
        {
            try
            {
                Write(value);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool WriteDouble(double value)
        {
            try
            {
                Write(value);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool WriteStruct(object value)
        {
            try
            {
                int size = Marshal.SizeOf(value);
                byte[] arr = new byte[size];

                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(value, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
                Marshal.FreeHGlobal(ptr);
                Write(arr);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool WriteHexArray(string _value)
        {
            try
            {
                _value = _value.Replace(" ", "");
                int _size = _value.Length / 2;
                byte[] _result = new byte[_size];
                for (int ii = 0; ii < _size; ii++)
                    WriteByte(Convert.ToByte(_value.Substring(ii * 2, 2), 16));
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void SaveWrite(string name)
        {
            if (Directory.Exists("savepacket") == false)
            {
                Directory.CreateDirectory("savepacket");
            }
            File.WriteAllBytes($"savepacket\\{name}{"-"}{new Random().Next()}", GetBytes());
        }
    }
}
