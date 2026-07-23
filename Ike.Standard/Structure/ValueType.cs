#if NETFRAMEWORK && !(NET471 || NET472 || NET481)

namespace Ike.Standard
{
    public partial class ChartOperation
    {
        /// <summary>
        /// 数据标签绑定的值类型
        /// </summary>
        public enum ValueType
        {
            /// <summary>
            /// X值
            /// </summary>
            ValueX,
            /// <summary>
            /// Y值
            /// </summary>
            ValueY,
            /// <summary>
            /// Y总数
            /// </summary>
            ValueSum,
            /// <summary>
            /// Y总数的百分比
            /// </summary>
            ValueSumPercentage,
            /// <summary>
            /// 数据点的索引
            /// </summary>
            ValueIndex,
            /// <summary>
            /// 数据点的轴标签
            /// </summary>
            AxisLabel,
            /// <summary>
            /// 数据点的标签
            /// </summary>
            Label,
            /// <summary>
            /// 图例文本
            /// </summary>
            LegendText,
            /// <summary>
            /// 序列名称
            /// </summary>
            SeriesName,
            /// <summary>
            /// Y的平均值
            /// </summary>
            AverageValueY,
            /// <summary>
            /// Y的最大值
            /// </summary>
            MaxY,
            /// <summary>
            /// Y的最小值
            /// </summary>
            MiniY,
            /// <summary>
            /// 最后一个Y值
            /// </summary>
            LastY,
            /// <summary>
            /// 第一个Y值
            /// </summary>
            FirstY
        }
    }
}
#endif