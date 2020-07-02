using System.Runtime.InteropServices;

namespace Py_Game.Client.Inventory.Data.ItemDecoration
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ItemDecorationData
    {
        public uint BackGroundTypeID { get; set; }
        public uint FrameTypeID { get; set; }
        public uint StickerTypeID { get; set; }
        public uint SlotTypeID { get; set; }
        public uint UnknownTypeID { get; set; }
        public uint TitleTypeID { get; set; }
    }
}
