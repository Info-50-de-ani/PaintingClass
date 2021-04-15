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
    /// Interaction logic for ProfesorSauElev.xaml
    /// </summary>
    public partial class ProfesorSauElev : Page
    {
        Frame CurrentFrame;
        public ProfesorSauElev(Frame frame)
        {
            InitializeComponent();
            CurrentFrame = frame;
        }

        private void BT_Elev_Click(object sender, RoutedEventArgs e)
        {
            CurrentFrame.Content = new ElevJoinRoom(CurrentFrame);
        }

        private void BT_Profesor_Click(object sender, RoutedEventArgs e)
        {
            CurrentFrame.Content = new LoginSauRegister(CurrentFrame);
        }

	}
}
