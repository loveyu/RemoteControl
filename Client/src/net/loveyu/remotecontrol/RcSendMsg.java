package net.loveyu.remotecontrol;

import java.net.DatagramPacket;
import android.text.format.Time;

/**
 * 发送消息生成类
 * 
 * @author loveyu
 * 
 */
public class RcSendMsg {
	/**
	 * 待发送的数据消息
	 */
	public DatagramPacket pack;
	/**
	 * 发送的消息类型
	 */
	public SendMsgType type;
	/**
	 * 存放消息的临时空间
	 */
	public byte[] buf;
	/**
	 * 消息ID
	 */
	public int id;
	/**
	 * ID转换的字符串
	 */
	public String IdString;
	/**
	 * 递增的消息ID
	 */
	public static int ID = 0;
	/**
	 * 消息的尝试重发次数
	 */
	public int tryNum = 0;
	/**
	 * 消息上次发送时间
	 */
	public Time time;

	/**
	 * 创建消息
	 */
	private RcSendMsg() {
		time = new Time();
		time.setToNow();
	}

	/**
	 * 创建登录消息
	 * 
	 * @return
	 */
	public static RcSendMsg createLogin() {
		RcSendMsg rt = new RcSendMsg();
		rt.buf = new String(RcConfig.GetUser() + "\n" + RcConfig.GetPassword() + "\n" + RcConfig.GetForceLogin())
				.getBytes();
		rt.type = SendMsgType.LOGIN;
		rt.make();
		return rt;
	}

	/**
	 * 创建截图消息
	 * 
	 * @param width
	 *            缩略图宽度
	 * @return
	 */
	public static RcSendMsg createScreenShot(String width) {
		RcSendMsg rt = new RcSendMsg();
		rt.buf = width.getBytes();
		rt.type = SendMsgType.SCREENSHOT;
		rt.make();
		return rt;
	}

	/**
	 * 创建终端消息
	 * 
	 * @param cmd
	 *            命令字符串
	 * @return
	 */
	public static RcSendMsg createTerminal(String cmd) {
		RcSendMsg rt = new RcSendMsg();
		rt.buf = cmd.getBytes();
		rt.type = SendMsgType.TERMINAL;
		rt.make();
		return rt;
	}

	/**
	 * 创建登出消息
	 * 
	 * @return
	 */
	public static RcSendMsg createLogout() {
		RcSendMsg rt = new RcSendMsg();
		rt.buf = "logout".getBytes();
		rt.type = SendMsgType.LOGOUT;
		rt.make();
		return rt;
	}

	/**
	 * 创建命令消息
	 * 
	 * @param cmd
	 *            命令字符串
	 * @return
	 */
	public static RcSendMsg createCommand(String cmd) {
		RcSendMsg rt = new RcSendMsg();
		rt.buf = cmd.getBytes();
		rt.type = SendMsgType.COMMAND;
		rt.make();
		return rt;
	}

	/**
	 * 创建任务消息
	 * 
	 * @param cmd
	 *            任务命令
	 * @return
	 */
	public static RcSendMsg createTask(String cmd) {
		RcSendMsg rt = new RcSendMsg();
		rt.buf = cmd.getBytes();
		rt.type = SendMsgType.TASK;
		rt.make();
		return rt;
	}

	/**
	 * 创建提示消息
	 * 
	 * @param str
	 *            提示字符串
	 * @return
	 */
	public static RcSendMsg createNotice(String str) {
		RcSendMsg rt = new RcSendMsg();
		rt.buf = str.getBytes();
		rt.type = SendMsgType.NOTICE;
		rt.make();
		return rt;
	}

	/**
	 * 创建警告消息
	 * 
	 * @param str
	 *            警告字符串
	 * @return
	 */
	public static RcSendMsg createWarning(String str) {
		RcSendMsg rt = new RcSendMsg();
		rt.buf = str.getBytes();
		rt.type = SendMsgType.WARNING;
		rt.make();
		return rt;
	}

	/**
	 * 创建功能消息
	 * 
	 * @param func
	 *            功能名称
	 * @return
	 */
	public static RcSendMsg createFunction(String func) {
		RcSendMsg rt = new RcSendMsg();
		rt.buf = func.getBytes();
		rt.type = SendMsgType.FUNCTION;
		rt.make();
		return rt;
	}

	/**
	 * 创建回执消息
	 * 
	 * @param id
	 *            消息ID
	 * @return
	 */
	public static RcSendMsg createCallback(String id) {
		RcSendMsg rt = new RcSendMsg();
		rt.buf = new String(id).getBytes();
		rt.type = SendMsgType.CALLBACK;
		rt.make();
		return rt;
	}

	/**
	 * 创建文件消息
	 * 
	 * @param msg
	 *            消息内容
	 * @return
	 */
	public static RcSendMsg createFile(String msg) {
		RcSendMsg rt = new RcSendMsg();
		rt.buf = msg.getBytes();
		rt.type = SendMsgType.FILE;
		rt.make();
		return rt;
	}

	/**
	 * 生成消息内容
	 */
	public void make() {
		id = ++ID;
		IdString = String.format("%010d", id);
		String t = IdString + getTypeNumber(type);
		// RcDebug.v("debug", "Make String:" + t);
		byte[] n = arraycat(t.getBytes(), buf);
		// RcDebug.v("debug", "new byte[]:" + n.length);
		pack = new DatagramPacket(n, n.length);
		RcDebug.Log("Make msg:" + t + buf);
	}

	/**
	 * 连接两个字节数组
	 * 
	 * @param buf1
	 *            数组1
	 * @param buf2
	 *            数组2
	 * @return 新的数组12
	 */
	private byte[] arraycat(byte[] buf1, byte[] buf2) {
		byte[] bufret = null;
		int len1 = 0;
		int len2 = 0;
		if (buf1 != null)
			len1 = buf1.length;
		if (buf2 != null)
			len2 = buf2.length;
		if (len1 + len2 > 0)
			bufret = new byte[len1 + len2];
		if (len1 > 0)
			System.arraycopy(buf1, 0, bufret, 0, len1);
		if (len2 > 0)
			System.arraycopy(buf2, 0, bufret, len1, len2);
		return bufret;
	}

	/**
	 * 根据消息类型转换为字符串
	 * 
	 * @param type
	 *            消息类型
	 * @return 用于连接的字符串
	 */
	private String getTypeNumber(SendMsgType type) {
		switch (type) {
		case COMMAND:
			return "01";
		case NOTICE:
			return "02";
		case FILE:
			return "03";
		case WARNING:
			return "04";
		case TERMINAL:
			return "05";
		case FUNCTION:
			return "06";
		case CALLBACK:
			return "07";
		case LOGIN:
			return "08";
		case LOGOUT:
			return "09";
		case SCREENSHOT:
			return "10";
		case TASK:
			return "11";
		default:
			return "00";
		}
	}
}
