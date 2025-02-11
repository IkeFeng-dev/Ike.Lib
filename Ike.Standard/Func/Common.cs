using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Ike.Standard
{
    /// <summary>
    /// 一些公共方法
    /// </summary>
    public class Common
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



        /// <summary>
        /// 使用通配符匹配字符串数组,通配符['*','?']
        /// </summary>
        /// <param name="inputs">待匹配的字符串集合</param>
        /// <param name="pattern">带通配符的匹配内容</param>
        /// <returns>成功匹配到的字符串集合</returns>
        public static string[] MatchPattern(IEnumerable<string> inputs, string pattern)
        {
            var matchedStrings = new List<string>();
            foreach (var input in inputs)
            {
                if (MatchPattern(input, pattern))
                {
                    matchedStrings.Add(input);
                }
            }
            return matchedStrings.ToArray();
        }

        /// <summary>
        /// 使用通配符匹配字符串['*','?']
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="pattern">带通配符的匹配内容</param>
        /// <returns>字符串是否匹配</returns>
        public static bool MatchPattern(string input, string pattern)
        {
            string regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
            return Regex.IsMatch(input, regexPattern);
        }

        /// <summary>
        /// 将程序内部镶嵌的资源文件保存包指定路径
        /// </summary>
        /// <param name="resPath">资源文件路径,格式为: [namespace.Res.source.png] 以'.'分割资源子级</param>
        /// <param name="outputPath">资源文件保存到的路径</param>
        /// <returns>写出后判断文件是否存在,存在则为<see langword="true"/>,反之为<see langword="false"/></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static bool GetResourceToFile(string resPath, string outputPath)
        {
            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resPath))
            {
                if (resourceStream is null)
                {
                    throw new FileNotFoundException(resPath);
                }
                using (FileStream fileStream = File.Create(outputPath))
                {
                    resourceStream.CopyTo(fileStream);
                }
                return File.Exists(outputPath);
            }
        }


        /// <summary>
        /// 检查参数是否为<see   langword="null"/>,是则抛出参数异常
        /// </summary>
        /// <param name="argumentValue">参数</param>
        /// <param name="argumentName">参数名,可使用<see langword="nameof"/>获取传递</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ThrowIfNull(object argumentValue, string argumentName)
        {
            if (argumentValue is null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }









    }
}