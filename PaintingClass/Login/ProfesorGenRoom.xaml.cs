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
        public ProfesorGenRoom(Frame frame)
        {
            InitializeComponent();
        }

        private void GenRoom_Button_Click(object sender, RoutedEventArgs e)
        {
            //User.UserInformation.Name = "Gigel";
            //WS.InitHost("ws://localhost:9000/home",User.UserInformation.Name,Ws_OnOpen);
        }

        private void Ws_OnOpen(object sender, EventArgs e)
        {
            //this.Dispatcher.Invoke(() => { RoomCode.Text = $"Room: {WS.Host.roomCode}"; });
        }
    }
}
