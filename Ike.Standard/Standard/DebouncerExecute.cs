using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ike.Standard
{
    /// <summary>
    /// 带有执行操作的防抖辅助类
    /// </summary>
    /// <remarks>
    /// [<see langword="2025年12月26日14:58:17" />]
    /// </remarks>
    public class DebouncerExecute : IDisposable
    {
        /// <summary>
        /// 防抖等待时间间隔，表示两次调用之间的最小时间间隔
        /// </summary>
        private readonly TimeSpan _waitTime;
        /// <summary>
        /// 标志位，指示是否在第一次调用时立即执行操作（true表示立即执行首次调用）
        /// </summary>
        private readonly bool _leadingExecution;
        /// <summary>
        /// 需要被防抖包装的实际操作委托，当防抖条件满足时执行
        /// </summary>
        private readonly Action _action;
        /// <summary>
        /// 线程同步锁对象，用于确保多线程环境下的线程安全操作
        /// </summary>
        private readonly object _locker = new object();
        /// <summary>
        /// 取消令牌源，用于取消正在等待的延迟执行任务,当新的调用到来时，通过取消之前的延迟任务来实现防抖效果 
        /// </summary>
        private CancellationTokenSource _cts;
        /// <summary>
        /// 对象释放标志，用于标识当前防抖器是否已被释放,防止对象被释放后仍然执行操作或访问资源
        /// </summary>
        private bool _disposed;


        /// <summary>
        /// 创建一个带有执行操作的防抖器
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <param name="waitTimeMs">等待时间(毫秒)</param>
        /// <param name="leadingExecution">是否在第一次调用时立即执行</param>
        public DebouncerExecute(Action action, int waitTimeMs, bool leadingExecution = false)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _waitTime = TimeSpan.FromMilliseconds(waitTimeMs);
            _leadingExecution = leadingExecution;
        }


        /// <summary>
        /// 触发防抖操作(只有创建的带有执行操作的防抖器才能执行)
        /// </summary>
        public void Invoke()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DebouncerExecute));

            lock (_locker)
            {
                // 取消之前的延迟执行
                _cts?.Cancel();
                _cts = new CancellationTokenSource();

                // 如果是首次调用且配置了leading执行，则立即执行
                if (_leadingExecution && _cts.Token.IsCancellationRequested == false)
                {
                    _action();
                    _cts.Token.ThrowIfCancellationRequested();
                }

                // 启动新的延迟执行
                Task.Delay(_waitTime, _cts.Token)
                    .ContinueWith(t =>
                    {
                        if (!t.IsCanceled && !_disposed)
                        {
                            lock (_locker)
                            {
                                if (!_cts.IsCancellationRequested && !_disposed)
                                {
                                    _action();
                                }
                            }
                        }
                    }, TaskScheduler.Default);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            lock (_locker)
            {
                if (!_disposed)
                {
                    _cts?.Cancel();
                    _cts?.Dispose();
                    _disposed = true;
                }
            }
        }
    }
}
