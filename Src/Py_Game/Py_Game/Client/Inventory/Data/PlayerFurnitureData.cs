using PangyaAPI.BinaryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.Client.Inventory.Data.Furniture
{
   public class PlayerFurnitureData
    {
        public UInt32 Index { get; set; }
        public UInt32 TypeID { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public float PosR { get; set; }
        public Byte Valid { get; set; }
        public bool Update { get; set; }

        public byte[] GetBytes()
        {
            var Packet = new PangyaBinaryWriter();

            Packet.WriteUInt32(Index);
            Packet.WriteUInt32(TypeID);
            Packet.WriteUInt16(0);
            Packet.WriteSingle(PosX);
            Packet.WriteSingle(PosY);
            Packet.WriteSingle(PosZ);
            Packet.WriteSingle(PosR);
            Packet.WriteByte(0);

            return Packet.GetBytes();
        }

        public byte[] GetBytesTemp()
        {
            var result = new byte[] {
                0x02, 0x00, 0x82, 0x48,
                0x00, 0x00,
                0x20, 0x68,  0x00, 0x48, 0x00, 0x00, 0x33, 0x33, 0x73, 0x41, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x48, 0x41, 0x00, 0x00, 0x18, 0x43, 0x00, 0x83, 0x48, 0x00, 0x00, 0x1D, 0x50, 0x00, 0x48, 0x00, 0x00, 0x00, 0x00, 0x62, 0x41, 0x00, 0x00, 0x80, 0x3F, 0x9E, 0xEF, 0x27, 0x3D, 0x00, 0x00, 0x00, 0x00, 0x00 };

            return result;
        }
    }
}
