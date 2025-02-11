using System.Runtime.InteropServices;

namespace Ike.Standard
{
	/// <summary>
	/// 通用数据结构体
	/// </summary>
	public class Structure
	{
		/// <summary>
		/// RGB结构
		/// </summary>
		public struct RGB
		{
			/// <summary>
			///Red
			/// </summary>
			public byte R;
			/// <summary>
			/// Green
			/// </summary>
			public byte G;
			/// <summary>
			/// Blue
			/// </summary>
			public byte B;

			/// <summary>
			/// 构造数据
			/// </summary>
			/// <param name="r">Red</param>
			/// <param name="g">Green</param>
			/// <param name="b">Blue</param>
			public RGB(byte r, byte g, byte b)
			{
				R = r;
				G = g;
				B = b;
			}

			/// <summary>
			/// 转十六进制格式
			/// </summary>
			/// <returns></returns>
			public string ToHex()
			{
				return Convert.RgbToHex(R,G,B);
			}
		}


		/// <summary>
		/// 矩形坐标结构
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			/// <summary>
			/// 左坐标
			/// </summary>
			public int Left;
			/// <summary>
			/// 上坐标
			/// </summary>
			public int Top;
			/// <summary>
			/// 右坐标
			/// </summary>
			public int Right;
			/// <summary>
			/// 下坐标
			/// </summary>
			public int Bottom;
		}


		/// <summary>
		/// 表示一个日期和时间结构，包含年、月、日、星期、小时、分钟、秒和毫秒的独立字段
		/// </summary>
		/// <remarks>
		/// 该结构体通常用于与需要 `SYSTEMTIME` 结构的本地代码（如 Win32 API 调用）进行互操作
		/// </remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct SystemTime
		{
			/// <summary>
			/// 年份,例如：2024
			/// </summary>
			public ushort Year;
			/// <summary>
			/// 月份,1 表示一月，2 表示二月，以此类推
			/// </summary>
			public ushort Month;
			/// <summary>
			/// 星期几,0 表示星期日，1 表示星期一，依此类推
			/// </summary>
			public ushort DayOfWeek;
			/// <summary>
			/// 月中的某一天,根据具体月份的天数，取值范围为 1 到 31
			/// </summary>
			public ushort Day;
			/// <summary>
			/// 小时,取值范围为 0 到 23
			/// </summary>
			public ushort Hour;
			/// <summary>
			/// 分钟,取值范围为 0 到 59
			/// </summary>
			public ushort Minute;
			/// <summary>
			/// 秒,取值范围为 0 到 59
			/// </summary>
			public ushort Second;
			/// <summary>
			/// 毫秒,取值范围为 0 到 999
			/// </summary>
			public ushort Miliseconds;
		}

	}
}
