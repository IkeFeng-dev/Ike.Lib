using System.Runtime.InteropServices;

namespace Ike.Standard
{
    public partial class MouseHook
    {
        /// <summary>
        /// Point坐标数据
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            /// <summary>
            /// X坐标
            /// </summary>
            public int X;
            /// <summary>
            /// Y坐标
            /// </summary>
            public int Y;
        }

    }
}
