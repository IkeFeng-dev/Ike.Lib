#if NETFRAMEWORK && !(NET471 || NET472 || NET481)

using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace Ike.Standard
{
    /// <summary>
    /// 图表控件操作
    /// </summary>
    public partial class ChartOperation
    {
        /// <summary>
        /// 当前图表
        /// </summary>
        public Chart CurrentChart { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="chart">图表控件</param>
        public ChartOperation(Chart chart)
        {
            CurrentChart = chart;
        }
      

        /// <summary>
        /// 获取指定数据绑定的值是否支持精度设置
        /// </summary>
        /// <param name="valueType">绑定值类型</param>
        /// <returns></returns>
        public bool ValueTypeIsPrecision(ValueType valueType)
        {
            if (valueType == ValueType.ValueIndex || valueType == ValueType.AxisLabel || valueType == ValueType.SeriesName || valueType == ValueType.LegendText || valueType == ValueType.Label)
            {
                return false;
            }
            return true;
        }



        


        /// <summary>
        /// 取消轴指定属性
        /// </summary>
        /// <param name="attribute">属性</param>
        /// <param name="chartAreasName">chartAreas名称</param>
        public void CancelAxisAttribute(AxisAttribute attribute, params string[] chartAreasName)
        {
            for (int i = 0; i < chartAreasName.Length; i++)
            {
                switch (attribute)
                {
                    case AxisAttribute.MajorGrid_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.MajorGrid.Enabled = false;
                        break;
                    case AxisAttribute.MajorGrid_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.MajorGrid.Enabled = false;
                        break;
                    case AxisAttribute.MajorGrid_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.MajorGrid.Enabled = false;
                        break;
                    case AxisAttribute.MajorGrid_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.MajorGrid.Enabled = false;
                        break;
                    case AxisAttribute.MajorTickMark_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.MajorTickMark.Enabled = false;
                        break;
                    case AxisAttribute.MajorTickMark_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.MajorTickMark.Enabled = false;
                        break;
                    case AxisAttribute.MajorTickMark_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.MajorTickMark.Enabled = false;
                        break;
                    case AxisAttribute.MajorTickMark_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.MajorTickMark.Enabled = false;
                        break;
                    case AxisAttribute.IsLabelAutoFit_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.IsLabelAutoFit = false;
                        break;
                    case AxisAttribute.IsLabelAutoFit_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.IsLabelAutoFit = false;
                        break;
                    case AxisAttribute.IsLabelAutoFit_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.IsLabelAutoFit = false;
                        break;
                    case AxisAttribute.IsLabelAutoFit_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.IsLabelAutoFit = false;
                        break;
                    case AxisAttribute.FirstAndLastValue_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.IsMarginVisible = true;
                        break;
                    case AxisAttribute.FirstAndLastValue_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.IsMarginVisible = true;
                        break;
                    case AxisAttribute.FirstAndLastValue_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.IsMarginVisible = true;
                        break;
                    case AxisAttribute.FirstAndLastValue_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.IsMarginVisible = true;
                        break;
                    case AxisAttribute.IsReversed_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.IsReversed = false;
                        break;
                    case AxisAttribute.IsReversed_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.IsReversed = false;
                        break;
                    case AxisAttribute.IsReversed_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.IsReversed = false;
                        break;
                    case AxisAttribute.IsReversed_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.IsReversed = false;
                        break;
                    case AxisAttribute.TrunCatedLabels_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.LabelStyle.TruncatedLabels = false;
                        break;
                    case AxisAttribute.TrunCatedLabels_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.LabelStyle.TruncatedLabels = false;
                        break;
                    case AxisAttribute.TrunCatedLabels_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.LabelStyle.TruncatedLabels = false;
                        break;
                    case AxisAttribute.TrunCatedLabels_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.LabelStyle.TruncatedLabels = false;
                        break;
                    case AxisAttribute.IsAxisEndLabelVisible_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.LabelStyle.IsEndLabelVisible = false;
                        break;
                    case AxisAttribute.IsAxisEndLabelVisible_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.LabelStyle.IsEndLabelVisible = false;
                        break;
                    case AxisAttribute.IsAxisEndLabelVisible_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.LabelStyle.IsEndLabelVisible = false;
                        break;
                    case AxisAttribute.IsAxisEndLabelVisible_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.LabelStyle.IsEndLabelVisible = false;
                        break;
                    case AxisAttribute.IsAxisEnabledLabel_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.LabelStyle.Enabled = false;
                        break;
                    case AxisAttribute.IsAxisEnabledLabel_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.LabelStyle.Enabled = false;
                        break;
                    case AxisAttribute.IsAxisEnabledLabel_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.LabelStyle.Enabled = false;
                        break;
                    case AxisAttribute.IsAxisEnabledLabel_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.LabelStyle.Enabled = false;
                        break;
                    case AxisAttribute.IsInterlaced_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.IsInterlaced = false;
                        break;
                    case AxisAttribute.IsInterlaced_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.IsInterlaced = false;
                        break;
                    case AxisAttribute.IsInterlaced_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.IsInterlaced = false;
                        break;
                    case AxisAttribute.IsInterlaced_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.IsInterlaced = false;
                        break;
                    case AxisAttribute.AxisEnabled_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.Enabled = AxisEnabled.False;
                        break;
                    case AxisAttribute.AxisEnabled_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.Enabled = AxisEnabled.False;
                        break;
                    case AxisAttribute.AxisEnabled_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.Enabled = AxisEnabled.False;
                        break;
                    case AxisAttribute.AxisEnabled_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.Enabled = AxisEnabled.False;
                        break;
                }
            }
        }




        /// <summary>
        /// 设置轴指定属性
        /// </summary>
        /// <param name="attribute">属性</param>
        /// <param name="chartAreasName">chartAreas名称</param>
        public void SetAxisAttribute(AxisAttribute attribute, params string[] chartAreasName)
        {
            for (int i = 0; i < chartAreasName.Length; i++)
            {
                switch (attribute)
                {
                    case AxisAttribute.MajorGrid_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.MajorGrid.Enabled = true;
                        break;
                    case AxisAttribute.MajorGrid_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.MajorGrid.Enabled = true;
                        break;
                    case AxisAttribute.MajorGrid_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.MajorGrid.Enabled = true;
                        break;
                    case AxisAttribute.MajorGrid_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.MajorGrid.Enabled = true;
                        break;
                    case AxisAttribute.MajorTickMark_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.MajorTickMark.Enabled = true;
                        break;
                    case AxisAttribute.MajorTickMark_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.MajorTickMark.Enabled = true;
                        break;
                    case AxisAttribute.MajorTickMark_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.MajorTickMark.Enabled = true;
                        break;
                    case AxisAttribute.MajorTickMark_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.MajorTickMark.Enabled = true;
                        break;
                    case AxisAttribute.IsLabelAutoFit_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.IsLabelAutoFit = true;
                        break;
                    case AxisAttribute.IsLabelAutoFit_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.IsLabelAutoFit = true;
                        break;
                    case AxisAttribute.IsLabelAutoFit_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.IsLabelAutoFit = true;
                        break;
                    case AxisAttribute.IsLabelAutoFit_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.IsLabelAutoFit = true;
                        break;
                    case AxisAttribute.FirstAndLastValue_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.IsMarginVisible = false;
                        break;
                    case AxisAttribute.FirstAndLastValue_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.IsMarginVisible = false;
                        break;
                    case AxisAttribute.FirstAndLastValue_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.IsMarginVisible = false;
                        break;
                    case AxisAttribute.FirstAndLastValue_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.IsMarginVisible = false;
                        break;
                    case AxisAttribute.IsReversed_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.IsReversed = true;
                        break;
                    case AxisAttribute.IsReversed_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.IsReversed = true;
                        break;
                    case AxisAttribute.IsReversed_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.IsReversed = true;
                        break;
                    case AxisAttribute.IsReversed_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.IsReversed = true;
                        break;
                    case AxisAttribute.TrunCatedLabels_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.LabelStyle.TruncatedLabels = true;
                        break;
                    case AxisAttribute.TrunCatedLabels_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.LabelStyle.TruncatedLabels = true;
                        break;
                    case AxisAttribute.TrunCatedLabels_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.LabelStyle.TruncatedLabels = true;
                        break;
                    case AxisAttribute.TrunCatedLabels_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.LabelStyle.TruncatedLabels = true;
                        break;
                    case AxisAttribute.IsAxisEndLabelVisible_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.LabelStyle.IsEndLabelVisible = true;
                        break;
                    case AxisAttribute.IsAxisEndLabelVisible_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.LabelStyle.IsEndLabelVisible = true;
                        break;
                    case AxisAttribute.IsAxisEndLabelVisible_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.LabelStyle.IsEndLabelVisible = true;
                        break;
                    case AxisAttribute.IsAxisEndLabelVisible_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.LabelStyle.IsEndLabelVisible = true;
                        break;
                    case AxisAttribute.IsAxisEnabledLabel_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.LabelStyle.Enabled = true;
                        break;
                    case AxisAttribute.IsAxisEnabledLabel_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.LabelStyle.Enabled = true;
                        break;
                    case AxisAttribute.IsAxisEnabledLabel_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.LabelStyle.Enabled = true;
                        break;
                    case AxisAttribute.IsAxisEnabledLabel_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.LabelStyle.Enabled = true;
                        break;
                    case AxisAttribute.IsInterlaced_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.IsInterlaced = true;
                        break;
                    case AxisAttribute.IsInterlaced_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.IsInterlaced = true;
                        break;
                    case AxisAttribute.IsInterlaced_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.IsInterlaced = true;
                        break;
                    case AxisAttribute.IsInterlaced_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.IsInterlaced = true;
                        break;
                    case AxisAttribute.AxisEnabled_X:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.Enabled = AxisEnabled.True;
                        break;
                    case AxisAttribute.AxisEnabled_Y:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.Enabled = AxisEnabled.True;
                        break;
                    case AxisAttribute.AxisEnabled_X2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.Enabled = AxisEnabled.True;
                        break;
                    case AxisAttribute.AxisEnabled_Y2:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.Enabled = AxisEnabled.True;
                        break;
                }
            }
        }




        /// <summary>
        /// 获取数据标签绑定数据的格式表达文本
        /// </summary>
        /// <param name="typeFormat">数据标签绑定数据的格式</param>
        /// <returns></returns>
        public string GetValueTypeFormatString(ValueTypeFormat typeFormat)
        {
            switch (typeFormat)
            {
                case ValueTypeFormat.Not:
                    return "G";
                case ValueTypeFormat.Money:
                    return "C";
                case ValueTypeFormat.Decimal:
                    return "D";
                case ValueTypeFormat.CientificNotation:
                    return "E";
                case ValueTypeFormat.Convention:
                    return "G";
                case ValueTypeFormat.Number:
                    return "N";
                case ValueTypeFormat.Percentage:
                    return "P";
                default:
                    return "G";
            }
        }




        /// <summary>
        /// 获取数据标签绑定数据的表达文本
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public string GetValueTypeString(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.ValueX:
                    return "#VALX";
                case ValueType.ValueY:
                    return "#VAL";
                case ValueType.ValueSum:
                    return "#TOTAL";
                case ValueType.ValueSumPercentage:
                    return "#PERCENT";
                case ValueType.ValueIndex:
                    return "#INDEX";
                case ValueType.Label:
                    return "#LABEL";
                case ValueType.AxisLabel:
                    return "#AXISLABEL";
                case ValueType.LegendText:
                    return "#LEGENDTEXT";
                case ValueType.SeriesName:
                    return "#SERIESNAME";
                case ValueType.AverageValueY:
                    return "#AVG";
                case ValueType.MiniY:
                    return "#MIN";
                case ValueType.MaxY:
                    return "#MAX";
                case ValueType.LastY:
                    return "#LAST";
                case ValueType.FirstY:
                    return "#FIRST";
                default:
                    return "#VAL";
            }
        }



        /// <summary>
        /// 设置数据标签绑定指定数据
        /// </summary>
        /// <param name="representation">数据表达式</param>
        /// <param name="SeriesName">Series名称</param>
        public void SetLabelBindData(string representation, params string[] SeriesName)
        {
            for (int i = 0; i < SeriesName.Length; i++)
            {
                CurrentChart.Series[SeriesName[i]].IsValueShownAsLabel = true;
                CurrentChart.Series[SeriesName[i]].Label = representation;
            }
        }




        /// <summary>
        /// 获取绑定数据的表达式文本
        /// </summary>
        /// <param name="typeFormat">绑定的数据的格式</param>
        /// <param name="type">绑定的值</param>
        /// <param name="precision">数据的精度</param>
        /// <returns></returns>
        public string GetBindDataLabel(ValueType type, ValueTypeFormat typeFormat, byte precision)
        {
            if (ValueTypeIsPrecision(type))
            {
                return GetValueTypeString(type) + "{" + GetValueTypeFormatString(typeFormat) + precision + "}";
            }
            else
            {
                return GetValueTypeString(type);
            }
        }





        /// <summary>
        /// 数据标签绑定数据
        /// </summary>
        /// <param name="SeriesName">Series名称</param>
        /// <param name="typeFormat">绑定的数据的格式</param>
        /// <param name="type">绑定的值</param>
        /// <param name="precision">数据的精度</param>
        public void SetDataLabelBindData(ValueType type, ValueTypeFormat typeFormat, byte precision, params string[] SeriesName)
        {
            for (int i = 0; i < SeriesName.Length; i++)
            {
                if (ValueTypeIsPrecision(type))
                {
                    SetLabelBindData(GetValueTypeString(type) + "{" + GetValueTypeFormatString(typeFormat) + precision + "}", SeriesName[i]);
                }
                else
                {
                    SetLabelBindData(GetValueTypeString(type), SeriesName[i]);
                }
            }
        }





        /// <summary>
        /// 数据标签绑定数据
        /// </summary>
        /// <param name="SeriesName">Series名称</param>
        /// <param name="typeFormat">绑定的数据的格式</param>
        /// <param name="type">绑定的值</param>
        public void SetDataLabelBindData(ValueType type, ValueTypeFormat typeFormat, params string[] SeriesName)
        {
            for (int i = 0; i < SeriesName.Length; i++)
            {
                SetLabelBindData(GetValueTypeString(type) + "{" + GetValueTypeFormatString(typeFormat) + "}", SeriesName[i]);
            }
        }






        /// <summary>
        /// 数据标签绑定数据
        /// </summary>
        /// <param name="SeriesName">Series名称</param>
        /// <param name="type">绑定的值</param>
        public void SetDataLabelBindData(ValueType type, params string[] SeriesName)
        {
            for (int i = 0; i < SeriesName.Length; i++)
            {
                SetLabelBindData(GetValueTypeString(type), SeriesName[i]);
            }
        }





        /// <summary>
        /// 设置轴标签自动调整的字体最小值
        /// </summary>
        /// <param name="chartAreasName">ChartAreas名称</param>
        /// <param name="axis">显示的轴</param>
        /// <param name="miniSize">自动调整的最大字体</param>
        public void SetAxisLabelIsAutoFitMiniSize(Axis axis, int miniSize, params string[] chartAreasName)
        {
            for (int i = 0; i < chartAreasName.Length; i++)
            {
                switch (axis)
                {
                    case Axis.AxisX:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.LabelAutoFitMinFontSize = miniSize;
                        break;
                    case Axis.AxisY:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.LabelAutoFitMinFontSize = miniSize;
                        break;
                    case Axis.SecondaryX:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.LabelAutoFitMinFontSize = miniSize;
                        break;
                    case Axis.SecondaryY:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.LabelAutoFitMinFontSize = miniSize;
                        break;
                }
            }
        }






        /// <summary>
        /// 设置轴标签自动调整的字体最大值
        /// </summary>
        /// <param name="chartAreasName">ChartAreas名称</param>
        /// <param name="axis">显示的轴</param>
        /// <param name="maxSize">自动调整的最大字体</param>
        public void SetAxisLabelIsAutoFitMaxSize(Axis axis, int maxSize, params string[] chartAreasName)
        {
            for (int i = 0; i < chartAreasName.Length; i++)
            {
                switch (axis)
                {
                    case Axis.AxisX:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.LabelAutoFitMaxFontSize = maxSize;
                        break;
                    case Axis.AxisY:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.LabelAutoFitMaxFontSize = maxSize;
                        break;
                    case Axis.SecondaryX:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.LabelAutoFitMaxFontSize = maxSize;
                        break;
                    case Axis.SecondaryY:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.LabelAutoFitMaxFontSize = maxSize;
                        break;
                }
            }
        }





        /// <summary>
        /// 设置轴标签是否自动调整
        /// </summary>
        /// <param name="chartAreasName">ChartAreas名称</param>
        /// <param name="axis">显示的轴</param>
        /// <param name="isAuto">是否自动调整</param>
        public void SetAxisLabelIsAutoFit(Axis axis, bool isAuto, params string[] chartAreasName)
        {
            for (int i = 0; i < chartAreasName.Length; i++)
            {
                switch (axis)
                {
                    case Axis.AxisX:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.IsLabelAutoFit = isAuto;
                        break;
                    case Axis.AxisY:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.IsLabelAutoFit = isAuto;
                        break;
                    case Axis.SecondaryX:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.IsLabelAutoFit = isAuto;
                        break;
                    case Axis.SecondaryY:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.IsLabelAutoFit = isAuto;
                        break;
                }
            }
        }






        /// <summary>
        /// 清空图标数据
        /// </summary>
        /// <param name="SeriesName">Series名称</param>
        public void ClearData(params string[] SeriesName)
        {
            for (int i = 0; i < SeriesName.Length; i++)
            {
                CurrentChart.Series[SeriesName[i]].Points.Clear();
            }
        }





        /// <summary>
        /// 图表背景颜色
        /// </summary>
        /// <param name="color">颜色</param>
        public void SetBackColor(Color color)
        {
            CurrentChart.BackColor = color;
        }






        /// <summary>
        /// 设置图表区背景颜色
        /// </summary>
        /// <param name="chartAreasName">ChartAreas名称</param>
        /// <param name="color">颜色</param>
        public void SetAxisBackgroundColor(Color color, params string[] chartAreasName)
        {
            for (int i = 0; i < chartAreasName.Length; i++)
            {
                CurrentChart.ChartAreas[chartAreasName[i]].BackColor = color;
            }
        }





        /// <summary>
        /// 设置轴标签颜色
        /// </summary>
        /// <param name="chartAreasName">ChartAreas名称</param>
        /// <param name="axis">显示的轴</param>
        /// <param name="color">颜色</param>
        public void SetAxisLabelColor(Axis axis, Color color, params string[] chartAreasName)
        {
            for (int i = 0; i < chartAreasName.Length; i++)
            {
                switch (axis)
                {
                    case Axis.AxisX:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.LabelStyle.ForeColor = color;
                        break;
                    case Axis.AxisY:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.LabelStyle.ForeColor = color;
                        break;
                    case Axis.SecondaryX:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.LabelStyle.ForeColor = color;
                        break;
                    case Axis.SecondaryY:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.LabelStyle.ForeColor = color;
                        break;
                }
            }
        }






        /// <summary>
        /// 设置轴标签字体
        /// </summary>
        /// <param name="chartAreasName">ChartAreas名称</param>
        /// <param name="axis">显示的轴</param>
        /// <param name="font">字体</param>
        public void SetAxisLabelFont(Axis axis, Font font, params string[] chartAreasName)
        {
            for (int i = 0; i < chartAreasName.Length; i++)
            {
                switch (axis)
                {
                    case Axis.AxisX:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.LabelStyle.Font = font;
                        break;
                    case Axis.AxisY:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.LabelStyle.Font = font;
                        break;
                    case Axis.SecondaryX:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.LabelStyle.Font = font;
                        break;
                    case Axis.SecondaryY:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.LabelStyle.Font = font;
                        break;
                }
            }
        }






        /// <summary>
        /// 设置数据标签字体
        /// </summary>
        /// <param name="SeriesName">Series名称</param>
        /// <param name="font">字体</param>
        public void SetDataLabelFont(Font font, params string[] SeriesName)
        {
            for (int i = 0; i < SeriesName.Length; i++)
            {
                CurrentChart.Series[SeriesName[i]].Font = font;
            }
        }






        /// <summary>
        /// 设置Y数据值标签的颜色
        /// </summary>
        /// <param name="SeriesName">Series名称</param>
        /// <param name="color">颜色</param>
        public void SetDataLabelColor(Color color, params string[] SeriesName)
        {
            for (int i = 0; i < SeriesName.Length; i++)
            {
                CurrentChart.Series[SeriesName[i]].LabelForeColor = color;
            }
        }






        /// <summary>
        /// 设置柱形图(其它没试过)宽度
        /// </summary>
        /// <param name="SeriesName">Series名称</param>
        /// <param name="width">宽度</param>
        public void SetPointWidth(double width, params string[] SeriesName)
        {
            if (width <= 0) width = 0.5;
            for (int i = 0; i < SeriesName.Length; i++)
            {
                CurrentChart.Series[SeriesName[i]]["PointWidth"] = width.ToString();
            }
        }







        /// <summary>
        /// 设置轴图形的颜色
        /// </summary>
        /// <param name="SeriesName">Series名称</param>
        /// <param name="color">颜色</param>
        public void SetAxisFigureColor(Color color, params string[] SeriesName)
        {
            for (int i = 0; i < SeriesName.Length; i++)
            {
                CurrentChart.Series[SeriesName[i]].Color = color;
            }
        }





        /// <summary>
        /// 设置轴数据显示的间隔
        /// </summary>
        /// <param name="chartAreasName">ChartAreas名称</param>
        /// <param name="axis">显示的轴</param>
        /// <param name="interval">间隔</param>
        public void SetAxisInterval(Axis axis, int interval, params string[] chartAreasName)
        {
            if (interval <= 0) interval = 1;
            for (int i = 0; i < chartAreasName.Length; i++)
            {
                switch (axis)
                {
                    case Axis.AxisX:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.Interval = interval;
                        break;
                    case Axis.AxisY:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.Interval = interval;
                        break;
                    case Axis.SecondaryX:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.Interval = interval;
                        break;
                    case Axis.SecondaryY:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.Interval = interval;
                        break;
                }
            }
        }






        /// <summary>
        /// 设置图表标题颜色
        /// </summary>
        /// <param name="chartAreasName">ChartAreas名称</param>
        /// <param name="color">标题颜色</param>
        /// <param name="axis">轴选择</param>
        public void SetAxisTitleColor(Color color, Axis axis, params string[] chartAreasName)
        {
            for (int i = 0; i < chartAreasName.Length; i++)
            {
                switch (axis)
                {
                    case Axis.AxisX:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.TitleForeColor = color;
                        break;
                    case Axis.AxisY:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.TitleForeColor = color;
                        break;
                    case Axis.SecondaryX:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.TitleForeColor = color;
                        break;
                    case Axis.SecondaryY:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.TitleForeColor = color;
                        break;
                }
            }
        }






        /// <summary>
        /// 设置图表标题
        /// </summary>
        /// <param name="chartAreasName">ChartAreas名称</param>
        /// <param name="axis">显示的轴</param>
        /// <param name="title">标题内容</param>
        /// <param name="color">标题颜色</param>
        public void SetAxisTitle(Axis axis, string title, Color color, params string[] chartAreasName)
        {
            Font font = new Font("微软雅黑", 8F);
            StringAlignment alignment = StringAlignment.Center;
            TextOrientation orientation = TextOrientation.Auto;
            for (int i = 0; i < chartAreasName.Length; i++)
            {
                switch (axis)
                {
                    case Axis.AxisX:
                        orientation = CurrentChart.ChartAreas[chartAreasName[i]].AxisX.TextOrientation;
                        alignment = CurrentChart.ChartAreas[chartAreasName[i]].AxisX.TitleAlignment;
                        font = CurrentChart.ChartAreas[chartAreasName[i]].AxisX.TitleFont;
                        break;
                    case Axis.AxisY:
                        orientation = CurrentChart.ChartAreas[chartAreasName[i]].AxisY.TextOrientation;
                        alignment = CurrentChart.ChartAreas[chartAreasName[i]].AxisY.TitleAlignment;
                        font = CurrentChart.ChartAreas[chartAreasName[i]].AxisY.TitleFont;
                        break;
                    case Axis.SecondaryX:
                        orientation = CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.TextOrientation;
                        alignment = CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.TitleAlignment;
                        font = CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.TitleFont;
                        break;
                    case Axis.SecondaryY:
                        orientation = CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.TextOrientation;
                        alignment = CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.TitleAlignment;
                        font = CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.TitleFont;
                        break;
                }
                SetAxisTitle(axis, title, color, font, orientation, alignment, chartAreasName[i]);
            }
        }






        /// <summary>
        /// 设置图表标题
        /// </summary>
        /// <param name="chartAreasName">ChartAreas名称</param>
        /// <param name="axis">显示的轴</param>
        /// <param name="title">标题内容</param>
        /// <param name="font">标题字体</param>
        public void SetAxisTitle(Axis axis, string title, Font font, params string[] chartAreasName)
        {
            Color color = Color.Black;
            StringAlignment alignment = StringAlignment.Center;
            TextOrientation orientation = TextOrientation.Auto;
            for (int i = 0; i < chartAreasName.Length; i++)
            {
                switch (axis)
                {
                    case Axis.AxisX:
                        orientation = CurrentChart.ChartAreas[chartAreasName[i]].AxisX.TextOrientation;
                        alignment = CurrentChart.ChartAreas[chartAreasName[i]].AxisX.TitleAlignment;
                        color = CurrentChart.ChartAreas[chartAreasName[i]].AxisX.TitleForeColor;
                        break;
                    case Axis.AxisY:
                        orientation = CurrentChart.ChartAreas[chartAreasName[i]].AxisY.TextOrientation;
                        alignment = CurrentChart.ChartAreas[chartAreasName[i]].AxisY.TitleAlignment;
                        color = CurrentChart.ChartAreas[chartAreasName[i]].AxisY.TitleForeColor;
                        break;
                    case Axis.SecondaryX:
                        orientation = CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.TextOrientation;
                        alignment = CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.TitleAlignment;
                        color = CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.TitleForeColor;
                        break;
                    case Axis.SecondaryY:
                        orientation = CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.TextOrientation;
                        alignment = CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.TitleAlignment;
                        color = CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.TitleForeColor;
                        break;
                }
                SetAxisTitle(axis, title, color, font, orientation, alignment, chartAreasName[i]);
            }
        }






        /// <summary>
        /// 设置图表标题
        /// </summary>
        /// <param name="chartAreasName">ChartAreas名称</param>
        /// <param name="axis">显示的轴</param>
        /// <param name="title">标题内容</param>
        /// <param name="orientation">标题文本方向</param>
        /// <param name="alignment">标题对齐方式</param>
        /// <param name="font">标题字体</param>
        /// <param name="color">标题颜色</param>
        public void SetAxisTitle(Axis axis, string title, Color color, Font font, TextOrientation orientation, StringAlignment alignment, params string[] chartAreasName)
        {
            for (int i = 0; i < chartAreasName.Length; i++)
            {

                switch (axis)
                {
                    case Axis.AxisX:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.Title = title;
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.TextOrientation = orientation;
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.TitleAlignment = alignment;
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.TitleFont = font;
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX.TitleForeColor = color;
                        break;
                    case Axis.AxisY:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.Title = title;
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.TextOrientation = orientation;
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.TitleAlignment = alignment;
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.TitleFont = font;
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY.TitleForeColor = color;
                        break;
                    case Axis.SecondaryX:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.Title = title;
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.TextOrientation = orientation;
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.TitleAlignment = alignment;
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.TitleFont = font;
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisX2.TitleForeColor = color;
                        break;
                    case Axis.SecondaryY:
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.Title = title;
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.TextOrientation = orientation;
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.TitleAlignment = alignment;
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.TitleFont = font;
                        CurrentChart.ChartAreas[chartAreasName[i]].AxisY2.TitleForeColor = color;
                        break;
                }
            }
        }

    }
}


#endif