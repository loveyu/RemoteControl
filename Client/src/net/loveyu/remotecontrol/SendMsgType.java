package net.loveyu.remotecontrol;

/**
 * 发送消息的类型
 * 
 * @author loveyu
 * 
 */
public enum SendMsgType {
	/**
	 * 命令消息
	 */
	COMMAND, // 01
	/**
	 * 提示消息
	 */
	NOTICE, // 02
	/**
	 * 文件消息
	 */
	FILE, // 03
	/**
	 * 警告消息
	 */
	WARNING, // 04
	/**
	 * 终端消息
	 */
	TERMINAL, // 05
	/**
	 * 功能消息
	 */
	FUNCTION, // 06
	/**
	 * 回执消息
	 */
	CALLBACK, // 07
	/**
	 * 登录消息
	 */
	LOGIN, // 08
	/**
	 * 登出消息
	 */
	LOGOUT, // 09
	/**
	 * 截图消息
	 */
	SCREENSHOT, // 10
	/**
	 * 任务管理消息
	 */
	TASK, // 11
}
