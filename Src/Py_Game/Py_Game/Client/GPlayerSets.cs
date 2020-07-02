using Py_Connector.DataBase;
using Py_Game.Client.Inventory;
using Py_Game.Client.Inventory.Data;
using Py_Game.Functions.EXP;
using System;
using System.Linq;

namespace Py_Game.Client
{
    public partial class GPlayer
    {
        public bool RemoveCookie(uint Amount)
        {
            if (GetCookie < Amount)
            {
                return (false);
            }
            GetCookie -= Amount;
            var table1 = $"UPDATE Pangya_Personal SET CookieAmt = '{GetCookie}' WHERE UID = '{GetUID}'";
            Db.Database.SqlQuery<PangyaEntities>(table1).FirstOrDefault();
            return (true);
        }

        public bool RemoveLockerPang(uint Amount)
        {
            if ((LockerPang < Amount)) return (false);
            LockerPang -= Amount;
            var table1 = $"UPDATE Pangya_Personal SET PangLockerAmt = '{LockerPang}' WHERE UID = '{GetUID}'";
            Db.Database.SqlQuery<PangyaEntities>(table1).FirstOrDefault();
            return (true);
        }

        public bool RemovePang(uint Amount)
        {
            if (UserStatistic.Pang < Amount)
            {
                return (false);
            }
            UserStatistic.Pang -= Amount;
            var table1 = $"UPDATE Pangya_User_Statistics SET Pang = '{UserStatistic.Pang}' WHERE UID = '{GetUID}'";
            Db.Database.SqlQuery<PangyaEntities>(table1).FirstOrDefault();
            return (true);
        }

        public bool AddLockerPang(uint Amount)
        {
            LockerPang += Amount;

            var table1 = $"UPDATE Pangya_Personal SET PangLockerAmt = '{LockerPang}' WHERE UID = '{GetUID}'";
            Db.Database.SqlQuery<PangyaEntities>(table1).FirstOrDefault();
            return (true);
        }

        public bool AddPang(uint Amount)
        {
            if (UserStatistic.Pang >= uint.MaxValue)
            {
                return false;
            }
            UserStatistic.Pang += Amount;
            var table1 = $"UPDATE Pangya_User_Statistics SET Pang = '{UserStatistic.Pang}' WHERE UID = '{GetUID}'";
            Db.Database.SqlQuery<PangyaEntities>(table1).FirstOrDefault();
            return true;
        }

        public bool AddCookie(uint Amount)
        {
            if (GetCookie >= uint.MaxValue)
            {
                return false;
            }
            GetCookie += Amount;
            var table1 = $"UPDATE Pangya_Personal SET CookieAmt = '{GetCookie}' WHERE UID = '{GetUID}'";
            Db.Database.SqlQuery<PangyaEntities>(table1).FirstOrDefault();
            return true;
        }
        public bool UpdateMapStatistic(Data.StatisticData Statistic, byte Map, sbyte Score, uint MaxPang)
        {
            var query = $" Exec dbo.ProcUpdateMapStatistics @UID = '{GetUID}', @MAP ='{Map}',@DRIVE = '{Statistic.Drive}', @PUTT = '{Statistic.Putt}', @HOLE = '{Statistic.Hole}',  @FAIRWAY = '{Statistic.Fairway}', @HOLEIN = '{Statistic.Holein}', @PUTTIN = '{Statistic.Puttin}', @TOTALSCORE = '{Score}',  @BESTSCORE = '{Score}',  @MAXPANG = '{MaxPang}',  @CHARTYPEID = '{this.Inventory.GetCharTypeID()}',  @ASSIST = '{this.Assist}'";

            var IsNewRecord = Db.Database.SqlQuery<int>(query).FirstOrDefault();
            return IsNewRecord == 1;
        }

        public bool AddExp(uint Count)
        {
            new EXPSystem(this, Count);
            return true;
        }

        public void SetCookie(uint Cookie)
        {
            GetCookie = Cookie;
        }

        public bool SetAuthKey1(string TAUTH_KEY_1)
        {
            GetAuth1 = TAUTH_KEY_1;
            return true;
        }

        public bool SetAuthKey2(string TAUTH_KEY_2)
        {
            GetAuth2 = TAUTH_KEY_2;
            return true;
        }

        public bool SetCapabilities(byte TCapa)
        {
            GetCapability = TCapa;
            if (TCapa == 4)
            {
                Visible = 4;
            }
            return true;
        }

        public void SetExp(uint Amount)
        {
            UserStatistic.EXP = Amount;
        }

        public void SetGameID(uint ID)
        {
            GameID = (ushort)ID;
        }

        public void SetLevel(byte Amount)
        {
            UserStatistic.Level = Amount;
        }

        public bool SetLogin(string TLogin)
        {
            GetLogin = TLogin;
            return true;
        }

        public bool SetNickname(string TNickname)
        {
            GetNickname = TNickname;
            return true;
        }

        public bool SetSex(Byte TSex)
        {
            GetSex = TSex;
            return true;
        }

        public bool SetUID(uint TUID)
        {
            GetUID = TUID;
            if (Inventory == null)
            {
                Inventory = new PlayerInventory(TUID);
            }
            return true;
        }

        public void SetTutorial(uint Type, uint value)
        {
            Db.ProcTutorialSet((int)GetUID,(int) Type, (int)value);

            SetTutorial();
        }

        public void SetTutorial()
        {
            TutorialCompleted = Db.Pangya_Member.Any(c => c.UID == GetUID && c.Tutorial == 2);
        }

      

        public AddData AddItem(AddItem ItemAddData)
        {
            switch (ItemAddData.ItemIffId)
            {
                // case pang and exp pocket
                case 0x1A00015D: // exp pocket
                    AddExp(ItemAddData.Quantity);
                    break;
                case 0x1A000010:// pang pocket
                    AddPang(ItemAddData.Quantity);
                    break;
            }
            return Inventory.AddItem(ItemAddData);
        }
    }
}
