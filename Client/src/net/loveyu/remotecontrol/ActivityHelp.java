package net.loveyu.remotecontrol;

import java.util.ArrayList;
import java.util.HashMap;
import android.app.Activity;
import android.content.pm.PackageManager.NameNotFoundException;
import android.os.Bundle;
import android.text.Html;
import android.text.method.ScrollingMovementMethod;
import android.widget.TextView;

/**
 * 帮助页面
 * @author loveyu
 *
 */
public class ActivityHelp extends Activity {
	/**
	 * 重写创建方法，加载xml布局
	 */
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_help);
		setHelpContent();
	}

	/**
	 * 设置帮助文档内容
	 */
	private void setHelpContent() {
		String html = "";

		ArrayList<HashMap<String, ArrayList<String>>> list = getData();
		for (HashMap<String, ArrayList<String>> map : list) {
			for (String name : map.keySet()) {
				html += "<div><h3>" + name + "</h3>";
				for (String item : map.get(name)) {
					html += "<p>" + item + "</p>";
				}
				html += "</div>";
			}
		}
		TextView tv = ((TextView) findViewById(R.id.help));
		tv.setMovementMethod(ScrollingMovementMethod.getInstance());
		tv.setText(Html.fromHtml(html));
	}

	/**
	 * 返回帮助内容的列表信息
	 * @return 使用多层次列表
	 */
	private ArrayList<HashMap<String, ArrayList<String>>> getData() {
		ArrayList<HashMap<String, ArrayList<String>>> rt = new ArrayList<HashMap<String, ArrayList<String>>>();

		HashMap<String, ArrayList<String>> map = new HashMap<String, ArrayList<String>>();
		ArrayList<String> list = new ArrayList<String>();
		list.add("一个简易的远程控制软件,包括各项基本功能。");
		list.add("版本：" + getVersion());
		list.add("如果出现Bug，可以反馈一下。");
		map.put("程序信息", list);
		rt.add(map);

		map = new HashMap<String, ArrayList<String>>();
		list = new ArrayList<String>();
		list.add("登录前需要知道服务器的IP地址，当然可以使用域名，如果能够解析");
		list.add("用户名和密码需首先到服务器设定，不存在默认密码。");
		list.add("如果用不到文件下载服务可以随意填写文件传输服务器IP及端口。");
		list.add("配置文件将保存到应用程序目录，无ROOT权限其他程序无法访问，当然，未加密。");
		map.put("登录说明", list);
		rt.add(map);

		map = new HashMap<String, ArrayList<String>>();
		list = new ArrayList<String>();
		list.add("提交的命令会在远程的新的DOS窗口中执行");
		list.add("这个命令不会返回任何输出内容到控制端，如果需要使用终端命令");
		list.add("如果需要多条命令同时执行，可以使用换行分隔或者&分隔，换行最后的作用一样");
		map.put("执行命令说明", list);
		rt.add(map);

		map = new HashMap<String, ArrayList<String>>();
		list = new ArrayList<String>();
		list.add("消息是正常的命令执行状态返回，可以清空，如果数据过多请手动清空");
		list.add("错误消息指服务端无法执行选定请求，或者出现错误所返回的状况，这个会存在一个提示信息");
		map.put("消息及错误消息说明", list);
		rt.add(map);

		map = new HashMap<String, ArrayList<String>>();
		list = new ArrayList<String>();
		list.add("这是服务器端内置的一系列功能，可以返回一部分消息");
		list.add("如果关闭服务器将没有任何提示，此时你可以自觉退出程序。");
		map.put("常用功能函数说明", list);
		rt.add(map);

		map = new HashMap<String, ArrayList<String>>();
		list = new ArrayList<String>();
		list.add("你可以获取到计算机桌面的实时图片，并且返回当前的列表");
		list.add("列表包括一个完整大小图片和缩略图");
		list.add("可手动指定缩略图宽度，如果数据异常将返回默认100的宽度值");
		list.add("此处下载后可到下载管理中查看");
		map.put("截图功能说明", list);
		rt.add(map);

		map = new HashMap<String, ArrayList<String>>();
		list = new ArrayList<String>();
		list.add("发送消息指在计算机桌面弹出一个提示框，在提示框关闭时会返回关闭信息");
		map.put("发送消息功能说明", list);
		rt.add(map);

		map = new HashMap<String, ArrayList<String>>();
		list = new ArrayList<String>();
		list.add("发送警告值在桌面显示警告框，与消息框类似");
		map.put("发送警告功能说明", list);
		rt.add(map);

		map = new HashMap<String, ArrayList<String>>();
		list = new ArrayList<String>();
		list.add("任务管理是将windows上所运行的所有任务分段发送过来");
		list.add("对名称进行排序，长按选择一个任务来结束它");
		list.add("没有排序功能，结束某一程序后返回状态，之后需要手动刷新列表");
		list.add("该列表不会时时更新");
		map.put("任务管理功能说明", list);
		rt.add(map);

		map = new HashMap<String, ArrayList<String>>();
		list = new ArrayList<String>();
		list.add("提供列表，会显示所有可列出盘符");
		list.add("如果要对文件进行操作必须先勾选之后在使用按钮");
		list.add("支持删除，重命名，和文件的复制操作。如果要对文件夹复制需要使用DOS命令。");
		list.add("在菜单中可以选择创建空文件和文件夹");
		list.add("可以单独执行文件操作命令，支持,get,delete,rename,move操作，每行一个参数，根目录使用ROOT，如果get对象是个文件，那个将会执行一个下载文件操作");
		map.put("文件管理功能说明", list);
		rt.add(map);

		map = new HashMap<String, ArrayList<String>>();
		list = new ArrayList<String>();
		list.add("该功能不保存任何状态信息，程序结束后内容清空");
		list.add("下载列表可能只会显示个别下载进度，添加的任务不会一次性全部添加");
		list.add("下载完成后能够调用系统打开文件");
		list.add("文件下载目录为SD卡RemoteControl下");
		list.add("感觉没有自定义的必要，难道你会用这个来下载大文件么？感觉不现实啊，虽然可以。");
		list.add("文件下载没有暂停和取消功能，需要的话自己结束整个程序");
		map.put("下载管理功能说明", list);
		rt.add(map);

		map = new HashMap<String, ArrayList<String>>();
		list = new ArrayList<String>();
		list.add("作者:<strong>恋羽</strong>");
		list.add("博客地址:<a href=\"http://www.loveyu.org\">http://www.loveyu.org</a>");
		list.add("项目地址[构建中]:<a href=\"http://www.loveyu.net/RemoteControl\">http://www.loveyu.net/RemoteControl</a>");
		map.put("反馈信息", list);
		rt.add(map);

		return rt;
	}
	
	/**
	 * 获取程序版本号
	 * @return 无法获取时返回1.0
	 */
	private String getVersion() {
		try {
			return getPackageManager().getPackageInfo(getPackageName(), 0).versionName;
		} catch (NameNotFoundException e) {
			return "1.0";
		}
	}
}
