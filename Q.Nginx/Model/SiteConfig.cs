using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Q.Nginx.Entity
{
    public class SiteConfig
    {
        /// <summary>
        /// 站点名称
        /// </summary>
        public string SiteName { set; get; }
        /// <summary>
        /// 绑定端口
        /// </summary>
        public string Port { set; get; }
        /// <summary>
        /// 绑定域名
        /// </summary>
        public List<string> HostNames { set; get; }
        /// <summary>
        /// 站点主目录
        /// </summary>
        public string RootPath { set; get; }

        public List<string> IndexPages { set; get; } = new List<string>() { "index.html", "index.htm", "index.php", "default.html" };
        /// <summary>
        /// 反代目标URL
        /// </summary>
        public string Proxy_Pass { set; get; }
        /// <summary>
        /// 反代发送域名
        /// </summary>
        public string Proxy_Host { set; get; } = "$host";

        /// <summary>
        /// 配置文件路径
        /// </summary>
        public string Config_Path { set; get; }

        /// <summary>
        /// 维护中
        /// </summary>
        public string Maintaining { set; get; }

        public string GetStringStr()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#").Append(SiteName).AppendLine();
            sb.AppendLine("server");
            sb.AppendLine("{");
            sb.Append("\t").Append("#基本配置-Start").AppendLine();
            sb.Append("\t").Append("listen ").Append(Port).Append(";").AppendLine();
            sb.Append("\t").Append("server_name ").Append(string.Join(" ", HostNames)).Append(";").AppendLine();
            sb.Append("\t").Append("index ").Append(string.Join(" ", IndexPages)).Append(";").AppendLine();
            sb.Append("\t").Append("root ").Append(RootPath).Append(";").AppendLine();
            sb.Append("\t").Append("#基本配置-End").AppendLine();

            sb.Append("\t").Append("#错误页配置-Start").AppendLine();
            sb.Append("\t").Append("error_page 404 ").Append(Path.Combine(Utils.HtmlDir, "404.html")).Append(";").AppendLine();
            sb.Append("\t").Append("error_page 502 ").Append(Path.Combine(Utils.HtmlDir, "502.html")).Append(";").AppendLine();
            sb.Append("\t").Append("#错误页配置-End").AppendLine();
            if (string.IsNullOrEmpty(Maintaining))
            {

                sb.Append("\t").Append("#禁止访问的文件或目录-Start").AppendLine();
                sb.Append("\t").Append(@"location ~ ^/ (\.user.ini |\.htaccess |\.git |\.svn |\.project | LICENSE | README.md) ").AppendLine();
                sb.Append("\t").Append("{").AppendLine();
                sb.Append("\t").Append("\t").Append("return 404;").AppendLine();
                sb.Append("\t").Append("}").AppendLine();
                sb.Append("\t").Append("#禁止访问的文件或目录-End").AppendLine();

                if (string.IsNullOrEmpty(Proxy_Pass))
                {

                    sb.Append("\t").Append("#图片资源缓存配置-Start").AppendLine();
                    sb.Append("\t").Append(@"location ~ .*\.(gif|jpg|jpeg|png|bmp|swf)$").AppendLine();
                    sb.Append("\t").Append("{").AppendLine();
                    sb.Append("\t").Append("\t").Append(" expires  30d;").AppendLine();
                    sb.Append("\t").Append("\t").Append("error_log off;").AppendLine();
                    sb.Append("\t").Append("\t").Append("access_log off;").AppendLine();
                    sb.Append("\t").Append("}").AppendLine();
                    sb.Append("\t").Append("#图片资源缓存配置-End").AppendLine();

                    sb.Append("\t").Append("#样式脚本资源缓存配置-Start").AppendLine();
                    sb.Append("\t").Append(@"location ~ .*\.(js|css)?$").AppendLine();
                    sb.Append("\t").Append("{").AppendLine();
                    sb.Append("\t").Append("\t").Append(" expires  12h;").AppendLine();
                    sb.Append("\t").Append("\t").Append("error_log off;").AppendLine();
                    sb.Append("\t").Append("\t").Append("access_log off;").AppendLine();
                    sb.Append("\t").Append("}").AppendLine();
                    sb.Append("\t").Append("#样式脚本资源缓存配置-End").AppendLine();

                }
                else
                {

                    sb.Append("\t").Append("#反向代理配置(通用)-Start").AppendLine();
                    sb.Append("\t").Append(@"location / ").AppendLine();
                    sb.Append("\t").Append("{").AppendLine();
                    sb.Append("\t").Append("\t").Append("proxy_pass  ").Append(Proxy_Pass).Append(";").AppendLine();
                    sb.Append("\t").Append("\t").Append("proxy_set_header Host ").Append(Proxy_Host).Append(";").AppendLine();
                    sb.Append("\t").Append("\t").Append("proxy_set_header X-Real-IP $remote_addr").Append(";").AppendLine();
                    sb.Append("\t").Append("\t").Append("proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for").Append(";").AppendLine();
                    sb.Append("\t").Append("\t").Append("proxy_set_header REMOTE-HOST $remote_addr").Append(";").AppendLine();
                    sb.Append("\t").Append("\t").Append("expires 12h").Append(";").AppendLine();
                    sb.Append("\t").Append("}").AppendLine();
                    sb.Append("\t").Append("#反向代理配置(通用)-End").AppendLine();

                    sb.Append("\t").Append("#反向代理配置(动态页面)-Start").AppendLine();
                    sb.Append("\t").Append(@"location ~ .*\.(php|jsp|cgi|asp|aspx|flv|swf|xml)?$").AppendLine();
                    sb.Append("\t").Append("{").AppendLine();
                    sb.Append("\t").Append("\t").Append("proxy_pass  ").Append(Proxy_Pass).Append(";").AppendLine();
                    sb.Append("\t").Append("\t").Append("proxy_set_header Host ").Append(Proxy_Host).Append(";").AppendLine();
                    sb.Append("\t").Append("\t").Append("proxy_set_header X-Real-IP $remote_addr").Append(";").AppendLine();
                    sb.Append("\t").Append("\t").Append("proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for").Append(";").AppendLine();
                    sb.Append("\t").Append("\t").Append("proxy_set_header REMOTE-HOST $remote_addr").Append(";").AppendLine();
                    sb.Append("\t").Append("}").AppendLine();
                    sb.Append("\t").Append("#反向代理配置(动态页面)-End").AppendLine();

                    sb.Append("\t").Append("#反向代理配置(静态资源)-Start").AppendLine();
                    sb.Append("\t").Append(@"location ~ .*\.(html|htm|png|gif|jpeg|jpg|bmp|js|css)?$").AppendLine();
                    sb.Append("\t").Append("{").AppendLine();
                    sb.Append("\t").Append("\t").Append("proxy_pass  ").Append(Proxy_Pass).Append(";").AppendLine();
                    sb.Append("\t").Append("\t").Append("proxy_set_header Host ").Append(Proxy_Host).Append(";").AppendLine();
                    sb.Append("\t").Append("\t").Append("proxy_set_header X-Real-IP $remote_addr").Append(";").AppendLine();
                    sb.Append("\t").Append("\t").Append("proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for").Append(";").AppendLine();
                    sb.Append("\t").Append("\t").Append("proxy_set_header REMOTE-HOST $remote_addr").Append(";").AppendLine();
                    sb.Append("\t").Append("\t").Append("expires 24h").Append(";").AppendLine();
                    sb.Append("\t").Append("}").AppendLine();
                    sb.Append("\t").Append("#反向代理配置(静态资源)-End").AppendLine();

                }
            }
            else
            {
                sb.Append("\t").Append("#维护状态-Start").AppendLine();
                sb.Append("\t").Append(@"location /").AppendLine();
                sb.Append("\t").Append("{").AppendLine();
                sb.Append("\t").Append("\t").Append("default_type application/json").AppendLine();
                sb.Append("\t").Append("\t").Append("add_header Content-Type 'application/json; charset=utf-8'").AppendLine();
                sb.Append("\t").Append("\t").Append(@"return 200 '{""ResCode"":-1503,""ResDesc"":""服务器升级维护，请稍后再试"",""ResData"":{""MaintenanceMsg"":""" + Maintaining + @"""}}'").AppendLine();
                sb.Append("\t").Append("\t").Append("if ($request_method !~* POST) {").AppendLine();
                sb.Append("\t").Append("\t").Append("try_files $uri ").Append(Path.Combine(Utils.HtmlDir, "Maintenance.html")).Append(";").AppendLine();
                sb.Append("\t").Append("\t").Append("}").AppendLine();
                sb.Append("\t").Append("}").AppendLine();

                sb.Append("\t").Append("#维护状态-End").AppendLine();
            }
            sb.Append("\t").Append("#请求日志-Start").AppendLine();
            sb.Append("\t").Append("access_log ").Append(Path.Combine(Utils.LogsDir, SiteName + ".log")).AppendLine();
            sb.Append("\t").Append("error_log ").Append(Path.Combine(Utils.LogsDir, SiteName + ".error.log")).AppendLine();
            sb.Append("\t").Append("#请求日志-End").AppendLine();
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
