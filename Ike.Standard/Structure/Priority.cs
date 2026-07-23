#if NETFRAMEWORK
namespace Ike.Standard
{
    public partial class Layout
    {
        /// <summary>
        /// 布局优先级
        /// </summary>
        public enum Priority
        {
            /// <summary>
            /// 相对
            /// </summary>
            Alignment,
            /// <summary>
            /// 绝对
            /// </summary>
            RelativePosition,
            /// <summary>
            /// 排序
            /// </summary>
            ControlSort
        }
    }
}
#endif