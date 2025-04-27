using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ike.Standard
{
    /// <summary>
    /// 计算类
    /// </summary>
    public static class Calculate
    {

        /// <summary>
        /// 计算集合中的平均时间
        /// </summary>
        /// <param name="times"><see cref="DateTime"/>集合</param>
        /// <returns></returns>
        public static DateTime GetAverageTime(DateTime[] times)
        {
            // 将 DateTime 转为 TimeSpan 与零时间差的偏移量
            double totalTicks = times.Sum(time => time.Ticks);

            // 计算平均时间
            long averageTicks = (long)(totalTicks / times.Length);

            // 使用平均 ticks 创建平均时间
            return new DateTime(averageTicks);
        }
    }
}
