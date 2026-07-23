#if NETFRAMEWORK
namespace Ike.Standard
{
    public partial class AnimationOperation
    {
        /// <summary>
        /// 控件缩放类型
        /// </summary>
        public enum ZoomSelect
        {
            /// <summary>
            /// 扩大
            /// </summary>
            Enlarge,
            /// <summary>
            /// 缩小
            /// </summary>
            Narrow,
            /// <summary>
            /// 大到小
            /// </summary>
            LargeToSmall,
            /// <summary>
            /// 小到大
            /// </summary>
            SmallToLarge
        }
    }
}
#endif