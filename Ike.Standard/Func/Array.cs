using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ike.Standard
{
	/// <summary>
	/// 数组操作类
	/// </summary>
	public static class Array
	{
		/// <summary>
		/// 泛型数组转换
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="value">值</param>
		/// <param name="split">分割符</param>
		/// <param name="converter">转换方法,例如:  <see cref="int.Parse(string)"/></param>
		/// <returns>转换结果</returns>
		public static T[] ConvertArray<T>(string value, char split, Func<string, T> converter)
		{
			return value.Split(split).Select(s => converter(s)).ToArray();
		}

		/// <summary>
		/// 将多个相同类型的数组合并为一个数组
		/// </summary>
		/// <typeparam name="T">数组元素的类型。</typeparam>
		/// <param name="arrays">需要合并的数组集合，支持传递任意数量的数组</param>
		/// <returns>
		/// 返回包含所有输入数组元素的新数组，元素顺序与传入顺序保持一致
		/// 如果未传入任何数组，则返回一个空数组
		/// </returns>
		/// <remarks>
		/// 该方法允许传递可变数量的数组参数, 它首先计算所有数组的总长度，然后创建一个新的数组，并依次将输入数组中的元素复制到结果数组中
		/// </remarks>
		public static T[] CombineArrays<T>(params T[][] arrays)
		{
			// 计算所有数组的总长度
			int totalLength = 0;
			foreach (T[] array in arrays)
			{
				totalLength += array.Length;
			}
			// 创建结果数组
			T[] result = new T[totalLength];
			// 将所有数组复制到结果数组中
			int currentIndex = 0;
			foreach (T[] array in arrays)
			{
				array.CopyTo(result, currentIndex);
				currentIndex += array.Length;
			}
			return result;
		}

	}
}
