using System;
using System.Security.Cryptography;
namespace Py_Game.GameTools
{
    /// <summary>
    /// Class Random Number Generator
    /// </summary>
    public class MathRand
    {
        public static MathRand Rand = new MathRand();
        const int RtsRNGLOWER_MASK = 0x7FFFFFFF;       
        int BufferSize = 1024;
        byte[] _randomBuffer;
        int _bufferOffset = 1024;
        readonly RNGCryptoServiceProvider _rng;       
        /// <summary>
        /// Construtor
        /// </summary>
        public MathRand()
        {
            _randomBuffer = new byte[BufferSize];
            _rng = new RNGCryptoServiceProvider();
        }
        /// <summary>
        /// Construtor + Launcher
        /// </summary>
        /// <param name="Seed">Program</param>
        public MathRand(int Seed)
        {
            BufferSize = Seed;
            _randomBuffer = new byte[BufferSize];
            _rng = new RNGCryptoServiceProvider();
            _bufferOffset = _randomBuffer.Length;
        }
        /// <summary>
        /// Random Number Generator
        /// </summary>
        public void Generate()
        {
            GetBytes(_randomBuffer);
            _bufferOffset = 0;
        }
        public byte NextByte()
        {
            var data = new byte[sizeof(byte)];
            _rng.GetBytes(data);


            return (byte)(Convert.ToByte(data[0]) & (byte.MaxValue - 1));
        }
        public byte NextByte(int maxValue)
        {
            return NextByte(0, maxValue);
        }
        public byte NextByte(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                Next(new Random().Next(minValue), minValue);
            }
            return (byte)Math.Floor((minValue + ((double)maxValue - minValue) * NextDouble()));
        }
        public int Next()
        {
            var data = new byte[sizeof(int)];
            _rng.GetBytes(data);
            return BitConverter.ToInt32(data, 0) & (int.MaxValue - 1);
        }

        public int Next(int maxValue)
        {
            return Next(0, maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                Next(new Random().Next(minValue),minValue);
            }
            return (int)Math.Floor((minValue + ((double)maxValue - minValue) * NextDouble()));
        }

        public UInt32 NextUInt()
        {
            byte[] data = new byte[4];
            Int32[] result = new Int32[1];

            do
            {
                _rng.GetBytes(data);
                Buffer.BlockCopy(data, 0, result, 0, 4);
            } while (result[0] < 0);

            return (UInt32)result[0];
        }
        public UInt32 NextUInt(uint MaxValue)
        {
            uint result = 0;

            do
            {
                result = (uint)Next();
            } while (result > MaxValue);

            return result;
        }
        public Int64 NextLong()
        {
            byte[] data = new byte[8];
            Int64[] result = new Int64[1];

            _rng.GetBytes(data);
            Buffer.BlockCopy(data, 0, result, 0, 8);

            return result[0];
        }

        public Int64 NextLong(Int64 MaxValue)
        {
            Int64 result = 0;

            do
            {
                result = NextLong();
            } while (result > MaxValue);

            return result;
        }

        public UInt64 NextULong()
        {
            byte[] data = new byte[8];
            Int64[] result = new Int64[1];

            do
            {
                _rng.GetBytes(data);
                Buffer.BlockCopy(data, 0, result, 0, 8);
            } while (result[0] < 0);

            return (UInt64)result[0];
        }
        
        public UInt64 NextULong(UInt64 MaxValue)
        {
            UInt64 result = 0;

            do
            {
                result = NextULong();
            } while (result > MaxValue);

            return result;
        }
        public double NextDouble()
        {
            var data = new byte[sizeof(uint)];
            _rng.GetBytes(data);
            var randUint = BitConverter.ToUInt32(data, 0);
            return randUint / (uint.MaxValue + 1.0);
        }
        public double NextDouble(double MaxValue)
        {
            double result = 0;
            do
            {
                result = NextDouble();
            } while (result > MaxValue);

            return result;
        }
        public string NextString(int Count)
        {
            string str = string.Empty;
            for (int i = 0; i < Count; i++)
            {
                int result = Convert.ToInt32(Next(48, 122).ToString());

                if ((result >= 48 && result <= 57) || (result >= 97 && result <= 122))
                {
                    string _char = ((char)result).ToString();
                    if (!str.Contains(_char))
                    {
                        str += _char;
                    }
                    else
                    {
                        i--;
                    }
                }
                else
                {
                    i--;
                }
            }
            return str;
        }

        public byte[] GetBytes(int Count)
        {
            var result = new byte[Count];

            _rng.GetBytes(result);

            return result;
        }
      
        protected void GetBytes(byte[] buff)
        {
            _rng.GetBytes(buff);
        }
    }
}
