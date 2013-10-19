using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
namespace RemoteControlService
{
	/// <summary>
	/// 发送消息类型
	/// </summary>
	public enum SendType
	{
		/// <summary>
		/// 回执，字符01
		/// </summary>
		Callback,//01

		/// <summary>
		/// 文本消息,字符02
		/// </summary>
		Text,//02

		/// <summary>
		/// 登录消息,字符03
		/// </summary>
		Login,//03

		/// <summary>
		/// 运行错误消息,字符04
		/// </summary>
		RunError,//04

		/// <summary>
		/// 文件消息,字符05
		/// </summary>
		File,//05

		/// <summary>
		/// 截图消息,字符06
		/// </summary>
		Picture,//06

		/// <summary>
		/// 终端消息,字符07
		/// </summary>
		Terminal,//07

		/// <summary>
		/// 终端错误消息，字符08
		/// </summary>
		TerminalError,//08

		/// <summary>
		/// 任务消息，字符09
		/// </summary>
		Task,//09
	}

	/// <summary>
	/// 发送的消息结构体
	/// </summary>
	public struct SendMessageStruct
	{
		/// <summary>
		/// 消息ID
		/// </summary>
		public string Id;

		/// <summary>
		/// 消息类型
		/// </summary>
		public SendType type;

		/// <summary>
		/// 消息内容
		/// </summary>
		public byte[] content;

		/// <summary>
		/// 消息目的地
		/// </summary>
		public IPEndPoint e;
	}

	/// <summary>
	/// 发送消息对象
	/// </summary>
	public class SendMessage
	{
		/// <summary>
		/// 消息ID
		/// </summary>
		private static long id = 0;

		/// <summary>
		/// 消息实例
		/// </summary>
		public SendMessageStruct smsg;

		/// <summary>
		/// 要发送的字节数据
		/// </summary>
		private byte[] send = null;

		/// <summary>
		/// 消息时间
		/// </summary>
		public DateTime time;

		/// <summary>
		/// 消息重试次数
		/// </summary>
		public int retransmission = 0;

		/// <summary>
		/// 构造新的消息
		/// </summary>
		/// <param name="e">消息目的地</param>
		public SendMessage(IPEndPoint e)
		{
			if (SendMessage.id > 9999999) SendMessage.id = 0;
			smsg.Id = DateTime.Now + "\t" + (++SendMessage.id);
			smsg.e = e;
		}

		/// <summary>
		/// 设置为文本消息
		/// </summary>
		/// <param name="str"></param>
		public void setText(string str)
		{
			smsg.type = SendType.Text;
			smsg.content = SendMessage.ToBytes(str);
		}

		/// <summary>
		/// 设置为回执消息
		/// </summary>
		/// <param name="str"></param>
		public void setCallback(string str)
		{
			smsg.type = SendType.Callback;
			smsg.content = SendMessage.ToBytes(str);
		}

		/// <summary>
		/// 设置为登录消息
		/// </summary>
		/// <param name="str"></param>
		public void setLogin(string str)
		{
			smsg.type = SendType.Login;
			smsg.content = SendMessage.ToBytes(str);
		}

		/// <summary>
		/// 设置为运行错误消息
		/// </summary>
		/// <param name="str"></param>
		public void setRunError(string str)
		{
			smsg.type = SendType.RunError;
			smsg.content = SendMessage.ToBytes(str);
		}

		/// <summary>
		/// 设置为终端消息
		/// </summary>
		/// <param name="bs"></param>
		public void setTerminal(byte[] bs)
		{
			smsg.type = SendType.Terminal;
			smsg.content = bs;
		}

		/// <summary>
		/// 设置为任务消息
		/// </summary>
		/// <param name="bs"></param>
		public void setTask(byte[] bs)
		{
			smsg.type = SendType.Task;
			smsg.content = bs;
		}

		/// <summary>
		/// 设置为终端错误消息
		/// </summary>
		/// <param name="bs"></param>
		public void setTerminalError(byte[] bs)
		{
			smsg.type = SendType.TerminalError;
			smsg.content = bs;
		}

		/// <summary>
		/// 创建文本消息
		/// </summary>
		/// <param name="text"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static SendMessage creatText(string text, IPEndPoint e)
		{
			SendMessage smsg = new SendMessage(e);
			smsg.setText(text);
			return smsg;
		}

		/// <summary>
		/// 创建回执消息
		/// </summary>
		/// <param name="id"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static SendMessage creatCallback(string id, IPEndPoint e)
		{
			SendMessage smsg = new SendMessage(e);
			smsg.setCallback(id);
			return smsg;
		}
		
		/// <summary>
		/// 创建运行错误消息
		/// </summary>
		/// <param name="id"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static SendMessage creatRunError(string id, IPEndPoint e)
		{
			SendMessage smsg = new SendMessage(e);
			smsg.setRunError(id);
			return smsg;
		}

