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
		/// 特殊字符集
		/// </summary>
		/// <remarks>
		/// [<see langword="2026年5月5日14:51:33" />]
		/// </remarks>
		public static readonly char[] SpecialCharacters = new char[] {
				'£','¢','¡','฿','¿','$','¥','﹉','△','○','□',
				'Ͱ','ͱ','Ͳ','ͳ','Ͷ','ͼ','ͽ','Ή', 'Ί','Ό','Ύ','Ώ','ΐ',
				'Α','Β','Γ','Δ','Ε','Ζ','Η','Θ','Ι','Κ','Λ','Μ','Ν','Ξ',
				'Ā','ā','Ă','ă','Ą','ą','Ć','ć','Ĉ','ĉ','Ċ','ċ','Č','č','Ď','ď','Đ','đ','Ē','ē','Ĕ','ĕ','Ė','ė','Ę','ę','Ě','ě','Ĝ',
				'ɐ','ɑ','ɒ','ɓ','ɔ','ɕ','ɖ','ɗ','ɘ','ə','ɚ','ɛ','ɜ','ɝ','ɞ','ɟ','ɠ','ɡ','ɢ','ɣ','ɤ','ɥ','ɦ','ɧ','ɨ',
				'Ѐ','Ё','Ђ','Ѓ','Є','Ѕ','І','Ї','Ј','Љ','Њ','Ћ','Ќ','Ѝ','Ў','Џ','А','Б','В','Г','Д','Е','Ж','З',
				'Ա','Բ','Գ','Դ','Ե','Զ','Է','Ը','Թ','Ժ','Ի','Լ','Խ','Ծ','Կ','Հ','Ձ','Ղ','Ճ','Մ','Յ','Ն','Շ','Ո','Չ','Պ',
				'Ⴥ','ა','ბ','გ','დ','ე','ვ','ზ','თ','ი','კ','ლ','მ','ნ','ო','პ','ჟ','რ','ს','ტ','უ','ფ','ქ','ღ','ყ','შ','ჩ','ც','ძ','წ',
				'Ꭰ','Ꭱ','Ꭲ','Ꭳ','Ꭴ','Ꭸ','Ꭹ','Ꭺ','Ꭻ','Ꭼ','Ꭽ','Ꭾ','Ꭿ','Ꮀ','Ꮁ',
				'ᴥ','ᴦ','ᴧ','ᴨ','ᴩ','ᴪ','ᴫ','ᴬ','ᴭ','ᴮ','ᴯ','ᴰ','ᴱ','ᴲ','ᴳ','ᴴ','ᴵ','ᵃ','ᵄ','ᵅ','ᵆ','ᵇ','ᵈ','ᵉ','ᵊ','ᵋ','ᵌ','ᵢ','ᵣ','ᵤ','ᵥ','ᵦ','ᵧ','ᵨ','ᵩ','ᵪ',
				'ἀ','ἁ','ἂ','ἃ','ἄ','ἅ','ἆ','ἇ','Ἀ','Ἁ','Ἂ','Ἃ',
				'Ⲁ','ⲁ','Ⲃ','ⲃ','Ⲅ','ⲅ','Ⲉ','ⲉ','Ⲋ','ⲋ','Ⲍ','ⲍ','Ⲏ','ⲏ','Ⲑ','ⲑ','Ⲓ','ⲓ','Ⲕ','ⲕ'
		};



		/// <summary>
		/// 对字符串进行填充或截断处理
		/// </summary>
		/// <param name="original">原始字符串</param>
		/// <param name="paddingChar">用于填充的字符</param>
		/// <param name="totalLength">目标长度</param>
		/// <param name="padLeft"><see langword="true"/> =左侧填充，<see langword="false"/> =右侧填充</param>
		/// <param name="overflowHandling">
		///     溢出处理方式：<br/>
		///     <see langword="1"/> = 从左侧截断（保留右侧字符）<br/>
		///     <see langword="2"/> = 从右侧截断（保留左侧字符）<br/>
		///     其余则全部保持原样不截断
		/// </param>
		/// <returns>处理后的字符串</returns>
		/// <remarks>
		/// [<see langword="2026年1月10日09:26:46" />]
		/// </remarks>
		public static string PadString(string original, char paddingChar, int totalLength, bool padLeft = false, int overflowHandling = 1)
		{
			if (original == null)
			{
				original = string.Empty;
			}

			// 处理字符串过长的情况
			if (original.Length > totalLength)
			{
				switch (overflowHandling)
				{
					case 1: // 从左侧截断（保留右侧字符）
						return original.Substring(original.Length - totalLength);
					case 2: // 从右侧截断（保留左侧字符）
						return original.Substring(0, totalLength);
					case 3: // 保持原样不截断
						return original;
					default:
						return original;
				}
			}

			// 处理填充
			return padLeft
				? original.PadLeft(totalLength, paddingChar)
				: original.PadRight(totalLength, paddingChar);
		}


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
		public static bool ContainsChinese(string input, bool chinesePunctuation)
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
				else if (checkChinese)
				{
					return true;
				}
			}
			return false;
		}


		/// <summary>
		/// 提取字符串中的纯数字
		/// </summary>
		/// <param name="text">要提取的原字符串</param>
		/// <returns>提取内容</returns>
		public static string ExtractNumber(string text)
		{
			return Regex.Replace(text, @"[^0-9]+", "");
		}


		/// <summary>
		/// 提取字符串中的小写字母
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string ExtractLowerLetter(string text)
		{
			return Regex.Replace(text, @"[^a-z]+", "");
		}


		/// <summary>
		/// 提取字符串中的大写字母
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string ExtractUpperLetter(string text)
		{
			return Regex.Replace(text, @"[^A-Z]+", "");
		}


		/// <summary>
		/// 提取字符串中的字母
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string ExtractLetter(string text)
		{
			return Regex.Replace(text, @"[^A-Za-z]+", "");
		}



	}
}
