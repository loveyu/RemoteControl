using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

namespace RemoteControlService
{
	/// <summary>
	/// 网络处理进程
	/// </summary>
	public class Process
	{
		/// <summary>
		/// 获取到到消息信息
		/// </summary>
		Message msg;

		/// <summary>
		/// 构造新的对象，并在构造函数中执行完全部操作
		/// </summary>
		/// <param name="msg">指定的消息信息</param>
		public Process(Message msg)
		{
			Log.d("Run Process of {" + msg.msg.e + "}");
			this.msg = msg;
			try
			{
				Run();
			}
			catch (Exception ex)
			{
				Log.p("Exception: " + ex.Message);
			}
		}

		/// <summary>
		/// 发送回执给服务器以移除发送队列
		/// </summary>
		private void SendCallback()
		{
			Net.Get().Send(SendMessage.creatCallback(msg.msg.Id, msg.msg.e));
		}

		/// <summary>
		/// 开始执行指定命令
		/// </summary>
		private void Run()
		{
			//Log.d("Command:" + msg.msg.Type.ToString());
			if (msg.msg.Type == MessageType.CALLBACK)
			{
				//进行消息回复
				ProcessCallback(msg.msg.Content);
				return;
			}
			else
			{
				SendCallback();
				if (User.IsLogin(msg.msg.e) == false)
				{
					//未登陆用户
					if (msg.msg.Type != MessageType.LOGIN)
					{
						Net.Get().Send(SendMessage.creatLogin("203 Please Login", msg.msg.e));
						return;
					}
					Net.Get().Send(SendMessage.creatLogin(User.Login(msg.msg.e, msg.msg.Content), msg.msg.e));
					return;
				}
			}
			switch (msg.msg.Type)
			{
				case MessageType.NOTICE:
					ShowNotice(msg.msg.Content);
					break;
				case MessageType.FUNCTION:
					RunFunc(msg.msg.Content);
					break;
				case MessageType.WARNING:
					ShowWarning(msg.msg.Content);
					break;
				case MessageType.COMMAND:
					RunCommand(msg.msg.Content);
					break;
				case MessageType.FILE:
					RunFileSystem(msg.msg.Content);
					break;
				case MessageType.SCREENSHOT:
					GetScreenshot(msg.msg.Content);
					break;
				case MessageType.TERMINAL:
					RunTreminal(msg.msg.Content);
					break;
				case MessageType.LOGOUT:
					User.Remove(msg.msg.e);
					break;
				case MessageType.LOGIN:
					SendLoggedInformation();
					break;
				case MessageType.TASK:
					SetTask();
					break;
				default:
					SendUnknownType();
					break;
			}
		}

		/// <summary>
		/// 发送已经登录的提示消息
		/// </summary>
		private void SendLoggedInformation()
		{
			Net.Get().Send(SendMessage.creatLogin(User.LoggedMsg(), msg.msg.e));
		}

		/// <summary>
		/// 发送当前消息类型未知提示消息
		/// </summary>
		private void SendUnknownType()
		{
			Net.Get().Send(SendMessage.creatText("Unknown Message Type", msg.msg.e));
		}

		/// <summary>
		/// 处理服务器发送过来的回执
		/// </summary>
		/// <param name="id">队列的ID号</param>
		private void ProcessCallback(string id)
		{
			Log.d("Callback:" + id.Split('\t')[1]);
			MessageQueue.Get().Remove(id);
		}

		/// <summary>
		/// 执行指定的功能函数，并且发送返回值作为提示信息
		/// </summary>
		/// <param name="str">功能方法名</param>
		private void RunFunc(string str)
		{
			Function func = new Function();
			MethodInfo[] mis = func.GetType().GetMethods();
			foreach (MethodInfo mi in mis)
			{
				if (mi.Name == str)
				{
					Log.p("Run Func: " + str);
					string rt = (string)mi.Invoke(func, null);
					if (rt != null && rt != "")
					{
						Net.Get().Send(SendMessage.creatText(rt, msg.msg.e));
					}
					return;
				}
			}
			Log.p("No found Func: " + str);
			Net.Get().Send(SendMessage.creatRunError("No found Function To Run", msg.msg.e));
		}

		/// <summary>
		/// 在计算机桌面显示一个Notice信息，关闭时返回关闭状态
		/// </summary>
		/// <param name="content">要显示的内容</param>
		private void ShowNotice(string content)
		{
			MessageBox.Show(content, "远程控制提示信息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			Net.Get().Send(SendMessage.creatText("Notice Closed!", msg.msg.e));
		}

		/// <summary>
		/// 在计算机桌面显示警告，关闭时返回关闭状态
		/// </summary>
		/// <param name="content">警告内容</param>
		private void ShowWarning(string content)
		{
			MessageBox.Show(content, "远程控制警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			Net.Get().Send(SendMessage.creatText("Warning Closed!", msg.msg.e));
		}

		/// <summary>
		/// 运行命令，运行结束后返回命令已执行消息
		/// </summary>
		/// <param name="cmd">命令字符串，多命令换行</param>
		private void RunCommand(string cmd)
		{
			cmd = cmd.Replace("\n", " & ");
			System.Diagnostics.Process cmdProcess = System.Diagnostics.Process.Start("cmd.exe", "/C " + cmd);
			cmdProcess.WaitForExit();
			Net.Get().Send(SendMessage.creatText("Command already running!\n"+cmd, msg.msg.e));
		}

		/// <summary>
		/// 执行文件操作信息
		/// </summary>
		/// <param name="path">要操作的文件路径</param>
		private void RunFileSystem(string path)
		{
			FileControl fc = new FileControl(path, msg.msg.e);
			byte[] rt = fc.Callback();
			if (rt == null || rt.Length < 1)
			{
				string err = fc.GetError();
				if (err == "") err = "File operation returns a null value";
				Net.Get().Send(SendMessage.creatRunError(err, msg.msg.e));
			}
			else
			{
				Net.Get().Send(SendMessage.creatFile(rt, msg.msg.e));
			}
		}

		/// <summary>
		/// 获取当前桌面截图,并返回两个文件路径信息
		/// </summary>
		/// <param name="width">缩略图宽度，默认100</param>
		private void GetScreenshot(string width)
		{
			Screenshot s = new Screenshot(width);
			if (!s.isOK())
			{
				Net.Get().Send(SendMessage.creatRunError("Screenshot cann't create or found", msg.msg.e));
			}
			else
			{
				Net.Get().Send(SendMessage.creatPicture(s.getFull(), s.getFullSize(), s.get(), s.getSize(), msg.msg.e));
			}
		}

		/// <summary>
		/// 执行终端命令，每用户一个终端
		/// </summary>
		/// <param name="content">命令内容</param>
		private void RunTreminal(string content)
		{
			Terminal.Get().Run(content, msg.msg.e);
		}

		/// <summary>
		/// 对任务进行操作，发送任务列表或操作提示
		/// </summary>
		private void SetTask()
		{
			byte[] rt = Task.Process(msg.msg.Content, msg.msg.e);
			if (rt != null && rt.Length > 0)
			{
				Net.Get().Send(SendMessage.creatTask(rt, msg.msg.e));
			}
		}
	}
}
