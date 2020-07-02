using PangyaAPI;
using Py_Game.Client;
using System.Runtime.InteropServices;
using Py_Game.Defines;
using PangyaAPI.PangyaPacket;

namespace Py_Game.Functions
{
    public class CharacterCoreSystem
    {
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct TCharacterStatus
        {
            public uint Slot { get; set; }
            public uint CharTypeID { get; set; }
            public uint CharIndex { get; set; }
        }
        public void PlayerUpgradeCharacter(GPlayer player, Packet packet)
        {
            uint PangUpgrade;
            TCharacterStatus Data;
            
            Data = (TCharacterStatus)packet.Read(new TCharacterStatus());

            if (Data.CharIndex == 0)
            {
                player.SendResponse(new byte[] { 0x6F, 0x02, 0x06, 0x00, 0x00, 0x00 });
                return;
            }
            
            var Char = player.Inventory.ItemCharacter.GetChar(Data.CharTypeID, CharType.bTypeID);

            if (Char == null)
            {
                player.SendResponse(new byte[] { 0x6F, 0x02, 0x06, 0x00, 0x00, 0x00 });
                return;
            }

            PangUpgrade = Char.GetPangUpgrade((byte)Data.Slot);

            if (PangUpgrade == 0)
            {
                player.SendResponse(new byte[] { 0x6F, 0x02, 0x06, 0x00, 0x00, 0x00 });
                return;
            }

            if (!player.RemovePang(PangUpgrade))
            {
                player.SendResponse(new byte[] { 0x6F, 0x02, 0x05, 0x00, 0x00, 0x00 });
                return;

            }

            if (Char.UpgradeSlot((byte)Data.Slot))
            {
                player.SendPang();

                player.Inventory.ItemTransaction.AddCharStatus(0xC9, Char);

                player.SendTransaction();

                player.Response.Write(new byte[] { 0x6F, 0x02, });
                player.Response.Write(0);
                player.Response.Write(Data.Slot);
                player.SendResponse();
            }
        }
        public void PlayerDowngradeCharacter(GPlayer player, Packet packet)
        {
            TCharacterStatus Data;

            Data = (TCharacterStatus)packet.Read(new TCharacterStatus());

            if (Data.CharIndex == 0)
            {
                player.SendResponse(new byte[] { 0x70, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                return;
            }


            var Char = player.Inventory.ItemCharacter.GetChar(Data.CharTypeID, CharType.bTypeID);

            if (Char == null)
            {
                player.SendResponse(new byte[] { 0x70, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                return;
            }
            else
            {
                if (Char.DowngradeSlot((byte)Data.Slot))
                {
                    player.Inventory.ItemTransaction.AddCharStatus(0xC9, Char);

                    player.SendTransaction();

                    player.Response.Write(new byte[] { 0x70, 0x02, });
                    player.Response.Write(0);
                    player.Response.Write(Data.Slot);
                    player.SendResponse();
                }
            }
        }
    }
}
