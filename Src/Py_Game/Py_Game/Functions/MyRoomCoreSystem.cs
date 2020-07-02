using Py_Game.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.Functions
{
    public class MyRoomCoreSystem
    {
        public void PlayerEnterPersonalRoom(GPlayer player)
        {
            player.Response.Write(new byte[] { 0x2B, 0x01 });
            player.Response.WriteUInt32(1);
            player.Response.WriteUInt32(player.GetUID);
            player.Response.WriteUInt32(1);
            player.Response.WriteZero(104);
            player.SendResponse();
        }

        public void PlayerEnterPersonalRoomGetCharData(GPlayer player)
        {
            var Inventory = player.Inventory;

            player.Response.Write(new byte[] { 0x6B, 0x00, 0x04 });
            player.Response.Write((byte)0);// Salva uma lista de itens do tipo active no inventario
            player.Response.Write(player.Inventory.GetCharData());
            player.SendResponse();

            if (Inventory.CaddieIndex > 0)
            {
                player.Response.Write(new byte[] { 0x6B, 0x00, 0x04 });
                player.Response.WriteByte(1);// Seta Index do Caddie
                player.Response.WriteUInt32(Inventory.CaddieIndex);
                player.SendResponse();
            }
            else if (Inventory.ItemSlot.Exist())
            {
                player.Response.Write(new byte[] { 0x6B, 0x00, 0x04 });
                player.Response.WriteByte(2);// Salva uma lista de itens do tipo active no inventario
                player.Response.Write(Inventory.ItemSlot.GetItemSlot());
                player.SendResponse();
            }
            else if (Inventory.BallTypeID > 0 && Inventory.ClubSetIndex > 0)
            {
                player.Response.Write(new byte[] { 0x6B, 0x00, 0x04 });
                player.Response.WriteByte(3);// Salva uma lista de itens do tipo active no inventario
                player.Response.Write(Inventory.GetGolfEQP());
                player.SendResponse();
            }
            else if (Inventory.GetTitleTypeID() > 0)
            {
                player.Response.Write(new byte[] { 0x6B, 0x00, 0x04 });
                player.Response.WriteByte(4);// Salva uma lista de itens do tipo active no inventario
                player.Response.Write(Inventory.GetDecorationData());
                player.SendResponse();
            }
            else if (Inventory.CharacterIndex > 0)
            {
                player.Response.Write(new byte[] { 0x6B, 0x00, 0x04 });
                player.Response.WriteByte(5);//Seta Index Char
                player.Response.WriteUInt32(Inventory.CharacterIndex);
                player.SendResponse();
            }
            else if (Inventory.MascotIndex > 0)
            {
                player.Response.Write(new byte[] { 0x6B, 0x00, 0x04 });
                player.Response.WriteByte(8);// Mascot
                player.Response.Write(Inventory.GetMascotData());
                player.SendResponse();
            }
            else if (Inventory.CharacterIndex > 0 && Inventory.CutinIndex > 0)
            {
                player.Response.Write(new byte[] { 0x6B, 0x00, 0x04 });
                player.Response.WriteByte(9);//seta cutin
                player.Response.WriteUInt32(Inventory.CharacterIndex);
                player.Response.WriteUInt32(Inventory.CutinIndex);
                player.Response.WriteZero(12);//12 byte vazios      
                player.SendResponse();
            }


            player.Response.Write(new byte[] { 0x68, 0x01 });
            player.Response.Write(player.GetGameInfomations(2));
            player.SendResponse();

            player.SendResponse(Inventory.ItemRoom.GetItemInfo());
        }
    }
}
