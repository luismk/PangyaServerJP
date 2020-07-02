using Py_Game.Defines;
using System.Runtime.InteropServices;
using Py_Game.IffManager.General;
namespace Py_Game.IffManager.Data.HairStyle
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct IffHairStyleData
    {
        [field: MarshalAs(UnmanagedType.Struct)]
        public IFFCommon Base;
        public byte HairColor { get; set; }
        public CharTypeByHairColor CharType { get; set; }
        public ushort Blank { get; set; }
    }
}
