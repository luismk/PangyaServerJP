using PangyaFileCore.Definitions;
using System;
using System.Runtime.InteropServices;
namespace PangyaFileCore.Struct
{
    /// <summary>
    /// Common data structure found at the head of many IFF datasets
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct IFFCommon //size = 0x90(144 bytes)
    {
        public uint Enabled { get; set; }//4 position
        public uint TypeID { get; set; }//8 position
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Name { get; set; }//48 position
        public byte MinLevel { get; set; }//49 position
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Icon { get; set; }//89 position
        public ushort UNK { get; set; }//91 position
        public uint ItemPrice { get; set; }//95 position
        public uint DiscountPrice { get; set; }//99 position
        public uint UsedPrice { get; set; }//103 position
        public byte PriceType { get; set; }//104 position
        public byte MoneyFlag { get; set; }//105 position(0x01 in stock; 0x02 disable gift; 0x03 Special; 0x08 new; 0x10 hot)
        public byte TimeFlag { get; set; }//106 position
        public byte TimeByte { get; set; }//107 position
        public uint TPItemCount { get; set; }//111 position
        public uint TPCount { get; set; }// 115 positon
        public ushort Mileage { get; set; }// 117 position
        public ushort BonusProb { get; set; }// 119 position
        public ushort Mileage2 { get; set; }// 121 position
        public ushort Mileage3 { get; set; }// 123 position
        public uint TikiPointShop { get; set; }// 127 position
        public uint TikiPang { get; set; }// 131 position
        public uint Active { get; set; }// 135 position
        [field: MarshalAs(UnmanagedType.Struct)]
        public SystemTime DateStart { get; set; }// 149 position
        [field: MarshalAs(UnmanagedType.Struct)]
        public SystemTime DateEnd { get; set; }// 163 position

        public bool ActiveAll()
        {
            MoneyFlag = 0;
            PriceType = 32;
            TimeFlag = 0x15;
            if (ItemPrice <= 10000000)
            {
                ItemPrice = 1;
            }
            return true;
        }

        public uint GenerateNewTypeID(int iffType, int characterId, int int_0, int group, int type, int serial)
        {
            if (group - 1 < 0)
            {
                group = 0;
            }
            return Convert.ToUInt32((iffType * Math.Pow(2.0, 26.0)) + (characterId * Math.Pow(2.0, 18.0)) + (int_0 * Math.Pow(2.0, 13.0)) + (group * Math.Pow(2.0, 11.0)) + (type * Math.Pow(2.0, 9.0)) + serial);
        }

        public uint TypeItem { get { return (uint)((int)((TypeID & 0x3fc0000) / Math.Pow(2.0, 18.0)));  } }      

        private uint[] GetTypeIdValues()
        {
            uint[] _TypeIDValues = new uint[6];
            _TypeIDValues[0] = ((uint)((TypeID & 0x3fc0000) / Math.Pow(2.0, 18.0)));
            _TypeIDValues[1] = (ushort)((TypeID & 0x3fc0000) / Math.Pow(2.0, 18.0));
            _TypeIDValues[2] = (ushort)((TypeID & 0xfc000000) / Math.Pow(2.0, 26.0));
            _TypeIDValues[3] = (ushort)((TypeID & 0x1f0000) / Math.Pow(2.0, 16.0));
            _TypeIDValues[4] = (ushort)((TypeID & 0x3e003) / Math.Pow(2.0, 13.0));
            _TypeIDValues[5] = (ushort)(TypeID & 0xff);
            return _TypeIDValues;
        }
    }
}
