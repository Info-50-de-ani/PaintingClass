using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Collections.ObjectModel;
namespace PaintingClass.Login
{
    /// <summary>
    /// Interaction logic for ProfesorGenRoom.xaml
    /// </summary>
    public partial class ProfesorGenRoom : Page  
    {
        public Frame currentFrame;
        public UserData userData;

        public ProfesorGenRoom(Frame frame,UserData userData)
        {
            InitializeComponent();
            this.userData = userData;
            currentFrame = frame;
        }

        private void GenRoom_Button_Click(object sender, RoutedEventArgs e)
        {
            (new MainWindow(userData)).Show();
            Window.GetWindow(currentFrame).Close();
        }
    }
}
