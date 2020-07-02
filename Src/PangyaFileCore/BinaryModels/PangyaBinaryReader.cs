using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
namespace PangyaFileCore.BinaryModels
{
    public class PangyaBinaryReader : BinaryReader
    {
        public PangyaBinaryReader(Stream baseStream) : base(baseStream) { }

        public PangyaBinaryReader(Stream input, Encoding encoding) : base(input, encoding) { }

        public PangyaBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen) { }
       
        public void Skip(int count)
        {
            Seek(count, 1);
        }

        public void Seek(int offset, int origin)
        {
            BaseStream.Seek(offset, (SeekOrigin)origin);
        }

        public uint GetSize()
        {
            return (uint)BaseStream.Length;
        }

        public byte[] GetRemainingData(int Count)
        {
            int previousOffset;
            previousOffset = (int)BaseStream.Position;
            var array = ReadBytes(Count);
            BaseStream.Position = previousOffset;
            return array;
        }
        public byte[] GetRemaining()
        {
            int previousOffset;
            previousOffset = (int)BaseStream.Position;
            var array = ReadBytes((int)GetSize());
            BaseStream.Position = previousOffset;
            return array;
        }

        public bool ReadPStr(out string value, uint Count)
        {
            try
            {
                var data = new byte[Count];
                //ler os dados
                BaseStream.Read(data, 0, (int)Count);

                value = Encoding.ASCII.GetString(data);
            }
            catch
            {
                value = null;
                return false;
            }
            return true;
        }
        public bool ReadPStr(out string value)
        {
            try
            {
                var size = ReadUInt16();
                value = Encoding.ASCII.GetString(ReadBytes(size));
            }
            catch
            {
                value = null;
                return false;
            }
            return true;
        }

        public string ReadPStr()
        {
            try
            {
                var size = ReadUInt16();
                return Encoding.ASCII.GetString(ReadBytes(size));
            }
            catch
            {
                return "";
            }
        }
        public string ReadPStr(uint Count)
        {
            try
            {
                var data = new byte[Count];
                //ler os dados
                BaseStream.Read(data, 0, (int)Count);

                return Encoding.ASCII.GetString(data).Replace("\0", "");
            }
            catch
            {
                return "";
            }
        }
        public uint GetPosition()
        {
            return (uint)BaseStream.Position;
        }

        public bool ReadDouble(out Double value)
        {
            try
            {
                value = ReadDouble();
            }
            catch
            {
                value = 0;
                return false;
            }
            return true;
        }

        public bool ReadByte(out byte value)
        {
            try
            {
                value = ReadByte();
            }
            catch
            {
                value = 0;
                return false;
            }
            return true;
        }
        public bool ReadInt16(out short value)
        {
            try
            {
                value = ReadInt16();
            }
            catch
            {
                value = 0;
                return false;
            }
            return true;
        }

      
        public bool ReadBytes(out byte[] value)
        {
            try
            {
                int size = ReadInt16();

                if (ushort.MaxValue < size)
                {
                    value = new byte[0];
                    return false;
                }
                value = ReadBytes(size);
            }
            catch
            {
                value = new byte[0];
                return false;
            }
            return true;
        }
        public bool ReadUInt16(out ushort value)
        {
            try
            {
                value = ReadUInt16();
            }
            catch
            {
                value = 0;
                return false;
            }
            return true;
        }

        public bool ReadUInt32(out uint value)
        {
            try
            {
                value = ReadUInt32();
            }
            catch
            {
                value = 0;
                return false;
            }
            return true;
        }

        public bool ReadInt32(out int value)
        {
            try
            {
                value = ReadInt32();
            }
            catch
            {
                value = 0;
                return false;
            }
            return true;
        }

        public bool ReadUInt64(out ulong value)
        {
            try
            {
                value = ReadUInt64();
            }
            catch
            {
                value = 0;
                return false;
            }
            return true;
        }

        public bool ReadInt64(out long value)
        {
            try
            {
                value = ReadInt64();
            }
            catch
            {
                value = 0;
                return false;
            }
            return true;
        }

        public bool ReadSingle(out float value)
        {
            try
            {
                value = ReadSingle();
            }
            catch
            {
                value = 0;
                return false;
            }
            return true;
        }

        public DateTime ReadDateTime()
        {
            DateTime result;
            try
            {
                var Year = ReadUInt16();
                var Month = ReadUInt16();
                var DayOfWeek = ReadUInt16();
                var Day = ReadUInt16();
                var Hour = ReadUInt16();
                var Minute = ReadUInt16();
                var Second = ReadUInt16();
                var Millisecond = ReadUInt16();

                result = new DateTime(Year, Month, Day, Hour, Minute, Second, Millisecond);
                return result;
            }
            catch
            {
                result = new DateTime();
                return result;
            }
        }

