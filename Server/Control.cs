using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace RemoteControlService
{
	/// <summary>
	/// 主控制器
	/// </summary>
	public class Control
	{
		/// <summary>
		/// 网络主线程
		/// </summary>
		Thread thread;

		/// <summary>
		/// 启动状态
		/// </summary>
		bool status = false;

		/// <summary>
		/// 启动网络服务
		/// </summary>
		public void Start()
		{
			if (!Configure.Load())
			{
				MessageBox.Show("Remote Sever Configure load faild!");
				return;
			}
			Log.a("Control Start");
			if (Net.Get().Start())
			{
				thread = new Thread(new ThreadStart(ReceivedThread));
				thread.Name = "Received thread";
				thread.Start();
				status = true;
			}
			else
			{
				Log.a("Net start error, Control exit");
				MessageBox.Show("Remote Sever Start error");
			}
		}

		/// <summary>
		/// 网络接收线程
		/// </summary>
		private void ReceivedThread()
		{
			Net.Get().Receive();
		}

		/// <summary>
		/// 停止网络服务
		/// </summary>
		public void Stop()
		{
			Log.a("Control Stop");
			Net.Get().Stop();
			Environment.Exit(0);
		}

		/// <summary>
		/// 控制台主操作
		/// </summary>
		public void Terminal()
		{
			string[] cmds;
			ControlUser.CheckUserEmpty();
			while (status)
			{
				Console.Write("Remote>");
				cmds = Console.ReadLine().Trim().Split(' ');
				switch (cmds[0].Trim().ToLower())
				{
					case "":
						continue;
					case "u":
					case "user":
						ControlUser.UserAction(cmds);
						break;
					case "config":
						Console.WriteLine("\n" + new Function().config());
						break;
					case "q":
					case "quit":
					case "exit":
						Stop();
						break;
					case "m":
					case "msg":
						Console.WriteLine("\n" + new Function().msg());
						break;
					case "f":
					case "file":
						Console.WriteLine("\n" + new Function().file());
						break;
					case "ter":
					case "t":
					case "terminal":
						TerminalControl(cmds);
						break;
					case "cls":
					case "c":
					case "clear":
						Console.Clear();
						break;
					case "s":
					case "startup":
						CreateStartupFile();
						break;
					case "v":
					case "version":
						version();
						break;
					case "h":
					case "help":
					case "--help":
					case "/?":
					case "?":
					case "\\?":
						help();
						break;
					default:
						Console.WriteLine("Usage: config | user | msg | file | terminal | exit | startup | clear | version | help\n");
						break;
				}
			}
		}

		/// <summary>
		/// 帮助信息
		/// </summary>
		private void help()
		{
			Console.WriteLine("帮助信息:");
			Console.WriteLine("恋羽作品:http://www.loveyu.org");
			version();
			Console.WriteLine("config\t查看当前运行信息");
			Console.WriteLine("user\t用户操作命令,别名:u");
			ControlUser.help();
			Console.WriteLine("msg\t查看消息队列长度,别名:m");
			Console.WriteLine("file\t查看文件传输服务队列信息,别名:f");
			Console.WriteLine("terminal\t查看运行的终端，并进行相应操作，别名:ter,t\n\t支持参数: reset(重置连接表)");
			Console.WriteLine("exit\t结束程序运行,别名:q,quit");
			Console.WriteLine("startup\t生成没有窗口的运行脚本，复制到启动目录实现开机启动,别名:s");
			Console.WriteLine("clear\t清屏，清空控制台,别名:cls,c");
			Console.WriteLine("version\t查看版本信息，别名:v");
			Console.WriteLine("help\t帮助信息,别名:h,--help,/?,\\?,?");
			Console.WriteLine();
		}

		/// <summary>
		/// 查看版本信息
		/// </summary>
		private void version()
		{
			Version vs = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			Console.WriteLine("程序版本: " + vs);
		}

		/// <summary>
		/// 创建无窗口运行脚本文件
		/// </summary>
		private void CreateStartupFile()
		{
			try
			{
				StreamWriter sw = new StreamWriter(Configure.RunVbPath(), false);
				sw.Write("Set ws = CreateObject(\"Wscript.Shell\")\r\nws.run \"cmd /c " + Configure.RunFilePath().Replace("\\", "\\\\") + "\",vbhide");
				sw.Close();
				Console.WriteLine("启动脚本已生成，请将其复制到启动目录。\n" + Configure.RunVbPath());
			}
			catch
			{
				Console.WriteLine("无法创建启动脚本,请检查路径异常。\n" + Configure.RunVbPath());
			}
		}

		/// <summary>
		/// 对客户端开启的DOS窗口进行管理
		/// </summary>
		/// <param name="cmds">命令行参数</param>
		private void TerminalControl(string[] cmds)
		{
			if (cmds.Length == 1)
			{
				string[] l = RemoteControlService.Terminal.Get().GetList();
				if (l.Length <= 0)
				{
					Console.WriteLine("没有远程终端运行");
				}
				else
				{
					Console.WriteLine("远程终端列表:");
					foreach (string s in l)
					{
						Console.WriteLine(" " + s);
					}
				}
			}
			else
			{
				if (cmds[1].ToLower() == "reset")
				{
					RemoteControlService.Terminal.Get().Stop();
					RemoteControlService.Terminal.Get().Start();
					Console.WriteLine("已重置");
				}
				else
				{
					Console.WriteLine("只支持 reset 重置所有连接操作");
				}
			}
		}
	}
}
