using System.Runtime.InteropServices;
namespace Py_Game.IffManager.General
{
    /// <summary>
    /// Common data structure found at the head of many IFF datasets
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct IFFCommon
    {
        public uint Enabled { get; set; }//4 position
        public uint TypeID { get; set; }//8 position

        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Name;//48 position
        public byte MinLevel { get; set; }//49 position
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Icon;//89 position
        public ushort UNK { get; set; }//91 position
        public uint ItemPrice { get; set; }//95 position
        public uint DiscountPrice { get; set; }//99 position
        public uint UsedPrice { get; set; }//103 position
        public byte PriceType { get; set; }//104 position
        public byte MoneyFlag { get; set; }//105 position
        public byte TimeFlag { get; set; }//106 position
        public byte TimeByte { get; set; }//107 position
        public uint TPItem { get; set; }//111 position
        public uint TPCount { get; set; }// 115 positon
        public ushort Mileage { get; set; }// 117 position
        public ushort BonusProb { get; set; }// 119 position
        public ushort Mileage2 { get; set; }// 121 position
        public ushort Mileage3 { get; set; }// 123 position
        public uint TikiPointShop { get; set; }// 127 position
        public uint TikiPang { get; set; }// 131 position
        public uint ActiveData { get; set; }// 135 position

        [field: MarshalAs(UnmanagedType.Struct)]
        public SystemTime DateStart;// 149 position

        [field: MarshalAs(UnmanagedType.Struct)]
        public SystemTime DateEnd;// 163 position
    }
}
