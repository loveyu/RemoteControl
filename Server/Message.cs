using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace RemoteControlService
{
	/// <summary>
	/// 消息类型
	/// </summary>
	public enum MessageType
	{
		/// <summary>
		/// 命令消息,对应字符01
		/// </summary>
		COMMAND,//01

		/// <summary>
		/// 提示消息，对应字符02
		/// </summary>
		NOTICE,//02

		/// <summary>
		/// 文件消息,对应字符03
		/// </summary>
		FILE,//03

		/// <summary>
		/// 警告消息，对应类型04
		/// </summary>
		WARNING,//04

		/// <summary>
		/// 终端消息，对应字符05
		/// </summary>
		TERMINAL,//05

		/// <summary>
		/// 函数功能消息，对应字符06
		/// </summary>
		FUNCTION,//06

		/// <summary>
		/// 回执消息，对应字符07
		/// </summary>
		CALLBACK,//07

		/// <summary>
		/// 登录消息,对应字符08
		/// </summary>
		LOGIN,//08

		/// <summary>
		/// 登出消息，对应字符09
		/// </summary>
		LOGOUT,//09

		/// <summary>
		/// 截图消息，对应字符10
		/// </summary>
		SCREENSHOT,//10

		/// <summary>
		/// 任务管理消息，对应字符11
		/// </summary>
		TASK,//11
	}

	/// <summary>
	/// 消息结构体
	/// </summary>
	public struct MessageStruct
	{
		/// <summary>
		/// 消息ID，长度10
		/// </summary>
		public string Id;//10L
		
		/// <summary>
		/// 消息类型，长度2
		/// </summary>
		public MessageType Type;//2L
		
		/// <summary>
		/// 消息内容
		/// </summary>
		public string Content;

		/// <summary>
		/// 消息发送者
		/// </summary>
		public IPEndPoint e;
	}

	/// <summary>
	/// 消息构造类
	/// </summary>
	public class Message
	{
		/// <summary>
		/// 原始消息数组
		/// </summary>
		private byte[] NavtiveMessage;
		
		/// <summary>
		/// 消息结构体数组
		/// </summary>
		public MessageStruct msg;

		/// <summary>
		/// 消息检测结果
		/// </summary>
		public bool isOK = false;

		/// <summary>
		/// 构造消息内容
		/// </summary>
		/// <param name="bytes">原始数据</param>
		/// <param name="e">消息发送者</param>
		public Message(byte[] bytes, IPEndPoint e)
		{
			NavtiveMessage = bytes;
			msg.e = e;
			parse();
		}

		/// <summary>
		/// 开始解析消息
		/// </summary>
		private void parse()
		{
			if (NavtiveMessage.Length < 13)
			{
				Log.m("Discard invalid data [length]:" + NavtiveMessage.Length);
				return;
			}
			Log.d("Received:" + Encoding.UTF8.GetString(NavtiveMessage));
			msg.Id = subString(0, 10);
			string type = subString(10, 2);
			switch (type)
			{
				case "01":
					msg.Type = MessageType.COMMAND;
					break;
				case "02":
					msg.Type = MessageType.NOTICE;
					break;
				case "03":
					msg.Type = MessageType.FILE;
					break;
				case "04":
					msg.Type = MessageType.WARNING;
					break;
				case "05":
					msg.Type = MessageType.TERMINAL;
					break;
				case "06":
					msg.Type = MessageType.FUNCTION;
					break;
				case "07":
					msg.Type = MessageType.CALLBACK;
					break;
				case "08":
					msg.Type = MessageType.LOGIN;
					break;
				case "09":
					msg.Type = MessageType.LOGOUT;
					break;
				case "10":
					msg.Type = MessageType.SCREENSHOT;
					break;
				case "11":
					msg.Type = MessageType.TASK;
					break;
				default:
					Log.m("Discard invalid data [type]:" + type);
					return;
			}
			msg.Content = subString(12, -1);
			isOK = true;
		}

		/// <summary>
		/// 重原始消失中分离数据
		/// </summary>
		/// <param name="index">起始位置</param>
		/// <param name="length">长度</param>
		/// <returns>字符串</returns>
		private string subString(int index, int length)
		{
			if (length == -1) length = NavtiveMessage.Length - index;
			byte[] n = new byte[length];
			int j = index;
			for (int i = 0; i < length; i++)
			{
				n[i] = NavtiveMessage[j++];
			}
			return Encoding.UTF8.GetString(n);
		}

	}
}
