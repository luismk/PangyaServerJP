using System.Runtime.InteropServices;
using PangyaFileCore.Struct;
namespace PangyaFileCore.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Skin
    {
        [field: MarshalAs(UnmanagedType.Struct)]
        public IFFCommon Base;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string MPet;

        public uint PriceUnk { get; set; }
        public uint Price7 { get; set; }
        public uint Price30 { get; set; }
    }
}
