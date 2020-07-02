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
    public struct GrandPrixData
    {
        public uint Enable { get; set; }
        public uint TypeID { get; set; }
        public uint TrueTypeID { get; set; }
        public uint TypeGP { get; set; }
        public ushort TimeHole { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 65 + 1)]
        public string Name;
        public uint TicketTypeID { get; set; }
        public uint Quantity { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 39 + 1)]
        public string Image;//[39 + 1];
        public byte Unknown1 { get; set; }
        public byte Natural { get; set; }
        public byte ShortBase { get; set; }
        public byte HoleSize { get; set; }
        public uint Artifact { get; set; }
        public uint Map { get; set; }
        public uint Mode { get; set; }
        public byte TotalHole { get; set; }
        public byte MinLevel { get; set; }
        public byte MaxLevel { get; set; }
        public byte Unknown2 { get; set; }
        public uint Condition1 { get; set; }
        public uint Condition2 { get; set; }
        public int ScoreBotMax { get; set; }
        public int ScoreBotMed { get; set; }
        public int ScoreBotMin { get; set; }
        public uint Diffucult { get; set; }
        public uint PangReward { get; set; }
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 4 + 1)]
        public uint[] RewardTypeID;
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 4 + 1)]
        public uint[] RewardQuantity;
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string Unknown3;//= new char[11 + 1];                                           
        [field: MarshalAs(UnmanagedType.Struct)]
        public SystemTime DateActive;
        public ushort Hour_Open { get; set; }
        public ushort Min_Open { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string Unknown4;//= new char[11 + 1];
        public ushort Hour_Program { get; set; }
        public ushort Min_Program { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string Unknown5;//= new char[11 + 1];
        public ushort Hour_End { get; set; }
        public ushort Min_End { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string Unknown6;//= new char[7 + 1];
        public uint TypeIDGPLock { get; set; }
        public uint Lock { get; set; }
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 516)]
        public string Info;
        public string GetName()
        {
            return Name;
        }

        public bool IsNovice
        {
            get { return (this.Hour_Open == 0) && (this.Min_Open == 0) && (this.Hour_End == 0) && (this.Min_End == 0); }
        }
    }
}
