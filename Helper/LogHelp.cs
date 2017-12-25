using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderForSis001.Helper
{
    public class LogHelp
    {
        static bool IsLog = true;
        public static void Log(string str,bool islog=false)
        {
            if (IsLog && islog)
                Console.WriteLine(str, null, null);
        }
        public static void Log(string str, object arg, bool islog = false)
        {
            if (IsLog && islog)
                Console.WriteLine(str, arg);
        }
        public static void Log(string str, object arg1, object arg2, bool islog = false)
        {
            if (IsLog && islog)
                Console.WriteLine(str, arg1, arg2);
        }
    }
}
