using System;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using System.Management;
using System.Net;

namespace RemoteControlService
{
	/// <summary>
	/// 功能函数类
	/// </summary>
	public class Function
	{

		/// <summary>
		/// 关闭服务器
		/// </summary>
		/// <returns></returns>
		public string close()
		{
			Log.a("Function Close Application");
			Net.Get().Stop();
			Environment.Exit(0);
			return "";
		}


		/// <summary>
		/// 获取计算机信息
		/// </summary>
		/// <returns></returns>
		public string info()
		{
			return "计算机名:" + ComputerInfo.GetComputerName() +
				"\n登录用户:" + ComputerInfo.GetUserName() +
			"\nIP:\n" + ComputerInfo.GetIPAddress() +
			"内存信息:\n" + ComputerInfo.GetMemInfo();
		}

		/// <summary>
		/// 配置文件信息
		/// </summary>
		/// <returns></returns>
		public string config()
		{
			string rt = "";
			rt += "程序路径: " + System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
			rt += "\n工作路径: " + Environment.CurrentDirectory;
			rt += "\n服务器端口: " + Net.Get().GetIp();
			rt += "\n文件服务器端口: " + FileSend.Get().getIP();
			rt += "\n配置文件路径: " + Configure.LogPath();
			rt += "\n截图目录: " + Configure.ScreenshotPath();
			rt += "\n用户列表: " + Configure.UserListPath();
			return rt + "\n";
		}

		/// <summary>
		/// 消息队列信息
		/// </summary>
		/// <returns></returns>
		public string msg()
		{
			return "消息队列长度:" + MessageQueue.Get().Length() + "\n";
		}

		/// <summary>
		/// 版本信息
		/// </summary>
		/// <returns></returns>
		public string version() {
			return "版本号："+System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}

		/// <summary>
		/// 文件传输队列信息
		/// </summary>
		/// <returns></returns>
		public string file()
		{
			string rt = "等待传输的文件列表：\n";
			string[] ls = FileSend.Get().GetPathList();
			if (ls.Length > 0)
			{
				foreach (string s in ls)
				{
					rt += s + "\n";
				}
			}
			else
			{
				rt += "列表为空";
			}
			return rt + "\n";
		}
	}

	/// <summary>
	/// 计算机信息获取类
	/// </summary>
	public class ComputerInfo
	{
		/// <summary>
		/// 获取IP地址列表
		/// </summary>
		/// <returns></returns>
		public static string GetIPAddress()
		{
			IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
			if (ips.Length > 0)
			{
				string rt = "";
				foreach (IPAddress s in ips)
				{
					rt += s.ToString() + "\n";
				}
				return rt;
			}
			else
			{
				return "127.0.0.1";
			}
		}

		/// <summary>
		/// 获取计算机用户名
		/// </summary>
		/// <returns></returns>
		public static string GetUserName()
		{
			try
			{
				string st = "";
				ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
				ManagementObjectCollection moc = mc.GetInstances();
				foreach (ManagementObject mo in moc)
				{
					st = mo["UserName"].ToString();
				}
				moc = null;
				mc = null;
				return st;
			}
			catch
			{
				return "unknow";
			}
			finally
			{
			}
		}

		/// <summary>
		/// 获取计算机名
		/// </summary>
		/// <returns></returns>
		public static string GetComputerName()
		{
			try
			{
				return System.Environment.GetEnvironmentVariable("ComputerName");
			}
			catch
			{
				return "unknow";
			}
			finally
			{
			}
		}

		/// <summary>
		/// 获取内存信息
		/// </summary>
		/// <returns></returns>
		public static string GetMemInfo()
		{
			Double PhysicalMemorySize = 0, VirtualMemorySize = 0, FreePhysicalMemory = 0;
			ManagementClass osClass = new ManagementClass("Win32_OperatingSystem");
			foreach (ManagementObject obj in osClass.GetInstances())
			{
				if (obj["TotalVisibleMemorySize"] != null)
					PhysicalMemorySize += (ulong)obj["TotalVisibleMemorySize"] / (double)(1024 * 1024);

				if (obj["TotalVirtualMemorySize"] != null)
					VirtualMemorySize += (ulong)obj["TotalVirtualMemorySize"] / (double)(1024 * 1024);

				if (obj["FreePhysicalMemory"] != null)
					FreePhysicalMemory += (ulong)obj["FreePhysicalMemory"] / (double)(1024 * 1024);
				break;
			}


			return "总内存: " + Math.Round(PhysicalMemorySize, 3).ToString() + "G\n"
				+ "虚拟内存: " + Math.Round(VirtualMemorySize, 3).ToString() + "G\n"
				+ "可用内存: " + Math.Round(FreePhysicalMemory, 3).ToString() + "G\n"
				+ "使用率: " + Math.Round((PhysicalMemorySize - FreePhysicalMemory) / PhysicalMemorySize * 100, 2).ToString() + "%";
		}

	}
}
