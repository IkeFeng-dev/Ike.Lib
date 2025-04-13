using System;
using static Ike.Standard.Log;

namespace Ike.Standard
{
	public partial class Console
	{
		/// <summary>
		/// 控制台日志输出类
		/// </summary>
		public class Log
		{
			/// <summary>
			/// 获取或设置调用基本日志方法时使用的输出方法
			/// <list type="table">
			/// <item><see langword="false" />: <see cref="Write(string, LogType)"/></item>
			/// <item><see langword="true" />: <see cref="WriteHighlight(string, LogType)"/></item>
			/// </list>
			/// </summary>
			public bool DefaultHighlight { get; set; } = false;
			/// <summary>
			/// 获取或设置日志输出的时间戳颜色
			/// </summary>
			public Structure.RGB TimestampColor { get; set; }
			/// <summary>
			/// 获取或设置调用<see cref="Write(string, LogType)"/>日志的输出颜色
			/// </summary>
			public Structure.RGB LogTextColor { get; set; } = new Structure.RGB(220, 220, 220);
			/// <summary>
			/// 获取或设置日志类型<see cref="LogType.Verbose"/>的输出颜色
			/// </summary>
			public Structure.RGB VerboseColor { get; set; } = new Structure.RGB(175,175,175);
			/// <summary>
			/// 获取或设置日志类型<see cref="LogType.Debug"/>的输出颜色
			/// </summary>
			public Structure.RGB DebugColor { get; set; } = new Structure.RGB(0,135,255);
			/// <summary>
			/// 获取或设置日志类型<see cref="LogType.Information"/>的输出颜色
			/// </summary>
			public Structure.RGB InformationColor { get; set; } = new Structure.RGB(255, 255, 255);
			/// <summary>
			/// 获取或设置日志类型<see cref="LogType.Succeed"/>的输出颜色
			/// </summary>
			public Structure.RGB SucceedColor { get; set; } = new Structure.RGB(32, 178, 170);
			/// <summary>
			/// 获取或设置日志类型<see cref="LogType.Warning"/>的输出颜色
			/// </summary>
			public Structure.RGB WarningColor { get; set; } = new Structure.RGB(175, 175, 0);
			/// <summary>
			/// 获取或设置日志类型<see cref="LogType.Error"/>的输出颜色
			/// </summary>
			public Structure.RGB ErrorColor { get; set; } = new Structure.RGB(215, 0, 135);
			/// <summary>
			/// 获取或设置日志类型<see cref="LogType.Critical"/>的输出颜色
			/// </summary>
			public Structure.RGB CriticalColor { get; set; } = new Structure.RGB(215, 0, 0);
			/// <summary>
			/// 获取或设置日志输出的时间戳格式
			/// </summary>
			public string TimestampFormat { get; set; }
			/// <summary>
			/// <inheritdoc cref="Log(Structure.RGB, string)"/>
			/// </summary>
			public Log()
			{
				TimestampColor = new Structure.RGB(150,150,150);
				TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff";
			}
			/// <summary>
			/// 构造控制台日志输出类
			/// </summary>
			/// <param name="timestampColor">日志输出的时间戳颜色</param>
			/// <param name="timestampFormat">日志输出的时间戳格式</param>
			public Log(Structure.RGB timestampColor, string timestampFormat)
			{
				TimestampColor = timestampColor;
				TimestampFormat = timestampFormat;
			}

			/// <summary>
			/// 写入控制台日志
			/// </summary>
			/// <param name="value">输出值</param>
			/// <param name="logType">日志类型</param>
			public void Write(string value, LogType logType)
			{
				Console.Write(DateTime.Now.ToString(TimestampFormat), TimestampColor);
				switch (logType)
				{
					case LogType.Verbose:
						Console.Write("  |Ver|  ", VerboseColor);
						break;
					case LogType.Debug:
						Console.Write("  |Deb|  ", DebugColor);
						break;
					case LogType.Information:
						Console.Write("  |Inf|  ", InformationColor);
						break;
					case LogType.Succeed:
						Console.Write("  |Suc|  ", SucceedColor);
						break;
					case LogType.Warning:
						Console.Write("  |War|  ", WarningColor);
						break;
					case LogType.Error:
						Console.Write("  |Err|  ", ErrorColor);
						break;
					case LogType.Critical:
						Console.Write("  |Cri|  ", CriticalColor);
						break;
				}
				WriteLine(value, LogTextColor);
			}


