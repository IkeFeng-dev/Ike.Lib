using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ike.Standard
{
	/// <summary>
	/// 文本相关
	/// </summary>
	public static class Text
	{
		/// <summary>
		/// 解析包含日期时间变量的路径,格式{yyyy-MM-dd}=2025-09-16
		/// </summary>
		/// <param name="templatePath">动态路径</param>
		/// <returns>返回解析后的路径</returns>
		public static string ResolvePathWithDateTime(string templatePath)
		{
			if (string.IsNullOrEmpty(templatePath))
				return templatePath;
			string resolvedPath = Regex.Replace(templatePath, @"\{(\w+(-\w+)*)\}", m => DateTime.Now.ToString(m.Groups[1].Value));
			return resolvedPath;
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
		/// 生成两个字符串的所有字符组合（全排列）
		/// </summary>
		/// <param name="str1">第一个字符串</param>
		/// <param name="str2">第二个字符串</param>
		/// <param name="distinct">是否去重，如果为 <see langword="true"></see> ，则返回去重后的组合</param>
		/// <returns>返回包含所有字符排列的字符串数组</returns>
		public static string[] GetAllCombinations(string str1, string str2, bool distinct = false)
		{
			return GetAllCombinations(str1 + str2, distinct);
		}

		/// <summary>
		/// 生成字符串的所有字符组合（全排列）
		/// </summary>
		/// <param name="str">字符串</param>
		/// <param name="distinct">是否去重，如果为 <see langword="true"></see>，则返回去重后的组合</param>
		/// <returns>返回包含所有字符排列的字符串数组</returns>
		public static string[] GetAllCombinations(string str, bool distinct = false)
		{
			string combined = str;
			List<string> permutations = new List<string>();
			// 转换为字符数组
			char[] array = combined.ToCharArray();
			// 使用栈模拟递归进行全排列生成
			Stack<Tuple<char[], int>> stack = new Stack<Tuple<char[], int>>();
			stack.Push(new Tuple<char[], int>(array, 0));
			while (stack.Count > 0)
			{
				var current = stack.Pop();
				char[] currentArray = current.Item1;
				int start = current.Item2;
				if (start == currentArray.Length - 1)
				{
					permutations.Add(new string(currentArray));
				}
				else
				{
					for (int i = start; i < currentArray.Length; i++)
					{
						// 使用元组交换字符
						(currentArray[i], currentArray[start]) = (currentArray[start], currentArray[i]);
						// 递归下一层，放入栈中
						char[] newArray = (char[])currentArray.Clone();
						stack.Push(new Tuple<char[], int>(newArray, start + 1));
						// 回溯，交换回来
						(currentArray[i], currentArray[start]) = (currentArray[start], currentArray[i]);
					}
				}
			}
			return distinct ? permutations.Distinct().ToArray() : permutations.ToArray();
		}



		/// <summary>
		/// 提取指定文本中两组字符串中间的文本
		/// </summary>
		/// <param name="source">提取的字符串源</param>
		/// <param name="start">开始位置字符串</param>
		/// <param name="end">结束位置字符串</param>
		/// <returns>正常返回提取结果,如果不包含<paramref name="start"/>或者<paramref name="end"/>时返回<see cref="string.Empty"></see></returns>
		/// <remarks>
		/// [<see langword="2025年4月14日18:23:57" />]
		/// </remarks>
		public static string ExtractString(string source, string start, string end)
		{
			if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
			{
				return string.Empty;
			}

			int startIndex = source.IndexOf(start);
			if (startIndex == -1)
			{
				return string.Empty;
			}

			startIndex += start.Length;

			int endIndex = source.IndexOf(end, startIndex);
			if (endIndex == -1)
			{
				return string.Empty;
			}

			return source.Substring(startIndex, endIndex - startIndex);
		}


		/// <summary>
		/// 提取指定文本中两个字符串中间的所有内容
		/// </summary>
		/// <param name="source">提取的字符串源</param>
		/// <param name="start">开始位置字符串</param>
		/// <param name="end">结束位置字符串</param>
		/// <returns>匹配到的字符串集合</returns>
		public static string[] ExtractAllStrings(string source, string start, string end)
		{
			List<string> matches = new List<string>();
			int startIndex = 0;
			while (true)
			{
				startIndex = source.IndexOf(start, startIndex);
				if (startIndex == -1) break;

				startIndex += start.Length;
				int endIndex = source.IndexOf(end, startIndex);
				if (endIndex == -1) break;
				string match = source.Substring(startIndex, endIndex - startIndex);
				matches.Add(match);
				startIndex = endIndex + end.Length;
			}
			return matches.ToArray();
		}

		/// <summary>
		/// 复制字符串指定次数
		/// </summary>
		/// <param name="text">复制的字符串</param>
		/// <param name="copyCount">复制次数</param>
		/// <returns>复制后拼接的的内容</returns>
		public static string CopyString(string text, int copyCount)
		{
			StringBuilder builder = new StringBuilder(text.Length * copyCount);
			for (int i = 0; i < copyCount; i++)
			{
				builder.Append(text);
			}
			return builder.ToString();
		}

		/// <summary>
		/// 判断文本是否包含中文字符
		/// </summary>
		/// <param name="input">输入字符</param>
		/// <param name="chinesePunctuation">是否检查包含中文状态的标点符号</param>
		/// <returns></returns>
		public static bool ContainsChinese(string input,bool chinesePunctuation)
		{
			foreach (char c in input)
			{
				bool checkChinese = c >= '\u4e00' && c <= '\u9fa5';
				if (chinesePunctuation)
				{
					if (checkChinese || (c >= '\u3000' && c <= '\u303f'))
					{
						return true;
					}
				}
				else if(checkChinese)
				{
					return true;
				}
			}
			return false;
		}

	}
}
