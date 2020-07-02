using PangyaFileCore.Struct;
using System.Runtime.InteropServices;
namespace PangyaFileCore.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Ball
    {
        [field: MarshalAs(UnmanagedType.Struct)]
        public IFFCommon Base;
        public uint Unknown0 { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Model;
        public uint Unknown2 { get; set; }
        public uint Unknown3 { get; set; }

        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string BallSequence1;

        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string BallSequence2;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string BallSequence3;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string BallSequence4;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string BallSequence5;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string BallSequence6;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string BallSequence7;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string BallFx1;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string BallFx2;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string BallFx3;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string BallFx4;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string BallFx5;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string BallFx6;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string BallFx7;
        public ushort Power { get; set; }
        public ushort Control { get; set; }
        public ushort Accuracy { get; set; }
        public ushort Spin { get; set; }
        public ushort Curve { get; set; }
        public ushort Unknown4 { get; set; }
    }
}
