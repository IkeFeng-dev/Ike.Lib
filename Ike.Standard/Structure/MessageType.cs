namespace Ike.Standard
{
	public partial class Sockets
	{
		/// <summary>
		/// 侦听消息类型
		/// </summary>
		public enum MessageType
		{
			/// <summary>
			/// 接收
			/// </summary>
			Receive,
			/// <summary>
			/// 发送
			/// </summary>
			Send,
			/// <summary>
			/// 异常
			/// </summary>
			Error,
			/// <summary>
			/// 信息
			/// </summary>
			Info
		}
	}
}
