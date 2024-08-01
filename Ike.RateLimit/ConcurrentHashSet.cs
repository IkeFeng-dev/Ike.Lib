using System.Collections.Concurrent;
using System.Collections;

namespace Ike.RateLimit
{
    /// <summary>
    /// 线程安全的HashSet实现
    /// </summary>
    /// <typeparam name="T">集合中项的类型</typeparam>
    public class ConcurrentHashSet<T> : IEnumerable<T> where T : notnull
    {
        // 使用ConcurrentDictionary实现线程安全的HashSet
        private readonly ConcurrentDictionary<T, byte> _dict = new();

        /// <summary>
        /// 添加一个项到集合中
        /// </summary>
        /// <param name="item">需要添加的项</param>
        /// <returns>如果项被成功添加，则返回true；如果项已存在，则返回false</returns>
        public bool Add(T item) => _dict.TryAdd(item, 0);

        /// <summary>
        /// 尝试移除集合中的一个项
        /// </summary>
        /// <param name="item">需要移除的项</param>
        /// <returns>如果项被成功移除，则返回true；否则返回false</returns>
        public bool TryRemove(T item) => _dict.TryRemove(item, out _);

        /// <summary>
        /// 清空集合中的所有项
        /// </summary>
        public void Clear()
        {
            foreach (var key in _dict.Keys)
            {
                _dict.TryRemove(key, out _);
            }
        }

        /// <summary>
        /// 获取集合中的所有项
        /// </summary>
        /// <returns>集合中的所有项</returns>
        public IEnumerator<T> GetEnumerator() => _dict.Keys.GetEnumerator();

        /// <summary>
        /// 获取集合中的所有项（非泛型实现）
        /// </summary>
        /// <returns>集合中的所有项</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
