using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PangyaAPI.Crypts
{
    public static class Cryptor
    {
        static readonly CryptsLib Pang = new CryptsLib();

        public static byte[] ClientEncrypt(this byte[] message, byte Key)
        {
            return Pang.Pangya_Client_Encrypt(message, Key, (byte)(Key + 1));
        }
        public static byte[] ClientDecrypt(this byte[] message, byte Key)
        {
            return Pang.Pangya_Client_Decrypt(message, Key);
        }
        public static byte[] ServerEncrypt(this byte[] message, byte Key)
        {
            return Pang.Pangya_Server_Encrypt(message, Key, (byte)new Random().Next(255));
        }
        public static byte[] ServerDecrypt(this byte[] message, byte Key)
        {
            return Pang.Pangya_Server_Decrypt(message, Key);
        }
    }
}
