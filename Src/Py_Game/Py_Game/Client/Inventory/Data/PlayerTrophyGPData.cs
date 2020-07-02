using PangyaAPI.BinaryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.Client.Inventory.Data.TrophyGrandPrix
{
    public class PlayerTrophyGPData
    {
        public uint Index { get; set; }
        public uint TypeID { get; set; }
        public uint Quantity { get; set; }

        public byte[] GetInfo()
        {
            using (var result = new PangyaBinaryWriter())
            {
                result.Write(Index);
                result.Write(TypeID);
                result.Write(Quantity);
                return result.GetBytes();
            }
        }
    }
}
