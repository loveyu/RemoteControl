using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;

namespace RemoteControlService
{
	/// <summary>
	/// 消息队列处理
	/// </summary>
	public class MessageQueue
	{
		/// <summary>
		/// 消息队列实例
		/// </summary>
		private static readonly MessageQueue instance = new MessageQueue();

		/// <summary>
		/// 消息队列hash表
		/// </summary>
		private Hashtable queue = new Hashtable();

		/// <summary>
		/// 消息处理线程
		/// </summary>
		private Thread thread;

		/// <summary>
		/// 线程运行状态
		/// </summary>
		private volatile bool threadRuning = true;

		/// <summary>
		/// 最大尝试次数
		/// </summary>
		private const int maxTry = 5;

		/// <summary>
		/// 私有构造函数
		/// </summary>
		private MessageQueue()
		{

		}

		/// <summary>
		/// 队列检测线程
		/// </summary>
		private void CheckThread()
		{
			while (threadRuning)
			{
				string[] t = GetQueueKeys();
				if (t.Length > 0)
				{
					foreach (string id in t)
					{
						Process(id);
					}
				}
				//Log.d("Check Thread");
				Thread.Sleep(4000);
			}
		}

		/// <summary>
		/// 队列ID处理
		/// </summary>
		/// <param name="id">消息ID</param>
		private void Process(string id)
		{
			Log.d("Prcoess queue: " + id);
			DateTime now = DateTime.Now;
			SendMessage sm;
			lock (queue)
			{
				sm = (SendMessage)queue[id];
			}
			try
			{
				if (sm.time.Equals(DateTime.Parse("0001/1/1 0:00:00")) || sm.retransmission > maxTry || sm.time > now)
				{
					//移除
					lock (queue)
					{
						queue.Remove(id);
					}
					Log.d("Remove queue: " + id);
					return;
				}
				if (sm.time.AddSeconds(5) < now)
				{
					//重发
					Log.d("Send agin queue: " + id);
					Net.Get().Send(sm);
					return;
				}
			}
			catch (Exception ex)
			{
				Log.d("Message Queue Ex:" + ex.Message);
				lock (queue)
				{
					if (queue.Contains(id))
						queue.Remove(id);
				}
			}

		}

		/// <summary>
		/// 获取队列的消息头
		/// </summary>
		/// <returns>队列keys值数组</returns>
		private string[] GetQueueKeys()
		{
			lock (queue)
			{
				string[] ids = new string[queue.Keys.Count];
				int i = 0;
				foreach (string id in queue.Keys)
				{
					ids[i++] = id;
				}
				return ids;
			}
		}

		/// <summary>
		/// 启动消息处理队列
		/// </summary>
		public void Start()
		{
			threadRuning = true;
			thread = new Thread(new ThreadStart(CheckThread));
			thread.Name = "Message queue thread";
			thread.Start();
		}

		/// <summary>
		/// 停止消息处理队列
		/// </summary>
		public void Stop()
		{
			threadRuning = false;
			if (thread != null && thread.IsAlive)
				thread.Abort();
		}

		/// <summary>
		/// 获取实例
		/// </summary>
		/// <returns>单一的消息处理队列实例</returns>
		public static MessageQueue Get()
		{
			return instance;
		}

		/// <summary>
		/// 获取队列长度
		/// </summary>
		/// <returns></returns>
		public int Length()
		{
			lock (queue)
			{
				return queue.Count;
			}
		}

		/// <summary>
		/// 添加一个未发送的消息到队列
		/// </summary>
		/// <param name="sm">发送消息的实例</param>
		public void Add(SendMessage sm)
		{
			lock (queue)
			{
				Log.d("Add queue: " + sm.smsg.Id);
				queue.Add(sm.smsg.Id, sm);
			}
		}

		/// <summary>
		/// 更新队列的消息
		/// </summary>
		/// <param name="sm">原始消失实例</param>
		public void Update(SendMessage sm)
		{
			lock (queue)
			{
				Log.d("Update queue: " + sm.smsg.Id);
				if (queue.Contains(sm.smsg.Id))
				{
					((SendMessage)queue[sm.smsg.Id]).time = DateTime.Now;
					++((SendMessage)queue[sm.smsg.Id]).retransmission;
				}
			}
		}

		/// <summary>
		/// 移除一个队列
		/// </summary>
		/// <param name="id">hash表key</param>
		public void Remove(string id)
		{
			lock (queue)
			{
				Log.d("Remove queue: " + id);
				if (queue.Contains(id))
				{
					queue.Remove(id);
				}
			}
		}

		/// <summary>
		/// 移除队列根据用户IP
		/// </summary>
		/// <param name="e">用户ip及端口</param>
		public void RemoveQueue(string e)
		{
			lock (queue)
			{
				List<string> list = GetUserKeys(e);
				foreach (string id in list)
				{
					queue.Remove(id);
				}
			}
		}

		/// <summary>
		/// 获取队列中中用户
		/// </summary>
		/// <param name="e">格式化为ip:port后的IP用户地址</param>
		/// <returns></returns>
		private List<string> GetUserKeys(string e)
		{
			List<string> list = new List<string>();
			foreach (string id in queue.Keys)
			{
				if (((SendMessage)queue[id]).smsg.e.ToString() == e)
				{
					list.Add(id);
				}
			}
			return list;
		}
	}
}
