using System;
using PangyaAPI.BinaryModels;
using Py_Game.Data;

namespace Py_Game.Client.Inventory.Data
{
    public struct Action
    {
        public UInt32 Animate;
        public UInt16 Unknown1;
        public UInt32 Posture;
        public Point3D Vector;
        public float Rotation;
        public byte[] Message;
        public Action Clear()
        {
             Animate = 0;
            Unknown1 = 0;
            Posture = 0;
            Vector = new Point3D();
            Message = new byte[5];
            Rotation = 0;
            return this;
        }

        public byte[] ToArray()
        {
            PangyaBinaryWriter Packet;
            Packet = new PangyaBinaryWriter();
            try
            {
                Packet.WriteUInt32(Animate);
                Packet.WriteUInt16(0);
                Packet.WriteUInt32(Posture);
                Packet.WriteStruct(Vector);               
                return Packet.GetBytes();
            }
            finally
            {
                Packet.Dispose();
            }
        }

        public Single Distance(Point3D Position)
        {
            return Vector.Distance(Position);
        }
    }
}

