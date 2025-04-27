using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using static Ike.Standard.Convert;

namespace Ike.Standard
{
	/// <summary>
	/// 扩展方法类
	/// </summary>
	public static class Extensions
	{


		/// <inheritdoc cref="NameToEnum"/>
		public static T ToEnum<T>(this string value) where T : struct, Enum
		{
			return NameToEnum<T>(value);
		}


		/// <inheritdoc cref="ValueToEnum"/>
		public static T ToEnum<T>(this int value) where T : struct, Enum
		{
			return ValueToEnum<T>(value);
		}


		/// <inheritdoc cref="StringToBytes(string,Encoding)"/>
		public static byte[] ToBytes(this string str, Encoding encoding)
		{
			return StringToBytes(str, encoding);
		}


		/// <inheritdoc cref="ColorToHex(Color)"/>
		public static string ToHex(this Color color)
		{
			return ColorToHex(color);
		}


		/// <inheritdoc cref="Text.ExtractString(string, string, string)"/> 
		public static string ExtractString(this string source, string start, string end)
		{
			return Text.ExtractString(source, start, end);
		}


		/// <inheritdoc cref="Convert.ConvertToBoolean(string)"/>
		public static bool ConvertToBoolean(this string value)
		{
			return Convert.ConvertToBoolean(value);
		}


		/// <summary>
		/// 检查值是否在<paramref name="minValue"/>和<paramref name="maxValue"/>之间(不包括<paramref name="minValue"/>)
		/// </summary>
		/// <param name="this">this</param>
		/// <param name="minValue">时间段起始值</param>
		/// <param name="maxValue">时间段结束值</param>
		/// <returns>如果值介于<paramref name="minValue"/>和<paramref name="maxValue"/>之间，则为<see langword="true"></see>，否则为<see langword="false"></see></returns>
		/// <remarks>
		/// [<see langword="2024年9月24日11:02:32" />]
		/// </remarks>
		public static bool Between(this DateTime @this, DateTime minValue, DateTime maxValue)
		{
			return minValue.CompareTo(@this) == -1 && @this.CompareTo(maxValue) == -1;
		}

		/// <summary>
		/// 安全地获取 <see cref="IEnumerable{T}"/> 中的元素数量，如果为 <see langword="null"/> 则返回 <see langword="0"/>
		/// </summary>
		/// <typeparam name="T">集合中元素的类型</typeparam>
		/// <param name="source">要检查的集合</param>
		/// <returns>集合中的元素数量</returns>
		public static int SafeCount<T>(this IEnumerable<T> source)
		{
			return source?.Count() ?? 0;
		}


		/// <summary>
		/// 判断 <see cref="IEnumerable{T}"/> 是否为空或为 <see langword="null"/>
		/// </summary>
		/// <typeparam name="T">集合中元素的类型</typeparam>
		/// <param name="source">要检查的集合</param>
		/// <returns>如果集合为空或为 <see langword="null"/>，返回 <see langword="true"/>；否则返回 <see langword="false"/></returns>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
		{
			return source == null || !source.Any();
		}
	}
}
