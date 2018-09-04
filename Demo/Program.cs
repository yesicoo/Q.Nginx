using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Q.Nginx.NginxHelper.Init(@"D:\Tools\nginx\nginx.exe");

            Console.ReadLine();
        }
    }
}
