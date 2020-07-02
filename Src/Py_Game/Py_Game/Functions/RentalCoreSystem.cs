using  PangyaAPI.BinaryModels;
using System;
using System.Linq;
using Py_Game.Client.Inventory;
using static PangyaFileCore.IffBaseManager;
using static Py_Game.GameTools.Tools;
using Py_Connector.DataBase;
using Py_Game.Client;
using PangyaAPI;
using PangyaAPI.PangyaPacket;
using PangyaAPI.Tools;

namespace Py_Game.Functions
{
    public class RentalCoreSystem
    {
        public void PlayerRenewRent(GPlayer player, Packet packet)
        {
            uint PartCharge;


            if (!packet.ReadUInt32(out uint PartIndex))
            {
                return;
            }

            var PPart = player.Inventory.ItemWarehouse.GetItem(PartIndex);

            if (PPart == null)
            {
                player.SendResponse(new byte[] { 0x8F, 0x01, 0x05 });

                WriteConsole.WriteLine("PlayerPlayerRenewRent: variable ppart is null");
                return;
            }

            if (!(GetItemGroup(PPart.ItemTypeID) == 2) || (!(PPart.ItemFlag == 0x62)))
            {
                player.SendResponse(new byte[] { 0x8F, 0x01, 0x03 });

                WriteConsole.WriteLine("PlayerPlayerRenewRent: Error 01");
                return;
            }
            PartCharge = IffEntry.GetRentalPrice(PPart.ItemTypeID);

            if (PartCharge <= 0)
            {
                player.SendResponse(new byte[] { 0x8F, 0x01, 0x05 });

                WriteConsole.WriteLine("PlayerPlayerRenewRent: Error RentalPrice");
                return;
            }

            if (!player.RemovePang(PartCharge))
            {
                player.SendResponse(new byte[] { 0x8F, 0x01, 0x04 });

                WriteConsole.WriteLine("PlayerPlayerRenewRent: Error RemovePang");
                return;
            }

            player.SendPang();

            PPart.Renew();


            try
            {
                packet.Write(new byte[] { 0x8F, 0x01, 0x00 });
                packet.WriteUInt32(PPart.ItemTypeID);
                packet.WriteUInt32(PPart.ItemIndex);
                player.SendResponse(packet.GetBytes());
            }
            finally
            {
                packet.Dispose();
            }
        }

        public void PlayerDeleteRent(GPlayer player, Packet packet)
        {

            if (!packet.ReadUInt32(out uint PartIndex))
            {
                return;
            }

            var PPart = player.Inventory.ItemWarehouse.GetItem(PartIndex);

            if (PPart == null)
            {
                player.SendResponse(new byte[] { 0x90, 0x01, 0x03 });

                WriteConsole.WriteLine("PlayerPlayerDeleteRent: variable ppart is nill");
                return;
            }

            if (!(GetItemGroup(PPart.ItemTypeID) == 2) || (!(PPart.ItemFlag == 0x62)))
            {
                player.SendResponse(new byte[] { 0x90, 0x01, 0x03 });

                WriteConsole.WriteLine("PlayerPlayerRenewRent: Error 01");
                return;
            }


            PPart.DeleteItem();

            try
            {
                packet.Write(new byte[] { 0x90, 0x01, 0x00 });
                packet.WriteUInt32(PPart.ItemTypeID);
                packet.WriteUInt32(PPart.ItemIndex);
                player.SendResponse(packet.GetBytes());
            }
            finally
            {
                packet.Dispose();
            }
        }

        public void PlayerRemoveItem(GPlayer player, Packet packet)
        {
            uint TypeId, Quantity;

            TypeId = packet.ReadUInt32();
            Quantity = packet.ReadUInt32();

            if (!(GetItemGroup(TypeId) == 6))
            {
                return;
            }


            var ItemAddedData = player.Inventory.Remove(TypeId, Quantity, false);

            if (!ItemAddedData.Status)
            {
                return;
            }

            try
            {
                packet.Write(new byte[] { 0xc5, 0x00, 0x01 });
                packet.WriteUInt32(ItemAddedData.ItemTypeID);
                packet.WriteUInt32(Quantity);
                packet.WriteUInt32(ItemAddedData.ItemIndex);
                player.SendResponse(packet.GetBytes());
            }
            finally
            {
                packet.Dispose();
            }
        }
    }
}
