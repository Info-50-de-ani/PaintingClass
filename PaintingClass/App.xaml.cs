using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PaintingClass.Login;

namespace PaintingClass
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public const string costumUrl = "PaintingClassLauncher";
		public static int startedAppFromBrowserroomId = 0;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			if (e.Args.Length > 0)
			{
				#region extragem roomId din event args
				{
					int startidx = e.Args[0].IndexOf("//");
					for(int i = 0; i< e.Args[0].Length; i++)
					{
						if (char.IsDigit(e.Args[0][i]))
						{
						startedAppFromBrowserroomId = startedAppFromBrowserroomId * 10 + e.Args[0][i] -'0';
						}
					}
				
				}
				#endregion
				Debug.Assert(startedAppFromBrowserroomId != 0);
			}
			else
			{
				var process = Process.GetCurrentProcess();
				foreach (var reg in Registry.ClassesRoot.GetSubKeyNames())
				{
					if (reg == "PaintingClassLauncher")
					{
						return;
					}
				}
				{
					RegistryKey key = Registry.ClassesRoot.CreateSubKey(costumUrl, true);
					key.SetValue("URL protocol", "");
					RegistryKey shellkey = key.CreateSubKey("shell", true);
					RegistryKey openkey = shellkey.CreateSubKey("open", true);
					RegistryKey coomandkey = openkey.CreateSubKey("command", true);
					string fullPath = process.MainModule.FileName;
					coomandkey.SetValue("", fullPath + " \"%1\"");
				}
			}
		}
	}
}
