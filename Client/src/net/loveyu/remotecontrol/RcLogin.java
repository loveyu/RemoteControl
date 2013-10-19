package net.loveyu.remotecontrol;

/**
 * 登录操作类
 * 
 * @author loveyu
 * 
 */
public class RcLogin {
	/**
	 * 登录实例
	 */
	private static RcLogin instance = new RcLogin();

	/**
	 * 私有构造函数
	 */
	private RcLogin() {
	}

	/**
	 * 获取实例
	 * 
	 * @return 实例
	 */
	public static RcLogin Get() {
		return instance;
	}

	/**
	 * 用户SID
	 */
	public String sid = "";
	/**
	 * 登录状态码
	 */
	public int status = 0;
	/**
	 * 登录提示信息
	 */
	public String info = "";

	/**
	 * 设置登录状态
	 * 
	 * @param status
	 *            状态码
	 * @param info
	 *            提示信息
	 * @return 状态码
	 */
	public int Set(String status, String info) {
		RcDebug.Log("Login msg: status : " + status + "," + info);
		try {
			this.status = Integer.parseInt(status);
			if (this.status == 200) {
				sid = info;
			} else {
				sid = "";
				this.info = info;
			}
		} catch (Exception ex) {
			sid = "";
			this.info = info;
		}
		return this.status;
	}

	/**
	 * 清除登录信息及状态码
	 */
	public void Clear() {
		sid = info = "";
		status = 0;
	}
}
