using PangyaAPI.BinaryModels;
using Py_Game.Client.Inventory.Data.TrophyGrandPrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.Client.Inventory.Collection
{
    public class TrophyGPCollection : List<PlayerTrophyGPData>
    {
        public TrophyGPCollection()
        {
            Add(new PlayerTrophyGPData());
        }
        public byte[] Build(byte Code)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x5D, 0x02, Code });
            result.Write(0);
            result.Write(0);
            foreach (var data in this)
            {
                result.Write(data.GetInfo());
            }
            return result.GetBytes();
        }
        public byte[] GetInfo()
        {
            var result = new PangyaBinaryWriter();
            result.Write((ushort)Count);
            foreach (var data in this)
            {
                result.Write(data.GetInfo());
            }
            return result.GetBytes();
        }
    }
}