			/// <summary>
			/// 高亮写入控制台日志
			/// </summary>
			/// <param name="value">输出值</param>
			/// <param name="logType">日志类型</param>
			public void WriteHighlight(string value, LogType logType)
			{
				Console.Write(DateTime.Now.ToString(TimestampFormat) + ":  ", TimestampColor);
				switch (logType)
				{
					case LogType.Verbose:
						WriteLine(value, VerboseColor);
						break;
					case LogType.Debug:
						WriteLine(value, DebugColor);
						break;
					case LogType.Information:
						WriteLine(value, InformationColor);
						break;
					case LogType.Succeed:
						WriteLine(value, SucceedColor);
						break;
					case LogType.Warning:
						WriteLine(value, WarningColor);
						break;
					case LogType.Error:
						WriteLine(value, ErrorColor);
						break;
					case LogType.Critical:
						WriteLine(value, CriticalColor);
						break;
				}
			}

			/// <summary>
			/// 异常输出到控制台
			/// </summary>
			/// <param name="exception">捕获的异常</param>
			public void Write(Exception exception)
			{
				Write(exception.ToString(), LogType.Critical);
			}

			/// <summary>
			/// 输出<see cref="LogType.Verbose"/>类型日志
			/// </summary>
			/// <param name="value">输出值</param>
			public void Verbose(string value)
			{
				if (DefaultHighlight)
				{
					WriteHighlight(value, LogType.Verbose);
				}
				else
				{
					Write(value, LogType.Verbose);
				}
			}
			/// <summary>
			/// 输出<see cref="LogType.Debug"/>类型日志
			/// </summary>
			/// <param name="value">输出值</param>
			public void Debug(string value)
			{
				if (DefaultHighlight)
				{
					WriteHighlight(value, LogType.Debug);
				}
				else
				{
					Write(value, LogType.Debug);
				}
			}
			/// <summary>
			/// 输出<see cref="LogType.Information"/>类型日志
			/// </summary>
			/// <param name="value">输出值</param>
			public void Information(string value)
			{
				if (DefaultHighlight)
				{
					WriteHighlight(value, LogType.Information);
				}
				else
				{
					Write(value, LogType.Information);
				}
			}
			/// <summary>
			/// 输出<see cref="LogType.Succeed"/>类型日志
			/// </summary>
			/// <param name="value">输出值</param>
			public void Succeed(string value)
			{
				if (DefaultHighlight)
				{
					WriteHighlight(value, LogType.Succeed);
				}
				else
				{
					Write(value, LogType.Succeed);
				}
			}
			/// <summary>
			/// 输出<see cref="LogType.Warning"/>类型日志
			/// </summary>
			/// <param name="value">输出值</param>
			public void Warning(string value)
			{
				if (DefaultHighlight)
				{
					WriteHighlight(value, LogType.Warning);
				}
				else
				{
					Write(value, LogType.Warning);
				}
			}
			/// <summary>
			/// 输出<see cref="LogType.Error"/>类型日志
			/// </summary>
			/// <param name="value">输出值</param>
			public void Error(string value)
			{
				if (DefaultHighlight)
				{
					WriteHighlight(value, LogType.Error);
				}
				else
				{
					Write(value, LogType.Error);
				}
			}

			/// <summary>
			/// 输出<see cref="LogType.Critical"/>类型日志
			/// </summary>
			/// <param name="value">输出值</param>
			public void Critical(string value)
			{
				if (DefaultHighlight)
				{
					WriteHighlight(value, LogType.Critical);
				}
				else
				{
					Write(value, LogType.Critical);
				}
			}

		}
	}
}
