using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Ike.Standard
{
	/// <summary>
	/// 数据校验
	/// </summary>
	public static class Validator
	{
		/// <summary>
		/// 校验PLC寄存器地址是否合法。该方法支持的寄存器类型包括：X、Y、M、D, 可通过可选参数设定各寄存器的合法范围
		/// </summary>
		/// <param name="address">寄存器地址，必须以 'X', 'Y', 'M', 或 'D' 开头，后跟数字，例如 'X100' 或 'D500'。</param>
		/// <param name="x">X寄存器的最大值（默认255）。X寄存器范围为0到x。</param>
		/// <param name="y">Y寄存器的最大值（默认255）。Y寄存器范围为0到y。</param>
		/// <param name="m">M寄存器的最大值（默认500）。M寄存器范围为0到m。</param>
		/// <param name="d">D寄存器的最大值（默认100）。D寄存器范围为0到d。</param>
		/// <returns>如果地址合法，返回<see langword="true"/>；否则返回<see langword="false"/>。</returns>
		public static bool IsValidPLCAddress(string address,int x = 255,int y = 255,int m = 500,int d = 100)
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


	}
}
