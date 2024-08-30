using System.Collections.Concurrent;

namespace Ike.AspNet.RateLimit
{
    /// <summary>
    /// 速率限制配置服务，实现 IRateLimitConfigService 接口
    /// </summary>
    public class RateLimitConfigService : IRateLimitConfigService
    {
        // 使用 ConcurrentDictionary 来存储路径和它们的速率限制配置，以保证线程安全
        private readonly ConcurrentDictionary<string, PathConfig> _pathsToLimit = new();

        /// <summary>
        /// 获取所有需要限制的路径和配置
        /// </summary>
        /// <returns>包含路径和对应配置的字典</returns>
        public IDictionary<string, PathConfig> GetPathsToLimit() => _pathsToLimit;

        /// <summary>
        /// 添加需要限制的路径和配置
        /// </summary>
        /// <param name="pathConfig">包含路径和配置的对象</param>
        public void AddPath(PathConfig pathConfig) => _pathsToLimit[pathConfig.Name] = pathConfig;

        /// <summary>
        /// 移除指定名称的路径配置
        /// </summary>
        /// <param name="name">路径配置的名称</param>
        public void RemovePath(string name) => _pathsToLimit.TryRemove(name, out _);

        /// <summary>
        /// 更新指定路径的配置
        /// </summary>
        /// <param name="pathConfig">包含更新后路径和配置的对象</param>
        public void UpdatePath(PathConfig pathConfig) => _pathsToLimit[pathConfig.Name] = pathConfig;

        /// <summary>
        /// 批量所有路径配置
        /// </summary>
        /// <param name="paths">包含多个路径和对应配置的字典</param>
        public void UpdatePaths(IDictionary<string, PathConfig> paths)
        {
            // 清空现有的路径配置
            _pathsToLimit.Clear();
            // 添加新的路径配置
            foreach (var kvp in paths)
            {
                _pathsToLimit[kvp.Key] = kvp.Value;
            }
        }
    }
}
