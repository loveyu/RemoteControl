package net.loveyu.remotecontrol;

import android.os.Bundle;
import android.view.View;
import android.widget.EditText;
import android.widget.Toast;
import android.app.Activity;

import java.net.InetAddress;

/**
 * 设置页面
 * 
 * @author loveyu
 * 
 */
public class ActivitySetting extends Activity {
	/**
	 * 服务器IP或名称
	 */
	String ServerName = "", FileServerName = "";
	/**
	 * 服务器端口
	 */
	int ServerPort = 0, FileServerPort = 0;

	/**
	 * 测试的状态
	 */
	boolean TestStatus;

	/**
	 * 重载加载布局
	 */
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_setting);
		load();
	}

	/**
	 * 保存事件
	 * 
	 * @param v
	 *            事件参数
	 */
	public void save(View v) {
		if (read() == false) {
			return;
		}
		if (write() == false) {
			Notice(getResources().getString(R.string.config_save_error));
			return;
		}
		Notice(getResources().getString(R.string.config_saved));
		RcConfig.Load(this);
	}

	/**
	 * 测试事件
	 * 
	 * @param v
	 *            事件参数
	 */
	public void test(View v) {
		if (RcConfig.NetConnected(getApplicationContext()) == false) {
			Notice(getResources().getString(R.string.network_disconnect));
			return;
		}
		if (read() == false)
			return;
		if (ServerName.length() > 2) {
			Thread thread = new Thread(new Runnable() {

				@Override
				public void run() {
					// TODO Auto-generated method stub
					TestStatus = false;
					try {
						InetAddress ipsn = InetAddress.getByName(ServerName);
						RcDebug.v("debug", ServerName + ":" + ipsn.getHostAddress());
						TestStatus = true;
					} catch (Exception e) {
						ErrorNotice(getResources().getString(R.string.server_name_error));
						return;
					}
				}
			});
			thread.start();
			try {
				thread.join();
				if (TestStatus == false)
					return;
			} catch (InterruptedException e1) {
				// TODO Auto-generated catch block
				e1.printStackTrace();
			}
		} else {
			ErrorNotice(getResources().getString(R.string.server_name_error));
			return;
		}
		if (FileServerName.length() > 2) {
			Thread thread2 = new Thread(new Runnable() {

				/**
				 * 网络线程中测试数据
				 */
				@Override
				public void run() {
					TestStatus = false;
					try {
						InetAddress ipsn = InetAddress.getByName(FileServerName);
						RcDebug.v("debug", FileServerName + ":" + ipsn.getHostAddress());
						TestStatus = true;
					} catch (Exception e) {
						ErrorNotice(getResources().getString(R.string.file_server_name_error));
						return;
					}
				}
			});
			thread2.start();
			try {
				thread2.join();
				if (TestStatus == false)
					return;
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		} else {
			ErrorNotice(getResources().getString(R.string.file_server_name_error));
			return;
		}
		if (ServerPort < 1) {
			Notice(getResources().getString(R.string.server_port_error));
			return;
		}
		if (FileServerPort < 1) {
			Notice(getResources().getString(R.string.file_server_port_error));
			return;
		}
		if (TestStatus)
			Notice(getResources().getString(R.string.test_success));
	}

	/**
	 * 输出错误提示
	 * 
	 * @param str
	 *            提示内容
	 */
	private void ErrorNotice(String str) {
		Notice(str);
	}

	/**
	 * 读取配置文件
	 * 
	 * @return 返回读取的状态
	 */
	private boolean read() {
		try {
			ServerName = ((EditText) findViewById(R.id.editText_ServerName)).getText().toString();
			String tmp = ((EditText) findViewById(R.id.editText_ServerPort)).getText().toString();
			if (tmp.equals("") == false && Integer.parseInt(tmp) > 0)
				ServerPort = Integer.parseInt(tmp);
			else
				ServerPort = 0;
			FileServerName = ((EditText) findViewById(R.id.EditText_FileServerName)).getText().toString();
			tmp = ((EditText) findViewById(R.id.EditText_FileServerPort)).getText().toString();
			if (tmp.equals("") == false && Integer.parseInt(tmp) > 0)
				FileServerPort = Integer.parseInt(tmp);
			else
				FileServerPort = 0;

			return true;
		} catch (Exception ex) {
			Notice(getResources().getString(R.string.config_read_error) + ":" + ex.getMessage());
		}
		return false;
	}

	/**
	 * 将配置文件写入到系统
	 * 
	 * @return 返回状态表示是否写入成功
	 */
	private boolean write() {
		try {
			FileAction.writeFile(RcConfig.GetSeverConfigPath(), "ServerName\t" + ServerName + "\nServerPort\t"
					+ ServerPort + "\nFileServerName\t" + FileServerName + "\nFileServerPort\t" + FileServerPort);
			return true;
		} catch (Exception e) {
			RcDebug.v("debug", e.getMessage());
			return false;
		}
	}

	/**
	 * 加载配置文件
	 */
	private void load() {
		ServerName = RcConfig.GetServerName();
		ServerPort = RcConfig.GetSevrePort();
		FileServerName = RcConfig.GetFileServerName();
		FileServerPort = RcConfig.GetFileSevrePort();

		((EditText) findViewById(R.id.editText_ServerName)).setText(ServerName);
		if (ServerPort > 0)
			((EditText) findViewById(R.id.editText_ServerPort)).setText("" + ServerPort);
		else
			((EditText) findViewById(R.id.editText_ServerPort)).setText("");
		((EditText) findViewById(R.id.EditText_FileServerName)).setText(FileServerName);
		if (FileServerPort > 0)
			((EditText) findViewById(R.id.EditText_FileServerPort)).setText("" + FileServerPort);
		else
			((EditText) findViewById(R.id.EditText_FileServerPort)).setText("");
	}

	/**
	 * 显示一个提示信息
	 * 
	 * @param notice
	 *            提示内容
	 */
	private void Notice(String notice) {
		Toast.makeText(this, notice, Toast.LENGTH_SHORT).show();
	}
}
