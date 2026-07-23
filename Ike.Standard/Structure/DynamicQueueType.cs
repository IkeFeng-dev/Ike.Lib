namespace Ike.Standard
{
    public partial class DynamicQueue<T>
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
    }
}