		/// <summary>
		/// 创建登录消息
		/// </summary>
		/// <param name="id"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static SendMessage creatLogin(string id, IPEndPoint e)
		{
			SendMessage smsg = new SendMessage(e);
			smsg.setLogin(id);
			return smsg;
		}

		/// <summary>
		/// 创建终端消息
		/// </summary>
		/// <param name="s"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static SendMessage creatTerminal(string s, IPEndPoint e)
		{
			SendMessage smsg = new SendMessage(e);
			smsg.setTerminal(SendMessage.ToBytes(s));
			return smsg;
		}

		/// <summary>
		/// 创建任务消息
		/// </summary>
		/// <param name="s"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static SendMessage creatTask(byte[] s, IPEndPoint e)
		{
			SendMessage smsg = new SendMessage(e);
			smsg.setTask(s);
			return smsg;
		}

		/// <summary>
		/// 创建终端错误消息
		/// </summary>
		/// <param name="s"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static SendMessage creatTerminalError(string s, IPEndPoint e)
		{
			SendMessage smsg = new SendMessage(e);
			smsg.setTerminalError(SendMessage.ToBytes(s));
			return smsg;
		}

		/// <summary>
		/// 设置为文件消息
		/// </summary>
		/// <param name="bytes"></param>
		public void setFile(byte[] bytes)
		{
			smsg.type = SendType.File;
			smsg.content = bytes;
		}

		/// <summary>
		/// 设置为截图消息
		/// </summary>
		/// <param name="bytes"></param>
		public void setPicture(byte[] bytes)
		{
			smsg.type = SendType.Picture;
			smsg.content = bytes;
		}

		/// <summary>
		/// 创建文件消息
		/// </summary>
		/// <param name="bytes"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static SendMessage creatFile(byte[] bytes, IPEndPoint e)
		{
			SendMessage smsg = new SendMessage(e);
			smsg.setFile(bytes);
			return smsg;
		}

		/// <summary>
		/// 创建截图消息
		/// </summary>
		/// <param name="full">完整路径</param>
		/// <param name="full_size">完整大小</param>
		/// <param name="get">缩略图路径</param>
		/// <param name="get_size">缩略图大小</param>
		/// <param name="e">目的地</param>
		/// <returns></returns>
		public static SendMessage creatPicture(string full,int full_size,string get,int get_size, IPEndPoint e)
		{
			SendMessage smsg = new SendMessage(e);
			smsg.setPicture(SendMessage.ToBytes(full+"\t"+full_size+"\n"+get+"\t"+get_size));
			return smsg;
		}

		/// <summary>
		/// 将消息转换为字节并存在send中
		/// </summary>
		/// <returns></returns>
		public byte[] getBytes()
		{
			if (send == null)
			{
				send = ConBytes(SendMessage.ToBytes(smsg.Id + "\t" + getTypeString(smsg.type) + "\n"), smsg.content);
			}
			return send;
		}

		/// <summary>
		/// 连接字节数组
		/// </summary>
		/// <param name="a">前一段</param>
		/// <param name="b">后一段</param>
		/// <returns>合并后的字节数组</returns>
		private static byte[] ConBytes(byte[] a, byte[] b)
		{
			if (a.Length == 0 || b.Length == 0) return null;
			byte[] rt = new byte[a.Length + b.Length];
			a.CopyTo(rt, 0);
			b.CopyTo(rt, a.Length);
			return rt;
		}

		/// <summary>
		/// 从0开始取字节数组的子数组
		/// </summary>
		/// <param name="a">元素数组</param>
		/// <param name="length">新数组长度</param>
		/// <returns></returns>
		private static byte[] SubBytes(byte[] a, int length)
		{
			if (a == null || length > a.Length)
			{
				throw new ArgumentOutOfRangeException("参数不合法");
			}
			byte[] rt = new byte[length];
			for (int i = 0; i < length; i++)
			{
				rt[i] = a[i];
			}
			return rt;
		}

		/// <summary>
		/// 将消息类型转换为字符
		/// </summary>
		/// <param name="type">消息类型</param>
		/// <returns></returns>
		private string getTypeString(SendType type)
		{
			switch (type)
			{
				case SendType.Callback:
					return "01";
				case SendType.Text:
					return "02";
				case SendType.Login:
					return "03";
				case SendType.RunError:
					return "04";
				case SendType.File:
					return "05";
				case SendType.Picture:
					return "06";
				case SendType.Terminal:
					return "07";
				case SendType.TerminalError:
					return "08";
				case SendType.Task:
					return "09";
			}
			return "";
		}

		/// <summary>
		/// 将字符串转换为字节数组
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static byte[] ToBytes(string str){
			return Encoding.UTF8.GetBytes(str);
		}
	}
}
