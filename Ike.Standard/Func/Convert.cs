using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace Ike.Standard
{
	/// <summary>
	/// 数据转换类
	/// </summary>
	public static class Convert
	{
		/// <summary>
		/// 十六进制颜色转RGB
		/// </summary>
		/// <param name="hexColor">颜色字符串,如"#ec4c8c" 或 "ec4c8c"格式</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">值无效</exception>
		public static Structure.RGB HexToRGB(string hexColor)
		{
			hexColor = hexColor.Replace("#", "");

			if (hexColor.Length != 6)
			{
				throw new ArgumentException("无效的颜色代码");
			}
			byte red = System.Convert.ToByte(hexColor.Substring(0, 2), 16);
			byte green = System.Convert.ToByte(hexColor.Substring(2, 2), 16);
			byte blue = System.Convert.ToByte(hexColor.Substring(4, 2), 16);
			return new Structure.RGB() { R = red, G = green, B = blue };
		}

		/// <summary>
		/// RGB转十六进制格式颜色
		/// </summary>
		/// <param name="r">Red</param>
		/// <param name="g">Green</param>
		/// <param name="b">Bule</param>
		/// <returns></returns>
		public static string RgbToHex(byte r,byte g,byte b)
		{
			return string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
		}

		/// <summary>
		/// RGB转十六进制格式颜色
		/// </summary>
		/// <param name="rgb">RGB颜色</param>
		/// <returns></returns>
		public static string RgbToHex(Structure.RGB rgb)
		{
			return RgbToHex(rgb.R, rgb.G, rgb.B);
		}

		/// <inheritdoc cref="RgbToHex(Structure.RGB)"/>
		public static string ToHex(this Structure.RGB rgb)
		{
			return RgbToHex(rgb);
		}


		/// <summary>
		/// 文件转<see  langword="byte[]"/>
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns></returns>
		public static byte[] FileToBytes(string filePath)
		{
			using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				byte[] bytes = new byte[fileStream.Length];
				fileStream.Read(bytes, 0, bytes.Length);
				return bytes;
			}
		}


		/// <summary>
		/// 文件转<see cref="Stream"/>
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns></returns>
		public static Stream FileToStream(string filePath)
		{
			using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				 MemoryStream memoryStream = new MemoryStream();
				fileStream.CopyTo(memoryStream);
				memoryStream.Position = 0;
				return memoryStream;
			}
		}

		/// <summary>
		/// <see cref="Color"/>转十六进制格式
		/// </summary>
		/// <param name="color">颜色</param>
		/// <returns>如果<see cref="Color"/>未初始化,则返回[#FFFFFF]</returns>
		public static string ColorToHex(Color color)
		{
			return RgbToHex(color.R, color.G, color.B);
		}

		/// <inheritdoc cref="ColorToHex(Color)"/>
		public static string ToHex(this Color color)
		{
			return ColorToHex(color);
		}

		/// <summary>
		/// 字符串转<see  cref="byte"/>
		/// </summary>
		/// <param name="str">字符串</param>
		/// <param name="encoding">编码</param>
		/// <returns></returns>
		public static byte[] StringToBytes(string str, Encoding encoding)
		{
			return encoding.GetBytes(str);
		}

		/// <inheritdoc cref="StringToBytes(string,Encoding)"/>
		public static byte[] ToBytes(this string str, Encoding encoding)
		{
			return StringToBytes(str, encoding);
		}



		/// <summary>
		/// 枚举数值转换为枚举值
		/// </summary>
		/// <typeparam name="T">泛型限定为<see cref="Enum"/>类型</typeparam>
		/// <param name="value">值</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">参数异常</exception>
		public static T ValueToEnum<T>(int value) where T : struct, Enum
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("泛型类型必须是枚举类型");
			}
			if (!Enum.IsDefined(typeof(T), value))
			{
				throw new ArgumentException($"值 '{value}'未在 {typeof(T).Name} 中定义");
			}
			return (T)Enum.ToObject(typeof(T), value);
		}

		/// <inheritdoc cref="ValueToEnum"/>
		public static T ToEnum<T>(this int value) where T : struct, Enum
		{
			return ValueToEnum<T>(value);
		}
		/// <summary>
		/// 根据枚举名称获取枚举值
		/// </summary>
		/// <typeparam name="T">泛型限定为<see cref="Enum"/>类型</typeparam>
		/// <param name="value">枚举的名称</param>
		/// <returns>对应的枚举值</returns>
		/// <exception cref="ArgumentException">如果输入的字符串不是枚举中的有效名称，则抛出异常</exception>
		public static T NameToEnum<T>(string value) where T : struct, Enum
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("泛型类型必须是枚举类型");
			}
			if (!Enum.IsDefined(typeof(T), value))
			{
				throw new ArgumentException($"值 '{value}'未在 {typeof(T).Name} 中定义");
			}
			return (T)Enum.Parse(typeof(T), value);
		}


		/// <inheritdoc cref="NameToEnum"/>
		public static T ToEnum<T>(this string value) where T : struct, Enum
		{
			return NameToEnum<T>(value);
		}


	}
}
