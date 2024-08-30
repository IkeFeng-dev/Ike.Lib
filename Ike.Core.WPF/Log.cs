using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Ike.Core.WPF
{
	public class Log
	{
		/// <summary>
		/// 锁
		/// </summary>
		private object lock_log = new object();
		/// <summary>
		/// 日志输出对象
		/// </summary>
		private RichTextBox richTextBox;
		/// <summary>
		/// 日志输出的最大行数
		/// </summary>
		private int maxLines = 200;
		/// <summary>
		/// 日志输出一行的最大字数限制
		/// </summary>
		private int outMaxLength = 1000;
		/// <summary>
		/// 日志文本的行高
		/// </summary>
		private int lineHeight = 1;
		/// <summary>
		/// 日志时间戳格式
		/// </summary>
		private string datetimeFormat = "HH:mm:ss.fff";
		/// <summary>
		/// 获取或设置日志类型<see cref="LogType.Normal"/>的颜色
		/// </summary>
		public SolidColorBrush NormalColor { get; set; } = Brushes.Gray;
		/// <summary>
		/// 获取或设置日志类型<see cref="LogType.Info"/>的颜色
		/// </summary>
		public SolidColorBrush InfoColor { get; set; } = Brushes.DarkBlue;
		/// <summary>
		/// 获取或设置日志类型<see cref="LogType.Warn"/>的颜色
		/// </summary>
		public SolidColorBrush WarnColor { get; set; } = Brushes.DarkOrange;
		/// <summary>
		/// 获取或设置日志类型<see cref="LogType.Error"/>的颜色
		/// </summary>
		public SolidColorBrush ErrorColor { get; set; } = Brushes.DarkRed;
		/// <summary>
		/// 获取或设置日志类型<see cref="LogType.Success"/>的颜色
		/// </summary>
		public SolidColorBrush SuccessColor { get; set; } = Brushes.DarkGreen;
		/// <summary>
		/// 获取或设置日志类型<see cref="LogType.Custom"/>的颜色
		/// </summary>
		public SolidColorBrush CustomColor { get; set; } = Brushes.Black;
		/// <summary>
		/// 获取或设置日志类型''的颜色
		/// </summary>
		/// <summary>
		/// LOG类型
		/// </summary>
		public enum LogType
		{
			/// <summary>
			/// 普通
			/// </summary>
			Normal,
			/// <summary>
			/// 信息
			/// </summary>
			Info,
			/// <summary>
			/// 警告
			/// </summary>
			Warn,
			/// <summary>
			/// 错误
			/// </summary>
			Error,
			/// <summary>
			/// 成功
			/// </summary>
			Success,
			/// <summary>
			/// 自定义
			/// </summary>
			Custom
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="richTextBox">输出日志的富文本框</param>
		/// <param name="maxLines">日志输出的最大行数</param>
		/// <param name="outMaxLength">单行日志输出字符的最大长度</param>
		/// <param name="datetimeFormat">时间戳的格式</param>
		/// <exception cref="ArgumentNullException"></exception>
		public Log(RichTextBox richTextBox, int maxLines = 200, int outMaxLength = 1000, int lineHeight = 1, string datetimeFormat = "HH:mm:ss.fff")
		{
			if (richTextBox is null)
			{
				throw new ArgumentNullException(nameof(richTextBox));
			}
			this.richTextBox = richTextBox;
			if (maxLines > 0)
			{
				this.maxLines = maxLines;
			}
			if (outMaxLength > 0)
			{
				this.outMaxLength = outMaxLength;
			}
			if (lineHeight > 0)
			{
				this.lineHeight = lineHeight;
			}
			if (!string.IsNullOrWhiteSpace(datetimeFormat))
			{
				this.datetimeFormat = datetimeFormat;
			}
		}

		/// <summary>
		/// 写入一行日志到富文本框
		/// </summary>
		/// <param name="text">日志内容</param>
		/// <param name="logType">日志类型</param>
		/// <param name="richTextBox">富文本框</param>
		/// <param name="isScrollToEnd">写入后是否滚动到底部</param>
		/// <returns></returns>
		public bool WriteLine(string text, LogType logType, RichTextBox richTextBox, bool isScrollToEnd = true)
		{
			if (text.Length > outMaxLength)
			{
				return false;
			}
			bool lockTaken = false;
			Monitor.Enter(lock_log,ref lockTaken);
			try
			{
				string datetime = DateTime.Now.ToString(datetimeFormat);
				SolidColorBrush color = CustomColor;
				switch (logType)
				{
					case LogType.Normal:
						color = NormalColor;
						break;
					case LogType.Info:
						color = InfoColor;
						break;
					case LogType.Warn:
						color = WarnColor;
						break;
					case LogType.Error:
						color = ErrorColor;
						break;
					case LogType.Success:
						color = SuccessColor;
						break;
				}
				System.Windows.Application.Current.Dispatcher.Invoke(() =>
					{
						if (richTextBox.Document.Blocks.Count >= maxLines)
						{
							richTextBox.Document.Blocks.Remove(richTextBox.Document.Blocks.FirstBlock);
						}
						FlowDocument mcFlowDoc = richTextBox.Document ?? new FlowDocument();
						Paragraph para = new Paragraph()
						{
							LineHeight = lineHeight
						};
						string line = string.Format("{0}  :{1}", datetime, text);
						para.Inlines.Add(new Run(line));
						para.Foreground = color;
						mcFlowDoc.Blocks.Add(para);
						richTextBox.Document = mcFlowDoc;
						if (isScrollToEnd)
						{
							richTextBox.ScrollToEnd();
						}
					});
				return true;
			}
			catch
			{
				return false;
			}
			finally
			{
				if (lockTaken)
				{
					Monitor.Exit(lock_log);
				}
			}
		}

		/// <summary>
		/// 写入一行日志到富文本框
		/// </summary>
		/// <param name="text">日志内容</param>
		/// <param name="logType">日志类型</param>
		/// <param name="richTextBox">富文本框</param>
		/// <param name="isScrollToEnd">写入后是否滚动到底部</param>
		/// <returns></returns>
		public bool WriteLine(string text, LogType logType, bool isScrollToEnd = true)
		{
			return WriteLine(text, logType, richTextBox, isScrollToEnd);
		}
		/// <summary>
		/// 输出<see cref="LogType.Normal"/>类型日志
		/// </summary>
		/// <param name="text">日志内容</param>
		/// <param name="isScrollToEnd">写入后是否滚动到底部</param>
		/// <returns></returns>
		public bool Normal(string text, bool isScrollToEnd = true)
		{
			return WriteLine(text, LogType.Normal, isScrollToEnd);
		}
		/// <summary>
		/// 输出<see cref="LogType.Info"/>类型日志
		/// </summary>
		/// <param name="text">日志内容</param>
		/// <param name="isScrollToEnd">写入后是否滚动到底部</param>
		/// <returns></returns>
		public bool Info(string text, bool isScrollToEnd = true)
		{
			return WriteLine(text, LogType.Info, isScrollToEnd);
		}
		/// <summary>
		/// 输出<see cref="LogType.Warn"/>类型日志
		/// </summary>
		/// <param name="text">日志内容</param>
		/// <param name="isScrollToEnd">写入后是否滚动到底部</param>
		/// <returns></returns>
		public bool Warn(string text, bool isScrollToEnd = true)
		{
			return WriteLine(text, LogType.Warn, isScrollToEnd);
		}
		/// <summary>
		/// 输出<see cref="LogType.Error"/>类型日志
		/// </summary>
		/// <param name="text">日志内容</param>
		/// <param name="isScrollToEnd">写入后是否滚动到底部</param>
		/// <returns></returns>
		public bool Error(string text, bool isScrollToEnd = true)
		{
			return WriteLine(text, LogType.Error, isScrollToEnd);
		}
		/// <summary>
		/// 输出<see cref="LogType.Success"/>类型日志
		/// </summary>
		/// <param name="text">日志内容</param>
		/// <param name="isScrollToEnd">写入后是否滚动到底部</param>
		/// <returns></returns>
		public bool Success(string text, bool isScrollToEnd = true)
		{
			return WriteLine(text, LogType.Success, isScrollToEnd);
		}
		/// <summary>
		/// 输出<see cref="LogType.Custom"/>类型日志
		/// </summary>
		/// <param name="text">日志内容</param>
		/// <param name="isScrollToEnd">写入后是否滚动到底部</param>
		/// <returns></returns>
		public bool Custom(string text, bool isScrollToEnd = true)
		{
			return WriteLine(text, LogType.Custom, isScrollToEnd);
		}
	}
}
