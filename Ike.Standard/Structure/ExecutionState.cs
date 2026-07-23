using System;

namespace Ike.Standard
{
    public partial class WinAPI
    {
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
