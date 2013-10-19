using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace UdpMessageTest
{
	public partial class UdpMessageTest : Form
	{
		UdpClient udp;
		public delegate void AddText(string str);
		AddText addTextDelegate;
		private volatile bool rcving = false;
		static long id = 0;
		string sid = "";
		public UdpMessageTest()
		{
			addTextDelegate = new AddText(addText);
			InitializeComponent();
		}
		private void addText(string str)
		{
			if (richTextBox_received.InvokeRequired)
			{
				richTextBox_received.Invoke(addTextDelegate, str);
			}
			else
			{
				richTextBox_received.Text += str;
			}
		}
		private void button_connect_Click(object sender, EventArgs e)
		{
			try
			{
				udp = new UdpClient();
				udp.Connect(new IPEndPoint(IPAddress.Parse(textBox_ip.Text), int.Parse(textBox_port.Text)));
				button_close.Enabled = true;
				button_connect.Enabled = false;
				button_send.Enabled = true;
				rcving = true;
				Thread thread = new Thread(new ThreadStart(ReceivedThread));
				thread.Start();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void ReceivedThread()
		{
			while (rcving)
			{
				try
				{
					IPEndPoint e = new IPEndPoint(IPAddress.Any, 0);
					byte[] bytes = udp.Receive(ref e);
					Process(bytes, e);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}

		private void Process(byte[] bs, IPEndPoint e)
		{

			StreamReader sr = new StreamReader(new MemoryStream(bs, false), Encoding.UTF8);
			try
			{
				string head = sr.ReadLine();
				string[] info = head.Split('\t');
				if (info[2] == "01") return;
				addText(string.Format("[{0}]:Date:{1};ID:{2};TYPE:{3};\n", e, info[0], info[1], info[2]));
				byte[] send = Encoding.UTF8.GetBytes(string.Format("{0:0000000000}", id++) + "07" + info[0] + "\t" + info[1]);
				udp.Send(send, send.Length);
				if (info[2] == "05")
				{
					head = sr.ReadLine();
					string[] info2 = head.Split('\t');
					switch (info2[0].ToLower())
					{
						case "dir":
							addText("Get Dir info:\n" + info2[1] + "\t" + info2[1] + "\t" + info2[1] + "\n");
							break;
						case "file":
							addText("Get File info:\npath:" + info2[1] + " size:" + info2[2] + " FileId:" + info2[3] + "\n");
							Thread thread = new Thread(new ParameterizedThreadStart(GetFile));
							thread.Name = "file received thread";
							thread.Start(info2);
							break;
						default:
							addText(head + "\n");
							break;
					}
				}
				if (info[2] == "03")
				{
					head = sr.ReadLine();
					if (head.Substring(0, 3) == "200")
					{
						sid = head.Substring(4);
						addText("Login Success" + "\n");
					}
					else
					{
						addText("Login error: " + head + "\n");
					}
					return;
				}
				if (info[2] == "07" || info[2] == "08")
					addText(sr.ReadToEnd());
				else addText(sr.ReadToEnd() + "\n");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				sr.Close();
			}
		}
		private void GetFile(Object obj)
		{
			string[] info = (string[])obj;
			TcpClient client = new TcpClient();
			client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9875));
			if (client.Connected)
			{
				try
				{
					addText("Begin Read file thread\n");
					NetworkStream ns = client.GetStream();
					StreamReader sr = new StreamReader(ns);
					byte[] btmp = stringToBytes(sid + "\n");
					ns.Write(btmp, 0, btmp.Length);
					btmp = stringToBytes(info[3] + "\n");
					ns.Write(btmp, 0, btmp.Length);
					ns.Flush();
					string tmp = sr.ReadLine();
					if (tmp == null)
					{
						client.Close();
						addText("File thread no file");
						return;
					}
					Console.WriteLine(tmp);
					long size = long.Parse(tmp);

					if (size >= 0)
					{
						addText("Begin Read file\n");
						FileStream fs = new FileStream("E:\\" + Path.GetFileName(info[1]), FileMode.Create);
						byte[] bytes = new byte[2048];
						int rl = 0;
						btmp = stringToBytes("send\nsend\n");
						ns.Write(btmp, 0, btmp.Length);
						ns.Flush();
						BinaryReader br = new BinaryReader(sr.BaseStream);
						long read = 0;
						double percentage = 0, tmp_f;
						while ((rl = br.Read(bytes, 0, 2048)) != 0)
						{
							fs.Write(bytes, 0, rl);
							read += rl;
							tmp_f = read * 100 / size;
							if (tmp_f > percentage)
							{
								percentage = tmp_f;
								addText("Download:" + percentage + "\n");
							}
						}
						addText("Download complete\n");
						fs.Close();
						if (read != size)
						{
							addText("Read size is" + read + ", but file size is " + size + "\n");
						}
					}
					else
					{
						addText("File size is " + size + ",read error\n");
					}
					ns.Close();
					client.Close();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.StackTrace);
					MessageBox.Show(ex.Message);
				}
			}
			else
			{
				MessageBox.Show("Cann't connected");
			}
		}
		private void button_close_Click(object sender, EventArgs e)
		{
			if (udp != null)
			{
				udp.Close();
				udp = null;
			}
			button_close.Enabled = false;
			button_connect.Enabled = true;
			button_send.Enabled = false;
			rcving = false;
		}

		private void button_send_Click(object sender, EventArgs e)
		{
			try
			{
				if (udp == null)
				{
					MessageBox.Show("未连接");
					return;
				}
				byte[] bytes = Encoding.UTF8.GetBytes(string.Format("{0:0000000000}", id++) + comboBox.Text + richTextBox_Send.Text);
				udp.Send(bytes, bytes.Length);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void UdpMessageTest_FormClosing(object sender, FormClosingEventArgs e)
		{
			button_close_Click(null, null);
		}

		private void button_clearReceived_Click(object sender, EventArgs e)
		{
			richTextBox_received.Text = "";
		}
		public byte[] stringToBytes(string s)
		{
			return Encoding.UTF8.GetBytes(s);
		}

		private void button_login_Click(object sender, EventArgs e)
		{
			try
			{
				if (udp == null)
				{
					MessageBox.Show("未连接");
					return;
				}
				byte[] bytes = Encoding.UTF8.GetBytes(string.Format("{0:0000000000}08admin\nadmin\ntrue", id++));
				udp.Send(bytes, bytes.Length);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}
