package net.loveyu.remotecontrol;

import java.io.File;
import java.io.IOException;
import java.net.InetAddress;
import java.net.NetworkInterface;
import java.net.SocketException;
import java.util.Enumeration;
import java.util.HashMap;
import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Environment;

/**
 * 配置信息类
 * 
 * @author loveyu
 * 
 */
public class RcConfig {
	/**
	 * 应用程序文件路径
	 */
	private static String FilePath;
	/**
	 * 临时文件路径
	 */
	private static String CachePath;
	/**
	 * 本地IP地址
	 */
	private static String LocalIpAddress;
	/**
	 * 远程登录信息
	 */
	private static String ServerName = "", FileServerName = "", User = "", Password = "";
	/**
	 * 远程端口信息
	 */
	private static int ServerPort = 0, FileServerPort = 0;
	/**
	 * 登录配置信息
	 */
	private static boolean SaveLoginInfo = true, ForceLogin = false, ConfigStatus = false;
	/**
	 * SD卡路径信息
	 */
	private static String Sdcard = "", SdcardDownload = "", SdcardLog = "";

	/**
	 * 返回配置状态信息
	 * 
	 * @return 状态
	 */
	public static boolean Status() {
		return ConfigStatus;
	}

	/**
	 * 加载配置文件
	 * 
	 * @param context
	 *            Context对象
	 */
	public static void Load(Context context) {
		FilePath = context.getFilesDir().getPath();
		CachePath = context.getCacheDir().getPath();
		LocalIpAddress = getLocalIpAddress();
		LoadSeverConfig();
		LoadLoginConfig();
		LoadSdcardPath(context);
		if (FileServerPort > 0 && ServerPort > 0 && "".equals(ServerName) == false
				&& "".equals(FileServerName) == false) {
			ConfigStatus = true;
		} else {
			ConfigStatus = false;
		}
	}

	/**
	 * 加载Sd卡路径信息
	 * 
	 * @param context
	 *            Context对象
	 */
	private static void LoadSdcardPath(Context context) {
		if (sdcardStatus()) {
			File sdCardDir = Environment.getExternalStorageDirectory();
			Sdcard = sdCardDir.toString() + "/" + context.getResources().getString(R.string.sdcard_path) + "/";
			SdcardDownload = Sdcard + context.getResources().getString(R.string.sdcard_path_download) + "/";
			SdcardLog = Sdcard + context.getResources().getString(R.string.sdcard_path_log) + "/";
			File sdf = new File(Sdcard);
			if (sdf.exists()) {
				if (sdf.isFile()) {
					Sdcard = SdcardDownload = SdcardLog = "";
					return;
				}
			} else {
				if (sdf.mkdir() == false) {
					Sdcard = SdcardDownload = SdcardLog = "";
					return;
				}
			}
			sdf = new File(SdcardDownload);
			if (sdf.exists()) {
				if (sdf.isFile()) {
					SdcardDownload = "";
				}
			} else {
				if (sdf.mkdir() == false) {
					SdcardDownload = "";
				}
			}
			sdf = new File(SdcardLog);
			if (sdf.exists()) {
				if (sdf.isFile()) {
					SdcardLog = "";
				}
			} else {
				if (sdf.mkdir() == false) {
					SdcardLog = "";
				}
			}

		}
	}

	/**
	 * 是否有读取内存卡的权限
	 * 
	 * @return 返回内存卡读取权限
	 */
	public static boolean sdcardStatus() {
		return Environment.getExternalStorageState().equals(Environment.MEDIA_MOUNTED);
	}

	/**
	 * 读取SD卡目录
	 * 
	 * @return SD卡目录
	 */
	public static String GetSdcardPath() {
		return Sdcard;
	}

	/**
	 * 获取下载路径
	 * 
	 * @return 下载路径
	 */
	public static String GetDownloadPath() {
		return SdcardDownload;
	}

	/**
	 * 获取日志文件夹
	 * 
	 * @return 日志文件夹
	 */
	public static String GetLogPath() {
		return SdcardLog;
	}

	/**
	 * 加载服务器配置
	 */
	public static void LoadSeverConfig() {
		HashMap<String, String> map = ReadConfig(GetSeverConfigPath());
		if (map.containsKey("ServerName"))
			ServerName = map.get("ServerName");
		if (map.containsKey("ServerPort")) {
			ServerPort = Integer.parseInt(map.get("ServerPort"));
			if (ServerPort < 0)
				ServerPort = 0;
		}
		if (map.containsKey("FileServerName"))
			FileServerName = map.get("FileServerName");
		if (map.containsKey("FileServerPort")) {
			FileServerPort = Integer.parseInt(map.get("FileServerPort"));
			if (FileServerPort < 0)
				FileServerPort = 0;
		}
	}

	/**
	 * 加载登陆配置
	 */
	public static void LoadLoginConfig() {
		HashMap<String, String> map = ReadConfig(GetLoginConfigPath());
		if (map.containsKey("User")) {
			User = map.get("User");
		}
		if (map.containsKey("Password")) {
			Password = map.get("Password");
		}
		if (map.containsKey("SaveInfo")) {
			SaveLoginInfo = Boolean.parseBoolean(map.get("SaveInfo"));
		}
		if (map.containsKey("ForceLogin")) {
			ForceLogin = Boolean.parseBoolean(map.get("ForceLogin"));
		}
	}

