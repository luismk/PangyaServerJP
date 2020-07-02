using PangyaAPI;
using Py_Connector.DataBase;
using Py_Game.Client;
using Py_Game.Defines;
using static Py_Game.GameTools.PacketCreator;
using System.Linq;
using System.Runtime.InteropServices;
using PangyaAPI.PangyaPacket;

namespace Py_Game.Functions
{
    public class TutorialCoreSystem
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct Tutorial
        {
            public ushort Code { get; set; }
            public uint MissionID { get; set; }
        }

        public TutorialCoreSystem(GPlayer player)
        {
            player.SendResponse(ShowTutorialPlayer(3, 0xff));
            //var db = new PangyaEntities();

            //try
            //{
            //    var GetTutorial = db.ProcGetTutorial((int)player.GetUID).First();

            //    player.SendResponse(ShowTutorialPlayer(3,0));
            //}
            //catch 
            //{
            //    player.Close();
            //}
            //finally
            //{
            //    if (db != null)
            //    {
            //        db.Dispose();
            //    }
            //}
        }

        public static void PlayerTutorialMission(GPlayer player, Packet packet)
        {
            var data = (Tutorial)packet.Read(new Tutorial());

            var SetTutorial = player.TutorialCompleted;

            if (SetTutorial == false)
            {
                player.SetTutorial(data.Code, data.MissionID);
            }
            var db = new PangyaEntities();
            try
            {
                 var GetTutorial = db.ProcGetTutorial((int)player.GetUID).First();

                switch ((TutorialType)data.Code)
                {
                    case TutorialType.Rookie:
                    case TutorialType.NewRookie:
                        {
                            data.Code = 0;
                            switch (data.MissionID)
                            {
                                case 1://não é necessario setar, mas eu seto, só por precausao
                                    {
                                        data.MissionID = 1;
                                    }
                                    break;
                                case 2:
                                    {
                                        data.MissionID = 3;
                                    }
                                    break;
                                case 4:
                                    {
                                        data.MissionID = (uint)GetTutorial.Rookie;
                                    }
                                    break;
                                case 8:
                                    {
                                        data.MissionID = (uint)GetTutorial.Rookie;
                                    }
                                    break;
                                case 16:
                                    {
                                        data.MissionID = (uint)GetTutorial.Rookie;
                                    }
                                    break;
                                case 32:
                                    {
                                        data.MissionID = (uint)GetTutorial.Rookie;
                                    }
                                    break;
                                case 64:
                                    {
                                        data.MissionID = (uint)GetTutorial.Rookie;
                                    }
                                    break;
                                case 128:
                                    {
                                        data.MissionID = byte.MaxValue;
                                    }
                                    break;
                            }
                        }
                        break;
                    case TutorialType.Beginner:
                        {
                            data.Code = 1;
                            switch (data.MissionID)
                            {
                                case 256:
                                    {
                                        data.MissionID = 256;//missão 10
                                    }
                                    break;
                                case 512:
                                    {
                                        data.MissionID = (uint)GetTutorial.Beginner;//missão 11
                                    }
                                    break;
                                case 1024:
                                    {
                                        data.MissionID = (uint)GetTutorial.Beginner;//missão 12
                                    }
                                    break;
                                case 2048:
                                    {
                                        data.MissionID = (uint)GetTutorial.Beginner;//missão 13
                                    }
                                    break;
                                case 4096:
                                    {
                                        data.MissionID = (uint)GetTutorial.Beginner;//missão 14
                                    }
                                    break;
                                case 8192:
                                    {
                                        data.MissionID = (uint)GetTutorial.Beginner;//missão 15
                                    }
                                    break;
                            }
                        }
                        break;
                }
                //1F 01 01 0100 01 00 00
                player.SendResponse(ShowTutorialPlayer(data.Code, data.MissionID));

                //player.SendMailPopup();
            }
            catch
            {
                player.Close();
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }
    }
}
