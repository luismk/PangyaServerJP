using System;
namespace PangyaAPI.Tools
{
    public static class WriteConsole
    {
        // Specifies how silent the console is.
        static readonly short msg_silent = 0;

        // enum with console color codes (FG and BG colors are the same)
        enum Color
        {
            CL_BLACK,
            CL_BLUE,
            CL_GREEN,
            CL_CYAN,
            CL_RED,
            CL_MAGENTA,
            CL_YELLOW,
            CL_GREY = 7,
            CL_WHITE = 15
        }
        enum Message_Type
        {
            MSG_NONE,
            MSG_STATUS,
            MSG_SQL,
            MSG_INFORMATION,
            MSG_NOTICE,
            MSG_WARNING,
            MSG_DEBUG,
            MSG_ERROR,
            MSG_FATALERROR
        }
        static bool _vShowMessage(Message_Type flag, string message = "")
        {
            if (message == "")
            {
                Error("Empty string passed to _vShowMessage()");
                return false;
            }

            if ((flag == Message_Type.MSG_INFORMATION && msg_silent == 1)
                || (flag == Message_Type.MSG_STATUS && msg_silent == 2)
                || (flag == Message_Type.MSG_NOTICE && msg_silent == 4)
                || (flag == Message_Type.MSG_WARNING && msg_silent == 8)
                || (flag == Message_Type.MSG_ERROR && msg_silent == 16)
                || (flag == Message_Type.MSG_SQL && msg_silent == 16)
                || (flag == Message_Type.MSG_DEBUG && msg_silent == 32))
                return false; // Do Not WriteLine it.

            string prefix = "";
            Color color = Color.CL_GREY;

            switch (flag)
            {
                case Message_Type.MSG_NONE: // direct WriteLine replacement.
                    break;
                case Message_Type.MSG_STATUS: // Bright Green (To inform about good things)
                    prefix = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss] ");
                    color = Color.CL_GREEN;
                    break;
                case Message_Type.MSG_SQL: // Bright Violet (For dumping out anything related with SQL)
                    prefix = "[SQL]";
                    color = Color.CL_MAGENTA;
                    break;
                case Message_Type.MSG_INFORMATION: // Bright White (Variable information)
                    prefix = "[Info]";
                    color = Color.CL_WHITE;
                    break;
                case Message_Type.MSG_NOTICE: // Bright White (Less than a warning)
                    prefix = "[Notice]";
                    color = Color.CL_WHITE;
                    break;
                case Message_Type.MSG_WARNING: // Bright Yellow
                    prefix = "[Warning]";
                    color = Color.CL_YELLOW;
                    break;
                case Message_Type.MSG_DEBUG: // Bright Cyan, important stuff!
                    prefix = "[Debug] => ";
                    color = Color.CL_CYAN;
                    break;
                case Message_Type.MSG_ERROR: // Bright Red  (Regular errors)
                    prefix = "[Error]";
                    color = Color.CL_RED;
                    break;
                case Message_Type.MSG_FATALERROR: // Bright Red (Fatal errors, abort(); If possible)
                    prefix = "";
                    color = Color.CL_RED;
                    break;
                default:
                    Error(String.Format("In function _vShowMessage() -> Invalid flag ({0}) passed.", flag));
                    return false;
            }

            Console.ForegroundColor = (ConsoleColor)Color.CL_GREY;
            char[] letters = prefix.ToCharArray();
            foreach (char c in letters)
            {
                Console.ForegroundColor = (ConsoleColor)color;
                Console.Write(c);
            }
            Console.ForegroundColor = (ConsoleColor)Color.CL_GREY;
            if (prefix != "")
                Console.Write(" ");

            letters = message.ToCharArray();
            foreach (char c in letters)
                Console.Write(c);

            Console.WriteLine();

            return true;
        }
        public static void Status(string format)
        {
            WriteColoredMessage(ConsoleColor.Green, DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss] "));
            WriteColoredMessage(ConsoleColor.Green, "[STATUS] ");
            WriteColoredMessage(ConsoleColor.White, format);
            Console.Write("\r");
        }
        public static void Packet(string format, byte Code = 0)
        {
            if (Code == 0)
            {
                WriteColoredMessage(ConsoleColor.White, DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss] "));
                WriteColoredMessage(ConsoleColor.Cyan, "[PACKET_UN] ");
                WriteColoredMessage(ConsoleColor.White, format);
                Console.Write("\r");
            }
            else
            {
                WriteColoredMessage(ConsoleColor.White, DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss] "));
                WriteColoredMessage(ConsoleColor.Cyan, "[PACKET] ");
                WriteColoredMessage(ConsoleColor.White, format);
                Console.Write("\r");
            }
        }
        public static void DebugPacketUnknown(byte type, string message)
        {
            _vShowMessage((Message_Type)type, message);
        }

        public static void Information(string message)
        {
            _vShowMessage(Message_Type.MSG_INFORMATION, message);
        }

        public static void Notice(string message)
        {
            _vShowMessage(Message_Type.MSG_NOTICE, message);
        }

        public static void Warning(string message)
        {
            _vShowMessage(Message_Type.MSG_WARNING, message);
        }

        public static void Debug(string message)
        {
            _vShowMessage(Message_Type.MSG_DEBUG, message);
        }

        public static void Error(string message)
        {
            _vShowMessage(Message_Type.MSG_ERROR, message);
        }

        public static void FatalError(string message)
        {
            _vShowMessage(Message_Type.MSG_FATALERROR, DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss] ") + message);
        }
        public static void WriteLine(string format)
        {
            Console.WriteLine(DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss] ") + format);
        }
        public static void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(string.Format(DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss] ") + format, args));
        }
        public static void WriteLine(string format, ConsoleColor Fore = ConsoleColor.White, params object[] args)
        {
            Console.ResetColor();
            Console.ForegroundColor = Fore;
            Console.WriteLine(string.Format(format, args));
            Console.ResetColor();
        }
        /// <summary>
        /// Format = [2020/07/08 13:06:35] [SERVER_START]: PORT 10103
        /// </summary>
        /// <param name="msg">Exemple: [SERVER_START]</param>
        /// <param name="Fore">Exemple: ConsoleColor.White</param>
        public static void WriteLine(string msg, ConsoleColor Fore = ConsoleColor.White)
        {
            Console.ResetColor();
            Console.ForegroundColor = Fore;
            Console.WriteLine(DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss] ") + msg);
            Console.ResetColor();
        }
        public static void Write(string msg = "", ConsoleColor Fore = ConsoleColor.White)
        {
            Console.ResetColor();
            Console.ForegroundColor = Fore;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        private static void WriteColoredMessage(ConsoleColor color, string message, params object[] args)
        {
            Console.ResetColor();
            Console.ForegroundColor = color;
            Console.Write(string.Format(message, args));
            Console.ResetColor();
        }
    }
}
