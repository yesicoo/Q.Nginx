using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Q.Nginx
{
   public class Utils
    {
        public static string BaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Q_Nginx");
        internal static string NginxPath;
        internal static string vHostDir;
        internal static string LogsDir;
        internal static string MainConfPath;
        internal static string ConfDir;
    }
}
