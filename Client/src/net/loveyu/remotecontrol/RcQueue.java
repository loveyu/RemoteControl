package net.loveyu.remotecontrol;

import java.util.HashMap;

import android.text.format.Time;

/**
 * 消息队列类
 * 
 * @author loveyu
 * 
 */
public class RcQueue {
	/**
	 * 队列单例
	 */
	private static RcQueue instance = new RcQueue();
	/**
	 * 消息队列列表
	 */
	private HashMap<String, RcSendMsg> queue = new HashMap<String, RcSendMsg>();

	/**
	 * 私有构造函数
	 */
	private RcQueue() {
	}

	/**
	 * 处理线程
	 */
	private Thread thread;
	/**
	 * 线程运行状态
	 */
	private volatile boolean threadRuning = true;
	/**
	 * 最大尝试次数
	 */
	private final int maxTry = 5;

	/**
	 * 获取队列对象实例
	 * 
	 * @return 实例
	 */
	public static RcQueue Get() {
		return instance;
	}

	/**
	 * 启动队列处理
	 */
	public void Start() {
		threadRuning = true;
		thread = new Thread(new Runnable() {

			@Override
			public void run() {
				ProcessThread();
			}
		});
		thread.start();
		RcDebug.v("debug", "queue start");
	}

	/**
	 * 停止队列处理
	 */
	public void Stop() {
		threadRuning = false;
		synchronized (queue) {
			queue.clear();
		}
	}

	/**
	 * 队列处理线程
	 */
	private void ProcessThread() {
		while (threadRuning) {
			try {
				Thread.sleep(2000);
			} catch (InterruptedException e) {
			}
			String[] ls = GetQueueHead();
			if (ls.length == 0) {
				continue;
			}
			Time time = new Time();
			time.setToNow();
			Time add = new Time();
			add.second = 2;
			time.after(add);
			for (String hd : ls) {
				boolean f = false;
				synchronized (queue) {
					f = queue.containsKey(hd);
				}
				if (f) {
					RcSendMsg msg;
					synchronized (queue) {
						msg = queue.get(hd);
					}
					if (Time.compare(time, msg.time) < 0) {
						// RcDebug.v("debug", time.toString() + ":::" +
						// msg.time.toString());
						continue;
					}
					if (msg.tryNum < maxTry) {
						RcNet.Get().Send(msg);
						RcDebug.v("debug", "Try Send:" + msg.IdString);
					} else {
						Remove(msg.IdString);
					}
				}
			}
		}
	}

	/**
	 * 获取队列的头列表
	 * 
	 * @return 字符串数组
	 */
	private String[] GetQueueHead() {
		String[] rt;
		synchronized (queue) {
			rt = new String[queue.keySet().size()];
			int i = 0;
			for (Object ob : queue.keySet().toArray()) {
				rt[i++] = (String) ob;
			}
		}
		return rt;
	}

	/**
	 * 统计列表
	 * 
	 * @return 数量
	 */
	public int Count() {
		synchronized (queue) {
			return queue.size();
		}
	}

	/**
	 * 添加一个已发送的消息到队列
	 * 
	 * @param smsg
	 *            发送的消息
	 */
	public void Add(RcSendMsg smsg) {
		synchronized (queue) {
			queue.put(smsg.IdString, smsg);
		}
	}

	/**
	 * 更新一个重复发送的消息
	 * 
	 * @param smsg
	 *            消息
	 */
	public void Update(RcSendMsg smsg) {
		synchronized (queue) {
			if (queue.containsKey(smsg.IdString)) {
				++smsg.tryNum;
				smsg.time.setToNow();
				queue.put(smsg.IdString, smsg);
			}
		}
		RcDebug.v("debug", "queue update:" + smsg);
	}

	/**
	 * 根据ID移除消息
	 * 
	 * @param id
	 *            消息ID
	 */
	public void Remove(String id) {
		synchronized (queue) {
			if (queue.containsKey(id)) {
				queue.remove(id);
			}
		}
		RcDebug.v("debug", "queue remove:" + id);
	}
}
