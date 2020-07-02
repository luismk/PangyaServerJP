using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PangyaFileCore.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MemorialRareItem
    {
        public uint Enable { get; set; }
        public uint GachaNum { get; set; }
        public uint SumGacha { get; set; }
        public uint TypeID { get; set; }
        public uint Probabilities { get; set; }
        public uint RareType { get; set; }
        public uint ItemType { get; set; }
        public uint Sex { get; set; }
        public uint Value_1 { get; set; }
        public uint Item { get; set; }
        public uint CharacterType { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string UN;
    }

    public class SpecialItem
    {
        public byte Number { get; set; }
        public UInt32 TypeID { get; set; }
        public UInt32 Quantity { get; set; }
        public uint RareType { get; set; }
        public SpecialItem()
        {
            RareType = uint.MaxValue;
        }
    }
}
