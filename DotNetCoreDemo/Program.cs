using Q.Nginx;
using System;

namespace DotNetCoreDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            INPUT:
            Console.WriteLine("输入Nginx路径：");

            var path = Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine("路径为：" + path);
            Console.WriteLine("确认？(y/n)  y");
            var ok = Console.ReadLine();
            if (ok != null || ok != "" || ok != "y")
            {
                Console.WriteLine();
                goto INPUT;
            }

            if (System.IO.File.Exists(path))
            {
                Console.WriteLine("文件不存在，请重新录入");
                Console.ReadLine();
                goto INPUT;
            }


            Console.WriteLine("正在初始化");
            NginxHelper.Init(@"D:\Tools\nginx\nginx.exe");

            Console.WriteLine("开始启动");
            NginxHelper.Start();

            var status = NginxHelper.CheckStatusStr();
            Console.WriteLine(status);

            Console.ReadLine();
        }
    }
}
