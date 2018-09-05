namespace Q.Nginx
{
    public class NginxStatus
    {
        /// <summary>
        /// 活动连接数
        /// </summary>
        public long ActiveConnections { set; get; }
        /// <summary>
        /// 总连接数
        /// </summary>
        public long SumConnections { set; get; }
        /// <summary>
        /// 处理完成连接数
        /// </summary>
        public long FinishedConnections { set; get; }

        /// <summary>
        /// 总请求数
        /// </summary>
        public long Requests { set; get; }

        /// <summary>
        /// 正在读取请求数量
        /// </summary>
        public long Reading { set; get; }

        /// <summary>
        /// 正在写入请求数量
        /// </summary>
        public long Writing { set; get; }

        /// <summary>
        /// 空闲客户端数量
        /// </summary>
        public long Waiting { set; get; }
    }
}
