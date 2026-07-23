#if NETFRAMEWORK && !(NET471 || NET472 || NET481)

namespace Ike.Standard
{
    public partial class ChartOperation
    {
        /// <summary>
        /// 轴选择
        /// </summary>
        public enum Axis
        {
            /// <summary>
            /// X轴
            /// </summary>
            AxisX,
            /// <summary>
            /// Y轴
            /// </summary>
            AxisY,
            /// <summary>
            /// 辅助轴X
            /// </summary>
            SecondaryX,
            /// <summary>
            /// 辅助轴Y
            /// </summary>
            SecondaryY
        }
    }
}
#endif