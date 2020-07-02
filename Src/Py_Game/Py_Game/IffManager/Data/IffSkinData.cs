using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Py_Game.IffManager.General;
namespace Py_Game.IffManager.Data.Skin
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct IffSkinData
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
