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
		public static int startedAppFromBrowserroomId = 0;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			return; // todo: temporar
#pragma warning disable CS0162 // Unreachable code detected
            if (e.Args.Length > 0)
#pragma warning restore CS0162 // Unreachable code detected
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
		}
	}
}
