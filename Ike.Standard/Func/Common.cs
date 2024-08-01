using System;
using System.Collections.Generic;
using System.IO;
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
				if (MatchPattern(input,pattern))
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