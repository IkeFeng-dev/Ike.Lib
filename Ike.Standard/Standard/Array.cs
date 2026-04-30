using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
		/// <typeparam name="T">数组元素的类型</typeparam>
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


		/// <summary>
		/// 向数组中添加一个或多个元素
		/// </summary>
		/// <typeparam name="T">数组元素的类型</typeparam>
		/// <param name="originalArray">原始数组</param>
		/// <param name="newElements">要添加的新元素（一个或多个）</param>
		/// <returns>新的数组</returns>
		public static T[] AddElementsToArray<T>(T[] originalArray, params T[] newElements)
		{
			if (originalArray == null)
			{
				return newElements;
			}

			if (newElements == null || newElements.Length == 0)
			{
				return originalArray;
			}

			// 创建一个新的数组，长度为原数组长度 + 新元素个数
			T[] newArray = new T[originalArray.Length + newElements.Length];

			// 复制原数组到新数组
			System.Array.Copy(originalArray, newArray, originalArray.Length);

			// 复制新元素到新数组
			System.Array.Copy(newElements, 0, newArray, originalArray.Length, newElements.Length);

			return newArray;
		}




		/// <summary>
		/// 删除二维数组中指定行
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array">二维数组</param>
		/// <param name="rowIndex">删除的行</param>
		/// <exception cref="IndexOutOfRangeException">索引超界限</exception>
		public static void DeleteRowFrom2DArray<T>(ref T[,] array, int rowIndex)
		{
			int rows = array.GetLength(0);
			int cols = array.GetLength(1);
			if (rowIndex < 0 || rowIndex >= rows)
			{
				throw new IndexOutOfRangeException("指定的行索引无效");
			}
			T[,] newArray = new T[rows - 1, cols];
			int newArrayRowIndex = 0;
			for (int i = 0; i < rows; i++)
			{
				if (i == rowIndex)
				{
					continue;
				}
				for (int j = 0; j < cols; j++)
				{
					newArray[newArrayRowIndex, j] = array[i, j];
				}
				newArrayRowIndex++;
			}
			array = newArray;
		}




		/// <summary>
		/// 去除数组中空和null的内容
		/// </summary>
		/// <param name="arr">数组</param>
		/// <returns></returns>
		public static string[] ArrayTrimEmpty(string[] arr)
		{
			List<string> ret = new List<string>();
			for (int i = 0; i < arr.Length; i++)
			{
				if (arr[i] != null && arr[i] != string.Empty)
				{
					ret.Add(arr[i]);
				}
			}
			return ret.ToArray();
		}




		/// <summary>
		/// 反转二维数组中内容[反转行,最后一行变成第一行,行中数据不反转]
		/// </summary>
		/// <param name="arr">二维数组</param>
		/// <returns></returns>
		public static T[,] ArrayReversal<T>(T[,] arr)
		{
			int length = arr.GetLength(0);
			int length1 = arr.GetLength(1);
			if (length > 0)
			{
				T[,] temp = new T[length, length1];
				for (int i = 0; i < arr.GetLength(0); i++)
				{
					length--;
					for (int j = 0; j < length1; j++)
					{
						temp[i, j] = arr[length, j];
					}
				}
				return temp;
			}
			return arr;
		}




		



		/// <summary>
		/// Int数组转Double数组
		/// </summary>
		/// <param name="arr">要转换的数组</param>
		/// <returns>转换后的数组</returns>
		public static double[] IntArrayToDoubleArray(int[] arr)
		{
			double[] ret = new double[arr.Length];
			if (ret.Length > 0)
			{
				for (int i = 0; i < arr.Length; i++)
				{
					ret[i] = arr[i];
				}
			}
			else
			{
				return new double[0];
			}
			return ret;
		}




		/// <summary>
		/// Double数组转Int数组
		/// </summary>
		/// <param name="arr">要转换的数组</param>
		/// <returns>转换后的数组</returns>
		public static int[] DoubleToInt(double[] arr)
		{
			int[] ret = new int[arr.Length];
			if (ret.Length > 0)
			{
				for (int i = 0; i < arr.Length; i++)
				{
					ret[i] = (int)arr[i];
				}
			}
			else
			{
				return new int[0];
			}
			return ret;
		}






		/// <summary>
		/// 获取两个数组中相同的内容
		/// </summary>
		/// <param name="arrayA">数组A</param>
		/// <param name="arrayB">数组B</param>
		/// <returns>两个数组中相同的内容</returns>
		public static T[] CompareArraySameContent<T>(T[] arrayA, T[] arrayB)
		{
			if (arrayA == null) throw new ArgumentNullException(nameof(arrayA));
			if (arrayB == null) throw new ArgumentNullException(nameof(arrayB));

			if (arrayA.Length == 0 || arrayB.Length == 0)
				return System.Array.Empty<T>();

			// 使用HashSet提高查找性能 O(n)
			var setA = new HashSet<T>(arrayA);
			var commonElements = new List<T>();

			foreach (var item in arrayB)
			{
				if (setA.Contains(item) && !commonElements.Contains(item))
				{
					commonElements.Add(item);
				}
			}

			return commonElements.ToArray();
		}


        /// <summary>
        /// 获取两个数组中不相同的内容（对称差集）
        /// </summary>
        /// <param name="arrayA">数组A</param>
        /// <param name="arrayB">数组B</param>
        /// <returns>两个数组中不相同的内容</returns>
        public static T[] CompareArrayDifferentContent<T>(T[] arrayA, T[] arrayB)
        {
            if (arrayA == null) throw new ArgumentNullException(nameof(arrayA));
            if (arrayB == null) throw new ArgumentNullException(nameof(arrayB));

            if (arrayA.Length == 0 && arrayB.Length == 0)
                return System.Array.Empty<T>();

            // 使用 HashSet 提高性能
            var setA = new HashSet<T>(arrayA);
            var setB = new HashSet<T>(arrayB);
            var diff = new HashSet<T>();

            foreach (var item in setA)
                if (!setB.Contains(item))
                    diff.Add(item);

            foreach (var item in setB)
                if (!setA.Contains(item))
                    diff.Add(item);

            return diff.ToArray();
        }




        /// <summary>
        /// 获取在 <paramref name="getDifferentContentArr"/> 中存在，但在 <paramref name="sourceArr"/> 中不存在的内容（差集）
        /// </summary>
        /// <param name="sourceArr">原数组</param>
        /// <param name="getDifferentContentArr">获取不同内容的数组</param>
        /// <returns>在 <paramref name="getDifferentContentArr"/> 中但不在 <paramref name="sourceArr"/> 中的元素</returns>
        public static T[] GetArrayDifferentContent<T>(T[] sourceArr, T[] getDifferentContentArr)
        {
            if (sourceArr == null) throw new ArgumentNullException(nameof(sourceArr));
            if (getDifferentContentArr == null) throw new ArgumentNullException(nameof(getDifferentContentArr));

            if (getDifferentContentArr.Length == 0)
                return System.Array.Empty<T>();

            // 使用 HashSet 提高查找性能
            var sourceSet = new HashSet<T>(sourceArr, EqualityComparer<T>.Default);
            var differentList = new List<T>();

            foreach (var item in getDifferentContentArr)
            {
                if (!sourceSet.Contains(item))
                {
                    differentList.Add(item);
                }
            }

            return differentList.ToArray();
        }




        /// <summary>
        /// 找到数组中指定内容的索引
        /// </summary>
        /// <param name="arr">数组</param>
        /// <param name="findContent">查找内容</param>
        /// <returns>找到的索引，未找到返回 -1</returns>
        public static int FindContentIndex<T>(T[] arr, T findContent)
        {
            if (arr == null) throw new ArgumentNullException(nameof(arr));

            // 使用 EqualityComparer<T>.Default 来正确比较值
            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < arr.Length; i++)
            {
                if (comparer.Equals(arr[i], findContent))
                {
                    return i;
                }
            }
            return -1;
        }




	}
}
