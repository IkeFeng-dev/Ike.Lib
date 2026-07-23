using System.Runtime.InteropServices;

namespace Ike.Standard
{
    public partial class WinAPI
    {
        /// <summary>
        /// 日期结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]

    public struct SYSTEMTIME
    {
        /// <summary>年（如 2026）</summary>
        public short Year;

        /// <summary>月（1-12）</summary>
        public short Month;

        /// <summary>星期（0=周日，1=周一，...，6=周六）</summary>
        public short DayOfWeek;

        /// <summary>日（1-31）</summary>
        public short Day;

        /// <summary>时（0-23）</summary>
        public short Hour;

        /// <summary>分（0-59）</summary>
        public short Minute;

        /// <summary>秒（0-59）</summary>
        public short Second;

        /// <summary>毫秒（0-999）</summary>
        public short Miliseconds;
    }

}
}
