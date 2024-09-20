using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Ike.Standard
{
	/// <summary>
	/// 通用枚举类型
	/// </summary>
	public class Enums
	{
		/// <summary>
		/// 表示日志的类型
		/// </summary>
		public enum LogType
		{
			/// <summary>
			/// 详细信息,记录详细的调试信息或其他较为冗长的输出
			/// </summary>
			Verbose,
			/// <summary>
			/// 调试信息,用于诊断和调试目的
			/// </summary>
			Debug,
			/// <summary>
			/// 提供程序状态信息,用于跟踪程序的执行情况
			/// </summary>
			Information,
			/// <summary>
			/// 表示成功
			/// </summary>
			Succeed,
			/// <summary>
			/// 警告信息,表示潜在的问题或不符合预期的情况,但不会影响程序的正常执行
			/// </summary>
			Warning,
			/// <summary>
			/// 错误信息,用于指示程序出现的错误,但不会导致程序终止执行
			/// </summary>
			Error,
			/// <summary>
			/// 严重错误信息,用于指示程序出现的严重错误,可能会导致程序终止执行或无法继续正常运行
			/// </summary>
			Critical
		}

		/// <summary>
		/// <see cref="ExecutionState"/> 枚举标志,表示 <see cref="WinAPI.SetThreadExecutionState"/> 方法的参数,用于指定执行状态
		/// </summary>
		[Flags]
		public enum ExecutionState : uint
		{
			/// <summary>
			/// 通过复位系统空闲定时器,强制系统进入工作状态
			/// </summary>
		    SystemRequired = 0x01,
			/// <summary>
			/// 通过重置显示空闲计时器来强制打开显示
			/// </summary>
			DisplayRequired = 0x02,
			/// <summary>
			/// 该值不支持,如果<see cref="UserPresent"/> 与其他esFlags值相结合,则调用将失败,没有指定的状态设置
			/// </summary>
			[Obsolete("该值不支持")]
			UserPresent = 0x04,
			/// <summary>
			/// 启用离开模式,此值必须指定<see cref="Continuous"/>
			/// <para />
			/// 离开模式应该只用于媒体记录和媒体分发应用程序,这些应用程序必须在桌面计算机上执行关键的后台处理,而计算机似乎处于睡眠状态
			/// </summary>
			AwaymodeRequired = 0x40,
			/// <summary>
			///通知系统正在设置的状态应该保持有效,直到下一次调用使用<see cref="Continuous"/>并且其他状态标志之一被清除
			/// </summary>
			Continuous = 0x80000000,
		}
	}
}
