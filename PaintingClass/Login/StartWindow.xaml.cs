using PaintingClass.Login;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PaintingClass
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {

        public StartWindow()
        {
            InitializeComponent();
            BackButton.Visibility = Visibility.Hidden;
            ///daca e inceputa aplicatia prin browser
            if (App.startedAppFromBrowserroomId != 0)
            {
                var ejr = new ElevJoinRoom(MainFrame);
                ejr.roomId.Text = App.startedAppFromBrowserroomId.ToString();
                MainFrame.Content =ejr;
                App.startedAppFromBrowserroomId = 0;
            }
			else
            {
            MainFrame.Content = new ProfesorSauElev(MainFrame);
			}
            MainFrame.Navigated += MainFrame_Navigated;

        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!Type.Equals(MainFrame.Content.GetType(), typeof(ProfesorSauElev)))
                BackButton.Visibility = Visibility.Visible;
            else
                BackButton.Visibility = Visibility.Hidden;
        }

        private void Skip_Button_Click(object sender, RoutedEventArgs e)
        {
            var mw = new MainWindow(null);
            mw.Show();
            Close();
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.GoBack();
        }

		private void CloseApplication_MouseDown(object sender, MouseButtonEventArgs e)
		{
            Application.Current.Shutdown();
        }

		private void Minimize_MouseDown(object sender, MouseButtonEventArgs e)
		{
            this.WindowState = WindowState.Minimized;
		}
	}
}
