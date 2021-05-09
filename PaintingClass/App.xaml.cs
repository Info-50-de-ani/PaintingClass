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
using System.Security.Principal;
using System.Reflection;

namespace PaintingClass
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public const bool DEBUG_MODE = true;
		public static int startedAppFromBrowserroomId = 0;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			Storage.Settings.Init();
			//check for roomId
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

			using var process = Process.GetCurrentProcess();
			string path = process.MainModule.FileName + " \"%1\"";

			//verificam daca cheia a fost adaugata
			if (CheckKey(path) == true) 
				return;
			
			if (IsAdministrator)
            {
				//scriem cheia
				WriteKey(path);
            }
			else
            {
				string msg = "Pentru a termina instalarea aplicației sunt necesare drepturi de administrator. Doriți să restartați aplicația în modul de administrator?";
				if (MessageBox.Show(msg, "PaintingClass",MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
					Elevate();
			}
		}

		bool CheckKey(string path)
		{
			var key = Registry.ClassesRoot.OpenSubKey(Networking.Constants.customProtocol, false)
				?.OpenSubKey("shell", false)
				?.OpenSubKey("open", false)
				?.OpenSubKey("command", false);

			return key != null && key.GetValue("") as string == path;
		}

		void WriteKey(string path)
        {
			var key = Registry.ClassesRoot.CreateSubKey(Networking.Constants.customProtocol, true);
			key.SetValue("URL Protocol", "");

			var key2 = key
				?.CreateSubKey("shell", true)
				?.CreateSubKey("open", true)
				?.CreateSubKey("command", true);
			Debug.Assert(key != null);

			key2.SetValue("", path);
		}

		void Elevate()
        {
			ProcessStartInfo proc = new ProcessStartInfo()
            {
				UseShellExecute=true,
				WorkingDirectory=Environment.CurrentDirectory,
				FileName= Process.GetCurrentProcess().MainModule.FileName,
				Verb = "runas"
            };

			try
			{
				Process.Start(proc);
			}
			catch (Exception e)
			{
				// The user refused the elevation
				Trace.WriteLine(e.Message);
				return;
			}
			Application.Current.Shutdown();
        }

		bool IsAdministrator =>new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
	}
}
