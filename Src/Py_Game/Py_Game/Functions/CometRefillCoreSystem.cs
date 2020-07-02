using Py_Game.Client.Inventory;
using PangyaAPI;
using Py_Game.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Py_Game.Client.Inventory.Data;
using PangyaAPI.PangyaPacket;

namespace Py_Game.Functions
{
    class CometRefillCoreSystem
    {
        public void PlayerOpenAzectBox(GPlayer PL, Packet packet)
        {
            AddData ItemAddedData;
            AddItem ItemAddData;


            if (!packet.ReadUInt32(out uint BoxTypeID))
            {

            }

            if (!packet.ReadUInt32(out uint BallTypeID))
            {

            }

            if (!(BoxTypeID == 436207877))
            { return; }

            // CHECK IF USE HAVE ITEM
            if (PL.Inventory.IsExist(BoxTypeID) && PL.Inventory.IsExist(BallTypeID))
            {
                PL.Inventory.Remove(BoxTypeID, 1, false);

                ItemAddData = new AddItem()
                {
                    ItemIffId = BallTypeID,
                    Quantity = (uint)new Random().Next(15, 25),
                    Transaction = false,
                    Day = 0,
                };

                ItemAddedData = PL.AddItem(ItemAddData);

                try
                {
                    packet.Write(new byte[] { 0x97, 0x01, 0x01 });
                    packet.WriteUInt32(BoxTypeID);
                    packet.WriteUInt32(BallTypeID);
                    packet.WriteUInt32(ItemAddedData.ItemNewQty);
                    PL.SendResponse(packet.GetBytes());
                }
                finally
                {
                    packet.Dispose();
                }
            }
            else { PL.SendResponse(new byte[] { 0x97, 0x01, }); }
        }
    }
}
