#if NETFRAMEWORK && !(NET471 || NET472 || NET481)

namespace Ike.Standard
{
    public partial class ChartOperation
    {
        /// <summary>
        /// 数据标签绑定数据的格式
        /// </summary>
        public enum ValueTypeFormat
        {
            /// <summary>
            /// 无[初始格式]
            /// </summary>
            Not,
            /// <summary>
            /// 货币[¥12,345.68]
            /// </summary>
            Money,
            /// <summary>
            /// 小数[0012345]
            /// </summary>
            Decimal,
            /// <summary>
            /// 科学计数[1.234568E+004]
            /// </summary>
            CientificNotation,
            /// <summary>
            /// 常规[12345.7]
            /// </summary>
            Convention,
            /// <summary>
            /// 数字[12,345.679]
            /// </summary>
            Number,
            /// <summary>
            /// 百分比[12.60%]
            /// </summary>
            Percentage
        }
    }
}
#endif