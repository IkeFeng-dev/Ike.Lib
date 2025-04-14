using System;
namespace Ike.Standard
{
	/// <summary>
	/// 控制台相关操作类
	/// </summary>
	public partial class Console
	{
		/// <summary>
		/// 是否为控制台环境
		/// </summary>
		/// <returns></returns>
		public static bool IsConsoleEnv()
		{
			try
			{
				return System.Console.WindowHeight > 0;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 将指定的字符以指定RGB颜色写入标准输出流
		/// </summary>
		/// <param name="value">写入标准输出流的字符串</param>
		/// <param name="rgb">RGB值</param>
		public static void Write(string value, Structure.RGB rgb)
		{
			Write(value, rgb.R, rgb.G, rgb.B);
		}

		/// <summary>
		///  <inheritdoc cref="Write(string, Structure.RGB)" path="member/summary"/>
		/// </summary>
		/// <param name="value"><inheritdoc cref="Write(string, Structure.RGB)" path="/param[@name='value']"/></param>
		/// <param name="r">Red</param>
		/// <param name="g">Green</param>
		/// <param name="b">Bule</param>
		public static void Write(string value, byte r, byte g, byte b)
		{
			string build = string.Format("\x1b[38;2;{0};{1};{2}m{3}\u001b[0m", r, g, b, value);
			System.Console.Write(build);
		}

		/// <summary>
		/// <inheritdoc cref="Write(string, Structure.RGB)" path="member/summary"/>
		/// </summary>
		/// <param name="value"><inheritdoc cref="Write(string, Structure.RGB)" path="/param[@name='value']"/></param>
		/// <param name="hexColor"><inheritdoc cref="Convert.HexToRGB(string)" path="/param[@name='hexColor']"/></param>
		public static void Write(string value, string hexColor)
		{
			Write(value, Convert.HexToRGB(hexColor));
		}

		/// <summary>
		/// 写入带有指定 RGB 颜色的字符串并换行
		/// </summary>
		/// <param name="value"><inheritdoc cref="Write(string, Structure.RGB)" path="/param[@name='value']"/> </param>
		/// <param name="rgb"><inheritdoc cref="Write(string, Structure.RGB)" path="/param[@name='rgb']"/></param>
		public static void WriteLine(string value, Structure.RGB rgb)
		{
			Write(value, rgb);
			System.Console.WriteLine();
		}

		/// <summary>
		/// <inheritdoc cref="WriteLine(string, Structure.RGB)" path="member/summary"/>
		/// </summary>
		/// <param name="value"><inheritdoc cref="Write(string, Structure.RGB)" path="/param[@name='value']"/></param>
		/// <param name="hexColor"><inheritdoc cref="Write(string, string)" path="/param[@name='hexColor']"/></param>
		public static void WriteLine(string value, string hexColor)
		{
			Write(value, hexColor);
			System.Console.WriteLine();
		}

		/// <summary>
		/// <inheritdoc cref="WriteLine(string, Structure.RGB)" path="member/summary"/>
		/// </summary>
		/// <param name="value"><inheritdoc cref="Write(string, Structure.RGB)" path="/param[@name='value']"/></param>
		/// <param name="r"><inheritdoc cref="Write(string, byte, byte, byte)" path="/param[@name='r']"/></param>
		/// <param name="g"><inheritdoc cref="Write(string, byte, byte, byte)" path="/param[@name='g']"/></param>
		/// <param name="b"><inheritdoc cref="Write(string, byte, byte, byte)" path="/param[@name='b']"/> </param>
		public static void WriteLine(string value, byte r, byte g, byte b)
		{
			Write(value, r, g, b);
			System.Console.WriteLine();
		}
		/// <summary>
		/// 设置控制台前景色
		/// </summary>
		/// <param name="r"><inheritdoc cref="Write(string, byte, byte, byte)" path="/param[@name='r']"/></param>
		/// <param name="g"><inheritdoc cref="Write(string, byte, byte, byte)" path="/param[@name='g']"/></param>
		/// <param name="b"><inheritdoc cref="Write(string, byte, byte, byte)" path="/param[@name='b']"/> </param>
		public static void Foreground(byte r, byte g, byte b)
		{
			string build = string.Format("\x1b[38;2;{0};{1};{2}m", r, g, b);
			System.Console.Write(build);
		}

		/// <summary>
		/// 设置控制台前景色
		/// </summary>
		/// <param name="rgb"> <inheritdoc cref="Write(string, Structure.RGB)" path="/param[@name='rgb']"/></param>
		public static void Foreground(Structure.RGB rgb)
		{
			Foreground(rgb.R, rgb.G, rgb.B);
		}

		/// <summary>
		/// 重置前景色
		/// </summary>
		public static void ResetColor()
		{
			System.Console.Write("\x1b[0m");
		}

		/// <summary>
		/// 以密码格式读取控制台输入的字符,按下回车结束输入
		/// </summary>
		/// <param name="prompt">提示文本</param>
		/// <param name="maskChar">掩码字符,输出后代替明文密码的<see langword="char"/>字符</param>
		/// <param name="maskColor">掩码字符的字体颜色</param>
		/// <returns>键入的 <see langword="password"/> 字符串</returns>
		public static string InputPassword(string prompt, char maskChar = '*', ConsoleColor maskColor = ConsoleColor.White)
		{
			ConsoleColor defaultForegroundColor = System.Console.ForegroundColor;
			System.Console.Write(prompt);
			System.Console.ForegroundColor = maskColor;
			string password = "";
			while (true)
			{
				ConsoleKeyInfo key = System.Console.ReadKey(true);
				if (key.Key == ConsoleKey.Enter)
				{
					System.Console.WriteLine();
					break;
				}
				if (key.Key == ConsoleKey.Backspace)
				{
					if (password.Length > 0)
					{
						password = password.Remove(password.Length - 1);
						System.Console.Write("\b \b");
					}
				}
				else
				{
					password += key.KeyChar;
					System.Console.Write(maskChar);
				}
			}
			System.Console.ForegroundColor = defaultForegroundColor;
			return password;
		}


	}
}
