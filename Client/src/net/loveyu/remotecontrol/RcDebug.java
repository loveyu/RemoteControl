package net.loveyu.remotecontrol;

import java.io.IOException;

import android.content.Context;
import android.os.Message;
import android.text.format.Time;
import android.util.Log;
import android.widget.Toast;

/**
 * 记录提示及调试类
 * 
 * @author loveyu
 * 
 */
public class RcDebug {
	public static void v(String type, String msg) {
		Log.v(type, msg);
	}

	public static void d(String type, String msg) {
		Log.d(type, msg);
	}

	public static void e(String type, String msg) {
		Log.e(type, msg);
	}

	public static void w(String type, String msg) {
		Log.w(type, msg);
	}

	/**
	 * 根据上下文输出提示
	 * 
	 * @param context
	 *            上下文
	 * @param str
	 *            提示字符串
	 */
	public static void N(Context context, String str) {
		v("Notice", str);
		Toast.makeText(context, str, Toast.LENGTH_SHORT).show();
	}

	/**
	 * 记录信息到文件日子中
	 * 
	 * @param str
	 *            内容
	 */
	public static void Log(String str) {
		String path = RcConfig.GetLogPath();
		if (path.length() == 0)
			return;
		try {
			String time = LogTime();
			String[] logName = time.split(" ");
			if (logName.length > 0)
				FileAction.addFile(path + logName[0] + ".log", time + ":\r\n" + str + "\r\n");
		} catch (IOException e) {
		}
	}

	/**
	 * 显示一个线程中的提示
	 * 
	 * @param str
	 *            提示内容
	 */
	public static void tN(String str) {
		if (ActivityMain.instance != null) {
			Message msg = new Message();
			msg.what = ActivityMain.MsgNotice;
			msg.obj = str;
			ActivityMain.instance.messageHandler.sendMessage(msg);
		}
	}

	/**
	 * 获取一个记录时间
	 * 
	 * @return 格式化后的时间
	 */
	private static String LogTime() {
		Time time = new Time();
		time.setToNow();
		return time.format("%Y-%m-%d %H:%M:%S");
	}

}
