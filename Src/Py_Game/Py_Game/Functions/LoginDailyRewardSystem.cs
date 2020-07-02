using  PangyaAPI.BinaryModels;
using Py_Game.Client;
using System.Linq;
using Py_Connector.DataBase;
namespace Py_Game.Functions
{
    public class LoginDailyRewardSystem
    {
        public void PlayerDailyLoginItem(GPlayer player)
        {
            PangyaBinaryWriter Packet;

            Packet = new PangyaBinaryWriter();
            try
            {
                var _db = new PangyaEntities();
                var DailyLogin = _db.ProcAlterDaily((int)player.GetUID).FirstOrDefault();

                Packet.Write(new byte[] { 0x49, 0x02 });
                Packet.WriteUInt32(0);
                Packet.WriteByte(1);
                Packet.WriteInt32(DailyLogin.Item_TypeID);
                Packet.WriteInt32(DailyLogin.Item_Quantity);
                Packet.WriteInt32(DailyLogin.Item_TypeID_Next);
                Packet.WriteInt32(DailyLogin.Item_Quantity_Next);
                Packet.WriteInt32(DailyLogin.counter);
                player.SendResponse(Packet);
            }
            catch
            {
                player.Close();
            }
            finally
            {
                if (Packet != null)
                {
                    Packet.Dispose();
                }
            }
        }

        public void PlayerDailyLoginCheck(GPlayer player, byte Code)
        {

            PangyaBinaryWriter Packet;

            Packet = new PangyaBinaryWriter();
            try
            {
                var _db = new PangyaEntities();
                var DailyLogin = _db.ProcAlterDaily((int)player.GetUID).FirstOrDefault();
                Packet.Write(new byte[] { 0x48, 0x02 });
                Packet.WriteUInt32(0);
                Packet.WriteByte(Code);
                Packet.WriteInt32(DailyLogin.Item_TypeID);
                Packet.WriteInt32(DailyLogin.Item_Quantity);
                Packet.WriteInt32(DailyLogin.Item_TypeID_Next);
                Packet.WriteInt32(DailyLogin.Item_Quantity_Next);
                Packet.WriteInt32(DailyLogin.counter);
                player.SendResponse(Packet.GetBytes());
            }
            catch
            {
                player.Close();
            }
            finally
            {
            }
        }
    }
}
