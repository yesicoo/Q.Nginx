using Q.Nginx;
using Q.Nginx.Entity;
using System;
using System.Collections.Generic;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            NginxHelper.Init(@"D:\Tools\nginx\nginx.exe");

            NginxHelper.Start();

            NginxHelper.CheckStatus();


            Console.ReadLine();

            SiteConfig sc = new SiteConfig();
           // sc.HostNames = new List<string>() { "xuqing.me", "niubi.me" };
            sc.Port = "5808";
            sc.RootPath = "D://niubi/haha";
            sc.SiteName = "TestSite";
            sc.Proxy_Pass = "http://xuqing.me";
            NginxHelper.AddSite(sc);
            Console.WriteLine("添加站点：" + sc.SiteName);

            Console.ReadLine();


            NginxHelper.Stop();
        }
    }
}