	/**
	 * 写入登陆配置
	 */
	public static void WriteLoginConfig() {
		try {
			if (SaveLoginInfo) {
				FileAction.writeFile(RcConfig.GetLoginConfigPath(), "User\t" + User + "\nPassword\t" + Password
						+ "\nSaveInfo\t" + SaveLoginInfo + "\nForceLogin\t" + ForceLogin);
			} else {
				FileAction.writeFile(RcConfig.GetLoginConfigPath(), "SaveInfo\t" + SaveLoginInfo);
			}
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	/**
	 * 设置登录信息
	 * 
	 * @param user
	 *            用户名
	 * @param password
	 *            密码
	 * @param save
	 *            是否保存信息
	 * @param force
	 *            是否强制登陆
	 */
	public static void SetLoginConfig(String user, String password, boolean save, boolean force) {
		User = user;
		Password = password;
		SaveLoginInfo = save;
		ForceLogin = force;
	}

	/**
	 * 获取本地IP地址
	 * 
	 * @return 失败返回null
	 */
	private static String getLocalIpAddress() {
		try {
			for (Enumeration<NetworkInterface> en = NetworkInterface.getNetworkInterfaces(); en.hasMoreElements();) {
				NetworkInterface intf = en.nextElement();
				for (Enumeration<InetAddress> enumIpAddr = intf.getInetAddresses(); enumIpAddr.hasMoreElements();) {
					InetAddress inetAddress = enumIpAddr.nextElement();
					if (!inetAddress.isLoopbackAddress()) {
						return inetAddress.getHostAddress().toString();
					}
				}
			}
		} catch (SocketException ex) {
			RcDebug.e("debug", ex.toString());
		}
		return null;
	}

	/**
	 * 读取配置文件为一个hash表
	 * 
	 * @param path
	 *            配置文件路径
	 * @return hasl表
	 */
	public static HashMap<String, String> ReadConfig(String path) {
		HashMap<String, String> map = new HashMap<String, String>();

		String c;
		try {
			c = FileAction.readFile(path);
		} catch (IOException e) {
			e.printStackTrace();
			return map;
		}
		for (String str : c.split("\n")) {
			String[] in = str.split("\t");
			if (in.length != 2)
				continue;
			in[0] = in[0].trim();
			in[1] = in[1].trim();
			if (in[0].length() > 0 && in[1].length() > 0)
				map.put(in[0], in[1]);
		}
		return map;
	}

	/**
	 * 检测网络是否已连接
	 * 
	 * @param context
	 *            Context对象
	 * @return 网络连接状态
	 */
	public static boolean NetConnected(Context context) {
		ConnectivityManager cm = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
		if (cm != null) {
			NetworkInfo[] info = cm.getAllNetworkInfo();
			if (info != null) {
				for (int i = 0; i < info.length; i++) {
					if (info[i].getState() == NetworkInfo.State.CONNECTED) {
						return true;
					}
				}
			}
		}
		return false;

	}

	/**
	 * 获取IP地址
	 * 
	 * @return IP地址
	 */
	public static String GetIpAddress() {
		return LocalIpAddress;
	}

	/**
	 * 获取服务器配置文件路径
	 * 
	 * @return 文件路径
	 */
	public static String GetSeverConfigPath() {
		return FilePath + "/server.conf";
	}

	/**
	 * 获取登录信息配置文件路径
	 * 
	 * @return 文件路径
	 */
	public static String GetLoginConfigPath() {
		return CachePath + "/login.conf";
	}

	/**
	 * 获取服务器名称
	 * 
	 * @return IP或域名
	 */
	public static String GetServerName() {
		return ServerName;
	}

	/**
	 * 获取文件传输服务器路径
	 * 
	 * @return IP或域名
	 */
	public static String GetFileServerName() {
		return FileServerName;
	}

	/**
	 * 获取服务器端口
	 * 
	 * @return 端口号,默认2001
	 */
	public static int GetSevrePort() {
		return ServerPort;
	}

	/**
	 * 获取文件传输端口
	 * 
	 * @return 端口号，默认2002
	 */
	public static int GetFileSevrePort() {
		return FileServerPort;
	}

	/**
	 * 获取是否保存登陆配置
	 * 
	 * @return 保存信息的状态
	 */
	public static boolean GetSaveLoginInfo() {
		return SaveLoginInfo;
	}

	/**
	 * 获取是否强制登陆
	 * 
	 * @return 强制登陆的状态
	 */
	public static boolean GetForceLogin() {
		return ForceLogin;
	}

	/**
	 * 获取用户名
	 * 
	 * @return 登陆用户名
	 */
	public static String GetUser() {
		return User;
	}

	/**
	 * 获取登录的密码
	 * 
	 * @return 登录密码
	 */
	public static String GetPassword() {
		return Password;
	}

}
