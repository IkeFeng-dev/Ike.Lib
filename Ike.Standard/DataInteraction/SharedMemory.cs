using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ike.Standard.DataInteraction
{
    /// <summary>
    /// 共享内存通讯
    /// </summary>
    public class SharedMemory : IDisposable
    {
        /// <summary>
        /// 共享区域名称
        /// </summary>
        private readonly string mapName;
        /// <summary>
        /// 判断等待读取的前缀标志
        /// </summary>
        private const string WRFLAG = "^{WR}";
        /// <summary>
        /// 分配给内存区域的最大容量
        /// </summary>
        private readonly long capacity = 1024;
        /// <summary>
        /// 表示是否等待接收的标识
        /// </summary>
        private bool isReceive = false;
        /// <summary>
        /// 表示是否等待读取的标识
        /// </summary>
        private bool isWaitRead = false;
        /// <summary>
        /// 表示是否等启用进程互斥
        /// </summary>
        private bool isMutexApp = false;
        /// <summary>
        /// 当前客户端身份标志
        /// </summary>
        private readonly Identity client;
        /// <summary>
        /// 内存映射文件对象A
        /// </summary>
        private readonly MemoryMappedFile mmfA;
        /// <summary>
        /// 内存映射文件对象B
        /// </summary>
        private readonly MemoryMappedFile mmfB;
        /// <summary>
        /// 等待事件-A
        /// </summary>
        private readonly EventWaitHandle eventA;
        /// <summary>
        /// 等待事件-B
        /// </summary>
        private readonly EventWaitHandle eventB;
        /// <summary>
        /// 等待事件-等待读取
        /// </summary>
        private readonly EventWaitHandle eventWaitRead;
        /// <summary>
        /// 等待事件-等待接收
        /// </summary>
        private readonly EventWaitHandle eventReceive;
        /// <summary>
        /// 进程互斥锁
        /// </summary>
        private readonly Mutex mutexApp;
        /// <summary>
        /// 进程是否运行
        /// </summary>
        private bool isRunning = false;
        /// <summary>
        /// 取消监听器的令牌
        /// </summary>
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        /// <summary>
        /// 获取分配给内存映射区域的名称
        /// </summary>
        public string MapName { get { return mapName; } }
        /// <summary>
        /// 当写入数据时触发的接收事件
        /// </summary>
        public event Action<string> ReceiveEvent;
        /// <summary>
        /// 客户端身份
        /// </summary>
        public enum Identity
        {
            /// <summary>
            /// 客户端A
            /// </summary>
            ClientA,
            /// <summary>
            /// 客户端B
            /// </summary>
            ClientB,

        }

        /// <summary>
        /// 构造函数,创建通讯对象并且启动监听器
        /// </summary>
        /// <param name="client">两端共享,分别指定为A或B,如两端标志相同则无法进行数据收发</param>
        /// <param name="mapName">分配给内存映射区域的名称,确保两端名称完全一致</param>
        /// <param name="capacity">分配的共享内存区域最大容量,确保两端容量完全一致</param>
        /// <param name="isMutexApp">是否启用进程互斥锁,确保实例唯一性</param>
        /// <exception cref="Exception"></exception>
        public SharedMemory(Identity client, string mapName = default, int capacity = 1024,bool isMutexApp = false)
        {
            this.capacity = capacity;
            this.isMutexApp = isMutexApp;
            if (mapName != default)
            {
                this.mapName = mapName;
            }
            else
            {
                this.mapName = Guid.NewGuid().ToString();
            }
            this.client = client;
            // 检测同步锁
            if (this.isMutexApp)
            {
                mutexApp = new Mutex(false, this.mapName + client);
                if (!mutexApp.WaitOne(100, false))
                {
                    throw new Exception($"An identical instance has been created ['{client}'-'{this.mapName}']");
                }
            }
            // 创建共享内存区域
            mmfA = MemoryMappedFile.CreateOrOpen(this.mapName + "-MMF-A", this.capacity);
            mmfB = MemoryMappedFile.CreateOrOpen(this.mapName + "-MMF-B", this.capacity);
            // 创建事件
            eventA = new EventWaitHandle(false, EventResetMode.AutoReset, this.mapName + "-EWH-A");
            eventB = new EventWaitHandle(false, EventResetMode.AutoReset, this.mapName + "-EWH-B");
            eventWaitRead = new EventWaitHandle(false, EventResetMode.AutoReset, this.mapName + "-EWH-Wait");
            eventReceive = new EventWaitHandle(false, EventResetMode.AutoReset, this.mapName + "-EWH-Receive");
            // 默认启动监听器
            StartListener();
        }

        /// <summary>
        /// 设置状态有信号状态,允许线程继续执行
        /// </summary>
        /// <param name="isCurrent">标志信号为本身信号还是对方信号</param>
        private void SetSignal(bool isCurrent)
        {
            var eventToSet = isCurrent
                ? (client == Identity.ClientA ? eventB : eventA)
                : (client == Identity.ClientA ? eventA : eventB);
            eventToSet.Set();
        }


        /// <summary>
        /// 监听共享内存中的数据
        /// </summary>
        private void ListenForData()
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested && isRunning)
            {
                // 等待对方触发事件
                if (client == Identity.ClientA)
                {
                    eventB.WaitOne();
                }
                else
                {
                    eventA.WaitOne();
                }
                if (!isRunning)
                {
                    break;
                }
                // 读取数据并触发对应事件(如果已指定)
                if (Read(out string read))
                {
                    // 触发接收事件
                    ReceiveEvent?.Invoke(read);
                }
            }
        }

        /// <summary>
        /// 写入数据到共享内存
        /// </summary>
        /// <param name="mmf">映射文件对象</param>
        /// <param name="text">写入的内容</param>
        private void Write(MemoryMappedFile mmf,string text)
        {
            using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                // 写入数据长度
                accessor.Write(0, buffer.Length);
                // 写入数据
                accessor.WriteArray(4, buffer, 0, buffer.Length);
            }
        }


        /// <summary>
        /// 启动监听器
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">监听器已启动</exception>
        public void StartListener()
        {
            if (isRunning)
            {
                throw new InvalidOperationException("The listener is already running.");
            }
            isRunning = true;
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                ListenForData();
            }, cancellationTokenSource.Token);
        }


        /// <summary>
        /// 停止监听器
        /// </summary>
        public void StopListener()
        {
            isRunning = false;
            cancellationTokenSource.Cancel();
            SetSignal(isCurrent: true);
        }


        /// <summary>
        /// 写入数据并通知对方读取
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public void Write(string text)
        {
            var mmf = client == Identity.ClientA ? mmfB : mmfA;
            Write(mmf, text);
            SetSignal(isCurrent: false);
            if (isWaitRead && !text.Equals(WRFLAG))
            {
                eventWaitRead.Set();
                isWaitRead = false;
            }
            if (isReceive && !text.EndsWith(WRFLAG))
            {
                eventReceive.Set();
                isReceive = false;
            }
        }


        /// <summary>
        /// 手动读取数据
        /// </summary>
        /// <returns>读取到的字符串</returns>
        public bool Read(out string read)
        {
            var mmf = client == Identity.ClientA ? mmfA : mmfB;
            // 读取数据
            using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
            {
                // 读取数据长度
                int length = accessor.ReadInt32(0);
                byte[] buffer = new byte[length];
                // 读取数据
                accessor.ReadArray(4, buffer, 0, length);
                read = Encoding.UTF8.GetString(buffer);
                if (read.Equals(WRFLAG, StringComparison.OrdinalIgnoreCase))
                {
                    isWaitRead = true;
                    read = string.Empty;
                    return false;
                }
                else if (read.EndsWith(WRFLAG))
                {
                    read = read.Replace(WRFLAG, string.Empty);
                    isReceive = true;
                }
                return true;
            }
        }


        /// <summary>
        /// 等待读取一个最新的数据
        /// </summary>
        /// <param name="millisecondsTimeout">等待的最长时间</param>
        /// <param name="readText">如果等待成功,则返回最新数据</param>
        /// <returns>返回读取状态</returns>
        public bool WaitRead(int millisecondsTimeout, out string readText)
        {
            eventWaitRead.Reset();
            Write(WRFLAG);
            readText = string.Empty;
            if (eventWaitRead.WaitOne(millisecondsTimeout) && Read(out readText))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 写入数据并且等待对方返回的数据
        /// </summary>
        /// <param name="text">写入数据</param>
        /// <param name="receive">如果等待成功,则返回接收的数据</param>
        /// <param name="millisecondsTimeout">等待的最长时间</param>
        /// <returns>是否等待成功</returns>
        public bool WaitReceive(string text,int millisecondsTimeout,out string receive)
        {
            eventReceive.Reset();
            receive = string.Empty;
            Write( text + WRFLAG);
            if (eventReceive.WaitOne(millisecondsTimeout) && Read(out receive))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 清空当前内存中数据
        /// </summary>
        public void Clear()
        {
            Clear(client);
        }

        /// <summary>
        /// 清空指定一端内存中数据
        /// </summary>
        public void Clear(Identity identity)
        {
            var mmf = identity == Identity.ClientA ? mmfA : mmfB;
            Write(mmf, string.Empty);
        }


        /// <summary>
        /// 获取当前实例属性值,用于DEBUG
        /// </summary>
        public Dictionary<string,object> GetProperty()
        {
            return new Dictionary<string, object>()
            {
                {"Client", client},
                {"MapName",mapName},
                {"Capacity",capacity},
                {"IsReceive",isReceive},
                {"IsWaitRead",isWaitRead},
                {"EventA",eventA.WaitOne(0)},
                {"EventB",eventB.WaitOne(0)},
                {"EventReceive",eventReceive.WaitOne(0)},
                {"EventWaitRead",eventWaitRead.WaitOne(0)}
            };
        }
       

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReceiveEvent = null;
            StopListener();
            if (isMutexApp)
            {
                mutexApp.ReleaseMutex();
                mutexApp.Dispose();
            }
            // 释放对象
            mmfA.Dispose();
            mmfB.Dispose();
            eventB.Dispose();
            eventA.Dispose();
            eventReceive.Dispose();
            eventWaitRead.Dispose();
        }

    }
}
