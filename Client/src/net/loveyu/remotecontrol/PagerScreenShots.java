package net.loveyu.remotecontrol;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

import android.annotation.SuppressLint;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.TextView;

/**
 * 截图页面
 * 
 * @author loveyu
 * 
 */
public class PagerScreenShots extends PagerEmpty {
	/**
	 * 页面视图
	 */
	private View view;
	/**
	 * 对应的截图列表
	 */
	private static List<HashMap<String, String>> map = new ArrayList<HashMap<String, String>>();
	/**
	 * 静态访问实例
	 */
	private static PagerScreenShots instance = null;
	/**
	 * ListView桥接器
	 */
	private ScreenShotListAdapter adapter;
	/**
	 * 消息处理实例
	 */
	private Handler handler;
	/**
	 * 消息值
	 */
	private static final int UPDATE = 1;

	/**
	 * 创建页面
	 * 
	 * @param content
	 *            页面标题
	 * @return 页面实例
	 */
	public static PagerScreenShots newInstance(String content) {
		PagerScreenShots fragment = new PagerScreenShots();
		fragment.mContent = content;
		instance = fragment;
		return fragment;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see net.loveyu.remotecontrol.PagerEmpty#onCreate(android.os.Bundle)
	 */
	@SuppressLint("HandlerLeak")
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		if ((savedInstanceState != null) && savedInstanceState.containsKey("Fragment:PagerMessage")) {
			mContent = savedInstanceState.getString("Fragment:PagerMessage");
		}
		adapter = new ScreenShotListAdapter();
		handler = new Handler() {
			@Override
			public void handleMessage(Message msg) {
				// TODO Auto-generated method stub
				super.handleMessage(msg);
				switch (msg.what) {
				case UPDATE:
					adapter.notifyDataSetChanged();
					ListView lv = (ListView) view.findViewById(R.id.listView);
					lv.setSelection(lv.getAdapter().getCount() - 1);
					break;
				}
			}
		};
	}

	/**
	 * 在线程中调用此方法将数据添加到页面
	 * 
	 * @param fn
	 *            完整文件名
	 * @param fs
	 *            完整大小
	 * @param sn
	 *            缩略图文件名
	 * @param ss
	 *            缩略图大小
	 */
	public static void AddListItme(String fn, String fs, String sn, String ss) {
		HashMap<String, String> item = new HashMap<String, String>();
		item.put("FullName", fn);
		item.put("FullSize", fs);
		item.put("FullBaseName", RcFile.BaseName(fn));
		item.put("SimpleBaseName", RcFile.BaseName(sn));
		item.put("SimpleName", sn);
		item.put("SimpleSize", ss);
		map.add(item);
		if (instance != null) {
			Message msg = instance.handler.obtainMessage(UPDATE);
			instance.handler.sendMessage(msg);
		}
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see
	 * net.loveyu.remotecontrol.PagerEmpty#onCreateView(android.view.LayoutInflater
	 * , android.view.ViewGroup, android.os.Bundle)
	 */
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		view = inflater.inflate(R.layout.pager_screen_shots, container, false);
		view.findViewById(R.id.get).setOnClickListener(new Button.OnClickListener() {
			@Override
			public void onClick(View v) {
				String width = ((EditText) view.findViewById(R.id.edit_text)).getText().toString() + "";
				if ("".equals(width)) {
					width = "0";
				}
				RcNet.Get().Send(RcSendMsg.createScreenShot(width));
			}
		});
		view.findViewById(R.id.clear).setOnClickListener(new Button.OnClickListener() {
			@Override
			public void onClick(View v) {
				map.clear();
				adapter.notifyDataSetChanged();
			}
		});
		view.findViewById(R.id.refresh).setOnClickListener(new Button.OnClickListener() {
			@Override
			public void onClick(View v) {
				adapter.notifyDataSetChanged();
			}
		});
		ListView lv = (ListView) view.findViewById(R.id.listView);
		lv.setAdapter(adapter);
		lv.setSelection(lv.getAdapter().getCount() - 1);
		return view;
	}

	/**
	 * 截图列表监听器
	 * 
	 * @author loveyu
	 * 
	 */
	class ScreenShotListAdapter extends BaseAdapter {
		/*
		 * (non-Javadoc)
		 * 
		 * @see android.widget.Adapter#getCount()
		 */
		@Override
		public int getCount() {
			return map.size();
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see android.widget.Adapter#getItem(int)
		 */
		@Override
		public Object getItem(int position) {
			return null;
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see android.widget.Adapter#getView(int, android.view.View,
		 * android.view.ViewGroup)
		 */
		@Override
		public View getView(int position, View convertView, ViewGroup parent) {
			View view = View.inflate(getActivity(), R.layout.pager_screen_shots_list_item, null);
			HashMap<String, String> hmap = map.get(position);
			((TextView) view.findViewById(R.id.FullName)).setText(hmap.get("FullBaseName"));
			((TextView) view.findViewById(R.id.FullName)).setText(hmap.get("FullBaseName"));
			((TextView) view.findViewById(R.id.SimpleName)).setText(hmap.get("SimpleBaseName"));
			((TextView) view.findViewById(R.id.SimpleName)).setText(hmap.get("SimpleBaseName"));
			((TextView) view.findViewById(R.id.FullSize)).setText(RcFile.ShowSize(
					Double.parseDouble(hmap.get("FullSize")), 2));
			((TextView) view.findViewById(R.id.SimpleSize)).setText(RcFile.ShowSize(
					Double.parseDouble(hmap.get("SimpleSize")), 2));
			view.findViewById(R.id.FullDown).setOnClickListener(new MyListener(hmap.get("FullName")));
			view.findViewById(R.id.SimpleDown).setOnClickListener(new MyListener(hmap.get("SimpleName")));
			return view;
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see android.widget.Adapter#getItemId(int)
		 */
		@Override
		public long getItemId(int position) {
			return 0;
		}
	}

	/**
	 * 下载按钮监听类
	 * 
	 * @author loveyu
	 * 
	 */
	class MyListener implements Button.OnClickListener {
		/**
		 * 远程路径
		 */
		String path;

		/**
		 * 构造一个监听器
		 * 
		 * @param ph
		 *            文件远程路径
		 */
		public MyListener(String ph) {
			path = ph;
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see android.view.View.OnClickListener#onClick(android.view.View)
		 */
		@Override
		public void onClick(View v) {
			RcDebug.N(getActivity(), "Send Screenshot download request.");
			RcNet.Get().Send(RcSendMsg.createFile("Get\n" + path));
		}
	}
}
