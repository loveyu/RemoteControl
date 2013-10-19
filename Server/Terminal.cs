using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;

namespace RemoteControlService
{
	/// <summary>
	/// 每个终端信息
	/// </summary>
	public class TerminalInfo
	{
		/// <summary>
		/// 终端消息写入事件
		/// </summary>
		private ManualResetEvent processWriteWaite = new ManualResetEvent(false);

		/// <summary>
		/// 终端的命令
		/// </summary>
		private string cmd = null;

		/// <summary>
		/// 写线程
		/// </summary>
		private Thread wThread;

		/// <summary>
		/// 读线程
		/// </summary>
		private Thread rThread;

		/// <summary>
		/// 错误读取线程
		/// </summary>
		private Thread eThread;

		/// <summary>
		/// 事件运行标记
		/// </summary>
		public bool eventHandled = false;

		/// <summary>
		/// 运行的终端进程
		/// </summary>
		System.Diagnostics.Process p;

		/// <summary>
		/// 用户地址
		/// </summary>
		IPEndPoint e;

		/// <summary>
		/// 构造函数，启用线程
		/// </summary>
		/// <param name="e">用户地址</param>
		public TerminalInfo(IPEndPoint e)
		{
			this.e = e;
			p = new System.Diagnostics.Process();
			p.StartInfo.FileName = "cmd.exe";
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			//p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
			p.StartInfo.CreateNoWindow = true;
			p.Exited += new EventHandler(myProcess_Exited);
			p.Start();
			wThread = new Thread(new ThreadStart(WriteThread));
			rThread = new Thread(new ThreadStart(ReadThread));
			eThread = new Thread(new ThreadStart(ErrorThread));
			wThread.Name = "Terminal write thread";
			rThread.Name = "Terminal read thread";
			eThread.Name = "Terminal error thread";
			wThread.Start();
			rThread.Start();
			eThread.Start();
		}

		/// <summary>
		/// 写线程，一旦收到数据立即写入
		/// </summary>
		private void WriteThread()
		{
			while (!eventHandled)
			{
				processWriteWaite.Reset();
				if (cmd != null)
				{
					p.StandardInput.Write(cmd);
					p.StandardInput.Flush();
					cmd = null;
				}
				processWriteWaite.WaitOne();
			}
		}

		/// <summary>
		/// 读线程，每次读取4097字节数据发送到客户端
		/// </summary>
		private void ReadThread()
		{
			while (!eventHandled)
			{
				Thread.Sleep(20);
				try
				{
					char[] buf = new char[4097];
					int rl = p.StandardOutput.Read(buf, 0, 4097);
					Log.d("Send terminal read:" + rl);
					Net.Get().Send(SendMessage.creatTerminal(new string(buf, 0, rl), e));
				}
				catch (Exception ex)
				{
					Log.d("Terminal ex:" + ex.Message);
					myProcess_Exited(null, null);
					return;
				}
			}
		}

		/// <summary>
		/// 读取错误输出发送到客户端
		/// </summary>
		private void ErrorThread()
		{
			while (!eventHandled)
			{
				Thread.Sleep(20);
				try
				{
					char[] buf = new char[4097];
					int rl = p.StandardError.Read(buf, 0, 4097);
					Log.d("Send terminal error:" + rl);
					Net.Get().Send(SendMessage.creatTerminalError(new string(buf, 0, rl), e));
				}
				catch (Exception ex)
				{
					Log.d("Terminal ex:" + ex.Message);
					myProcess_Exited(null, null);
					return;
				}
			}
		}

		/// <summary>
		/// 退出终端
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void myProcess_Exited(object sender, System.EventArgs e)
		{
			eventHandled = true;
			Log.d("Terminal Exited");
			processWriteWaite.Set();
			Remove();
		}

		/// <summary>
		/// 写入数据到终端
		/// </summary>
		/// <param name="str"></param>
		public void Write(string str)
		{
			cmd = str;
			Log.d("Terminal get char:" + cmd);
			processWriteWaite.Set();
		}

		/// <summary>
		/// 移除终端
		/// </summary>
		public void Remove()
		{
			try
			{
				eventHandled = true;
				wThread.Abort();
				rThread.Abort();
				eThread.Abort();
				p.Kill();
			}
			catch
			{
			}
		}
	}

	/// <summary>
	/// 终端控制
	/// </summary>
	public class Terminal
	{
		/// <summary>
		/// 终端单例模式实例
		/// </summary>
		private static readonly Terminal instance = new Terminal();

		/// <summary>
		/// 终端列表
		/// </summary>
		private Hashtable table = new Hashtable();

		/// <summary>
		/// 私有构造函数
		/// </summary>
		private Terminal() { }

		/// <summary>
		/// 获取实例
		/// </summary>
		/// <returns></returns>
		public static Terminal Get()
		{
			return instance;
		}

		/// <summary>
		/// 获取终端列表为一个字符串数组输出
		/// </summary>
		/// <returns></returns>
		public string[] GetList()
		{
			string[] rt = null;
			lock (table)
			{
				rt = new string[table.Count];
				int i = 0;
				foreach (string e in table.Keys)
				{
					rt[i++] = e;
				}
			}
			return rt;
		}

		/// <summary>
		/// 启动终端
		/// </summary>
		public void Start()
		{

		}

		/// <summary>
		/// 终止所有终端
		/// </summary>
		public void Stop()
		{
			lock (table)
			{
				if (table.Keys.Count > 0)
				{
					string[] hl = new string[table.Keys.Count];
					table.Keys.CopyTo(hl, 0);
					foreach (string s in hl)
					{
						((TerminalInfo)table[s]).Remove();
						table.Remove(s);
					}
				}
			}
		}

		/// <summary>
		/// 在用户自己终端上运行指定线程
		/// </summary>
		/// <param name="cmd">用户命令</param>
		/// <param name="e">用户地址</param>
		public void Run(string cmd, IPEndPoint e)
		{
			string name = e.ToString();
			lock (table)
			{
				if (table.Contains(name) == false)
				{
					TerminalInfo ti = new TerminalInfo(e);
					table.Add(name, ti);
				}
				else if (((TerminalInfo)table[name]).eventHandled == true)
				{
					TerminalInfo ti = new TerminalInfo(e);
					table.Remove(name);
					table.Add(name, ti);
				}
				((TerminalInfo)table[name]).Write(cmd);
			}
		}
	}
}
