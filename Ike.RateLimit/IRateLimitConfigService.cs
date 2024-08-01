namespace Ike.RateLimit
{
    /// <summary>
    /// 速率限制配置服务接口，定义了管理速率限制路径配置的方法
    /// </summary>
    public interface IRateLimitConfigService
    {
        /// <summary>
        /// 获取所有需要限制的路径和配置
        /// </summary>
        /// <returns>包含路径和对应配置的字典</returns>
        IDictionary<string, PathConfig> GetPathsToLimit();

        /// <summary>
        /// 添加需要限制的路径和配置
        /// </summary>
        /// <param name="pathConfig">包含路径和配置的对象</param>
        void AddPath(PathConfig pathConfig);

        /// <summary>
        /// 移除指定名称的路径配置
        /// </summary>
        /// <param name="name">路径配置的名称</param>
        void RemovePath(string name);

        /// <summary>
        /// 更新指定路径的配置
        /// </summary>
        /// <param name="pathConfig">包含更新后路径和配置的对象</param>
        void UpdatePath(PathConfig pathConfig);

        /// <summary>
        /// 更新所有路径配置
        /// </summary>
        /// <param name="paths">包含多个路径和对应配置的字典</param>
        void UpdatePaths(IDictionary<string, PathConfig> paths);
    }
}
