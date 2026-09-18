using System;
using System.Collections.Generic;
using System.Linq;

namespace Ike.Standard
{

    /// <summary>
    /// 防抖辅助类
    /// </summary>
    /// <remarks>
    /// [<see langword="2025年12月26日14:58:17" />]
    /// </remarks>
    public class Debouncer
    {
        /// <summary>
        /// 记录防抖器时间
        /// </summary>
        private readonly Dictionary<string, DateTime> _debounceTimers = new Dictionary<string, DateTime>();
        /// <summary>
        /// 记录防抖器间隔
        /// </summary>
        private readonly Dictionary<string, int> _debounceIntervals = new Dictionary<string, int>();
        /// <summary>
        /// 防抖器锁
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// 创建一个防抖器
        /// </summary>
        /// <param name="id">默认防抖器唯一标识</param>
        /// <param name="millisecond">防抖时间间隔(毫秒)</param>
        public Debouncer(string id, int millisecond)
        {
            AddDebouncer(id, millisecond);
        }

        /// <summary>
        /// 添加或更新防抖器
        /// </summary>
        /// <param name="id">防抖器唯一标识</param>
        /// <param name="millisecond">防抖时间间隔(毫秒)</param>
        /// <returns>true表示新增，false表示更新</returns>
        public bool AddDebouncer(string id, int millisecond)
        {
            if (millisecond <= 0)
                throw new ArgumentException("The anti-shake time must be greater than 0", nameof(millisecond));

            lock (_lock)
            {
                bool isNew = !_debounceIntervals.ContainsKey(id);
                _debounceIntervals[id] = millisecond;
                if (isNew)
                {
                    _debounceTimers[id] = DateTime.MinValue;
                }

                return isNew;
            }
        }

        /// <summary>
        /// 移除防抖器
        /// </summary>
        /// <param name="id">要移除的防抖器ID,默认移除第一个</param>
        /// <returns><see langword="true"/> 表示移除成功，<see langword="false"/>表示ID不存在</returns>
        public bool RemoveDebouncer(string id = default)
        {
            lock (_lock)
            {
                if (id == default)
                {
                    id = _debounceIntervals.First().Key;
                }
                bool existed = _debounceIntervals.Remove(id);
                if (existed)
                {
                    _debounceTimers.Remove(id);
                }
                return existed;
            }
        }


        /// <summary>
        /// 查询第一个防抖器
        /// </summary>
        /// <returns>
        /// <see langword="true"/>: 可以触发(不在防抖时间内) <br/>
        /// <see langword="false"/>: 不可触发(在防抖时间内)
        /// </returns>
        /// <remarks>
        /// [<see langword="2026-9-18 10:46:49" />]
        /// </remarks>
        public bool QueryDebouncer()
        {
            return QueryDebouncer(default);
        }


        /// <summary>
        /// 查询防抖器状态
        /// </summary>
        /// <param name="id">防抖器ID</param>
        /// <returns>
        /// <see langword="true"/>: 可以触发(不在防抖时间内) <br/>
        /// <see langword="false"/>: 不可触发(在防抖时间内)
        /// </returns>
        public bool QueryDebouncer(string id)
        {
            lock (_lock)
            {
                if (id == default)
                {
                    id = _debounceIntervals.First().Key;
                }
                if (!_debounceIntervals.TryGetValue(id, out var interval))
                    throw new KeyNotFoundException($"ID'{id}' not exist");

                var lastTrigger = _debounceTimers[id];
                var elapsed = (DateTime.Now - lastTrigger).TotalMilliseconds;
                return elapsed >= interval;
            }
        }

        /// <summary>
        /// 触发第一个防抖器(重置计时)
        /// </summary>
        /// <returns><see langword="true"/> 表示触发成功，<see langword="false"/>表示ID不存在</returns>
        /// <remarks>
        /// [<see langword="2026-9-18 10:45:04" />]
        /// </remarks>
        public bool TriggerDebouncer()
        {
           return TriggerDebouncer(default);
        }

        /// <summary>
        /// 触发防抖器(重置计时)
        /// </summary>
        /// <param name="id">防抖器ID</param>
        /// <returns><see langword="true"/> 表示触发成功，<see langword="false"/>表示ID不存在</returns>
        public bool TriggerDebouncer(string id)
        {
            lock (_lock)
            {
                if (id == default)
                {
                    id = _debounceIntervals.First().Key;
                }
                if (!_debounceIntervals.ContainsKey(id))
                    return false;

                _debounceTimers[id] = DateTime.Now;
                return true;
            }
        }
    }
}
