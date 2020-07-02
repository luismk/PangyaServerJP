using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PangyaFileCore.Struct;

namespace PangyaFileCore.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ClubSet
    {
        [field: MarshalAs(UnmanagedType.Struct)]
        public IFFCommon Base;

        public uint Club1 { get; set; }
        public uint Club2 { get; set; }
        public uint Club3 { get; set; }
        public uint Club4 { get; set; }
        public ushort C0 { get; set; }
        public ushort C1 { get; set; }
        public ushort C2 { get; set; }
        public ushort C3 { get; set; }
        public ushort C4 { get; set; }
        public ushort MaxPow { get; set; }
        public ushort MaxCon { get; set; }
        public ushort MaxImp { get; set; }
        public ushort MaxSpin { get; set; }
        public ushort MaxCurve { get; set; }
        public uint ClubType { get; set; }
        public uint ClubSPoint { get; set; }
        public uint RecoveryLimit { get; set; }
        public float RateWorkshop { get; set; }
        public uint Rank_WorkShop { get; set; }
        public ushort Transafer { get; set; }
        public ushort Flag1 { get; set; }
        public uint Unknown7 { get; set; }
        public uint Real_TypeID { get; set; }

    }
}
