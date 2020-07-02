using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PangyaFileCore.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Desc
    {
        public uint TypeID { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public string Description { get; set; }
    }
}
