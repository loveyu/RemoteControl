package net.loveyu.remotecontrol;

/**
 * 消息类型
 * 
 * @author loveyu
 * 
 */
public enum MessageType {
	/**
	 * 回执消息
	 */
	Callback, // 01

	/**
	 * 常规文本提示
	 */
	Text, // 02

	/**
	 * 登录状态消息
	 */
	Login, // 03

	/**
	 * 执行错误消息
	 */
	RunError, // 04

	/**
	 * 文件消息
	 */
	File, // 05

	/**
	 * 截图消息
	 */
	Picture, // 06

	/**
	 * 终端消息
	 */
	Terminal, // 07

	/**
	 * 终端错误消息
	 */
	TerminalError, // 08

	/**
	 * 任务消息
	 */
	Task, // 09

	/**
	 * 未知消息
	 */
	Unknow
}
