using System.Collections.Generic;
using System.Text;

namespace Ike.Standard
{
	/// <summary>
	/// 文本相关
	/// </summary>
	public class Text
	{
		/// <summary>
		/// 提取指定文本中两组字符串中间的文本
		/// </summary>
		/// <param name="source">提取的字符串源</param>
		/// <param name="start">开始位置字符串</param>
		/// <param name="end">结束位置字符串</param>
		/// <returns>正常返回提取结果,如果不包含<paramref name="start"/>或者<paramref name="end"/>时返回<see cref="string.Empty"></see></returns>
		public string ExtractString(string source, string start, string end)
		{
			int startIndex = source.IndexOf(start) + start.Length;
			if (startIndex < start.Length) return string.Empty;
			int endIndex = source.IndexOf(end, startIndex);
			if (endIndex < 0) return string.Empty;
			return source.Substring(startIndex, endIndex - startIndex);
		}

		/// <summary>
		/// 提取指定文本中两个字符串中间的所有内容
		/// </summary>
		/// <param name="source">提取的字符串源</param>
		/// <param name="start">开始位置字符串</param>
		/// <param name="end">结束位置字符串</param>
		/// <returns>匹配到的字符串集合</returns>
		public List<string> ExtractAllStrings(string source, string start, string end)
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
			return matches;
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
