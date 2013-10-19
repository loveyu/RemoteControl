package net.loveyu.remotecontrol;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

/**
 * 远程任务列表分割类
 * 
 * @author loveyu
 * 
 */
public class RcTask {
	/**
	 * 获取任务列表
	 * 
	 * @param content
	 *            原始的几段任务
	 * @return 分割好的任务列表
	 */
	public static List<HashMap<String, String>> GetTaskList(String content) {
		List<HashMap<String, String>> rt = new ArrayList<HashMap<String, String>>();
		String[] list = content.split("\\n");
		for (String s : list) {
			String[] info = s.split("\\t");
			if (info.length != 5)
				continue;
			HashMap<String, String> map = new HashMap<String, String>();
			map.put("id", info[0]);
			map.put("name", info[1]);
			map.put("memory", info[2]);
			map.put("time", info[3]);
			map.put("path", info[4]);
			rt.add(map);
		}
		return rt;
	}
}
