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
    }
}
