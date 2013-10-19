using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace RemoteControlService
{
	static class Program
	{
		static void Main()
		{
			Control control = new Control();
			control.Start();//启动服务
			control.Terminal();//启动控制台
		}
	}
}
