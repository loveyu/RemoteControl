package net.loveyu.remotecontrol;

import com.viewpagerindicator.TabPageIndicator;
import android.annotation.SuppressLint;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;
import android.support.v4.view.ViewPager;
import android.view.KeyEvent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.Window;

/**
 * 应用操作主页面
 * 
 * @author loveyu
 * 
 */
public class ActivityMain extends FragmentActivity {
	/**
	 * 指示改界面是否激活
	 */
	public static boolean runing = false;

	/**
	 * 指示该界面是否在后台运行
	 */
	public static boolean isActive = true;

	/**
	 * 用户滑块页面显示的数据
	 */
	private String[] CONTENT;

	/**
	 * 自身对象的单例模式
	 */
	public static ActivityMain instance = null;

	/**
	 * 消息处理对象
	 */
	public Handler messageHandler;

	/**
	 * 所处理的消息类型
	 */
	public static final int MsgNotice = 1;

	/**
	 * 对返回事件的监听，直接跳转到后台运行
	 */
	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {
		if (keyCode == KeyEvent.KEYCODE_BACK) {
			moveTaskToBack(false);
			return true;
		}
		return super.onKeyDown(keyCode, event);
	}

	/**
	 * 初始化页面并加载Pager,和设置消息监听
	 */
	@SuppressLint("HandlerLeak")
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		instance = this;
		this.requestWindowFeature(Window.FEATURE_NO_TITLE);
		setContentView(R.layout.activity_main);
		FragmentPagerAdapter adapter = new RemotePagerAdapter(getSupportFragmentManager());

		ViewPager pager = (ViewPager) findViewById(R.id.main_pager);
		pager.setAdapter(adapter);

		TabPageIndicator indicator = (TabPageIndicator) findViewById(R.id.main_indicator);
		indicator.setViewPager(pager);
		runing = true;
		messageHandler = new Handler() {
			public void handleMessage(Message msg) {
				switch (msg.what) {
				case MsgNotice:
					RcDebug.N(getApplicationContext(), (String) msg.obj);
					break;
				}
			}
		};
	}

	/**
	 * 菜单选择
	 */
	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		try {
			switch (item.getItemId()) {
			case R.id.menu_download:
				Intent download_intent = new Intent(this, ActivityDownload.class);
				startActivity(download_intent);
				break;
			case R.id.menu_file_manager:
				Intent file_manager_intent = new Intent(this, ActivityFile.class);
				startActivity(file_manager_intent);
				break;
			case R.id.menu_settings:
				Intent intent = new Intent(this, ActivitySetting.class);
				startActivity(intent);
				break;
			case R.id.menu_task_manager:
				Intent task_manager = new Intent(this, ActivityTask.class);
				startActivity(task_manager);
				break;
			case R.id.menu_help:
				Intent help = new Intent(this, ActivityHelp.class);
				startActivity(help);
				break;
			case R.id.menu_terminal:
				Intent terminal = new Intent(this, ActivityTerminal.class);
				startActivity(terminal);
				break;
			case R.id.menu_logout:
				RcNet.Get().Send(RcSendMsg.createLogout());
				Intent loginInit = new Intent(this, ActivityLogin.class);
				startActivity(loginInit);
				finish();
				RcNet.Get().Stop();
				runing = false;
				break;
			case R.id.menu_exit:
				RcNet.Get().Send(RcSendMsg.createLogout());
				RcNet.Get().Stop();
				finish();
				runing = false;
				break;
			default:
				break;
			}
		} catch (Exception ex) {

		}
		return true;
	}

	/**
	 * 创建菜单
	 */
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		getMenuInflater().inflate(R.menu.main, menu);
		return true;
	}

	/**
	 * 代理页面桥接器
	 * 
	 * @author loveyu
	 * 
	 */
	class RemotePagerAdapter extends FragmentPagerAdapter {
		// { "执行命令", "功能", "消息", "截图", "发送消息", "发送警告", "错误消息" };
		/**
		 * 桥接类构造方面，初始化各个页面标题，及设置数量
		 * 
		 * @param fm
		 *            管理对象实例
		 */
		public RemotePagerAdapter(FragmentManager fm) {
			super(fm);
			CONTENT = new String[] { getResources().getString(R.string.run_command),
					getResources().getString(R.string.function), getResources().getString(R.string.message),
					getResources().getString(R.string.screen_shots), getResources().getString(R.string.send_message),
					getResources().getString(R.string.send_warning), getResources().getString(R.string.error_message) };
		}

		/**
		 * 获取对应的元素的页面
		 */
		@Override
		public Fragment getItem(int position) {
			String str = CONTENT[position % CONTENT.length];
			switch (position % CONTENT.length) {
			case 0:
				return PagerCommand.newInstance(str);
			case 1:
				return PagerFunction.newInstance(str);
			case 2:
				return PagerMessage.newInstance(str);
			case 3:
				return PagerScreenShots.newInstance(str);
			case 4:
				return PagerSendNotice.newInstance(str);
			case 5:
				return PagerSendWarning.newInstance(str);
			case 6:
				return PagerErrorMessage.newInstance(str);
			}
			return PagerEmpty.newInstance(str);
		}

		/**
		 * 获取页面的标题信息
		 */
		@Override
		public CharSequence getPageTitle(int position) {
			return CONTENT[position % CONTENT.length];
		}

		/**
		 * 获取页面的数量
		 */
		@Override
		public int getCount() {
			return CONTENT.length;
		}

	}

}
