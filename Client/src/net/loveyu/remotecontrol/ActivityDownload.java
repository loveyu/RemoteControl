package net.loveyu.remotecontrol;

import java.io.File;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import android.annotation.*;
import android.app.Activity;
import android.graphics.Color;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.ProgressBar;
import android.widget.TabHost;
import android.widget.TextView;

/**
 * 文件下载页面
 * 
 * @author loveyu
 */
public class ActivityDownload extends Activity {
	/**
	 * 下载中的任务列表
	 */
	private static List<HashMap<String, String>> downloading = new ArrayList<HashMap<String, String>>();

	/**
	 * 已下载的任务列表
	 */
	private static List<HashMap<String, String>> downloaded = new ArrayList<HashMap<String, String>>();

	/**
	 * 消息处理对象
	 */
	private Handler handler;

	/**
	 * 单例模式对象实例
	 */
	private static ActivityDownload instance = null;

	/**
	 * 下载中消息处理标记
	 */
	private static final int UpdateDownloading = 1;

	/**
	 * 已下载消息标记
	 */
	private static final int UpdateDownloaded = 2;

	/**
	 * 判断页面是否在后台运行
	 */
	private static boolean isPause = false;

	/**
	 * 添加一个文件到下载列表，如果文件存在则覆盖
	 * 
	 * @param file
	 *            服务器文件路径
	 * @param size
	 *            文件大小
	 */
	public static void AddDownloading(String file, long size) {
		synchronized (downloading) {
			for (int i = 0; i < downloading.size(); i++) {
				if (downloading.get(i).get("Name").equals(file)) {
					downloading.remove(i);
				}
			}
			HashMap<String, String> map = new HashMap<String, String>();
			map.put("BaseName", RcFile.BaseName(file));
			map.put("Name", file);
			map.put("Size", RcFile.ShowSize(size, 2));
			map.put("Percentage", "0");
			map.put("IsError", "false");
			downloading.add(map);
		}
	}

	/**
	 * 当页面暂停时设置一个标记
	 */
	@Override
	protected void onPause() {
		super.onPause();
		isPause = true;
	}

	/**
	 * 当页面恢复时设置标记
	 */
	@Override
	protected void onResume() {
		super.onResume();
		isPause = false;
	}

	/**
	 * 更新下载进度条
	 * 
	 * @param file
	 *            服务器文件路径
	 * @param p
	 *            1000分的进度比
	 * @param size
	 *            文件大小，多余保留参数
	 */
	public static void UpdateDownloading(String file, int p, long size) {
		synchronized (downloading) {
			for (int i = 0; i < downloading.size(); i++) {
				if (downloading.get(i).get("Name").equals(file)) {
					HashMap<String, String> map = downloading.get(i);
					map.put("Percentage", p + "");
					downloading.set(i, map);
				}
			}
		}
		if (instance != null && isPause == false) {
			Message msg = instance.handler.obtainMessage(UpdateDownloading);
			instance.handler.sendMessage(msg);
		}
	}

	/**
	 * 更新下载列表为下载错误
	 * 
	 * @param file
	 *            下载文件的服务器路径
	 */
	public static void UpdateDownloadingIsError(String file) {
		synchronized (downloading) {
			for (int i = 0; i < downloading.size(); i++) {
				if (downloading.get(i).get("Name").equals(file)) {
					HashMap<String, String> map = downloading.get(i);
					map.put("IsError", "true");
					downloading.set(i, map);
				}
			}
		}
		if (instance != null && isPause == false) {
			Message msg = instance.handler.obtainMessage(UpdateDownloading);
			instance.handler.sendMessage(msg);
		}
	}

	/**
	 * 将下载项从下载列表中移除
	 * 
	 * @param file
	 *            服务器文件路径
	 */
	public static void RemoveDownloading(String file) {
		synchronized (downloading) {
			for (int i = 0; i < downloading.size(); i++) {
				if (downloading.get(i).get("Name").equals(file)) {
					downloading.remove(i);
					break;
				}
			}
		}
		if (instance != null && isPause == false) {
			Message msg = instance.handler.obtainMessage(UpdateDownloading);
			instance.handler.sendMessage(msg);
		}
	}

	/**
	 * 将文件添加到已下载列表中
	 * 
	 * @param file
	 *            服务器文件路径
	 * @param size
	 *            文件大小
	 */
	public static void AddDownloaded(String file, long size) {
		synchronized (downloaded) {
			for (int i = 0; i < downloaded.size(); i++) {
				if (downloaded.get(i).get("Name").equals(file)) {
					downloaded.remove(i);
					break;
				}
			}
			HashMap<String, String> map = new HashMap<String, String>();
			map.put("BaseName", RcFile.BaseName(file));
			map.put("Name", file);
			map.put("Size", RcFile.ShowSize(size, 2));
			downloaded.add(map);
		}
		if (instance != null && isPause == false) {
			Message msg = instance.handler.obtainMessage(UpdateDownloaded);
			instance.handler.sendMessage(msg);
		}
	}

