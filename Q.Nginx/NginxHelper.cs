using Q.Lib;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace Q.Nginx
{
    public class NginxHelper
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="nginx_path"></param>
        public static void Init(string nginx_path, string DefaultPort = "5800")
        {
            Utils.DefaultPort = DefaultPort;
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
            nginx_conf = nginx_conf.Replace("↱Logs↲", Utils.LogsDir).Replace("↱BasePath↲", Utils.BaseDirectory).Replace("↱vHost↲", Utils.vHostDir).Replace("↱Conf↲", Utils.ConfDir).Replace("↱DefPort↲", DefaultPort);



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
                wr.Write("start /D \"" + dirpath + "\" nginx -c " + Path.Combine(Utils.ConfDir, "nginx.conf"));
            }
            using (var wr = File.CreateText(Path.Combine(Utils.BaseDirectory, "stop.bat")))
            {
                wr.Write("start /D \"" + dirpath + "\" nginx -s stop");
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
            Console.WriteLine("初始化完成");
        }


        public static void Start()
        {
            RunShell.Start(Path.Combine(Utils.BaseDirectory, "start.bat")).Run();
            Console.WriteLine("服务已启动");
        }

        public static void Stop()
        {
            RunShell.Start(Path.Combine(Utils.BaseDirectory,"stop.bat")).Run();
            Console.WriteLine("服务正在停止");
        }

        public static void Reload()
        {
            RunShell.Start(Path.Combine(Utils.BaseDirectory,"reload.bat")).Run();
            Console.WriteLine("服务正在重新载入配置");
        }

        public static void CheckStatus()
        {
            using (WebClient wc = new WebClient())
            {
                string status_Str = string.Empty;
                try
                {
                    status_Str = wc.DownloadString("http://127.0.0.1:" + Utils.DefaultPort + "/ngx_status");


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
