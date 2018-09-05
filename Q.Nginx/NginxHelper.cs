using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Q.Nginx
{
    public class NginxHelper
    {
        public static void Init(string nginx_path)
        {
            if (!Directory.Exists(Utils.BaseDirectory))
            {
                Directory.CreateDirectory(Utils.BaseDirectory);
            }

            Utils.vHostDir = Path.Combine(Utils.BaseDirectory, "vHost");
            if (!Directory.Exists(Utils.vHostDir))
            {
                Directory.CreateDirectory(Utils.vHostDir);
            }

            Utils.LogsDir = Path.Combine(Utils.BaseDirectory, "Logs");
            if (!Directory.Exists(Utils.LogsDir))
            {
                Directory.CreateDirectory(Utils.LogsDir);
            }

            Utils.ConfDir = Path.Combine(Utils.BaseDirectory, "conf");
            if (!Directory.Exists(Utils.ConfDir))
            {
                Directory.CreateDirectory(Utils.ConfDir);
            }

            Utils.NginxPath = nginx_path;
            if (!File.Exists(Utils.NginxPath))
            {
                throw new Exception("not find nginx!");
            }

            var nginx_conf = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TemplateConf", "nginx.conf"));
            nginx_conf = nginx_conf.Replace("↱Logs↲", Utils.LogsDir).Replace("↱BasePath↲", Utils.BaseDirectory).Replace("↱vHost↲", Utils.vHostDir).Replace("↱Conf↲", Utils.ConfDir);



            using (var wr = File.CreateText(Path.Combine(Utils.ConfDir, "nginx.conf")))
            {
                wr.Write(nginx_conf);
            }

            using (var wr = File.CreateText(Path.Combine(Utils.ConfDir, "mime.types")))
            {
                wr.Write(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TemplateConf", "mime.types")));
            }




#if NETSTANDARD2_0
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
#endif
                string dirpath = new FileInfo(Utils.NginxPath).DirectoryName;
                using (var wr = File.CreateText(Path.Combine(Utils.BaseDirectory, "start.bat")))
                {
                    wr.Write("start /D \"" + dirpath + "\" nginx -c "+ Path.Combine(Utils.ConfDir, "nginx.conf"));
                }
                using (var wr = File.CreateText(Path.Combine(Utils.BaseDirectory, "quit.bat")))
                {
                    wr.Write("start /D \"" + dirpath + "\" nginx -s quit");
                }
                using (var wr = File.CreateText(Path.Combine(Utils.BaseDirectory, "reload.bat")))
                {
                    wr.Write("start /D \"" + dirpath + "\" nginx -s reload");
                }
#if NETSTANDARD2_0
            }
            else
            {

            }
#endif
        }
    }
}