	/**
	 * 下载列表桥接器
	 */
	private DownloadingAdapter diAdapter = new DownloadingAdapter();

	/**
	 * 已下载列表桥接器
	 */
	private DownloadedAdapter deAdapter = new DownloadedAdapter();

	/**
	 * 创建下载界面及消息事件与列表桥接器
	 */
	@SuppressLint("HandlerLeak")
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		this.requestWindowFeature(Window.FEATURE_NO_TITLE);
		setContentView(R.layout.activity_download);
		((ListView) findViewById(R.id.downloading)).setAdapter(diAdapter);
		((ListView) findViewById(R.id.downloaded)).setAdapter(deAdapter);
		instance = this;
		TabHost mTabHost = (TabHost) findViewById(R.id.download_tabhost);
		mTabHost.setup();
		mTabHost.addTab(mTabHost.newTabSpec("downloading").setIndicator(getResources().getString(R.string.downloading))
				.setContent(R.id.downloading));
		mTabHost.addTab(mTabHost.newTabSpec("downloaded").setIndicator(getResources().getString(R.string.downloaded))
				.setContent(R.id.downloaded));
		mTabHost.setCurrentTab(0);

		handler = new Handler() {
			@Override
			public void handleMessage(Message msg) {
				// TODO Auto-generated method stub
				super.handleMessage(msg);
				switch (msg.what) {
				case UpdateDownloading:
					diAdapter.notifyDataSetChanged();
					break;
				case UpdateDownloaded:
					deAdapter.notifyDataSetChanged();
					break;
				}
			}
		};
	}

	/**
	 * 下载中文件列表桥接器
	 * 
	 * @author loveyu
	 * 
	 */
	class DownloadingAdapter extends BaseAdapter {
		/**
		 * 返回下载中列表的数量
		 */
		@Override
		public int getCount() {
			return downloading.size();
		}

		/**
		 * 获取下载中列表中的某一个项目，返回空值
		 */
		@Override
		public Object getItem(int position) {
			return null;
		}

		/**
		 * 获取下载列表的某一个对象
		 */
		@Override
		public View getView(int position, View convertView, ViewGroup parent) {
			View view = View.inflate(ActivityDownload.this, R.layout.activity_downloading_list_item, null);
			HashMap<String, String> map = downloading.get(position);
			((TextView) view.findViewById(R.id.name)).setText(map.get("BaseName"));
			if (Boolean.parseBoolean(map.get("IsError"))) {
				((TextView) view.findViewById(R.id.name)).setTextColor(Color.RED);
			}
			((TextView) view.findViewById(R.id.size)).setText(map.get("Size"));
			((ProgressBar) view.findViewById(R.id.progressBar)).setMax(1000);
			int p = 0;
			try {
				p = Integer.parseInt(map.get("Percentage"));
			} catch (Exception ex) {
			}
			((ProgressBar) view.findViewById(R.id.progressBar)).setProgress(p);
			((TextView) view.findViewById(R.id.percentage)).setText(String.format("%.1f%%", (float) p / 10));
			return view;
		}

		/**
		 * 获取某一列表的ID
		 */
		@Override
		public long getItemId(int position) {
			return 0;
		}
	}

	/**
	 * 已下载文件桥接器
	 * 
	 * @author loveyu
	 * 
	 */
	class DownloadedAdapter extends BaseAdapter {
		/**
		 * 返回已下载的文件数量
		 */
		@Override
		public int getCount() {
			return downloaded.size();
		}

		/**
		 * 返回已下载的对应项目
		 */
		@Override
		public Object getItem(int position) {
			return null;
		}

		/**
		 * 返回已下载的对应view
		 */
		@Override
		public View getView(int position, View convertView, ViewGroup parent) {
			View view = View.inflate(ActivityDownload.this, R.layout.activity_downloaded_list_item, null);
			HashMap<String, String> map = downloaded.get(position);
			((TextView) view.findViewById(R.id.BaseName)).setText(map.get("BaseName"));
			((TextView) view.findViewById(R.id.SeverPath)).setText(RcFile.ParentName(map.get("Name")));
			((TextView) view.findViewById(R.id.size)).setText(map.get("Size"));
			((Button) view.findViewById(R.id.open_file)).setOnClickListener(new OpenFileListener(map.get("Name")));
			return view;
		}

		/**
		 * 返回对应元素ID
		 */
		@Override
		public long getItemId(int position) {
			return 0;
		}

		/**
		 * 从系统打开文件监听类
		 * 
		 * @author loveyu
		 * 
		 */
		class OpenFileListener implements Button.OnClickListener {
			String path;

			/**
			 * 设置要打开的文件路径
			 * 
			 * @param path
			 *            对应的文件路径
			 */
			public OpenFileListener(String path) {
				this.path = RcConfig.GetDownloadPath() + RcFile.BaseName(path);
			}

			/**
			 * 文件打开监听事件
			 */
			@Override
			public void onClick(View v) {
				startActivity(FileAction.openFile(new File(path)));
			}
		}
	}
}
