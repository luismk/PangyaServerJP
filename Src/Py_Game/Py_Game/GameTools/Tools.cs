using System;
using System.Linq;
using static System.Math;
using System.Runtime.CompilerServices;
using System.Text;
using Py_Game.Defines;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace Py_Game.GameTools
{
    public static class Tools
    {
        public static int[] _THole18 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
        public static int[] _TMap19 = { 0x14, 0x12, 0x13, 0x10, 0x0F, 0x0E, 0x0D, 0x0B, 0x08, 0x0A, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x09 };
     
        
        public static ushort GetMap()
        {
            var Map = _TMap19;
            byte I;
            byte S;
            byte A;
            byte B;

            for (I = 0; I <= _TMap19.Length - 1; I++)
            {
                S = (byte)new Random().Next(_TMap19.Length);
                A = (byte)Map[S];
                B = (byte)Map[I];
                Map[I] = A;
                Map[S] = B;
            }
            return (ushort)(Map[new Random().Next(Map.Length)]);
        }
        
        public static int[] RandomHole()
        {
            int I;
            var Values = _THole18;
            for (I = 0; I <= _THole18.GetUpperBound(0); I++)
            {
                var Rnd = new Random();
                if (I != _THole18.Length)
                {
                    SwapX(ref Values[I], ref Values[I + Rnd.Next(Values.Length - I)]);
                }
            }
            return Values;
        }

        public static int[] RandomMap()
        {
            int I;
            var Values = _TMap19;
            for (I = 0; I <= _TMap19.GetUpperBound(0); I++)
                SwapX(ref Values[I], ref Values[I + new Random().Next(Values.Length - I)]);
            return Values;
        }


        public static string IsUCCNull(string UNIQUE, string IfNull = "0")
        {
            if (UNIQUE == null || UNIQUE.Length <= 0)
            {
                return IfNull;
            }
            return UNIQUE;
        }

        public static string StringFormat(string Format, object[] Args)
        {
            return string.Format(Format, Args);
        }


        public static bool CardCheckPosition(uint TypeID, uint Slot)
        {
            bool result = true;

            switch (Slot)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    {
                        if (!(GetCardType(TypeID) == CARDTYPE.tNormal))
                        {
                            result = false;
                        }

                    }
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                    {
                        if (!(GetCardType(TypeID) == CARDTYPE.tCaddie))
                        {
                            result = false;
                        }

                    }
                    break;
                case 9:
                case 10:
                    {
                        if (!(GetCardType(TypeID) == CARDTYPE.tNPC))
                        {
                            result = false;
                        }
                    }
                    break;
            }

            return result;
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetSQLTime(DateTime? Date)
        {
            if (Date.HasValue == false || Date?.Ticks == 0)
            {
                Date = DateTime.Now;
            }

            StringBuilder BuilderString;
           
            BuilderString = new StringBuilder();
            try
            {
               
                BuilderString.Append(Date.Value.ToString("yyyy-MM-dd"));
                BuilderString.Append('T');
                BuilderString.Append(Date.Value.ToString("hh:mm:ss"));
                return BuilderString.ToString();
            }
            finally
            {
                BuilderString.Clear();
            }

        }
        /// <summary>
        /// Format : yyyy-MM-dd HH:mm:ss:fff
        /// </summary>
        /// <param name="Time">Use for formated</param>
        /// <returns></returns>
        public static string GetSQLTimeFormat(DateTime? Date)
        {
            DateTime Time = (DateTime)Date;
            return TCompare.IfCompare(Time == null, DateTime.MinValue.ToString("yyyy/dd/mm HH:mm:ss:fff"), Time.ToString("yyyy/dd/mm HH:mm:ss:fff"));
        }

        public static uint DaysBetween(DateTime? d1, DateTime d2)
        {
            TimeSpan span = d1.Value.Subtract(d2);
            return Convert.ToUInt32(span.Days);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static byte[] GetFixTime(DateTime? date)
        {
            if (date.HasValue == false || date?.Ticks == 0)
                return GetZero(16);
           
            var DayOfWeek = Convert.ToUInt16(date?.DayOfWeek);
            return new ushort[]
            {
                (ushort)date?.Year,
                (ushort)date?.Month,
                 DayOfWeek,
                (ushort)date?.Day,
                (ushort)date?.Hour,
                (ushort)date?.Minute,
                (ushort)date?.Second,
                (ushort)date?.Millisecond
            }
            .SelectMany(v => BitConverter.GetBytes(v))
            .ToArray();
        }
        public static void SwapX(ref int lhs, ref int rhs)
        {
            int tmp;
            tmp = lhs;
            lhs = rhs;
            rhs = tmp;
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        public static byte[] GameTime()
        {
            return GetFixTime(DateTime.Now);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static long UnixTimeConvert(DateTime? unixtime)
        {
            if (unixtime.HasValue == false || unixtime?.Ticks == 0)
            { return 0; }
            TimeSpan timeSpan = (TimeSpan)(unixtime - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static DateTime UnixTimeConvert(long unixtime)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixtime).ToLocalTime();
            return dtDateTime;
        }

     

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static uint GetCaddieTypeIDBySkinID(uint SkinTypeID)
        {
            uint result;
            uint CaddieTypeID;
            CaddieTypeID = (uint)Round(a: ((SkinTypeID & 0x0FFF0000) >> 16) / 32);
            result = (CaddieTypeID + 0x1C000000) + ((SkinTypeID & 0x000F0000) >> 16);
            return result;
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static uint GetItemGroup(uint TypeID)
        {
            var result = (uint)Round((TypeID & 4227858432) / Pow(2.0, 26.0));

            return result;
        }

        public static uint GetCharacter(uint TypeID)
        {
            var Character = ((uint)((TypeID & 0x3fc0000) / Pow(2.0, 18.0)));
            return Character;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static byte GetAuxType(uint ID)
        {
            byte result;

            result = (byte)Round((ID & 0x001F0000) / Pow(2.0, 16.0));

            return result;
        }

        public static ItemTypeEnum GetItemType(uint ID)
        {
            var Item = (ItemTypeEnum)((int)((ID & 0x3fc0000) / Pow(2.0, 18.0)));
            return Item;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static CARDTYPE GetCardType(uint TypeID)
        {
            if (Round((TypeID & 0xFF000000) / Pow(2.0, 24.0)) == 0x7C)
            { return (CARDTYPE)Round((TypeID & 0x00FF0000) / Pow(2.0, 16.0)); }

            if (Round((TypeID & 0xFF000000) / Pow(2.0, 24.0)) == 0x7D)
            {
                if (Round((TypeID & 0x00FF0000) / Pow(2.0, 16.0)) == 0x40)
                { }
            }
            return CARDTYPE.tNPC;
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static byte[] GetZero(int length)
        {
            return new byte[length];
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static DateTime CreateGPDateTime(ushort Hour, ushort Min)
        {
            var Date = new PangyaFileCore.Struct.SystemTime
            {
                Year = (ushort)DateTime.Now.Year,
                Month = (ushort)DateTime.Now.Month,
                Day = (ushort)DateTime.Now.Day,
                Hour = (ushort)DateTime.Now.Hour,
                Minute = (ushort)DateTime.Now.Minute,
                Second = (ushort)DateTime.Now.Second,
                MilliSecond = (ushort)DateTime.Now.Millisecond
            };
            var result = new DateTime(Date.Year, Date.Month, Date.Day, Hour, Min, 0, 0);
            return result;
        }
        public static string GetMethodName(MethodBase methodBase)
        {
            string str = methodBase.Name + "(";
            foreach (ParameterInfo info in methodBase.GetParameters())
            {
                string[] textArray1 = new string[] { str, info.ParameterType.Name, " ", info.Name, ", " };
                str = string.Concat(textArray1);
            }
            return (str.Remove(str.Length - 2) + ")");
        }

        public static void PrintError(MethodBase methodBase, string msg)
        {
            string[] textArray1 = new string[] { "[", methodBase.DeclaringType.ToString(), "::", GetMethodName(methodBase), "]" };
            Console.WriteLine(string.Concat(textArray1));
            Console.WriteLine("Error : " + msg);
        }
        public static int Checksum(string dataToCalculate)
        {
            byte[] byteToCalculate = Encoding.ASCII.GetBytes(dataToCalculate);
            int checksum = 0;
            foreach (byte chData in byteToCalculate)
            {
                checksum += chData;
            }
            checksum &= 0xff;
            return checksum;
        }

    }
    public class TCompare
    {
        public static T IfCompare<T>(bool expression, T trueValue, T falseValue)
        {
            if (expression)
            {
                return trueValue;
            }
            else
            {
                return falseValue;
            }
        }
    }
}
