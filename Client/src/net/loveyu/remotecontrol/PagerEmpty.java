package net.loveyu.remotecontrol;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.LinearLayout.LayoutParams;

/**
 * 空的页面
 * 
 * @author loveyu
 * 
 */
public class PagerEmpty extends Fragment {
	/**
	 * 对应的唯一字符串
	 */
	private static final String KEY_CONTENT = "Fragment:Content";
	/**
	 * 标记
	 */
	protected String mContent = "???";
	/**
	 * 是否被暂停
	 */
	protected boolean isPause = true;

	/**
	 * 创建一个空的页面
	 * 
	 * @param content
	 *            页面标题
	 * @return 页面实例
	 */
	public static PagerEmpty newInstance(String content) {
		PagerEmpty fragment = new PagerEmpty();
		fragment.mContent = content;
		return fragment;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see android.support.v4.app.Fragment#onPause()
	 */
	@Override
	public void onPause() {
		super.onPause();
		isPause = true;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see android.support.v4.app.Fragment#onCreate(android.os.Bundle)
	 */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		if ((savedInstanceState != null) && savedInstanceState.containsKey(KEY_CONTENT)) {
			mContent = savedInstanceState.getString(KEY_CONTENT);
		}
		isPause = false;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see
	 * android.support.v4.app.Fragment#onCreateView(android.view.LayoutInflater,
	 * android.view.ViewGroup, android.os.Bundle)
	 */
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		TextView text = new TextView(getActivity());
		text.setGravity(Gravity.CENTER);
		text.setText(mContent);
		text.setTextSize(20 * getResources().getDisplayMetrics().density);
		text.setPadding(20, 20, 20, 20);
		LinearLayout layout = new LinearLayout(getActivity());
		layout.setLayoutParams(new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT));
		layout.setGravity(Gravity.CENTER);
		layout.addView(text);
		isPause = false;
		return layout;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see
	 * android.support.v4.app.Fragment#onSaveInstanceState(android.os.Bundle)
	 */
	@Override
	public void onSaveInstanceState(Bundle outState) {
		super.onSaveInstanceState(outState);
		outState.putString(KEY_CONTENT, mContent);
	}

	/**
	 * 返回是否暂停运行的状态
	 * 
	 * @return
	 */
	protected boolean get_pause() {
		return isPause;
	}

}
