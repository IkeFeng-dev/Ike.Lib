using System.Windows.Media;

namespace Ike.ActionEngine.Model
{
    /// <summary>
    /// 用于 UI 绑定的日志显示模型
    /// </summary>
    public class LogDisplayItem
    {
        /// <summary> 格式化后的时间字符串 </summary>
        public string Time { get; set; }

        /// <summary> 日志类型标签文字 </summary>
        public string TypeLabel { get; set; }

        /// <summary> 当前步骤号 </summary>
        public int Step { get; set; }

        /// <summary> 所有步骤号 </summary>
        public int StepNumbers { get; set; }

        /// <summary> 当前动作名称 </summary>
        public string ActionName { get; set; }

        /// <summary> 日志信息内容 </summary>
        public string Info { get; set; }

        /// <summary> 类型标签前景色 </summary>
        public Brush LabelForeground { get; set; }

        /// <summary> 类型标签背景色 </summary>
        public Brush LabelBackground { get; set; }

        /// <summary> 信息文字颜色 </summary>
        public Brush InfoForeground { get; set; }
    }
}
