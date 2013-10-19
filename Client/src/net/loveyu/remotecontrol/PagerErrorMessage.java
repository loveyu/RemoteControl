package net.loveyu.remotecontrol;

import android.annotation.SuppressLint;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.text.Html;
import android.text.Spanned;
import android.text.TextUtils;
import android.text.format.Time;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ScrollView;
import android.widget.TextView;

/**
 * 错误消息页面
 * 
 * @author loveyu
 * 
 */
public class PagerErrorMessage extends PagerEmpty {
	/**
	 * html消息内容
	 */
	private static String message = "";
	/**
	 * 页面单例
	 */
	private static PagerErrorMessage instance = null;
	/**
	 * 消息实例
	 */
	private Handler handler;
	/**
	 * 消息值
	 */
	private final static int UPDATE = 1;
	/**
	 * 页面视图
	 */
	private View view;

	/**
	 * 获取页面实例
	 * 
	 * @param content
	 *            标题
	 * @return 页面实例
	 */
	public static PagerErrorMessage newInstance(String content) {
		PagerErrorMessage fragment = new PagerErrorMessage();
		fragment.mContent = content;
		instance = fragment;
		return fragment;
	}

	/**
	 * 线程通过此方法添加内容到页面
	 * 
	 * @param str
	 *            纯文本内容
	 */
	public static void appendText(String str) {
		Time time = new Time();
		time.setToNow();
		message += "<div><small><em><font color=\"#66aaaa\">" + time.format("%Y-%m-%d %H:%M:%S")
				+ ":</font></em></small><br><font color=\"#ff0000\">"
				+ TextUtils.htmlEncode(str).replaceAll("\\n", "<br>") + "</font></div>";
		if (instance != null && instance.get_pause() == false) {
			Message msg = instance.handler.obtainMessage(UPDATE);
			instance.handler.sendMessage(msg);
		}
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
		if ((savedInstanceState != null) && savedInstanceState.containsKey("Fragment:PagerErrorMessage")) {
			mContent = savedInstanceState.getString("Fragment:PagerErrorMessage");
		}
		handler = new Handler() {
			@Override
			public void handleMessage(Message msg) {
				// TODO Auto-generated method stub
				super.handleMessage(msg);
				switch (msg.what) {
				case UPDATE:
					((TextView) view.findViewById(R.id.pager_message_textView)).setText(GetMessage());
					break;
				}
			}
		};
		isPause = false;
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
		view = inflater.inflate(R.layout.pager_message, container, false);
		view.findViewById(R.id.pager_message_button_clear).setOnClickListener(new Button.OnClickListener() {

			@Override
			public void onClick(View v) {
				((TextView) view.findViewById(R.id.pager_message_textView)).setText("");
			}
		});
		view.findViewById(R.id.pager_message_button_refresh).setOnClickListener(new Button.OnClickListener() {
			@Override
			public void onClick(View v) {
				((TextView) view.findViewById(R.id.pager_message_textView)).setText(GetMessage());
				message = "";
			}
		});
		view.findViewById(R.id.pager_message_button_bottom).setOnClickListener(new Button.OnClickListener() {
			@Override
			public void onClick(View v) {
				((ScrollView) view.findViewById(R.id.pager_message_scrollView)).fullScroll(ScrollView.FOCUS_DOWN);
			}
		});
		view.findViewById(R.id.pager_message_button_top).setOnClickListener(new Button.OnClickListener() {
			@Override
			public void onClick(View v) {
				((ScrollView) view.findViewById(R.id.pager_message_scrollView)).fullScroll(ScrollView.FOCUS_UP);
			}
		});
		((TextView) view.findViewById(R.id.pager_message_textView)).setText(GetMessage());
		isPause = false;
		return view;
	}

	/**
	 * 将html标签转换为可布局内容
	 * 
	 * @return 可布局内容
	 */
	private Spanned GetMessage() {
		return Html.fromHtml(message);
	}
}
