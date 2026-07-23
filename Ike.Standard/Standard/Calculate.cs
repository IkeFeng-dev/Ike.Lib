using System;
using System.Linq;

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




        /// <summary>
        /// 比较时间是否在指定时间段内
        /// </summary>
        /// <param name="time">比较的时间</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static bool CompareTime(DateTime time, DateTime startTime, DateTime endTime)
        {
            return DateTime.Compare(startTime, time) == DateTime.Compare(time, endTime);
        }



        /// <summary>
        /// 计算百分比
        /// </summary>
        /// <param name="total">总数</param>
        /// <param name="number">计算数</param>
        /// <param name="decimal">保留位数</param>
        /// <returns></returns>
        public static double CalculationPercentage(int total, int number, byte @decimal = 2)
        {
            return CalculationPercentage((double)total, (double)number, @decimal);
        }



        /// <summary>
        /// 计算百分比
        /// </summary>
        /// <param name="total">总数</param>
        /// <param name="number">计算数</param>
        /// <param name="decimal">保留位数</param>
        /// <returns></returns>
        public static double CalculationPercentage(double total, double number, byte @decimal = 2)
        {
            double value = Math.Round((1 - number / total) * 100, @decimal);
            if (value.ToString() == "NaN")
            {
                value = 0;
            }
            return value;
        }

    }
}
