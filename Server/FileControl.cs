using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace RemoteControlService
{
	/// <summary>
	/// 远程文件控制类
	/// </summary>
	public class FileControl
	{
		/// <summary>
		/// 远程操作路径
		/// </summary>
		string path;

		/// <summary>
		/// 对应的命令参数
		/// </summary>
		string[] cmd;

		/// <summary>
		/// 首条命令
		/// </summary>
		string action = "";

		/// <summary>
		/// 错误信息
		/// </summary>
		string error = "";
		
		/// <summary>
		/// 用户IP路径
		/// </summary>
		private IPEndPoint e;

		/// <summary>
		/// 构造函数，创建一个新的对象
		/// </summary>
		/// <param name="cmd">命令字符串</param>
		/// <param name="e">ip地址路径</param>
		public FileControl(string cmd, IPEndPoint e)
		{
			this.e = e;
			this.cmd = cmd.Split('\n');
			if (this.cmd.Length < 2)
			{
				error = "102 Invalid file manipulation commands";
				return;
			}
			this.action = this.cmd[0];
			this.path = this.cmd[1];
		}

		/// <summary>
		/// 回调函数，通过新的对象进行操作，返回对应byte数据进行返回
		/// </summary>
		/// <returns>返回null表示一个错误，将对客户端返回错误消息</returns>
		public byte[] Callback()
		{
			try
			{
				if (error != "") return null;
				switch (action.ToUpper())
				{
					case "GET":
						if (path == "ROOT")
						{
							return GetRootFileList();
						}
						if (File.Exists(path))
						{
							return ReadFile(path);
						}
						if (Directory.Exists(path))
						{
							return ReadDirectory(path);
						}
						error = "101 File no found";
						return null;
					case "DELETE":
						return DeletePath();
					case "RENAME":
						return Rename();
					case "MKFILE":
						return MakeFile();
					case "MKDIR":
						return MakeDir();
					case "COPY":
						return Copy();
				}
			}
			catch (Exception ex)
			{
				Log.f("File Exception:" + ex.Message);
			}
			return null;
		}

		/// <summary>
		/// 获取错误消息
		/// </summary>
		/// <returns>错误消息</returns>
		public string GetError()
		{
			return error;
		}

		/// <summary>
		/// 获取根目录的文件列表
		/// </summary>
		/// <returns>读取到的所有盘符及盘符大小</returns>
		private byte[] GetRootFileList()
		{
			string rt = "DIR\tTYPE\tNAME\tDATE\tSIZE\tROOT\n";
			string tmp;
			DriveInfo[] drives = DriveInfo.GetDrives();
			foreach (DriveInfo info in drives)
			{
				try
				{
					tmp = "DIR\t" + info.Name + "\t0\t" + info.TotalSize + "\n";

				}
				catch
				{
					tmp = "DIR\t" + info.Name + "\t0\t0\n";
				}
				rt += tmp;
			}
			return SendMessage.ToBytes(rt);
		}

		/// <summary>
		/// 读取文件信息，且加入文件传输队列
		/// </summary>
		/// <param name="path">要读取的文件路径</param>
		/// <returns>返回一个文件下载操作，null为错误</returns>
		private byte[] ReadFile(string path)
		{
			try
			{
				FileInfo fi = new FileInfo(path);
				long size = fi.Length;
				string fileid = FileSend.Get().AddFile(fi, User.GetSid(e));
				return SendMessage.ToBytes("FILE\t" + path + "\t" + size + "\t" + fileid);
			}
			catch
			{
				error = "File no found or read error";
			}
			return null;
		}

		/// <summary>
		/// 读取一个存在路径的文件夹列表信息
		/// </summary>
		/// <param name="dir">要读取的文件夹</param>
		/// <returns></returns>
		private byte[] ReadDirectory(string dir)
		{
			DirectoryInfo di = new DirectoryInfo(dir + "\\");
			string rt = "DIR\tTYPE\tNAME\tDATE\tSIZE\t" + dir + "\n";
			string tmp;
			try
			{
				foreach (DirectoryInfo fi in di.GetDirectories())
				{
					tmp = "DIR\t" + fi.FullName + "\t" + ConvertDateTimeInt(fi.LastWriteTime) + "\n";
					rt += tmp;
				}
				foreach (FileInfo fi in di.GetFiles())
				{
					tmp = "FILE\t" + fi.FullName + "\t" + ConvertDateTimeInt(fi.LastWriteTime) + "\t" + fi.Length + "\n";
					rt += tmp;
				}
			}
			catch
			{
				error = "Directory no found";
			}
			return SendMessage.ToBytes(rt);
		}

		/// <summary>
		/// 删除指定路径
		/// </summary>
		/// <returns>删除路径的状态</returns>
		private byte[] DeletePath()
		{
			try
			{
				if (File.Exists(path))
				{
					File.Delete(path);
					return SendMessage.ToBytes("DELETE\tFILE\t" + path);
				}
				if (Directory.Exists(path))
				{
					if (cmd.Length > 2)
					{
						Directory.Delete(path, (cmd[2].ToLower() == "true") ? true : false);
					}
					else
					{
						Directory.Delete(path);
					}
					return SendMessage.ToBytes("DELETE\tDIR\t" + path);
				}
				error = "Path no found";
				return null;
			}
			catch
			{
				error = "Delete path error";
				return null;
			}
		}

		/// <summary>
		/// 对文件或文件夹进行重命名操作
		/// </summary>
		/// <returns>返回操作的状态</returns>
		private byte[] Rename()
		{
			string src = path;
			if (cmd.Length < 3)
			{
				error = "Target file parameter does not exist";
				return null;
			}
			string dst = cmd[2];
			if (dst == "" || src == "")
			{
				error = "Parameter can not be null";
				return null;
			}
			if (Exists(dst))
			{
				error = "Rename the target already exists";
				return null;
			}
			if (File.Exists(src))
			{
				try
				{
					File.Move(src, dst);
					return SendMessage.ToBytes("RENAME\tFILE\t" + src + "\t" + dst);
				}
				catch
				{
					error = "File move error";
					return null;
				}
			}
			if (Directory.Exists(src))
			{
				try
				{
					Directory.Move(src, dst);
					return SendMessage.ToBytes("RENAME\tDIR\t" + src + "\t" + dst);
				}
				catch
				{
					error = "Directory move error";
					return null;
				}
			}
			return null;
		}

		/// <summary>
		/// 创建一个空的文件
		/// </summary>
		/// <returns>返回状态信息</returns>
		private byte[] MakeFile()
		{
			if (Exists(path) == false)
			{
				try
				{
					File.Create(path).Close();
					return SendMessage.ToBytes("MKFILE\t" + path);
				}
				catch
				{
					error = "file carete error";
					return null;
				}
			}
			error = "file path is exists";
			return null;
		}

		/// <summary>
		/// 创建指定文件夹
		/// </summary>
		/// <returns>返回状态信息</returns>
		private byte[] MakeDir()
		{
			if (Exists(path) == false)
			{
				try
				{
					Directory.CreateDirectory(path);
					return SendMessage.ToBytes("MKDIR\t" + path);
				}
				catch
				{
					error = "directrory create error";
					return null;
				}
			}
			error = "directory path is exists";
			return null;
		}

		/// <summary>
		/// 复制文件操作，如果是文件夹则操作失败
		/// </summary>
		/// <returns>返回状态</returns>
		private byte[] Copy()
		{
			if (cmd.Length < 3)
			{
				error = "Copy destination parameter does not exist";
				return null;
			}
			string dst = cmd[2];
			if (Exists(dst))
			{
				error = "Target file already exists";
				return null;
			}
			try
			{
				if (File.Exists(path))
				{
					File.Copy(path, dst);
					return SendMessage.ToBytes("COPY\tFILE\t" + path + "\t" + dst);
				}
				if (Directory.Exists(path))
				{
					error = "Please copy the files using the command line";
					return null;
				}
			}
			catch
			{
				error = "Copy path error";
				return null;
			}
			error = "unknown file type can not copy";
			return null;
		}

		/// <summary>
		/// 存在性判断，不区分文件或文件夹
		/// </summary>
		/// <param name="path">指定路径</param>
		/// <returns>任意存在则为真</returns>
		private bool Exists(string path)
		{
			return File.Exists(path) || Directory.Exists(path);
		}

		/// <summary>
		/// 将时间转换为时间戳
		/// </summary>
		/// <param name="time">指定的时间</param>
		/// <returns>至1970年来的时间</returns>
		public static int ConvertDateTimeInt(System.DateTime time)
		{
			System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
			return (int)(time - startTime).TotalSeconds;
		}
	}
}
