using System;
using System.Collections.Generic;
using System.Text;

namespace Ike.Standard
{
    /// <summary>
    /// 常量值
    /// </summary>
    public class ConstValue
    {
        /// <summary>
        /// 表示标准输入设备的句柄标识符
        /// </summary>
        public const int STD_INPUT_HANDLE = -10;
        /// <summary>
        /// 控制台扩展功能标志
        /// </summary>
        public const uint ENABLE_EXTENDED_FLAGS = 0x0080;
        /// <summary>
        /// 控制台快速编辑模式标志
        /// </summary>
        public const uint ENABLE_QUICK_EDIT_MODE = 0x0040;

        /// <summary>
        /// 标准输出设备句柄标识符
        /// <para>Windows API 常量，对应标准输出流的句柄值</para>
        /// </summary>
        public const int STD_OUTPUT_HANDLE = -11;

        /// <summary>
        /// 启用虚拟终端处理的控制台模式标志
        /// <para>允许控制台解析ANSI转义序列(如颜色代码)</para>
        /// </summary>
        public const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
    }
}
