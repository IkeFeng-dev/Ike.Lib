using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ike.Standard
{
	/// <summary>
	/// 数据校验
	/// </summary>
	public static class Validator
	{

        /// <summary>
        /// 判断文件名是否合法
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="checkReservedKeyword">是否检查Windows保留关键字</param>
        /// <returns></returns>
        /// <remarks>
        /// [<see langword="2026-8-27 13:30:17" />]
        /// </remarks>
        public static bool IsValidFileName(string fileName,bool checkReservedKeyword = false)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            // 检查是否包含非法字符
            char[] invalidChars = Path.GetInvalidFileNameChars();
            if (fileName.IndexOfAny(invalidChars) >= 0)
                return false;

			// 检查保留设备名（Windows）
			if (checkReservedKeyword)
			{
				string[] reservedNames = { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };

				string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName).ToUpper();
				if (reservedNames.Contains(nameWithoutExt))
					return false;
			}
            // 检查是否以点或空格结尾（Windows 不允许）
            if (fileName.EndsWith(".") || fileName.EndsWith(" "))
                return false;

            return true;
        }



        /// <summary>
        /// 判断文件名中字符是否合法
        /// </summary>
        /// <param name="FileName">文件名称</param>
        public static bool IsValidFileChars(string FileName)
		{
			char[] invalidFileChars = Path.GetInvalidFileNameChars();
			char[] NameChar = FileName.ToCharArray();
			foreach (char C in NameChar)
			{
				if (System.Array.BinarySearch(invalidFileChars, C) >= 0)
				{
					return false;
				}
			}
			return true;
		}


		/// <summary>
		///  判断是否是不带 BOM 的 UTF8 格式
		/// </summary>
		/// <param name="data">数据</param>
		/// <returns></returns>
		public static bool IsUTF8Bytes(byte[] data)
		{
			int charByteCounter = 1;
			//计算当前正分析的字符应还有的字节数 
			byte curByte; //当前分析的字节. 
			for (int i = 0; i < data.Length; i++)
			{
				curByte = data[i];
				if (charByteCounter == 1)
				{
					if (curByte >= 0x80)
					{
						//判断当前 
						while (((curByte <<= 1) & 0x80) != 0)
						{
							charByteCounter++;
						}
						//标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　 
						if (charByteCounter == 1 || charByteCounter > 6)
						{
							return false;
						}
					}
				}
				else
				{
					//若是UTF-8 此时第一位必须为1 
					if ((curByte & 0xC0) != 0x80)
					{
						return false;
					}
					charByteCounter--;
				}
			}
			if (charByteCounter > 1)
			{
				throw new Exception("非预期的byte格式");
			}
			return true;
		}



		/// <summary>
		/// 校验PLC寄存器地址是否合法。该方法支持的寄存器类型包括：X、Y、M、D, 可通过可选参数设定各寄存器的合法范围
		/// </summary>
		/// <param name="address">寄存器地址，必须以 'X', 'Y', 'M', 或 'D' 开头，后跟数字，例如 'X100' 或 'D500'。</param>
		/// <param name="x">X寄存器的最大值（默认255）。X寄存器范围为0到x。</param>
		/// <param name="y">Y寄存器的最大值（默认255）。Y寄存器范围为0到y。</param>
		/// <param name="m">M寄存器的最大值（默认500）。M寄存器范围为0到m。</param>
		/// <param name="d">D寄存器的最大值（默认100）。D寄存器范围为0到d。</param>
		/// <returns>如果地址合法，返回<see langword="true"/>；否则返回<see langword="false"/>。</returns>
		public static bool IsValidPLCAddress(string address, int x = 255, int y = 255, int m = 500, int d = 100)
		{
			string pattern = @"^[DMXY][0-9]+$";
			if (Regex.IsMatch(address, pattern))
			{
				int numericPart = int.Parse(address.Substring(1));
				char type = address[0];
				switch (type)
				{
					case 'D':
						return numericPart >= 0 && numericPart <= d;
					case 'M':
						return numericPart >= 0 && numericPart <= m;
					case 'X':
						return numericPart >= 0 && numericPart <= x;
					case 'Y':
						return numericPart >= 0 && numericPart <= y;
					default:
						return false;
				}
			}
			else
			{
				return false;
			}
		}




		/// <summary>
		/// 是否IP地址
		/// </summary>
		/// <param name="ipAddress">IP</param>
		/// <returns></returns>
		public static bool IsPAddress(string ipAddress)
		{
			Regex validipregex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
			return (ipAddress != "" && validipregex.IsMatch(ipAddress.Trim()));
		}


		/// <summary>
		/// 是否为链接
		/// </summary>
		/// <param name="link">链接[http:// ,https://,ftp://开头的]</param>
		/// <returns></returns>
		public static bool IsLink(string link)
		{
			return Regex.IsMatch(link, @"((http|ftp|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-]*)?");
		}


		/// <summary>
		/// 字符串是否包含数字
		/// </summary>
		/// <param name="text">内容</param>
		/// <returns></returns>
		public static bool ContainNumber(string text)
		{
			return Regex.IsMatch(text, @"^(?=.*[0-9].*)");
		}


		/// <summary>
		/// 字符串是否包含小写字母
		/// </summary>
		/// <param name="text">内容</param>
		/// <returns></returns>
		public static bool ContainLowerLetter(string text)
		{
			return Regex.IsMatch(text, @"^(?=.*[a-z].*)");
		}


		/// <summary>
		/// 字符串是否包含大写字母
		/// </summary>
		/// <param name="text">内容</param>
		/// <returns></returns>
		public static bool ContainUpperLetter(string text)
		{
			return Regex.IsMatch(text, @"^(?=.*[A-Z].*)");
		}


		/// <summary>
		/// 字符串是否包含大小写字母
		/// </summary>
		/// <param name="text">内容</param>
		/// <returns></returns>
		public static bool ContainUpperAndLowerLetter(string text)
		{
			return Regex.IsMatch(text, @"^(?=.*[a-z].*)(?=.*[A-Z])");
		}


		/// <summary>
		/// 判断字符串是否是数字
		/// </summary>
		/// <param name="value">是否数字,包含小数点</param>
		/// <returns></returns>
		public static bool IsNumeric(string value)
		{
			return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
		}


		/// <summary>
		/// 是否为日期型字符串
		/// </summary>
		/// <param name="text">日期字符串(2008-05-08)</param>
		/// <returns></returns>
		public static bool StringIsDate(string text)
		{
			return Regex.IsMatch(text, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
		}


		/// <summary>
		/// 是否为时间型字符串
		/// </summary>
		/// <param name="text">时间字符串(15:00:00)</param>
		/// <returns></returns>
		public static bool StringIsTime(string text)
		{
			return Regex.IsMatch(text, @"^((20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$");
		}


		/// <summary>
		/// 是否为日期+时间型字符串
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static bool StringIsDateTime(string text)
		{
			return Regex.IsMatch(text, @"^(((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$ ");
		}


		/// <summary>
		/// 字符串是否为带小数点的数字
		/// </summary>
		/// <param name="text">字符串</param>
		/// <returns></returns>
		public static bool StringIsDouble(string text)
		{
			return Regex.IsMatch(text, @"^([0-9]{1,}[.][0-9]*)$");
		}


		/// <summary>
		/// 字符串是否全为中文字符
		/// </summary>
		/// <param name="text">字符串</param>
		/// <returns></returns>
		public static bool StringIsChineseCharacters(string text)
		{
			return Regex.IsMatch(text, @"[\u4e00-\u9fa5]+$");
		}


		/// <summary>
		/// 字符串是否全为数字
		/// </summary>
		/// <param name="text">字符串</param>
		/// <returns></returns>
		public static bool StringIsNumber(string text)
		{
			return Regex.IsMatch(text, @"^(-?[0-9]*[.]*[0-9]{0,3})$");
		}


		/// <summary>
		/// 字符串是否包含中文字符
		/// </summary>
		/// <param name="text">字符串</param>
		/// <returns></returns>
		public static bool StringContainChineseCharacters(string text)
		{
			return Regex.IsMatch(text, @"[\u4e00-\u9fa5]");
		}


		/// <summary>
		/// 字符串是否为邮箱
		/// </summary>
		/// <param name="text">字符串</param>
		/// <returns></returns>
		public static bool StringIsMail(string text)
		{
			return Regex.IsMatch(text, @"^(\w)+(\.\w+)*@(\w)+((\.\w{2,3}){1,3})$");
		}

	}
}
