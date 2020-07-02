using PangyaAPI.BinaryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.Client.Inventory.Data.Trophy
{
    public class PlayerTrophyData
    {
        public ushort AMA_6_G { get; set; }
        public ushort AMA_6_S { get; set; }
        public ushort AMA_6_B { get; set; }
        public ushort AMA_5_G { get; set; }
        public ushort AMA_5_S { get; set; }
        public ushort AMA_5_B { get; set; }
        public ushort AMA_4_G { get; set; }
        public ushort AMA_4_S { get; set; }
        public ushort AMA_4_B { get; set; }
        public ushort AMA_3_G { get; set; }
        public ushort AMA_3_S { get; set; }
        public ushort AMA_3_B { get; set; }
        public ushort AMA_2_G { get; set; }
        public ushort AMA_2_S { get; set; }
        public ushort AMA_2_B { get; set; }
        public ushort AMA_1_G { get; set; }
        public ushort AMA_1_S { get; set; }
        public ushort AMA_1_B { get; set; }
        public ushort PRO_1_G { get; set; }
        public ushort PRO_1_S { get; set; }
        public ushort PRO_1_B { get; set; }
        public ushort PRO_2_G { get; set; }
        public ushort PRO_2_S { get; set; }
        public ushort PRO_2_B { get; set; }
        public ushort PRO_3_G { get; set; }
        public ushort PRO_3_S { get; set; }
        public ushort PRO_3_B { get; set; }
        public ushort PRO_4_G { get; set; }
        public ushort PRO_4_S { get; set; }
        public ushort PRO_4_B { get; set; }
        public ushort PRO_5_G { get; set; }
        public ushort PRO_5_S { get; set; }
        public ushort PRO_5_B { get; set; }
        public ushort PRO_6_G { get; set; }
        public ushort PRO_6_S { get; set; }
        public ushort PRO_6_B { get; set; }
        public ushort PRO_7_G { get; set; }
        public ushort PRO_7_S { get; set; }
        public ushort PRO_7_B { get; set; }

        public byte[] GetTrophiesInfo()
        {
            var result = new PangyaBinaryWriter();
            result.WriteUInt16(AMA_6_G);
            result.WriteUInt16(AMA_6_S);
            result.WriteUInt16(AMA_6_B);
            result.WriteUInt16(AMA_5_G);
            result.WriteUInt16(AMA_5_S);
            result.WriteUInt16(AMA_5_B);
            result.WriteUInt16(AMA_4_G);
            result.WriteUInt16(AMA_4_S);
            result.WriteUInt16(AMA_4_B);
            result.WriteUInt16(AMA_3_G);
            result.WriteUInt16(AMA_3_S);
            result.WriteUInt16(AMA_3_B);
            result.WriteUInt16(AMA_2_G);
            result.WriteUInt16(AMA_2_S);
            result.WriteUInt16(AMA_2_B);
            result.WriteUInt16(AMA_1_G);
            result.WriteUInt16(AMA_1_S);
            result.WriteUInt16(AMA_1_B);
            result.WriteUInt16(PRO_6_G);
            result.WriteUInt16(PRO_6_S);
            result.WriteUInt16(PRO_6_B);
            result.WriteUInt16(PRO_5_G);
            result.WriteUInt16(PRO_5_S);
            result.WriteUInt16(PRO_5_B);
            result.WriteUInt16(PRO_4_G);
            result.WriteUInt16(PRO_4_S);
            result.WriteUInt16(PRO_4_B);
            result.WriteUInt16(PRO_3_G);
            result.WriteUInt16(PRO_3_S);
            result.WriteUInt16(PRO_3_B);
            result.WriteUInt16(PRO_2_G);
            result.WriteUInt16(PRO_2_S);
            result.WriteUInt16(PRO_2_B);
            result.WriteUInt16(PRO_1_G);
            result.WriteUInt16(PRO_1_S);
            result.WriteUInt16(PRO_1_B);
            result.WriteUInt16(PRO_7_G);
            result.WriteUInt16(PRO_7_S);
            result.WriteUInt16(PRO_7_B);
            return result.GetBytes();
        }

    }
}
