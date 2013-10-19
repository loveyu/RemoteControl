using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RemoteControlService
{
	/// <summary>
	/// 异步接收状态信息
	/// </summary>
	public class UdpState
	{
		/// <summary>
		/// udp服务对象
		/// </summary>
		public UdpClient u;

		/// <summary>
		/// 当前连接的用户地址
		/// </summary>
		public IPEndPoint e;
	}

	/// <summary>
	/// 异步发送状态信息
	/// </summary>
	public class UdpSendState
	{
		/// <summary>
		/// udp服务对象
		/// </summary>
		public UdpClient u;
		
		/// <summary>
		/// 要发送的消息对象
		/// </summary>
		public SendMessage sm;
	}

	/// <summary>
	/// 网络处理线程
	/// </summary>
	public class Net
	{
		/// <summary>
		/// udp客户端
		/// </summary>
		private UdpClient udp;

		/// <summary>
		/// 绑定的ip及端口
		/// </summary>
		private IPEndPoint ipEP;

		/// <summary>
		/// 网络的单例模式
		/// </summary>
		private static readonly Net instance = new Net();

		/// <summary>
		/// 接收线程运行状态
		/// </summary>
		private bool Runing = false;

		/// <summary>
		/// 网络接收事件
		/// </summary>
		private ManualResetEvent udpArrived = new ManualResetEvent(false);

		/// <summary>
		/// 网络发送事件
		/// </summary>
		private ManualResetEvent udpSent = new ManualResetEvent(false);

		/// <summary>
		/// 私有构造函数
		/// </summary>
		private Net()
		{

		}

		/// <summary>
		/// 获取单例模式实例
		/// </summary>
		/// <returns></returns>
		public static Net Get()
		{
			return instance;

		}

		/// <summary>
		/// 启动网络线程
		/// </summary>
		/// <returns>启动状态</returns>
		public bool Start()
		{
			try
			{
				ipEP = new IPEndPoint(Configure.ServerIP(), Configure.ServerPort());
				Runing = true;
				udp = new UdpClient(ipEP);
				MessageQueue.Get().Start();
				FileSend.Get().Start();
				Terminal.Get().Start();
			}
			catch (Exception ex)
			{
				Log.a("Net Start Exception:" + ex.StackTrace + ":" + ex.Message);
				return false;
			}
			return true;
		}

		/// <summary>
		/// 停止网络线程
		/// </summary>
		public void Stop()
		{
			Runing = false;
			Terminal.Get().Stop();
			FileSend.Get().Stop();
			MessageQueue.Get().Stop();
		}

		/// <summary>
		/// 网络接收线程
		/// </summary>
		public void Receive()
		{
			UdpState s = new UdpState();
			s.e = ipEP;
			s.u = udp;
			Log.a("Net reveive is runing");
			while (Runing)
			{
				try
				{
					udpArrived.Reset();
					udp.BeginReceive(new AsyncCallback(ReceiveCallback), s);
					udpArrived.WaitOne();
				}
				catch (Exception ex)
				{
					Log.n("Exception:" + ex.Message);
				}

			}
		}

		/// <summary>
		/// 异步网络接收回调函数
		/// </summary>
		/// <param name="ar">回调参数</param>
		private void ReceiveCallback(IAsyncResult ar)
		{
			UdpClient u = (UdpClient)((UdpState)(ar.AsyncState)).u;
			IPEndPoint e = (IPEndPoint)((UdpState)(ar.AsyncState)).e;
			byte[] bytes = null;
			try
			{
				bytes = u.EndReceive(ar, ref e);
			}
			catch (Exception ex)
			{
				Log.n("Net data Callback Exception:" + ex.Message);
			}
			udpArrived.Set();
			Log.d("Received");
			if (bytes != null)
			{
				Message msg = new Message(bytes, e);
				if (msg.isOK)
					new Process(msg);
			}

		}

		/// <summary>
		/// 发送数据包
		/// </summary>
		/// <param name="sm">发送消息数据对象</param>
		public void Send(SendMessage sm)
		{
			try
			{
				byte[] bytes = sm.getBytes();
				if (bytes == null)
				{
					Log.n("empty data to {" + sm.smsg.e + "}");
					return;
				}
				UdpSendState uss = new UdpSendState();
				uss.u = udp;
				uss.sm = sm;
				udpSent.Reset();
				if (sm.smsg.type != SendType.Callback)
				{
					if (sm.time.Equals(DateTime.Parse("0001/1/1 0:00:00")))
					{
						sm.time = DateTime.Now;
						MessageQueue.Get().Add(sm);
					}
					else
					{
						MessageQueue.Get().Update(sm);
					}
				}
				udp.BeginSend(bytes, bytes.Length, sm.smsg.e, new AsyncCallback(SendCallback), uss);
				udpSent.WaitOne();
			}
			catch (Exception ex)
			{
				Log.n(ex.Message);
			}
		}

		/// <summary>
		/// 发送回调方法
		/// </summary>
		/// <param name="ar">回调参数</param>
		private void SendCallback(IAsyncResult ar)
		{
			UdpSendState u = (UdpSendState)ar.AsyncState;
			try
			{
				u.u.EndSend(ar);
			}
			catch (Exception ex)
			{
				Log.n("Exception:{" + u.sm.smsg.type.ToString() + "}" + ex.Message);
				Net.Get().Send(SendMessage.creatRunError("Data Send ERROR", u.sm.smsg.e));
				return;
			}
			finally
			{
				udpSent.Set();
			}
		}

		/// <summary>
		/// 获取运行的IP信息
		/// </summary>
		/// <returns>IP结点信息</returns>
		public IPEndPoint GetIp()
		{
			return ipEP;
		}
	}
}
