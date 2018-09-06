using Q.Lib;
using Q.Nginx.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

namespace Q.Nginx
{
    public class NginxHelper
    {

        static List<SiteConfig> scs = null;
        static string SiteConfPath = Path.Combine(Utils.BaseDirectory, "Sites.jb");

        #region 初始化
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
            ReadSiteConfig();
            CreateMainConf();

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
        #endregion

        #region 启动
        /// <summary>
        /// 启动
        /// </summary>
        public static void Start()
        {
#if NETSTANDARD2_0
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                RunShell.Start(Path.Combine(Utils.BaseDirectory, "start.bat")).Run();
            }
            else
            {
                RunShell.Start(Utils.NginxPath).AddArguments("-c").AddArguments(Path.Combine(Utils.ConfDir, "nginx.conf")).Run();
            }
#else
            RunShell.Start(Path.Combine(Utils.BaseDirectory, "start.bat")).Run();
#endif

            Console.WriteLine("服务已启动");

        }
        #endregion

        #region 停止
        /// <summary>
        /// 停止
        /// </summary>
        public static void Stop()
        {
#if NETSTANDARD2_0
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                RunShell.Start(Path.Combine(Utils.BaseDirectory, "stop.bat")).Run();
            }
            else
            {
                RunShell.Start(Utils.NginxPath).AddArguments("-s").AddArguments("stop").Run();
            }
#else
            RunShell.Start(Path.Combine(Utils.BaseDirectory, "stop.bat")).Run();
#endif
            Console.WriteLine("服务正在停止");
        }
        #endregion

        #region 重新载入
        /// <summary>
        /// 重新载入
        /// </summary>
        public static void Reload()
        {
#if NETSTANDARD2_0
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                RunShell.Start(Path.Combine(Utils.BaseDirectory, "reload.bat")).Run();
            }
            else
            {
                RunShell.Start(Utils.NginxPath).AddArguments("-s").AddArguments("reload").Run();
            }
#else
            RunShell.Start(Path.Combine(Utils.BaseDirectory, "reload.bat")).Run();
#endif
            Console.WriteLine("服务正在重新载入配置");
        }
        #endregion

        #region 添加编辑站点配置
        /// <summary>
        /// 添加站点配置
        /// </summary>
        /// <param name="sc"></param>
        public static void AddEditSite(SiteConfig sc)
        {
            scs.RemoveAll(x => x.SiteName == sc.SiteName);
            scs.Add(sc);
            sc.Config_Path = Path.Combine(Utils.vHostDir, sc.SiteName.Replace(" ", "_") + ".conf");
            var configStr = sc.GetStringStr();
            using (var wr = File.CreateText(sc.Config_Path))
            {
                wr.Write(configStr);
            }
            WriteSiteConfig();
            CreateMainConf();
            Reload();
        }
        #endregion

        #region 获取站点信息
        /// <summary>
        /// 获取门店信息
        /// </summary>
        /// <param name="sitename"></param>
        /// <returns></returns>

        public static SiteConfig GetSite(string sitename)
        {
            return scs.FirstOrDefault(x => x.SiteName == sitename);
        } 
        #endregion

        #region 状态检查

        /// <summary>
        /// 状态检查
        /// </summary>
        /// <returns></returns>
        public static string CheckStatusStr()
        {
            string status_Str = string.Empty;

            using (WebClient wc = new WebClient())
            {
                try
                {
                    status_Str = wc.DownloadString("http://127.0.0.1:" + Utils.DefaultPort + "/ngx_status");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
            return status_Str;
        }
        public static NginxStatus CheckStatus()
        {

            try
            {
                var status_Str = CheckStatusStr();

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

        #endregion

        #region 读取站点配置
        /// <summary>
        /// 读取配置
        /// </summary>
        private static void ReadSiteConfig()
        {
            if (File.Exists(SiteConfPath))
            {
                scs = JsonHelper.JsonDeserialize<List<SiteConfig>>(File.ReadAllText(SiteConfPath));
            }
            if (scs == null)
            {
                scs = new List<SiteConfig>();
            }
        }
        #endregion

        #region 保存站点配置

        private static void WriteSiteConfig()
        {
            using (var wr = File.CreateText(SiteConfPath))
            {
                wr.Write(JsonHelper.JsonSerializer(scs));
            }
        }
        #endregion

        #region 生成主配置文件
        /// <summary>
        /// 生成主配置文件
        /// </summary>
        private static void CreateMainConf()
        {
            var nginx_conf = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "nginx.conf"));
            nginx_conf = nginx_conf.Replace("↱Logs↲", Utils.LogsDir).Replace("↱BasePath↲", Utils.BaseDirectory).Replace("↱Conf↲", Utils.ConfDir).Replace("↱DefPort↲", Utils.DefaultPort);
            StringBuilder sb = new StringBuilder();

            if (scs != null)
            {
                foreach (var item in scs)
                {
                    sb.Append("\t").Append("include ").Append(item.Config_Path).Append(";").AppendLine();
                }
            }
            nginx_conf = nginx_conf.Replace("↱vHost↲", sb.ToString());

            using (var wr = File.CreateText(Path.Combine(Utils.ConfDir, "nginx.conf")))
            {
                wr.Write(nginx_conf);
            }

        }
        #endregion
    }
}
