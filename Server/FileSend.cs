using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace RemoteControlService
{
	/// <summary>
	/// 文件传输队列中的文件信息
	/// </summary>
	public class FileSendInfo
	{
		/// <summary>
		/// 对应用户的sid，验证用户有效性
		/// </summary>
		public string sid;

		/// <summary>
		/// 对应的文件信息
		/// </summary>
		public FileInfo fileInfo;

		/// <summary>
		/// 文件ID，发送给用户，标志该文件的唯一性
		/// </summary>
		public string fileId;
	}

	/// <summary>
	/// 文件传输服务
	/// </summary>
	public class FileSend
	{
		/// <summary>
		/// 单例模式实例
		/// </summary>
		private static readonly FileSend instance = new FileSend();

		/// <summary>
		/// 存储文件队列的hash表
		/// </summary>
		private Hashtable table = new Hashtable();

		/// <summary>
		/// Tcp监听服务器
		/// </summary>
		private TcpListener tcp;

		/// <summary>
		/// 服务器监听运行线程
		/// </summary>
		private Thread thread;

		/// <summary>
		/// 服务器运行标记
		/// </summary>
		private volatile bool runing;

		/// <summary>
		/// 本地监听信息
		/// </summary>
		private IPEndPoint ep;

		/// <summary>
		/// 连接监听事件
		/// </summary>
		private ManualResetEvent tcpArrived = new ManualResetEvent(false);

		/// <summary>
		/// 私有构造函数
		/// </summary>
		private FileSend()
		{

		}

		/// <summary>
		/// 获取对象实例
		/// </summary>
		/// <returns>返回文件传输对象</returns>
		public static FileSend Get()
		{
			return instance;
		}

		/// <summary>
		/// 获取文件传输IP
		/// </summary>
		/// <returns>IP信息</returns>
		public IPEndPoint getIP()
		{
			return ep;
		}

		/// <summary>
		/// 获取文件队列列表
		/// </summary>
		/// <returns>用户打印的数组</returns>
		public string[] GetPathList() {
			string[] rt = null;
			lock (table) {
 				int i = 0;
				rt = new string[table.Keys.Count];
				foreach(FileSendInfo fsi in table.Values){
					rt[i++] = fsi.fileInfo.FullName;
				}
			}
			return rt;
		}

		/// <summary>
		/// 加载配置并启动服务
		/// </summary>
		public void Start()
		{
			ep = new IPEndPoint(Configure.FileIP(), Configure.FilePort());
			tcp = new TcpListener(ep);
			thread = new Thread(new ThreadStart(FileSendProcess));
			thread.Name = "FileSend Thread";
			runing = true;
			thread.Start();
		}

		/// <summary>
		/// 文件传输处理进程
		/// </summary>
		private void FileSendProcess()
		{
			tcp.Start();
			while (runing)
			{
				Log.n("Filesend waiting for a tcp client");
				tcpArrived.Reset();
				tcp.BeginAcceptTcpClient(new AsyncCallback(tcpCallback), tcp);
				tcpArrived.WaitOne();
			}
		}

		/// <summary>
		/// 异步调用返回值
		/// </summary>
		/// <param name="ar">异步传输值</param>
		private void tcpCallback(IAsyncResult ar)
		{
			TcpListener listener = (TcpListener)ar.AsyncState;
			TcpClient client = listener.EndAcceptTcpClient(ar);
			tcpArrived.Set();
			IPEndPoint e = (IPEndPoint)client.Client.RemoteEndPoint;
			if (User.IpInUser(e) == false)
			{
				client.Close();
				return;
			}
			NetworkStream ns = client.GetStream();
			StreamReader sr = new StreamReader(ns, Encoding.UTF8);
			string sid = sr.ReadLine();
			Log.d("sid:" + sid);
			if (User.TcpUserConfirm(e, sid) == false)
			{
				client.Close();
				return;
			}
			string fileid = sr.ReadLine();
			Log.d("fileid:" + fileid);
			FileSendInfo fsi;
			try
			{
				lock (table)
				{
					if (fileid == null || table.Contains(fileid) == false)
					{
						client.Close();
						return;
					}
					fsi = (FileSendInfo)table[fileid];
				}
				if (fsi.sid != sid)
				{
					client.Close();
					return;
				}
				lock (table)
				{
					table.Remove(fileid);
				}
				FileStream fs = fsi.fileInfo.OpenRead();
				byte[] tmp = stringToBytes(fs.Length.ToString() + "\n");
				ns.Write(tmp, 0, tmp.Length);
				ns.Flush();
				while (true)
				{
					if (sr.ReadLine().ToLower() == "send")
					{
						break;
					}
				}
				Log.d("File output begin");
				tmp = new byte[2048];
				int rl = 0;
				while ((rl = fs.Read(tmp, 0, 2048)) != 0)
				{
					ns.Write(tmp, 0, rl);
				}
				fs.Close();
				sr.Close();
				ns.Close();
				Log.d("File output finish");
			}
			catch (Exception ex)
			{
				Log.d("Tcp running execption:" + ex.Message);
			}
			finally
			{
				client.Close();
			}
		}

		/// <summary>
		/// 停止文件传输服务
		/// </summary>
		public void Stop()
		{
			runing = false;
			if (thread != null && thread.IsAlive)
				thread.Abort();
			if (tcp != null)
				tcp.Stop();
		}

		/// <summary>
		/// 添加一个文件到传输队列
		/// </summary>
		/// <param name="fi">文件信息</param>
		/// <param name="sid">用户sid</param>
		/// <returns>文件传输唯一ID</returns>
		public string AddFile(FileInfo fi, string sid)
		{
			FileStream fs = fi.OpenRead();//进行打开测试
			fs.Close();
			FileSendInfo fsi = new FileSendInfo();
			fsi.fileInfo = fi;
			fsi.sid = sid;
			System.Security.Cryptography.SHA1 hash = System.Security.Cryptography.SHA1.Create();
			fsi.fileId = Convert.ToBase64String(hash.ComputeHash(SendMessage.ToBytes(fi.FullName + sid)));
			if (table.Contains(fsi.fileId)) return fsi.fileId;
			table.Add(fsi.fileId, fsi);
			return fsi.fileId;
		}

		/// <summary>
		/// 字符串转字节数组
		/// </summary>
		/// <param name="s">非空字符串</param>
		/// <returns>字符数组</returns>
		public byte[] stringToBytes(string s)
		{
			return SendMessage.ToBytes(s);
		}
	}
}
