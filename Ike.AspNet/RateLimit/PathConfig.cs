namespace Ike.AspNet.RateLimit
{
    /// <summary>
    /// 表示路径及其速率限制配置的类
    /// </summary>
    public class PathConfig
    {
        /// <summary>
        /// 获取或设置路径配置的名称
        /// </summary>
        public string Name { get; set; } = "*";

        /// <summary>
        /// 获取或设置需要限制的路径
        /// </summary>
        public string Path { get; set; } = "/*";

        /// <summary>
        /// 获取或设置时间窗口（秒）
        /// </summary>
        public int TimeWindowInSeconds { get; set; }

        /// <summary>
        /// 获取或设置时间窗口内的最大请求数
        /// </summary>
        public int MaxRequests { get; set; }
    }
}
