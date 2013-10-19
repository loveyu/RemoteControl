using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;

namespace RemoteControlService
{
	/// <summary>
	/// 截图操作类
	/// </summary>
	public class Screenshot
	{
		/// <summary>
		/// 缩略图宽度
		/// </summary>
		int width;

		/// <summary>
		/// 文件保存名（时间格式）
		/// </summary>
		string name;

		/// <summary>
		/// 完整大小文件保存路径
		/// </summary>
		string fullName;

		/// <summary>
		/// 完整文件大小
		/// </summary>
		int fullSize;

		/// <summary>
		/// 缩略图文件保存路径
		/// </summary>
		string thumName;

		/// <summary>
		/// 缩略图大小
		/// </summary>
		int thumSize;

		/// <summary>
		/// 截图状态
		/// </summary>
		bool status = true;

		/// <summary>
		/// 截图构造函数，并进行相应的处理
		/// </summary>
		/// <param name="width">网络发送过来的缩略图宽度字符串</param>
		public Screenshot(string width)
		{
			try
			{
				this.width = int.Parse(width);
			}
			catch
			{
				this.width = 0;
			}
			if (this.width < 100) this.width = 100;
			Process();
		}

		/// <summary>
		/// 生成截图，并生成文件信息
		/// </summary>
		private void Process()
		{
			if (CreateName() == false)//文件已存在且创建
				return;
			Bitmap myImage = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
			Graphics g = Graphics.FromImage(myImage);
			g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));
			IntPtr dc1 = g.GetHdc();
			g.ReleaseHdc(dc1);
			myImage.Save(fullName, ImageFormat.Jpeg);
			fullSize = (int)new FileInfo(fullName).Length;
			if (width >= myImage.Width)
			{
				thumName = fullName;
				thumSize = fullSize;
			}
			else
			{
				CreateThumbnails(myImage);
				myImage.Dispose();
			}
		}

		/// <summary>
		/// 创建截图名称及路径
		/// </summary>
		/// <returns></returns>
		private bool CreateName()
		{
			name = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
			fullName = Configure.ScreenshotPath() + "\\" + name + ".jpg";
			thumName = Configure.ScreenshotPath() + "\\" + name + "_w" + width + ".jpg";
			if (File.Exists(fullName))
			{
				try
				{
					Bitmap myImage = new Bitmap(fullName);
					int fullWidth = myImage.Width;
					fullSize = (int)new FileInfo(fullName).Length;
					if (width >= fullWidth)
					{
						thumName = fullName;
						thumSize = fullSize;
					}
					else
					{
						CreateThumbnails(myImage);
						myImage.Dispose();
					}
				}
				catch
				{
					status = false;
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// 获取完整路径
		/// </summary>
		/// <returns></returns>
		public string getFull()
		{
			return fullName;
		}

		/// <summary>
		/// 获取完整大小
		/// </summary>
		/// <returns></returns>
		public int getFullSize()
		{
			return fullSize;
		}

		/// <summary>
		/// 获取缩略图路径
		/// </summary>
		/// <returns></returns>
		public string get()
		{
			return thumName;
		}

		/// <summary>
		/// 获取缩略图大小
		/// </summary>
		/// <returns></returns>
		public int getSize()
		{
			return thumSize;
		}

		/// <summary>
		/// 返回截图状态
		/// </summary>
		/// <returns></returns>
		public bool isOK()
		{
			return status;
		}

		/// <summary>
		/// 根据原始图像创建缩略图，并保存
		/// </summary>
		/// <param name="img">截图后的数据</param>
		private void CreateThumbnails(Bitmap img)
		{
			int nw, nh;
			nw = width;
			nh = width * img.Height / img.Width;
			Bitmap nimg = new Bitmap(img, new Size(nw, nh));
			nimg.Save(thumName, ImageFormat.Jpeg);
			thumSize = (int)(new FileInfo(thumName)).Length;
		}
	}
}
