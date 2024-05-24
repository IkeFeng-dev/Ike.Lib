using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ike.Core
{
	/// <summary>
	/// 通用方法
	/// </summary>
	public class Common
	{
		/// <summary>
		/// 提取指定文本中两组字符串中间的文本
		/// </summary>
		/// <param name="source">提取的字符串源</param>
		/// <param name="start">开始位置字符串</param>
		/// <param name="end">结束位置字符串</param>
		/// <returns>正常返回提取结果,如果不包含<paramref name="start"/>或者<paramref name="end"/>时返回<see cref="string.Empty"></see></returns>
		public static string ExtractString(string source, string start, string end)
		{
			ReadOnlySpan<char> span = source.AsSpan();
			int startIndex = span.IndexOf(start.AsSpan());
			if (startIndex == -1)
			{
				return string.Empty;
			}
			startIndex += start.Length;
			int endIndex = span.Slice(startIndex).IndexOf(end.AsSpan());
			if (endIndex == -1)
			{
				return string.Empty;
			}
			return span.Slice(startIndex, endIndex).ToString();
		}

		/// <summary>
		/// 提取指定文本中两个字符串中间的所有内容
		/// </summary>
		/// <param name="source">提取的字符串源</param>
		/// <param name="start">开始位置字符串</param>
		/// <param name="end">结束位置字符串</param>
		/// <returns>匹配到的字符串集合</returns>
		public List<string> ExtractAllString(string source, string start, string end)
		{
			List<string> matches = new List<string>();
			ReadOnlySpan<char> remaining = source.AsSpan();
			while (true)
			{
				int startIndex = remaining.IndexOf(start.AsSpan());
				if (startIndex == -1) break;
				startIndex += start.Length;
				ReadOnlySpan<char> searchableSpan = remaining.Slice(startIndex);
				int endIndex = searchableSpan.IndexOf(end.AsSpan());
				if (endIndex == -1) break;
				ReadOnlySpan<char> match = searchableSpan.Slice(0, endIndex);
				matches.Add(match.ToString());
				remaining = searchableSpan.Slice(endIndex + end.Length);
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
			return string.Create(text.Length * copyCount, (text, copyCount), (chars, state) =>
			{
				var (str, count) = state;
				ReadOnlySpan<char> source = str.AsSpan();
				for (int i = 0; i < count; i++)
				{
					source.CopyTo(chars.Slice(i * str.Length));
				}
			});
		}



	}
}
