using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PaintingClass.Login
{
    /// <summary>
    /// Interaction logic for LoginSauRegister.xaml
    /// </summary>
    public partial class LoginSauRegister : Page
    {
        private Frame CurrentFrame;
        public LoginSauRegister(Frame frame)
        {
            InitializeComponent();
            CurrentFrame = frame;
        }

        private void Select_Login_Button_Click(object sender, RoutedEventArgs e)
        {
            LoginOrRegister.Visibility = Visibility.Hidden;
            LoginMenu.Visibility = Visibility.Visible;
        }
        private void Select_Register_Button_Click(object sender, RoutedEventArgs e)
        {
            LoginOrRegister.Visibility = Visibility.Hidden;
            RegisterMenu.Visibility = Visibility.Visible;
        }

        private void EnterINFO_Register_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EnterINFO_Login_Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
