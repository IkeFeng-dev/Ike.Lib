#if NETFRAMEWORK
namespace Ike.Standard.Forms
{
    public partial class Layout
    {

        /// <summary>
        /// 对齐方式
        /// </summary>
        public enum Alignment
        {
            /// <summary>
            /// 居中对齐
            /// </summary>
            Center,
            /// <summary>
            /// 左对齐
            /// </summary>
            Left,
            /// <summary>
            /// 右对齐
            /// </summary>
            Right,
            /// <summary>
            /// 顶部对齐
            /// </summary>
            Top,
            /// <summary>
            /// 底部对齐
            /// </summary>
            Bottom,
            /// <summary>
            /// 水平居中
            /// </summary>
            CenterHorizontal,
            /// <summary>
            /// 在右侧垂直居中
            /// </summary>
            CenterVerticalOnRight,
            /// <summary>
            /// 在底部水平居中
            /// </summary>
            CenterHorizontalOnBottom,
            /// <summary>
            /// 垂直居中
            /// </summary>
            CenterVertical,
            /// <summary>
            /// 左上位置对齐
            /// </summary>
            TopLeft,
            /// <summary>
            /// 右上位置对齐
            /// </summary>
            TopRight,
            /// <summary>
            /// 左下位置对齐
            /// </summary>
            BottomLeft,
            /// <summary>
            /// 右下位置对齐
            /// </summary>
            BottomRight,
            /// <summary>
            /// 当前位置横向居中,不改变Y值
            /// </summary>
            CurrentHorizontal,
            /// <summary>
            /// 当前位置横向居中,不改变Y值
            /// </summary>
            CurrentVertical,
        }
    }
}
#endif