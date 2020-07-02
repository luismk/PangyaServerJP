using Py_Game.Defines;
using System;
namespace Py_Game.Game.Data
{
    public class GameInformation
    {
        public byte Unknown1;
        public UInt32 VSTime;
        public UInt32 GameTime;
        public byte MaxPlayer;
        public GAME_TYPE GameType;
        public byte HoleTotal;
        public byte Map;
        public byte Mode;
        // Natural
        public UInt32 NaturalMode;
        public bool GMEvent;
        // Hole Repeater
        public byte HoleNumber;
        public UInt32 LockHole;

        // Game Data
        public string Name;
        public string Password;
        public UInt32 Artifact;
        // Grandprix
        public bool GP;
        public UInt32 GPTypeID;
        public UInt32 GPTypeIDA;
        public UInt32 GPTime;
        public DateTime GPStart;
        public byte Time30S = 0x30;
    }

    public class GameHoleInfo
    {
        public byte Hole;
        public byte Weather;
        public ushort WindPower;
        public ushort WindDirection;
        public byte Map;
        public byte Pos;
    }
}
