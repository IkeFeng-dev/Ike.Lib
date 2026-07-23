using System.Runtime.InteropServices;

namespace Ike.Standard
{
    public partial class Keyboard
    {
        /// <summary>
        /// <inheritdoc cref="Ike.Standard.Keyboard.Hook"/>
        /// </summary>
        public partial class Hook
        {
            /// <summary>
            /// 钩子结构
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public class KeyBoardHookStruct
            {
                /// <summary>
                /// 按键码
                /// </summary>
                public int KeyCode;
                /// <summary>
                /// 扫描码，一般不用
                /// </summary>
                public int ScanCode;
                /// <summary>
                /// 按键状态[0 按下,128 松开]
                /// </summary>
                public int Flags;
                /// <summary>
                /// 时间戳
                /// </summary>
                public int Time;
                /// <summary>
                /// 一般为0
                /// </summary>
                public int DwExtraInfo;
            }
        }

    }
}
