using System.Runtime.InteropServices;
using PangyaFileCore.Definitions;
using PangyaFileCore.Struct;
namespace PangyaFileCore.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct HairStyle
    {
        [field: MarshalAs(UnmanagedType.Struct)]
        public IFFCommon Base;
        public byte HairColor { get; set; }
        public CharTypeByHairColor CharType { get; set; }
        public ushort Blank { get; set; }
    }
}
