using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Collections;
using System.Text.RegularExpressions;

namespace RemoteControlService
{
	/// <summary>
	/// 用户信息
	/// </summary>
	public struct UserInfo
	{
		/// <summary>
		/// 用户地址字符串
		/// </summary>
		public string e;

		/// <summary>
		/// 用户名
		/// </summary>
		public string user;

		/// <summary>
		/// 用户密码
		/// </summary>
		public string password;

		/// <summary>
		/// 最后访问时间
		/// </summary>
		public DateTime lastTime;

		/// <summary>
		/// 用户唯一ID
		/// </summary>
		public string sid;
	}

	/// <summary>
	/// 用户操作类
	/// </summary>
	public class User
	{
		/// <summary>
		/// 用户列表
		/// </summary>
		private static Hashtable list = new Hashtable();

		/// <summary>
		/// 在线用户列表
		/// </summary>
		private static Hashtable now = new Hashtable();

		/// <summary>
		/// 判断用户是否登录
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static bool IsLogin(IPEndPoint e)
		{
			foreach (UserInfo ui in now.Values)
			{
				if (ui.e == e.ToString()) return true;
			}
			return false;
		}

		/// <summary>
		/// 统计所有用户数量
		/// </summary>
		/// <returns></returns>
		public static int UserListCount()
		{
			lock (list)
			{
				return list.Count;
			}
		}

		/// <summary>
		/// 统计在线用户数量
		/// </summary>
		/// <returns></returns>
		public static int UserNowCount()
		{
			lock (now)
			{
				return now.Count;
			}
		}

		/// <summary>
		/// 获取所有用户用户名
		/// </summary>
		/// <returns></returns>
		public static string[] UserList()
		{
			string[] rt = null;
			lock (list)
			{
				int i = 0;
				rt = new string[list.Keys.Count];
				foreach (string n in list.Keys)
				{
					rt[i] = n;
				}
			}
			return rt;
		}

		/// <summary>
		/// 获取在线用户名
		/// </summary>
		/// <returns></returns>
		public static string[] UserNowList()
		{
			string[] rt = null;
			lock (now)
			{
				int i = 0;
				rt = new string[now.Keys.Count];
				foreach (string n in now.Keys)
				{
					rt[i] = n;
				}
			}
			return rt;
		}

		/// <summary>
		/// 获取所有用户为一个制表符分割的列表
		/// </summary>
		/// <returns></returns>
		public static string GetUserListTable()
		{
			string rt = "";
			lock (list)
			{
				foreach (UserInfo u in list.Values)
				{
					rt += u.user + "\t" + u.password + "\t" + u.e + "\n";
				}
			}
			return rt;
		}

		/// <summary>
		/// 添加用户到数据表
		/// </summary>
		/// <param name="user">用户名</param>
		/// <param name="passwrod">明文密码</param>
		/// <param name="ip">IP规则</param>
		/// <returns></returns>
		public static bool AddUserToList(string user, string passwrod, string ip)
		{
			lock (list)
			{
				if (list.ContainsKey(user)) return false;
				list.Add(user, new UserInfo()
				{
					user = user,
					password = passwrod,
					e = ip,
					sid = ""
				});
				return true;
			}
		}

		/// <summary>
		/// 删除所有用户
		/// </summary>
		public static void DeleteAllUser()
		{
			string[] names = null;
			lock (list)
			{
				names = new string[list.Keys.Count];
				int i = 0;
				foreach (string n in list.Keys)
				{
					names[i++] = n;
				}
			}
			if (names == null) return;
			foreach (string name in names)
			{
				DeleteUser(name);
			}
		}

