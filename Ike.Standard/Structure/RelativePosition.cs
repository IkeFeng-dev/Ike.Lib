#if NETFRAMEWORK
namespace Ike.Standard
{
    public partial class Layout
    {
        /// <summary>
        /// 相对位置
        /// </summary>
        public enum RelativePosition
        {
            /// <summary>
            /// 在指定控件下方
            /// </summary>
            Below,
            /// <summary>
            /// 在指定控件上方
            /// </summary>
            Above,
            /// <summary>
            /// 在指定控件左侧
            /// </summary>
            OnLeft,
            /// <summary>
            /// 在指定控件右侧
            /// </summary>
            OnRight,
            /// <summary>
            /// 与指定控件左对齐
            /// </summary>
            AlignLeft,
            /// <summary>
            /// 与指定控件右对齐
            /// </summary>
            AlignRight,
            /// <summary>
            /// 与指定控件底对齐
            /// </summary>
            AlignBottom,
            /// <summary>
            /// 与指定控件顶对齐
            /// </summary>
            AlignTop,
            /// <summary>
            /// 在指定控件内部居中
            /// </summary>
            Center,
        }
    }
}
#endif