        public IEnumerable<uint> Read(uint count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return ReadUInt32();
            }
        }
        //não testado
        public bool Read(out object value, int Count)
        {
            try
            {
                var obj = new object();
                byte[] BytesRead = ReadBytes(Count);

                IntPtr ptr = Marshal.AllocHGlobal(Count);

                Marshal.Copy(BytesRead, 0, ptr, Count);

                value = Marshal.PtrToStructure(ptr, obj.GetType());
                Marshal.FreeHGlobal(ptr);
            }
            catch
            {
                value = 0;
                return false;
            }
            return true;
        }

        public T ReadStruct<T>()
        {
            var byteLength = Marshal.SizeOf(typeof(T));
            var bytes = ReadBytes(byteLength);
            var pinned = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var stt = (T)Marshal.PtrToStructure(
                pinned.AddrOfPinnedObject(),
                typeof(T));
            pinned.Free();
            return stt;
        }
        public T Read<T>() where T : struct
        {
            T local;
            int count = (typeof(T) == typeof(bool)) ? 1 : Marshal.SizeOf(typeof(T));
            GCHandle handle = GCHandle.Alloc(this.ReadBytes(count), GCHandleType.Pinned);
            try
            {
                local = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
            return local;
        }

        public object Read(object value)
        {
            var Count = Marshal.SizeOf(value);

            byte[] BytesRead = ReadBytes(Count);

            if (BytesRead.Length != Count)
            {
                throw new Exception(
                    $"The record length ({BytesRead.Length}) mismatches the length of the passed structure ({Count})");
            }

            IntPtr ptr = Marshal.AllocHGlobal(Count);

            Marshal.Copy(BytesRead, 0, ptr, Count);

            value = Marshal.PtrToStructure(ptr, value.GetType());
            Marshal.FreeHGlobal(ptr);
            return value;
        }

        public object Read(object value, int Count)
        {
            byte[] BytesRead = ReadBytes(Count);

            IntPtr ptr = Marshal.AllocHGlobal(Count);

            Marshal.Copy(BytesRead, 0, ptr, Count);

            value = Marshal.PtrToStructure(ptr, value.GetType());
            Marshal.FreeHGlobal(ptr);
            return value;
        }

        public void ReadObject(out object obj)
        {
            obj = new object();
            foreach (var property in obj.GetType().GetProperties())
            {
                Type type = property.PropertyType;

                TypeCode typeCode = Type.GetTypeCode(type);
                switch (typeCode)
                {
                    case TypeCode.Empty:
                        break;
                    case TypeCode.Object:
                        break;
                    case TypeCode.DBNull:
                        break;
                    case TypeCode.Boolean:
                        {
                            property.SetValue(obj, ReadBoolean());
                        }
                        break;
                    case TypeCode.Char:
                        {
                            property.SetValue(obj, ReadChar());
                        }
                        break;
                    case TypeCode.SByte:
                        {
                            property.SetValue(obj, ReadSByte());
                        }
                        break;

                    case TypeCode.Byte:
                        {
                            property.SetValue(obj, ReadByte());
                        }
                        break;
                    case TypeCode.Int16:
                        {
                            property.SetValue(obj, ReadInt16());
                        }
                        break;
                    case TypeCode.UInt16:
                        {
                            property.SetValue(obj, ReadUInt16());
                        }
                        break;
                    case TypeCode.Int32:
                        {
                            property.SetValue(obj, ReadInt32());
                        }
                        break;
                    case TypeCode.UInt32:
                        property.SetValue(obj, ReadUInt32());
                        break;
                    case TypeCode.Int64:
                        {
                            property.SetValue(obj, ReadInt64());
                        }
                        break;
                    case TypeCode.UInt64:
                        {
                            property.SetValue(obj, ReadUInt64());
                        }
                        break;
                    case TypeCode.Single:
                        {
                            property.SetValue(obj, ReadSingle());
                        }
                        break;
                    case TypeCode.Double:
                        {
                            property.SetValue(obj, ReadDouble());
                        }
                        break;
                    case TypeCode.Decimal:
                        {
                            property.SetValue(obj, ReadDecimal());
                        }
                        break;
                    case TypeCode.DateTime:
                        {
                           property.SetValue(obj, ReadDateTime());
                        }
                        break;
                    case TypeCode.String:
                        {
                            property.SetValue(obj, ReadPStr());
                        }
                        break;
                    default:
                        {
                            Console.WriteLine("Object Type Name: " + typeCode);
                        }
                        break;
                }
            }
           // return obj;
        }       
    }
}
