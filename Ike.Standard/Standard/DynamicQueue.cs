using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Ike.Standard
{
    /// <summary>
    /// 动态队列类，支持用户自定义类型和消费逻辑,基于<see  cref="System.Collections.Concurrent.BlockingCollection{T}"/>实现
    /// </summary>
    /// <typeparam name="T">队列中的元素类型</typeparam>
    public class DynamicQueue<T>
    {
        /// <summary>
        /// 队列行为
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// 先进先出，即先添加的元素会先被移除
            /// </summary>
            FIFO,
            /// <summary>
            /// 后进先出，即后添加的元素会先被移除
            /// </summary>
            LIFO
        }
        /// <summary>
        /// 队列对象
        /// </summary>
        private readonly BlockingCollection<T> _collection;
        /// <summary>
        /// 消费队列调用函数
        /// </summary>
        private Action<T> _consumerAction;
        /// <summary>
        /// 是否开始消费队列
        /// </summary>
        private bool _startConsuming = false;
        /// <summary>
        /// 获取或设置是否暂停添加到队列
        /// </summary>
        public bool PauseAdd { get; set; } = false;
        /// <summary>
        /// 获取队列数量
        /// </summary>
        public int Count { get { return _collection.Count; } }
        /// <summary>
        /// 当消费一个元素时触发的事件
        /// </summary>
        public event Action<T> ItemConsumed;

        /// <summary>
        /// 构造函数，初始化队列和可选的消费函数
        /// </summary>
        /// <param name="consumerAction">消费函数（可选）,也可以订阅<see cref="ItemConsumed"/>事件进行消费 </param>
        /// <param name="type">队列消费行为</param>
        /// <param name="boundedCapacity">队列最大长度限制,0表示无限制</param>
        public DynamicQueue(Action<T> consumerAction = null,Type type = Type.FIFO, int boundedCapacity = 0)
        {
            if (boundedCapacity < 0)
            {
                boundedCapacity = 0;
            }
            if (boundedCapacity == 0 && type == Type.FIFO)
            {
                 _collection = new BlockingCollection<T>();
            }
            else if (boundedCapacity > 0 && type == Type.LIFO)
            {
                _collection = new BlockingCollection<T>(new ConcurrentStack<T>(), boundedCapacity);
            }
            else if (boundedCapacity == 0 && type == Type.LIFO)
            {
                _collection = new BlockingCollection<T>(new ConcurrentStack<T>());
            }
            else if(boundedCapacity > 0 && type == Type.FIFO)
            {
                _collection = new BlockingCollection<T>(boundedCapacity);
            }
            SetConsumerAction(consumerAction);
        }

        /// <summary>
        /// 开始消费队列中的元素
        /// </summary>
        public void StartConsuming()
        {
            if (_startConsuming)
            {
                return;
            }
            Task.Run(() =>
            {
                foreach (var item in _collection.GetConsumingEnumerable())
                {
                    // 调用用户传递的消费函数
                    _consumerAction?.Invoke(item);
                    // 触发消费事件
                    ItemConsumed?.Invoke(item);
                }
            });
            _startConsuming = true;
        }

        /// <summary>
        /// 向队列中添加一个元素
        /// </summary>
        /// <param name="item">要添加的元素</param>
        public bool Add(T item)
        {
            if (PauseAdd)
            {
                return false;
            }
           return _collection.TryAdd(item);
        }

        /// <summary>
        /// 标记队列为完成状态,调用后表示此队列不接收任何添加
        /// </summary>
        public void CompleteAdding()
        {
            _collection.CompleteAdding();
            _startConsuming = false;
        }

        /// <summary>
        /// 设置消费队列函数
        /// </summary>
        /// <param name="consumerAction">触发队列消费时执行</param>
        public void SetConsumerAction(Action<T> consumerAction)
        {
            _consumerAction = consumerAction;
        }
    }
}