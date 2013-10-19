using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteControlService
{
	/// <summary>
	/// 控制台对用户管理的类
	/// </summary>
	public class ControlUser
	{
		/// <summary>
		/// 检测是否存在用户，不存在时在终端进行输出
		/// </summary>
		public static void CheckUserEmpty()
		{
			if (User.UserListCount() == 0)
			{
				Console.WriteLine("当前不存在任何用户,使用[ user add ]添加用户");
			}
		}

		/// <summary>
		/// 对用户操作命令进行操作
		/// </summary>
		/// <param name="list">参数列表</param>
		public static void UserAction(string[] list)
		{
			if (list.Length <= 1)
			{
				int count = 0;
				Console.WriteLine("当前在线用户: " + (count = User.UserNowCount()));
				Console.WriteLine("所有用户数量: " + User.UserListCount());
				if (count > 0)
				{
					Console.WriteLine("在线用户列表:");
					foreach (string u in User.UserNowList())
					{
						Console.WriteLine(" " + u);
					}
				}
			}
			else
			{
				switch (list[1].Trim().ToLower())
				{
					case "a":
					case "add":
						string name, password, ip;
						name = ReadNoEmptyValue("用户名:");
						password = ReadNoEmptyValue("密码:");
						ip = ReadNoEmptyValue("IP地址规则,支持正则(*将做完全匹配替换):");
						if (User.AddUserToList(name, password, ip))
						{
							Console.WriteLine("添加用户成功!");
						}
						else
						{
							Console.WriteLine("添加用户失败，或许用户已存在!");
						}
						break;
					case "d":
					case "delete":
						if (list.Length < 3)
						{
							Console.WriteLine("请尝试命令: user delete [name] | user deleteAll");
						}
						else
						{
							if (User.DeleteUser(list[2].Trim()))
							{
								Console.WriteLine("删除成功!");
							}
							else
							{
								Console.WriteLine("删除 " + list[2].Trim() + " 出错！");
							}
						}
						break;
					case "off":
					case "offline":
						if (list.Length < 3)
						{
							Console.WriteLine("尝试: user offline [name]");
						}
						else
						{
							if (User.OfflineUser(list[2].Trim()))
							{
								Console.WriteLine("下线成功!");
							}
							else
							{
								Console.WriteLine("下线 " + list[2].Trim() + " 出错！");
							}
						}
						break;
					case "da":
					case "deleteall":
						User.DeleteAllUser();
						if (User.UserListCount() == 0)
						{
							Console.WriteLine("用户已清空!");
						}
						else
						{
							Console.WriteLine("用户清空出错!");
						}
						break;
					case "s":
					case "save":
						if (Configure.WriteUserList(User.GetUserListTable()))
						{
							Console.WriteLine("成功保存用户列表!");
						}
						else
						{
							Console.WriteLine("保存出现异常");
						}
						break;
					case "l":
					case "list":
						string[] l = User.UserNowList();
						if (l.Length > 0)
						{
							Console.WriteLine("在线用户列表:");
							foreach (string u in l)
							{
								Console.WriteLine(" " + u);
							}
						}
						else
						{
							Console.WriteLine("当前没有用户在线.");
						}
						l = User.UserList();
						if (l.Length > 0)
						{
							Console.WriteLine("\n所有用户列表:");
							foreach (string u in l)
							{
								Console.WriteLine(" " + u);
							}
						}
						else
						{
							Console.WriteLine("\n系统没有任何用户.");
						}
						break;
					case "h":
					case "?":
					case "/?":
					case "\\?":
					case "--help":
					case "help":
						help();
						break;
					default:
						Console.WriteLine("Usage: user [ add | delete [name] | offline [user] | save | deleteAll | list | help]");
						break;
				}
			}
		}

		/// <summary>
		/// 用户帮助详细信息
		/// </summary>
		public static void help() {
			Console.WriteLine("  详细参数列表:");
			Console.WriteLine("  add\t添加一个用户,别名:a");
			Console.WriteLine("  delete [name]\t删除用户,别名:d");
			Console.WriteLine("  deleteAll\t删除全部用户,别名:da");
			Console.WriteLine("  offline [name]\t下线指定用户,别名:off");
			Console.WriteLine("  save\t保存用户到配置文件,别名:s");
			Console.WriteLine("  list\t查看当前加载的用户信息,别名:s");
			Console.WriteLine("  help\t帮助信息,别名:h,?");
		}

		/// <summary>
		/// 获取控制台输入的字符串
		/// </summary>
		/// <param name="notice">输入提示字符</param>
		/// <returns>返回输入信息，不允许输入空字符，否则将永远阻塞</returns>
		private static string ReadNoEmptyValue(string notice)
		{
			string read = "";
			while (true)
			{
				Console.Write(notice);
				read = Console.ReadLine().Trim();
				if (read.Length > 0)
				{
					break;
				}
			}
			return read;
		}
	}
}
