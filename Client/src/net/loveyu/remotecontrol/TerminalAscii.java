package net.loveyu.remotecontrol;

/**
 * 终端字符模式ASCII码转换类
 * 
 * @author loveyu
 * 
 */
public class TerminalAscii {
	/**
	 * 对应的字符表
	 */
	public static String[][] list = { { "\\^@", "0" }, { "\\^A", "1" }, { "\\^B", "2" }, { "\\^C", "3" },
			{ "\\^D", "4" }, { "\\^E", "5" }, { "\\^F", "6" }, { "\\^G", "7" }, { "\\^H", "8" }, { "\\^I", "9" },
			{ "\\^J", "10" }, { "\\^K", "11" }, { "\\^L", "12" }, { "\\^M", "13" }, { "\\^N", "14" }, { "\\^O", "15" },
			{ "\\^P", "16" }, { "\\^Q", "17" }, { "\\^R", "18" }, { "\\^S", "19" }, { "\\^T", "20" }, { "\\^U", "21" },
			{ "\\^V", "22" }, { "\\^W", "23" }, { "\\^X", "24" }, { "\\^Y", "25" }, { "\\^Z", "26" },
			{ "\\^\\[", "27" }, { "\\^\\\\", "28" }, { "\\^]", "29" }, { "\\^\\^", "30" }, { "\\^_", "31" } };

	/**
	 * 根据字符表替换所有对应字符
	 * 
	 * @param str
	 *            要替换的数据
	 * @return 新的字符串
	 */
	public static String replace(String str) {
		for (String[] tab : list) {
			String ch = String.format("%c\r", Integer.parseInt(tab[1]));
			str = str.replaceAll(tab[0].toLowerCase(), ch);
			str = str.replaceAll(tab[0].toUpperCase(), ch);
		}
		return str;
	}

}
