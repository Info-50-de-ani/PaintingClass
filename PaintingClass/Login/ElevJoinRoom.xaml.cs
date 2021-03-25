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
    /// Interaction logic for ElevJoinRoom.xaml
    /// </summary>
    public partial class ElevJoinRoom : Page
    {
        Frame CurrentFrame;
        public ElevJoinRoom(Frame frame)
        {
            InitializeComponent();
            CurrentFrame = frame;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UserData ud = new UserData
            {
                name = name.Text,
                roomId = int.Parse(roomId.Text),
                clientID = PaintingClass.Storage.Settings.instance.clientID,
                profToken=0
            };

            (new MainWindow(ud)).Show();
            Window.GetWindow(CurrentFrame).Close();
        }
    }
}
