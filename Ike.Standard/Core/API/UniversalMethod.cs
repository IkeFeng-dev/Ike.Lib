#if NET5_0_OR_GREATER


using System.Diagnostics;

namespace Ike.Standard.Controller
{
    /// <summary>
    /// 控制器通用方法
    /// </summary>
    internal class UniversalMethod
    {
        /// <summary>
        /// 获取耗时
        /// </summary>
        /// <param name="sw"></param>
        /// <returns></returns>
        internal static long StopAndGetMilliseconds(Stopwatch sw)
        {
            if (sw.IsRunning)
            {
                sw.Stop();
            }

            return sw.ElapsedMilliseconds;
        }
    }
}

#endif