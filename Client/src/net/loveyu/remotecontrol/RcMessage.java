package net.loveyu.remotecontrol;

import java.net.DatagramPacket;

/**
 * 消息处理生成类
 * 
 * @author loveyu
 * 
 */
public class RcMessage {

	/**
	 * 消息类型
	 */
	public MessageType type;
	/**
	 * 消息ID
	 */
	public String id;
	/**
	 * 消息状态
	 */
	public boolean Status = false;
	/**
	 * 消息内容
	 */
	public String content;

	/**
	 * 根据收到的数据包来构造消息数据
	 * 
	 * @param pack
	 *            原始数据包
	 */
	public RcMessage(DatagramPacket pack) {
		String all = new String(GetBytes(pack));
		try {
			int index = all.indexOf('\n');
			String head = all.substring(0, index);
			String[] info = head.split("\t");
			if (info.length < 3)
				return;
			id = info[0] + "\t" + info[1];
			type = getType(info[2]);
			content = all.substring(index + 1);
			Status = true;
		} catch (Exception e) {
		}
	}

	/**
	 * 将网络包转为字节数组
	 * 
	 * @param pack
	 *            原始网络包
	 * @return
	 */
	private byte[] GetBytes(DatagramPacket pack) {
		byte[] rt = new byte[pack.getLength()];
		System.arraycopy(pack.getData(), 0, rt, 0, pack.getLength());
		return rt;
	}

	/**
	 * 获取消息类型
	 * @param type 消息类型字符串
	 * @return 消息类型
	 */
	private MessageType getType(String type) {
		if ("01".equals(type)) {
			return MessageType.Callback;
		}
		if ("02".equals(type)) {
			return MessageType.Text;
		}
		if ("03".equals(type)) {
			return MessageType.Login;
		}
		if ("04".equals(type)) {
			return MessageType.RunError;
		}
		if ("05".equals(type)) {
			return MessageType.File;
		}
		if ("06".equals(type)) {
			return MessageType.Picture;
		}
		if ("07".equals(type)) {
			return MessageType.Terminal;
		}
		if ("08".equals(type)) {
			return MessageType.TerminalError;
		}
		if ("09".equals(type)) {
			return MessageType.Task;
		}
		return MessageType.Unknow;
	}
}
