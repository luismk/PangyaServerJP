using PangyaAPI.BinaryModels;
using Py_Game.Client.Inventory.Data.Trophy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.Client.Inventory.Collection
{
    public class TrophyCollection : List<PlayerTrophyData>
    {
        public TrophyCollection()
        {
            Add(new PlayerTrophyData());
        }

        public byte[] GetTrophy()
        {
            var result = new PangyaBinaryWriter();

            if (Count > 0)
            {
                foreach (var trophies in this)
                {
                    result.Write(trophies.GetTrophiesInfo());
                }
            }
            else
            {
                result.Write(78);
            }
            return result.GetBytes();
        }

        public byte[] Build(byte Code)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0x69, 0x01, Code });
            if (Count > 0)
            {
                foreach (var trophies in this)
                {
                    result.Write(trophies.GetTrophiesInfo());
                }
            }
            else
            {
                result.Write(78);
            }
            return result.GetBytes();
        }
    }
}
