#if NETFRAMEWORK
namespace Ike.Standard
{
    public partial class Layout
    {
        /// <summary>
        /// 控件最大化填充方向
        /// </summary>
        public enum FillDirection
        {
            /// <summary>
            /// 顶部填充
            /// </summary>
            Top,
            /// <summary>
            /// 左侧填充
            /// </summary>
            Left,
            /// <summary>
            /// 右侧填充
            /// </summary>
            Right,
            /// <summary>
            /// 底部填充
            /// </summary>
            Bottom,
            /// <summary>
            /// 横向填充
            /// </summary>
            Transverse,
            /// <summary>
            /// 竖向填充
            /// </summary>
            Vertical
        }
    }
}
#endif