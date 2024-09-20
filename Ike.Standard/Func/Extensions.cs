using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ike.Standard.Func
{
	/// <summary>
	/// 扩展方法类
	/// </summary>
	public static class Extensions
	{
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
