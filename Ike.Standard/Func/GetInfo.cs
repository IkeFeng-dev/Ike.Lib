using System;
using System.Collections.Generic;
using System.Text;

namespace Ike.Standard
{
    /// <summary>
    /// 
    /// </summary>
    public static class GetInfo
    {
        /// <summary>
        /// 返回表示所提供标志中显式设置的枚举值的字符串数组
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="enumValue">一个或多个枚举值的组合[使用位运算功能,定义为标志位枚举，并显式赋值]</param>
        /// <returns>一个字符串数组，对应于显式设置的枚举值</returns>
        /// <exception cref="ArgumentException">如果T不是枚举类型</exception>
        public static string[] GetEnumFlagsAsStringArray<T>(T enumValue) where T : Enum
        {
            // 确保输入是一个有效的枚举类型
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enum type.");

            // 创建一个列表存储显式设置的枚举值
            var resultList = new List<string>();

            // 遍历枚举的所有值
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                // 转换为数值进行位运算
                var intValue = System.Convert.ToInt64(value);
                var intEnumValue = System.Convert.ToInt64(enumValue);

                // 检查是否显式设置了当前值
                if ((intEnumValue & intValue) == intValue && intValue != 0)
                {
                    resultList.Add(value.ToString());
                }
            }

            return resultList.ToArray();
        }


        /// <summary>
        /// 返回表示所提供标志中显式设置的枚举值的枚举数组
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="enumValue">一个或多个枚举值的组合[使用位运算功能,定义为标志位枚举，并显式赋值]</param>
        /// <returns>一个枚举数组，对应于显式设置的枚举值</returns>
        /// <exception cref="ArgumentException">如果T不是枚举类型</exception>
        public static T[] GetEnumFlagsAsEnumArray<T>(T enumValue) where T : Enum
        {
            // 确保输入是一个有效的枚举类型
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enum type.");

            // 创建一个列表存储显式设置的枚举值
            var resultList = new List<T>();

            // 遍历枚举的所有值
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                // 转换为数值进行位运算
                var intValue = System.Convert.ToInt64(value);
                var intEnumValue = System.Convert.ToInt64(enumValue);

                // 检查是否显式设置了当前值
                if ((intEnumValue & intValue) == intValue && intValue != 0)
                {
                    resultList.Add(value);
                }
            }

            return resultList.ToArray();
        }





    }
}
