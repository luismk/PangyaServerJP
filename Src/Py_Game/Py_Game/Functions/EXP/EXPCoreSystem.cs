using PangyaAPI.BinaryModels;
using Py_Game.Defines;
using Py_Game.Client;
using Py_Game.Functions.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using Py_Connector.DataBase;
namespace Py_Game.Functions.EXP
{
    #region HandleSystemExperienceGame
    public class EXPSystem
    {
        readonly Dictionary<uint, uint> EXPList = new Dictionary<uint, uint> { { 0, 30 }, { 1, 40 }, { 2, 50 }, { 3, 60 }, { 4, 70 }, { 5, 140 }, { 6, 105 }, { 7, 125 }, { 8, 145 }, { 9, 165 }, { 10, 330 }, { 11, 248 }, { 12, 278 }, { 13, 308 }, { 14, 338 }, { 15, 675 }, { 16, 506 }, { 17, 546 }, { 18, 586 }, { 19, 626 }, { 20, 1253 }, { 21, 1002 }, { 22, 1052 }, { 23, 1102 }, { 24, 1152 }, { 25, 2304 }, { 26, 1843 }, { 27, 1903 }, { 28, 1963 }, { 29, 2023 }, { 30, 4046 }, { 31, 3237 }, { 32, 3307 }, { 33, 3377 }, { 34, 3447 }, { 35, 6894 }, { 36, 5515 }, { 37, 5595 }, { 38, 5675 }, { 39, 5755 }, { 40, 11511 }, { 41, 8058 }, { 42, 8148 }, { 43, 8238 }, { 44, 8328 }, { 45, 16655 }, { 46, 8328 }, { 47, 8428 }, { 48, 8528 }, { 49, 8628 }, { 50, 17255 }, { 51, 9490 }, { 52, 9690 }, { 53, 9890 }, { 54, 10090 }, { 55, 20181 }, { 56, 20181 }, { 57, 20481 }, { 58, 20781 }, { 59, 21081 }, { 60, 42161 }, { 61, 37945 }, { 62, 68301 }, { 63, 122942 }, { 64, 221296 }, { 65, 442592 }, { 66, 663887 }, { 67, 995831 }, { 68, 1493747 }, { 69, 2240620 }, { 70, 0 } };
      
        public EXPSystem(GPlayer player, uint Total)
        {
            PlayerGetEXP(player, Total);                
        }

        protected void PlayerGetEXP(GPlayer player, uint Total)
        {
            PangyaBinaryWriter Resp;
            bool IsUpdate = false;
            MailSender MailSender;

            Resp = new PangyaBinaryWriter();
            if (player.Level >= 70)
            {
                player.SendResponse(new byte[] { 0x0F, 0x00, 0x01, 0x00, 0x00, 0x00 });
                return;
            }
            player.Exp = player.Exp += Total;

            while (true)
            {
                if (player.Level >= 70)
                {
                    break;
                }
                EXPList.TryGetValue(player.Level, out uint EXPTotal);
                if (player.Exp >= EXPTotal)
                {
                    player.Level = Convert.ToByte(player.Level + 1);                    
                    MailSender = new MailSender();
                    try
                    {
                        MailSender.Sender = "@GM";
                        MailSender.AddItemLevel((TLEVEL)player.Level);
                        MailSender.Send(player.GetUID);
                    }
                    finally
                    {
                        MailSender.Dispose();
                    }
                    player.Exp = player.Exp -= EXPTotal;
                    IsUpdate = true;
                }
                else
                {
                    break;
                }
            }
            var _db = new PangyaEntities();
            try
            {
                if (IsUpdate)
                {
                    var table1 = $"UPDATE Pangya_User_Statistics SET Game_Point = '{player.Exp}', Game_Level = '{player.Level}'  WHERE UID = '{player.GetUID}'";
                    _db.Database.SqlQuery<PangyaEntities>(table1).FirstOrDefault();
                    player.SendLevelUp();
                }

            }
            finally
            {
                if (Resp != null)  { Resp.Dispose(); }
                else if (_db != null) { _db.Dispose(); }
            }
          
            player.LoadStatistic();
        }
    }
    #endregion
}
