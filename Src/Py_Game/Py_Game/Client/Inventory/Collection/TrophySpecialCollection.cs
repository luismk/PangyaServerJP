using PangyaAPI.BinaryModels;
using Py_Game.Client.Inventory.Data.TrophySpecial;
using System.Collections.Generic;

namespace Py_Game.Client.Inventory.Collection
{
    public class TrophySpecialCollection : List<PlayerTrophySpecialData>
    {
        /// <summary>
        /// Cria as informações
        /// </summary>
        /// <param name="Code"> 0 == Todas as sessoes, Sessão 5 </param>
        /// <returns></returns>
        public byte[] Build(byte Code = 5)
        {
            var result = new PangyaBinaryWriter();
            result.Write(new byte[] { 0xB4, 0x00, Code });
            result.Write((ushort)Count);
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
