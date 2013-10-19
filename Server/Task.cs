using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace RemoteControlService
{
	/// <summary>
	/// 任务管理
	/// </summary>
	public class Task
	{
		/// <summary>
		/// 获取任务列表
		/// </summary>
		/// <param name="e">对应的用户</param>
		/// <returns>返回最后一段数组或空，其他数据在获取过程中分段发送</returns>
		private static byte[] Get(IPEndPoint e)
		{
			System.Diagnostics.Process[] all;
			all = System.Diagnostics.Process.GetProcesses();
			string head = "Process\t";
			int count = (int)Math.Ceiling(all.Length / 5f);
			string rt = head + count + "\n";
			string id, name, me, path, time;
			int i = 0;
			foreach (System.Diagnostics.Process p in all)
			{
				id = p.Id.ToString();
				name = p.ProcessName;
				try
				{
					time = p.StartTime.ToString();
				}
				catch
				{
					time = "0";
				}
				me = p.WorkingSet64.ToString();
				try
				{
					path = p.MainModule.FileName;
				}
				catch
				{
					path = "null";
				}
				rt += id + "\t" + name + "\t" + me + "\t" + time + "\t" + path + "\n";
				if (i++ == 4)
				{
					Net.Get().Send(SendMessage.creatTask(SendMessage.ToBytes(rt), e));
					rt = head + --count + "\n";
					i = 0;
				}
			}
			return SendMessage.ToBytes(rt);
		}

		/// <summary>
		/// 结束掉某一进程
		/// </summary>
		/// <param name="id">发送过来的进程ID字符串</param>
		/// <returns>返回状态</returns>
		private static byte[] Kill(string id)
		{
			try
			{
				System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(int.Parse(id));
				p.Kill();
				return SendMessage.ToBytes("Killed\n" + id);
			}
			catch
			{
				return SendMessage.ToBytes("KillError\n" + id);
			}
		}

		/// <summary>
		/// 处理任务命令
		/// </summary>
		/// <param name="cmd">客户端发送的命令</param>
		/// <param name="e">客户地址</param>
		/// <returns></returns>
		public static byte[] Process(string cmd, IPEndPoint e)
		{
			string[] list = cmd.Split('\n');
			if (list.Length == 0) return null;
			switch (list[0].ToLower())
			{
				case "get":
					return Get(e);
				case "kill":
					if (list.Length >= 2)
					{
						return Kill(list[1]);
					}
					break;
			}
			return null;
		}
	}
}
