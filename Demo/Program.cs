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

            INPUT:
            Console.WriteLine("输入Nginx路径：");

            var path = Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine("路径为："+ path);
            Console.WriteLine("确认？(y/n)  y");
            var ok = Console.ReadLine();
            if(ok=="n" || ok == "N")
            {
                Console.WriteLine();
                goto INPUT;
            }

            if (!System.IO.File.Exists(path))
            {
                Console.WriteLine("文件不存在，请重新录入");
                Console.ReadLine();
                goto INPUT;
            }


            Console.WriteLine("正在初始化");
            NginxHelper.Init(path);

            Console.WriteLine("开始启动");
            NginxHelper.Start();

           var status = NginxHelper.CheckStatusStr();
            Console.WriteLine(status);

            Console.ReadLine();

            SiteConfig sc = new SiteConfig();
           // sc.HostNames = new List<string>() { "xuqing.me", "niubi.me" };
            sc.Port = "5808";
            sc.RootPath = "D://niubi/haha";
            sc.SiteName = "TestSite";
            sc.Proxy_Pass = "http://xuqing.me";
            NginxHelper.AddEditSite(sc);
            Console.WriteLine("添加站点：" + sc.SiteName);

            Console.ReadLine();


            NginxHelper.Stop();
        }
    }
}
