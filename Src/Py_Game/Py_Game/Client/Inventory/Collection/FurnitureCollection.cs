using PangyaAPI.BinaryModels;
using Py_Connector.DataBase;
using Py_Game.Client.Inventory.Data.Furniture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.Client.Inventory.Collection
{
    public class FurnitureCollection : List<PlayerFurnitureData>
    {
        public FurnitureCollection(int PlayerUID)
        {
            Build(PlayerUID);
        }
        public int FurnitureAdd(PlayerFurnitureData Value)
        {
            Value.Update = false;
            Add(Value);
            return Count;
        }

        void Build(int UID)
        {
            var _db = new PangyaEntities();
            foreach (var info in _db.ProcGetRoomData(UID))
            {
                var Furniture = new PlayerFurnitureData()
                {
                    Index = (uint)info.IDX,
                    TypeID = (uint)info.TYPEID,
                    PosX = (float)info.POS_X,
                    PosY = (float)info.POS_Y,
                    PosZ = (float)info.POS_Z,
                    PosR = (float)info.POS_R,
                    Valid = 1
                };
                Add(Furniture);
            }
        }

        public byte[] GetItemInfo()
        {
            var Packet = new PangyaBinaryWriter();
            Packet.Write(new byte[] { 0x2D, 0x01 });
            Packet.WriteUInt32(1);
            Packet.WriteUInt16((ushort)Count);
            foreach (var Furniture in this)
            {
                Packet.Write(Furniture.GetBytes());
            }
            return Packet.GetBytes();
        }
    }
}
