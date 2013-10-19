package net.loveyu.remotecontrol;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.text.Html;
import android.view.View;
import android.view.Window;
import android.widget.Button;
import android.widget.ScrollView;
import android.widget.TextView;

/**
 * 终端页面
 * 
 * @author loveyu
 * 
 */
public class ActivityTerminal extends Activity {
	/**
	 * 消息处理实例
	 */
	private Handler handler;

	/**
	 * 单例模式终端
	 */
	private static ActivityTerminal instance = null;

	/**
	 * 消息
	 */
	private static final int Update = 1;

	/**
	 * 是否后台运行
	 */
	private static boolean isPause = false;

	/**
	 * 终端文本内容
	 */
	private static String termina_text = "";

	/**
	 * 是否为命令行模式
	 */
	private static boolean isCmdLineType = true;

	/**
	 * 加载布局并设置事件监听
	 */
	@SuppressLint("HandlerLeak")
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		instance = this;
		requestWindowFeature(Window.FEATURE_NO_TITLE);
		setContentView(R.layout.activity_terminal);
		((Button) findViewById(R.id.terminal_send)).setOnClickListener(new Button.OnClickListener() {
			@Override
			public void onClick(View v) {
				String cmd = ((TextView) findViewById(R.id.terminal_cmd)).getText().toString();
				if ("".equals(cmd)) {
					RcDebug.N(ActivityTerminal.this, "cmd is empty!");
				} else {
					if (isCmdLineType) {
						cmd += "\n";
					} else {
						cmd = TerminalAscii.replace(cmd);
					}
					RcNet.Get().Send(RcSendMsg.createTerminal(cmd));
				}
				((TextView) findViewById(R.id.terminal_cmd)).setText("");
			}
		});
		findViewById(R.id.terminal_bottom).setOnClickListener(new Button.OnClickListener() {
			@Override
			public void onClick(View v) {
				((ScrollView) findViewById(R.id.terminal_scrollView)).fullScroll(ScrollView.FOCUS_DOWN);
			}
		});
		findViewById(R.id.terminal_top).setOnClickListener(new Button.OnClickListener() {
			@Override
			public void onClick(View v) {
				((ScrollView) findViewById(R.id.terminal_scrollView)).fullScroll(ScrollView.FOCUS_UP);
			}
		});
		findViewById(R.id.terminal_clear).setOnClickListener(new Button.OnClickListener() {
			@Override
			public void onClick(View v) {
				((TextView) findViewById(R.id.terminal_text)).setText("");
				termina_text = "";
			}
		});
		findViewById(R.id.terminal_type).setOnClickListener(new TermianlTypeListener());
		handler = new Handler() {
			@Override
			public void handleMessage(Message msg) {
				// TODO Auto-generated method stub
				super.handleMessage(msg);
				switch (msg.what) {
				case Update:
					ChangeData();
					break;
				}
			}
		};
	}

	/**
	 * 终端发送监听事件
	 * 
	 * @author loveyu
	 * 
	 */
	class TermianlTypeListener implements Button.OnClickListener {
		@Override
		public void onClick(View v) {
			isCmdLineType = !isCmdLineType;
			if (isCmdLineType) {
				((Button) v).setText(getResources().getString(R.string.terminal_type_line));
			} else {
				((Button) v).setText(getResources().getString(R.string.terminal_type_char));
			}
		}
	}

	/**
	 * 后台运行
	 */
	@Override
	protected void onPause() {
		super.onPause();
		isPause = true;
	}

	/**
	 * 恢复运行
	 */
	@Override
	protected void onResume() {
		super.onResume();
		isPause = false;
	}

	/**
	 * 线程中更新数据
	 * 
	 * @param text
	 *            终端文本
	 */
	public static void UpData(String text) {
		termina_text += text.replaceAll("\\n", "<br />");
		if (instance != null && isPause == false) {
			Message msg = new Message();
			msg.what = Update;
			instance.handler.sendMessage(msg);
		}
	}

	/**
	 * 线程中更新错误数据
	 * 
	 * @param text
	 *            错误的终端文本
	 */
	public static void UpErrorData(String text) {
		termina_text += "<font color=\"#ff0000\">" + text.replaceAll("\\n", "<br />") + "</font>";
		if (instance != null && isPause == false) {
			Message msg = new Message();
			msg.what = Update;
			instance.handler.sendMessage(msg);
		}
	}

	/**
	 * 修改终端的数据
	 */
	private void ChangeData() {
		((TextView) findViewById(R.id.terminal_text)).setText(Html.fromHtml(termina_text));
		((ScrollView) findViewById(R.id.terminal_scrollView)).fullScroll(ScrollView.FOCUS_DOWN);
	}
}
