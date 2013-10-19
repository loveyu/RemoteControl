using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace RemoteControlService
{
	/// <summary>
	/// 配文件静态类
	/// </summary>
	public class Configure
	{
		/// <summary>
		/// 程序运行路径
		/// </summary>
		private static string RunPath;

		/// <summary>
		/// 程序文件所在路径
		/// </summary>
		private static string RunFile = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

		/// <summary>
		/// 程序名称
		/// </summary>
		private static string RunName;

		/// <summary>
		/// 配置文件路径
		/// </summary>
		private static string ConfigPath;

		/// <summary>
		/// 服务器监听的UDP端口
		/// </summary>
		private static int _ServerPort = 0;

		/// <summary>
		/// 文件传输服务的tcp监听端口
		/// </summary>
		private static int _FilePort = 0;

		/// <summary>
		/// 服务器监听IP
		/// </summary>
		private static IPAddress _ServerIP;

		/// <summary>
		/// 文件传输监听IP
		/// </summary>
		private static IPAddress _FileIP;

		/// <summary>
		/// 根目录地址
		/// </summary>
		private static string _RootPath;

		/// <summary>
		/// 日志文件路径
		/// </summary>
		private static string _LogPath;

		/// <summary>
		/// 截图保存文件夹
		/// </summary>
		private static string _ScreenshotPath;

		/// <summary>
		/// 用户列表文件路径
		/// </summary>
		private static string _UserListPath;

		/// <summary>
		/// 生成的vb启动文件路径
		/// </summary>
		private static string RunVb;

		/// <summary>
		/// 开始加载配置文件
		/// </summary>
		/// <returns>能否成功加载</returns>
		public static bool Load()
		{
			try
			{
				FileInfo fi = new FileInfo(RunFile);
				RunPath = fi.DirectoryName;
				RunName = fi.Name;
				ConfigPath = RunFile + ".conf";
				RunVb = RunFile + ".vbs";
				if (File.Exists(ConfigPath))
				{
					ReadConfig();
				}
				else
				{
					WriteConfig();
				}
				Environment.CurrentDirectory = _RootPath;

				ReadUserList();

				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 读取用户列表并写入内存
		/// </summary>
		/// <returns>异常或文件过大则返回false</returns>
		private static bool ReadUserList()
		{
			try
			{
				if (new FileInfo(UserListPath()).Exists == false)
				{
					return true;
				}
				StreamReader sr = new StreamReader(UserListPath());
				if (sr.BaseStream.Length > 100000)
				{
					sr.Close();
					return true;
				}
				string content = sr.ReadToEnd();
				sr.Close();
				User.GetUserInfo(content.Split('\n'));
			}
			catch (Exception ex)
			{
				Log.a("Read User list Exception:" + ex.Message);
				return false;
			}
			return true;
		}

		/// <summary>
		/// 将用户列表数据保存到文件
		/// </summary>
		/// <param name="content">使用制表符分割的数据列表</param>
		/// <returns>能否成功写入数据</returns>
		public static bool WriteUserList(string content)
		{
			if (content.Trim().Length == 0) return false;
			try
			{
				StreamWriter sw = new StreamWriter(UserListPath(), false);
				sw.Write(content);
				sw.Close();
				return true;
			}
			catch (Exception ex)
			{
				Log.a("Write config Ex:" + ex.Message);
				return false;
			}
		}

		/// <summary>
		/// 读取配置文件，信息并创建默认信息
		/// </summary>
		private static void ReadConfig()
		{
			StreamReader sr = new StreamReader(ConfigPath);
			if (sr.BaseStream.Length > 100000)
			{
				sr.Close();
				WriteConfig();
				return;
			}
			string content = sr.ReadToEnd();
			string[] list = content.Split('\n');
			foreach (string str in list)
			{
				string[] t = str.Trim().Split('\t');
				if (t.Length == 2)
				{
					switch (t[0].Trim().ToLower())
					{
						case "serverip":
							_ServerIP = IPAddress.Parse(t[1].Trim());
							break;
						case "serverport":
							_ServerPort = int.Parse(t[1].Trim());
							break;
						case "fileip":
							_FileIP = IPAddress.Parse(t[1].Trim());
							break;
						case "fileport":
							_FilePort = int.Parse(t[1].Trim());
							break;
						case "rootpath":
							if (Directory.Exists(t[1].Trim()))
							{
								_RootPath = t[1].Trim();
							}
							break;
						case "logpath":
							if (new FileInfo(t[1].Trim()).Directory.Exists)
							{
								_LogPath = t[1].Trim();
							}
							break;
						case "screenshotpath":
							if (Directory.Exists(t[1].Trim()))
							{
								_ScreenshotPath = t[1].Trim();
							}
							break;
					}
				}
			}
			_UserListPath = RootPath() + "\\user.conf";
			Log.a("Config Prot:udp->" + _ServerPort + ",tcp->" + _FilePort);
			sr.Close();
		}

		/// <summary>
		/// 将默认数据写入配置文件
		/// </summary>
		private static void WriteConfig()
		{
			StreamWriter fs = new StreamWriter(ConfigPath);
			fs.WriteLine("ServerIP\t" + ServerIP());
			fs.WriteLine("ServerPort\t" + ServerPort());
			fs.WriteLine("FileIP\t" + FileIP());
			fs.WriteLine("FilePort\t" + FilePort());
			fs.WriteLine("RootPath\t" + RootPath());
			fs.WriteLine("LogPath\t" + LogPath());
			fs.WriteLine("ScreenshotPath\t" + ScreenshotPath());
			fs.Close();
		}

		/// <summary>
		/// 获取服务器UDP端口
		/// </summary>
		/// <returns>指定或默认端口2001</returns>
		public static int ServerPort()
		{
			if (_ServerPort == 0)
				_ServerPort = 2001;
			return _ServerPort;
		}

		/// <summary>
		/// 文件传输TCP端口号
		/// </summary>
		/// <returns>指定值或默认2002</returns>
		public static int FilePort()
		{
			if (_FilePort == 0)
				_FilePort = 2002;
			return _FilePort;
		}

		/// <summary>
		/// 返回服务器UDP监听IP
		/// </summary>
		/// <returns>未指定则为默认Any</returns>
		public static IPAddress ServerIP()
		{
			if (_ServerIP == null)
				_ServerIP = IPAddress.Any;
			return _ServerIP;
		}

		/// <summary>
		/// 返回文件服务器TCP监听IP
		/// </summary>
		/// <returns>未指定则为默认Any</returns>
		public static IPAddress FileIP()
		{
			if (_FileIP == null)
				_FileIP = IPAddress.Any;
			return _FileIP;
		}

		/// <summary>
		/// 获取运行更目录
		/// </summary>
		/// <returns>默认为执行文件下的Remote</returns>
		public static string RootPath()
		{
			if (_RootPath == null)
			{
				_RootPath = RunPath + "\\Remote";
				if (Directory.Exists(_RootPath) == false)
				{
					Directory.CreateDirectory(_RootPath);
				}
			}
			return _RootPath;
		}

		/// <summary>
		/// 获取日志文件路径
		/// </summary>
		/// <returns>默认为Root目录下run.log</returns>
		public static string LogPath()
		{
			if (_LogPath == null)
			{
				string dir = RunPath + "\\Remote";
				if (Directory.Exists(dir) == false)
				{
					Directory.CreateDirectory(dir);
				}
				_LogPath = dir + "\\run.log";
			}
			return _LogPath;
		}

		/// <summary>
		/// 获取截图文件保存路径
		/// </summary>
		/// <returns>默认为Root目录下Screenshot</returns>
		public static string ScreenshotPath()
		{
			if (_ScreenshotPath == null)
			{
				_ScreenshotPath = RunPath + "\\Remote\\Screenshot";
				if (Directory.Exists(_ScreenshotPath) == false)
				{
					Directory.CreateDirectory(_ScreenshotPath);
				}
			}
			return _ScreenshotPath;
		}

		/// <summary>
		/// 获取用户列表路径
		/// </summary>
		/// <returns>为Root下user.conf,无法修改</returns>
		public static string UserListPath()
		{
			if (_UserListPath == null)
			{
				_UserListPath = _RootPath + "\\user.conf";
			}
			return _UserListPath;
		}

		/// <summary>
		/// 生成的无窗口运行脚本路径
		/// </summary>
		/// <returns>返回可执行文件后加.vbs的脚本路径</returns>
		public static string RunVbPath()
		{
			if (RunVb == null)
			{
				RunVb = RunFile + ".vbs";
			}
			return RunVb;
		}

		/// <summary>
		/// 获取可执行文件路径
		/// </summary>
		/// <returns>返回路径</returns>
		public static string RunFilePath()
		{
			return RunFile;
		}
	}
}
