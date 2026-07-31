#if NETFRAMEWORK && !(NET471 || NET472 || NET481)

namespace Ike.Standard.Forms
{
    public partial class ChartOperation
    {
        /// <summary>
        /// 取消指定属性
        /// </summary>
        public enum AxisAttribute
        {
            /// <summary>
            /// X轴主要网格线
            /// </summary>
            MajorGrid_X,
            /// <summary>
            /// Y轴主要网格线
            /// </summary>
            MajorGrid_Y,
            /// <summary>
            /// 辅助X轴主要网格线
            /// </summary>
            MajorGrid_X2,
            /// <summary>
            /// 辅助Y轴主要网格线
            /// </summary>
            MajorGrid_Y2,
            /// <summary>
            /// X轴主要刻度线
            /// </summary>
            MajorTickMark_X,
            /// <summary>
            /// Y轴主要刻度线
            /// </summary>
            MajorTickMark_Y,
            /// <summary>
            /// 辅助X轴主要刻度线
            /// </summary>
            MajorTickMark_X2,
            /// <summary>
            /// 辅助Y轴主要刻度线
            /// </summary>
            MajorTickMark_Y2,
            /// <summary>
            /// X轴自动调整标签
            /// </summary>
            IsLabelAutoFit_X,
            /// <summary>
            /// Y轴自动调整标签
            /// </summary>
            IsLabelAutoFit_Y,
            /// <summary>
            /// 辅助X轴自动调整标签
            /// </summary>
            IsLabelAutoFit_X2,
            /// <summary>
            /// 辅助Y轴自动调整标签
            /// </summary>
            IsLabelAutoFit_Y2,
            /// <summary>
            /// X轴第一个和最后一个轴显示数值
            /// </summary>
            FirstAndLastValue_X,
            /// <summary>
            /// Y轴第一个和最后一个轴显示数值
            /// </summary>
            FirstAndLastValue_Y,
            /// <summary>
            /// 辅助X轴第一个和最后一个轴显示数值
            /// </summary>
            FirstAndLastValue_X2,
            /// <summary>
            /// 辅助Y轴第一个和最后一个轴显示数值
            /// </summary>
            FirstAndLastValue_Y2,
            /// <summary>
            /// X轴逆序
            /// </summary>
            IsReversed_X,
            /// <summary>
            /// Y轴逆序
            /// </summary>
            IsReversed_Y,
            /// <summary>
            /// X辅助轴逆序
            /// </summary>
            IsReversed_X2,
            /// <summary>
            /// Y辅助轴逆序
            /// </summary>
            IsReversed_Y2,
            /// <summary>
            /// 截断轴标签
            /// </summary>
            TrunCatedLabels_X,
            /// <summary>
            /// 截断轴标签
            /// </summary>
            TrunCatedLabels_Y,
            /// <summary>
            /// 截断轴标签
            /// </summary>
            TrunCatedLabels_X2,
            /// <summary>
            /// 截断轴标签
            /// </summary>
            TrunCatedLabels_Y2,
            /// <summary>
            /// X轴标签显示在轴尾
            /// </summary>
            IsAxisEndLabelVisible_X,
            /// <summary>
            /// Y轴标签显示在轴尾
            /// </summary>
            IsAxisEndLabelVisible_Y,
            /// <summary>
            /// 辅助X轴标签显示在轴尾
            /// </summary>
            IsAxisEndLabelVisible_X2,
            /// <summary>
            /// 辅助Y轴标签显示在轴尾
            /// </summary>
            IsAxisEndLabelVisible_Y2,
            /// <summary>
            /// X轴标签是否显示
            /// </summary>
            IsAxisEnabledLabel_X,
            /// <summary>
            /// Y轴标签是否显示
            /// </summary>
            IsAxisEnabledLabel_Y,
            /// <summary>
            /// 辅助X轴标签是否显示
            /// </summary>
            IsAxisEnabledLabel_X2,
            /// <summary>
            /// 辅助Y轴标签是否显示
            /// </summary>
            IsAxisEnabledLabel_Y2,
            /// <summary>
            /// X轴显示交错带线
            /// </summary>
            IsInterlaced_X,
            /// <summary>
            /// Y轴显示交错带线
            /// </summary>
            IsInterlaced_Y,
            /// <summary>
            /// 辅助X轴显示交错带线
            /// </summary>
            IsInterlaced_X2,
            /// <summary>
            /// 复制Y轴显示交错带线
            /// </summary>
            IsInterlaced_Y2,
            /// <summary>
            /// 启用X轴
            /// </summary>
            AxisEnabled_X,
            /// <summary>
            /// 启用Y轴
            /// </summary>
            AxisEnabled_Y,
            /// <summary>
            /// 启用辅助X轴
            /// </summary>
            AxisEnabled_X2,
            /// <summary>
            /// 启用辅助Y轴
            /// </summary>
            AxisEnabled_Y2,

        }

    }
}
#endif