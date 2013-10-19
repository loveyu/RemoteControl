using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RemoteControlService
{
	/// <summary>
	/// 日志记录类
	/// </summary>
	public class Log
	{
		/// <summary>
		/// 调试信息，仅在调试模式时输出
		/// </summary>
		/// <param name="log">字符串</param>
		public static void d(string log)
		{
#if DEBUG
			write("[Debug]: " + log);
#endif
		}

		/// <summary>
		/// 主要的运行记录
		/// </summary>
		/// <param name="log">字符串</param>
		public static void a(string log)
		{
			write("[running]: " + log);
		}

		/// <summary>
		/// 网络记录
		/// </summary>
		/// <param name="log"></param>
		public static void n(string log)
		{
			write("[network]: " + log);
		}

		/// <summary>
		/// 处理记录
		/// </summary>
		/// <param name="log"></param>
		public static void p(string log)
		{
			write("[process]: " + log);
		}

		/// <summary>
		/// 消息记录
		/// </summary>
		/// <param name="log"></param>
		public static void m(string log)
		{
			write("[message]: " + log);
		}

		/// <summary>
		/// 文件记录
		/// </summary>
		/// <param name="log"></param>
		public static void f(string log)
		{
			write("[file]: " + log);
		}

		/// <summary>
		/// 将记录写入到输出目标
		/// </summary>
		/// <param name="log"></param>
		private static void write(string log)
		{
#if DEBUG
			Console.WriteLine("[" + notice() + "]" + log);
#else
			FileStream fs = new FileStream(Configure.LogPath(),FileMode.Append);
			byte[] rb = SendMessage.ToBytes("["+notice()+"]"+log+"\r\n");
			fs.Write(rb,0,rb.Length);
			fs.Close();
#endif
		}

		/// <summary>
		/// 返回时间提示
		/// </summary>
		/// <returns>格式化后的时间字符串</returns>
		private static string notice()
		{
			return DateTime.Now.ToString();
		}
	}
}
