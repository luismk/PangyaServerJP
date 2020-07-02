using System;
using System.Runtime.InteropServices;
namespace PangyaFileCore.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CutinInfo
    {
        public UInt32 Enable { get; set; }
        public UInt32 TypeID { get; set; }

        public UInt32 Seq { get; set; }
        public UInt32 Sector { get; set; }

        public UInt32 Num1 { get; set; }
        public UInt32 Num2 { get; set; }
        public UInt32 NumImg1 { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string IMG1;
        public UInt32 NumImg2 { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string IMG2;//{ get; set; }
        public UInt32 NumImg3 { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string IMG3;
        public UInt32 Time { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
        public byte[] UN;
        public UInt32 Num4 { get; set; }
    }
}
