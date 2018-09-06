using Q.Lib;
using Q.Nginx.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace Q.Nginx
{
    public class NginxHelper
    {

        static List<SiteConfig> scs = null;
        static string SiteConfPath = Path.Combine(Utils.BaseDirectory, "Sites.jb");
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

            Utils.HtmlDir = Path.Combine(Utils.BaseDirectory, "html");
            if (!Directory.Exists(Utils.HtmlDir))
            {
                Directory.CreateDirectory(Utils.HtmlDir);
            }

            Utils.NginxPath = nginx_path;
            if (!File.Exists(Utils.NginxPath))
            {
                throw new Exception("not find nginx!");
            }

            var nginx_conf = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "nginx.conf"));
            nginx_conf = nginx_conf.Replace("↱Logs↲", Utils.LogsDir).Replace("↱BasePath↲", Utils.BaseDirectory).Replace("↱vHost↲", Utils.vHostDir).Replace("↱Conf↲", Utils.ConfDir).Replace("↱DefPort↲", DefaultPort);



            using (var wr = File.CreateText(Path.Combine(Utils.ConfDir, "nginx.conf")))
            {
                wr.Write(nginx_conf);
            }

            using (var wr = File.CreateText(Path.Combine(Utils.ConfDir, "mime.types")))
            {
                wr.Write(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "mime.types")));
            }


            foreach (var item in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "html")))
            {
                FileInfo file = new FileInfo(item);
                using (var wr = File.CreateText(Path.Combine(Utils.HtmlDir, file.Name)))
                {
                    wr.Write(File.ReadAllText(item));
                }
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

            SiteConfig sc = new SiteConfig();
            sc.HostNames = new System.Collections.Generic.List<string>() { "xuqing.me", "niubi.me" };
            sc.Port = "80";
            sc.RootPath = "D://niubi/haha";
            sc.SiteName = "测试站点";
            sc.Maintaining = "维护中";
            string str = sc.GetStringStr();



        }

        public static void Stop()
        {
            RunShell.Start(Path.Combine(Utils.BaseDirectory, "stop.bat")).Run();
            Console.WriteLine("服务正在停止");
        }

        public static void Reload()
        {
            RunShell.Start(Path.Combine(Utils.BaseDirectory, "reload.bat")).Run();
            Console.WriteLine("服务正在重新载入配置");
        }




        public static void AddSite(SiteConfig sc)
        {

        }


        public static NginxStatus CheckStatus()
        {
            using (WebClient wc = new WebClient())
            {
                string status_Str = string.Empty;
                try
                {
                    status_Str = wc.DownloadString("http://127.0.0.1:" + Utils.DefaultPort + "/ngx_status");

                    NginxStatus ns = new NginxStatus();
                    var ss = status_Str.Split('\n');
                    ns.ActiveConnections = long.Parse(ss[0].Replace("Active connections:", "").Trim());
                    var ss_2 = ss[2].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    ns.SumConnections = long.Parse(ss_2[0]);
                    ns.FinishedConnections = long.Parse(ss_2[1]);
                    ns.Requests = long.Parse(ss_2[2]);
                    var ss_3 = ss[3].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    ns.Reading = long.Parse(ss_3[1]);
                    ns.Writing = long.Parse(ss_3[3]);
                    ns.Waiting = long.Parse(ss_3[5]);
                    //Active connections: 1
                    //server accepts handled requests
                    // 1 1 1
                    //Reading: 0 Writing: 1 Waiting: 0

                    return ns;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }






        private  static void ReadSiteConfig()
        {
            if (File.Exists(SiteConfPath))
            {
                scs = JsonHelper.JsonDeserialize<List<SiteConfig>>(File.ReadAllText(SiteConfPath));
                if (scs == null)
                {
                    scs = new List<SiteConfig>();
                }
            }
        }

        private static void WriteSiteConfig()
        {
            using (var wr = File.CreateText(SiteConfPath))
            {
                wr.Write(JsonHelper.JsonSerializer(scs));
            }
        }
    }
}