		/// <summary>
		/// 删除指定用户
		/// </summary>
		/// <param name="name">用户名</param>
		/// <returns>是否成功删除</returns>
		public static bool DeleteUser(string name)
		{
			lock (list)
			{
				if (list.Contains(name))
				{
					list.Remove(name);
					lock (now)
					{
						if (now.Contains(name))
						{
							MessageQueue.Get().RemoveQueue(((UserInfo)now[name]).e);
							now.Remove(name);
						}
					}
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// 将用户下线
		/// </summary>
		/// <param name="name">用户名</param>
		/// <returns>下线状态</returns>
		public static bool OfflineUser(string name)
		{
			lock (list)
			{
				if (list.Contains(name))
				{
					lock (now)
					{
						if (now.Contains(name))
						{
							MessageQueue.Get().RemoveQueue(((UserInfo)now[name]).e);
							now.Remove(name);
						}
					}
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// 移除一个用户重在线用户列表
		/// </summary>
		/// <param name="e">用户地址</param>
		public static void Remove(IPEndPoint e)
		{
			string name = null;
			lock (now)
			{
				foreach (string n in now.Keys)
				{
					if (((UserInfo)now[n]).e == e.ToString())
					{
						name = n;
					}
				}
				if (name != null)
				{
					now.Remove(name);
					MessageQueue.Get().RemoveQueue(e.ToString());
				}
			}
		}

		/// <summary>
		/// 用户登录请求
		/// </summary>
		/// <param name="e">用户地址</param>
		/// <param name="data">登录数据</param>
		/// <returns></returns>
		public static string Login(IPEndPoint e, string data)
		{
			string[] login = GetLoginData(data);
			if (login == null) return "500 Logon information is incorrect format";
			if (list.Contains(login[0]))
			{
				UserInfo ui = (UserInfo)list[login[0]];
				if (ui.password == login[1])
				{
					if (IpMatch(ui.e, e) == false)
					{
						return "301 The user can not log in from this IP";
					}
					if (login[2].ToLower() != "true")
					{
						if (now.Contains(login[0]))
						{
							return "502 User has logged in, try forcing login";
						}
					}
					string sid = "";
					if (AddUserInList(login[0], login[1], ref sid, e) == false)
					{
						return "505 Server exception";
					}
					return "200 " + sid;
				}
				return "403 Wrong password";
			}
			return "404 User Not Found";

		}

		/// <summary>
		/// 进行IP规则匹配
		/// </summary>
		/// <param name="set">规则</param>
		/// <param name="client">客户IP</param>
		/// <returns>是否匹配</returns>
		private static bool IpMatch(string set, IPEndPoint client)
		{
			try
			{
				string[] ipl = set.Split('\n');
				Regex reg;
				Match match;
				string ips = client.ToString();
				foreach (string s in ipl)
				{
					reg = new Regex(@s.Replace("*", "([0-9.:]+)"));
					match = reg.Match(ips);
					if (match.Value == ips) return true;
				}
			}
			catch (Exception ex)
			{
				Log.a("IP match ex:" + ex.Message);
			}
			return false;
		}

		/// <summary>
		/// 添加一个用户到当前列表，表示该用户已登录
		/// </summary>
		/// <param name="user">用户名</param>
		/// <param name="password">密码</param>
		/// <param name="sid">唯一ID</param>
		/// <param name="e">地址</param>
		/// <returns></returns>
		private static bool AddUserInList(string user, string password, ref string sid, IPEndPoint e)
		{
			try
			{
				System.Security.Cryptography.SHA1 hash = System.Security.Cryptography.SHA1.Create();
				sid = Convert.ToBase64String(hash.ComputeHash(SendMessage.ToBytes(user + password + e + DateTime.Now)));
				UserInfo info = new UserInfo()
				{
					user = user,
					password = password,
					e = e.ToString(),
					lastTime = DateTime.Now,
					sid = sid
				};
				if (now.Contains(user))
				{
					now[user] = info;
				}
				else
				{
					lock (now)
					{
						now.Add(user, info);
					}
				}
			}
			catch (Exception ex)
			{
				Log.a("Users can not be increased:" + ex.Message);
				return false;
			}
			return true;
		}

		/// <summary>
		/// 返回已登录的信息
		/// </summary>
		/// <returns></returns>
		public static string LoggedMsg()
		{
			return "201 You are already logged.";
		}

		/// <summary>
		/// 生成用户列表，从已读取的列表中
		/// </summary>
		/// <param name="userTable"></param>
		public static void GetUserInfo(string[] userTable)
		{
			list.Clear();
			if (userTable != null && userTable.Length > 0)
			{
				foreach (string lt in userTable)
				{
					string[] uinfo = lt.Trim().Split('\t');
					if (uinfo.Length != 3 || list.Contains(uinfo[0]))
					{
						continue;
					}
					list.Add(uinfo[0], new UserInfo()
					{
						user = uinfo[0],
						password = uinfo[1],
						e = uinfo[2],
						sid = ""
					});
				}
			}
			Log.d("Load user list");
		}

		/// <summary>
		/// 分割登录信息数据
		/// </summary>
		/// <param name="str">登录字符串</param>
		/// <returns>失败返回null，否则为三个长度的数组</returns>
		private static string[] GetLoginData(string str)
		{
			if (str == null || str.Length < 3)
			{
				return null;
			}
			string[] rt = str.Split('\n');
			if (rt.Length != 3) return null;
			return rt;
		}

		/// <summary>
		/// 判断地址是否为一个已登录的用户
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static bool IpInUser(IPEndPoint e)
		{
			lock (now)
			{
				foreach (string str in now.Keys)
				{
					string s = e.Address.ToString();
					if (((UserInfo)now[str]).e.IndexOf(s) == 0)
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 确定文件传输中的用户是否在列表中
		/// </summary>
		/// <param name="e">用户地址</param>
		/// <param name="sid">用户唯一ID</param>
		/// <returns></returns>
		public static bool TcpUserConfirm(IPEndPoint e, string sid)
		{
			lock (now)
			{
				foreach (string str in now.Keys)
				{
					string s = e.Address.ToString();
					if (((UserInfo)now[str]).e.IndexOf(s) == 0 && sid == ((UserInfo)now[str]).sid)
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 获取指定IP用户的SID
		/// </summary>
		/// <param name="e">用户地址</param>
		/// <returns></returns>
		public static string GetSid(IPEndPoint e)
		{
			lock (now)
			{
				foreach (string str in now.Keys)
				{
					string s = e.Address.ToString();
					if (((UserInfo)now[str]).e.IndexOf(s) == 0)
						return ((UserInfo)now[str]).sid;
				}
			}
			return "";
		}
	}
}
