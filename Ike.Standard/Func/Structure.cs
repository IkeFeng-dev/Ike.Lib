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
	}
}
