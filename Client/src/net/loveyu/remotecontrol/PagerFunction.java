package net.loveyu.remotecontrol;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.ListView;

/**
 * 常用功能页面
 * 
 * @author loveyu
 * 
 */
public class PagerFunction extends PagerEmpty {
	/**
	 * 创建一个功能页面的实例
	 * 
	 * @param content
	 *            唯一标题
	 * @return 页面实例
	 */
	public static PagerFunction newInstance(String content) {
		PagerFunction fragment = new PagerFunction();
		fragment.mContent = content;
		return fragment;
	}

	/**
	 * 列表内容
	 */
	private ListView lv;
	/**
	 * 功能信息列表
	 */
	private HashMap<String, String> funcMap = new HashMap<String, String>();
	/**
	 * 用于存储button的列表
	 */
	private List<String> list;

	/*
	 * (non-Javadoc)
	 * 
	 * @see
	 * net.loveyu.remotecontrol.PagerEmpty#onCreateView(android.view.LayoutInflater
	 * , android.view.ViewGroup, android.os.Bundle)
	 */
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		CreateFuncMap();
		lv = new ListView(getActivity());
		FunctionListViewAdapter adapter = new FunctionListViewAdapter();
		lv.setAdapter(adapter);

		return lv;
	}

	/**
	 * 添加功能图
	 */
	private void CreateFuncMap() {
		funcMap.put("关闭服务器程序", "close");
		funcMap.put("计算机信息", "info");
		funcMap.put("服务器配置信息", "config");
		funcMap.put("文件传输队列信息", "file");
		funcMap.put("消息传输队列", "msg");
		funcMap.put("服务器版本信息", "version");
		list = new ArrayList<String>(funcMap.keySet());
	}

	/**
	 * 用于功能列表的列表桥接器
	 * 
	 * @author loveyu
	 * 
	 */
	class FunctionListViewAdapter extends BaseAdapter {
		/*
		 * (non-Javadoc)
		 * 
		 * @see android.widget.Adapter#getCount()
		 */
		@Override
		public int getCount() {
			return list.size();
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
		 * @see android.widget.Adapter#getItemId(int)
		 */
		@Override
		public long getItemId(int position) {
			return 0;
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see android.widget.Adapter#getView(int, android.view.View,
		 * android.view.ViewGroup)
		 */
		@Override
		public View getView(int position, View convertView, ViewGroup parent) {
			Button button = new Button(getActivity());
			button.setText(list.get(position));
			button.setOnClickListener(new Button.OnClickListener() {
				@Override
				public void onClick(View v) {
					String txt = ((Button) v).getText().toString();
					// Notice("T:" + txt + ",S:" + funcMap.get(txt));
					RcNet.Get().Send(RcSendMsg.createFunction(funcMap.get(txt)));
				}
			});
			return button;
		}
	}
}
