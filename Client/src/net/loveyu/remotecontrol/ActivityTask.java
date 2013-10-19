package net.loveyu.remotecontrol;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.DialogInterface.OnClickListener;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;

/**
 * 任务管理页
 * 
 * @author loveyu
 * 
 */
public class ActivityTask extends Activity {
	/**
	 * 任务列表桥接器
	 */
	private static TaskAdapter adapter;

	/**
	 * 任务列表
	 */
	private static List<HashMap<String, String>> list = new ArrayList<HashMap<String, String>>();

	/**
	 * 临时存储的任务列表
	 */
	private static ArrayList<HashMap<String, String>> data = new ArrayList<HashMap<String, String>>();

	/**
	 * 消息处理实例
	 */
	private Handler handler;

	/**
	 * 单例对象
	 */
	private static ActivityTask instance = null;

	/**
	 * 消息值
	 */
	private static final int Update = 1;

	/**
	 * 是否后台运行
	 */
	private static boolean isPause = false;

	/**
	 * 加载布局文件并开始监听消息
	 */
	@SuppressLint("HandlerLeak")
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		this.requestWindowFeature(Window.FEATURE_NO_TITLE);
		setContentView(R.layout.activity_task);
		adapter = new TaskAdapter();
		((ListView) findViewById(R.id.task_listView)).setAdapter(adapter);
		((Button) findViewById(R.id.task_refresh)).setOnClickListener(new Button.OnClickListener() {
			@Override
			public void onClick(View v) {
				list.clear();
				RcNet.Get().Send(RcSendMsg.createTask("get"));
			}
		});
		instance = this;
		handler = new Handler() {
			@Override
			public void handleMessage(Message msg) {
				// TODO Auto-generated method stub
				super.handleMessage(msg);
				switch (msg.what) {
				case Update:
					adapter.notifyDataSetChanged();
					break;
				}
			}
		};
		if (list.isEmpty()) {
			RcNet.Get().Send(RcSendMsg.createTask("get"));
		}
	}

	/**
	 * 在线程中更新消息
	 * 
	 * @param ls
	 *            获取到的任务列表
	 * @param i
	 *            任务列表的递减id,当数据为1时表示结束
	 */
	public static void UpdateList(List<HashMap<String, String>> ls, int i) {
		synchronized (data) {
			data.addAll(ls);
			if (i == 1 && instance != null && isPause == false) {
				ls.clear();
				Collections.sort(data, new Comparator<HashMap<String, String>>() {
					public int compare(HashMap<String, String> lhs, HashMap<String, String> rhs) {
						return lhs.get("name").toLowerCase().compareTo(rhs.get("name").toLowerCase());
					};
				});
				list.addAll(data);
				data.clear();
				Message msg = new Message();
				msg.what = Update;
				instance.handler.sendMessage(msg);
			}
		}
	}

	/**
	 * 后台运行事件
	 */
	@Override
	protected void onPause() {
		super.onPause();
		isPause = true;
	}

	/**
	 * 恢复事件
	 */
	@Override
	protected void onResume() {
		super.onResume();
		isPause = false;
	}

	/**
	 * 任务列表桥接器
	 * 
	 * @author loveyu
	 * 
	 */
	class TaskAdapter extends BaseAdapter {
		/**
		 * 获取任务列表数量
		 */
		@Override
		public int getCount() {
			return list.size();
		}

		@Override
		public Object getItem(int position) {
			return null;
		}

		@Override
		public long getItemId(int position) {
			return 0;
		}

		/**
		 * 获取某一任务的视图
		 */
		@Override
		public View getView(int position, View convertView, ViewGroup parent) {
			HashMap<String, String> map = list.get(position);
			View view = View.inflate(ActivityTask.this, R.layout.activity_task_item, null);
			((TextView) view.findViewById(R.id.task_name)).setText(map.get("name"));
			((TextView) view.findViewById(R.id.task_id)).setText(map.get("id"));
			((TextView) view.findViewById(R.id.task_time)).setText(map.get("time"));
			((TextView) view.findViewById(R.id.task_memory)).setText(RcFile.ShowSize(
					Double.parseDouble(map.get("memory")), 2));
			((TextView) view.findViewById(R.id.task_path)).setText(map.get("path"));
			view.setOnLongClickListener(new TaskLongListener(position));
			return view;
		}

		/**
		 * 长时间按住人物事件监听类
		 * 
		 * @author loveyu
		 * 
		 */
		class TaskLongListener implements View.OnLongClickListener {
			/**
			 * 当前任务在任务列表中的序号
			 */
			int position;

			/**
			 * 构造器 设置任务序号
			 * 
			 * @param position
			 *            任务在列表中序号
			 */
			public TaskLongListener(int position) {
				this.position = position;
			}

			/**
			 * 长时间按住的监听事件
			 */
			@Override
			public boolean onLongClick(View v) {
				HashMap<String, String> map = list.get(position);
				View view = View.inflate(ActivityTask.this, R.layout.activity_task_item, null);
				((TextView) view.findViewById(R.id.task_name)).setText(map.get("name"));
				((TextView) view.findViewById(R.id.task_id)).setText(map.get("id"));
				((TextView) view.findViewById(R.id.task_time)).setText(map.get("time"));
				((TextView) view.findViewById(R.id.task_memory)).setText(RcFile.ShowSize(
						Double.parseDouble(map.get("memory")), 2));
				((TextView) view.findViewById(R.id.task_path)).setText(map.get("path"));
				new AlertDialog.Builder(ActivityTask.this).setTitle(getResources().getString(R.string.input_kill_task))
						.setView(view)
						.setPositiveButton(getResources().getString(R.string.ok), new MyDialogListener(map.get("id")))
						.setNegativeButton(getResources().getString(R.string.cancel), null).show();
				return false;
			}

			/**
			 * 弹出对话框的监听事件
			 * 
			 * @author loveyu
			 * 
			 */
			class MyDialogListener implements OnClickListener {
				/**
				 * 该任务在系统中对应的PID
				 */
				int pid;

				/**
				 * 构造器设置任务PID
				 * 
				 * @param pid
				 */
				public MyDialogListener(String pid) {
					this.pid = Integer.parseInt(pid);
				}

				/**
				 * 发送kill消息到服务器
				 */
				@Override
				public void onClick(DialogInterface dialog, int which) {
					RcNet.Get().Send(RcSendMsg.createTask("kill\n" + pid));
				}
			}
		}
	}
}
