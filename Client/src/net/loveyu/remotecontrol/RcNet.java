package net.loveyu.remotecontrol;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;

/**
 * 网络操作类
 * 
 * @author loveyu
 * 
 */
public class RcNet {
	/**
	 * 网络单例模式
	 */
	private static RcNet instance = new RcNet();

	/**
	 * 私有构造函数
	 */
	private RcNet() {

	}

	/**
	 * 获取实例
	 * 
	 * @return 网络实例
	 */
	public static RcNet Get() {
		return instance;
	}

	/**
	 * Udp客户端
	 */
	private DatagramSocket udp;
	/**
	 * 远程IP地址
	 */
	private InetAddress ip;
	/**
	 * 远程端口
	 */
	private int port;
	/**
	 * 数据接收线程
	 */
	private Thread receiveThread;
	/**
	 * 线程运行提示
	 */
	public boolean Runing = false;

	/**
	 * 启动网络
	 * 
	 * @throws Exception
	 *             网络启动的任何异常
	 */
	public void Start() throws Exception {
		if (RcConfig.Status() == false) {
			throw new Exception("Config status is false");
		}
		udp = new DatagramSocket();
		ip = InetAddress.getByName(RcConfig.GetServerName());
		port = RcConfig.GetSevrePort();
		udp.connect(ip, port);
		receiveThread = new Thread(new Runnable() {

			@Override
			public void run() {
				ReceiveThread();
			}
		});
		Runing = true;
		receiveThread.start();
		RcQueue.Get().Start();
	}

	/**
	 * 发送数据包
	 * 
	 * @param msg
	 *            发送数据对象
	 */
	public void Send(RcSendMsg msg) {
		try {
			if (udp.isClosed()) {
				RcDebug.tN("Net is stop! Please re-login.");
				return;
			}
			if (msg.type != SendMsgType.CALLBACK) {
				if (msg.tryNum == 0) {
					RcQueue.Get().Add(msg);
				} else {
					RcQueue.Get().Update(msg);
				}
			}
			udp.send(msg.pack);
		} catch (IOException e) {
		}
	}

	/**
	 * 停止网络服务运行
	 */
	public void Stop() {
		Runing = false;
		udp.close();
		RcQueue.Get().Stop();
	}

	/**
	 * 网络接收线程
	 */
	private void ReceiveThread() {
		try {
			while (Runing) {
				byte[] buf = new byte[5120];
				DatagramPacket pack = new DatagramPacket(buf, buf.length);
				udp.receive(pack);
				RcNetRecvieThread runThread = new RcNetRecvieThread(pack);
				new Thread(runThread).start();
			}
		} catch (Exception e) {
			RcDebug.tN("Get a Network Ex:" + e.getMessage());
		}
	}
}

/**
 * 网络接收线程的消息处理线程
 * 
 * @author loveyu
 * 
 */
class RcNetRecvieThread implements Runnable {
	/**
	 * 收到的消息
	 */
	DatagramPacket pack;

	/**
	 * 构造一个运行对象
	 * 
	 * @param pack
	 *            收到的数据包
	 */
	public RcNetRecvieThread(DatagramPacket pack) {
		this.pack = pack;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see java.lang.Runnable#run()
	 */
	@Override
	public void run() {
		new RcProcess().Run(pack);
	}
}