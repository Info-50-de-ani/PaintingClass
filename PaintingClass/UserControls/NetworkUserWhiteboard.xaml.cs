using System;
using System.Collections.Generic;
using System.Globalization;
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
using PaintingClass.Networking;
using PaintingClassCommon;

namespace PaintingClass.UserControls
{
    public partial class NetworkUserWhiteboard : UserControl
    {
        NetworkUser nu;
        public NetworkUserWhiteboard()
        {
            InitializeComponent();

            if (MainWindow.userData==null || MainWindow.userData.isTeacher==false)
            {
                ((Grid)shareButton.Parent).Children.Remove(shareButton);
                shareButton = null;
            }

            DataContextChanged += (sender,args) =>
            {
                nu = args.NewValue as NetworkUser;
                if (nu!=null)
                {
                    whiteboard.collection = nu.collection;
                }
            };
        }

        private void shareButton_Click(object sender, RoutedEventArgs e)
        {
            if (nu==null) return;
            ShareRequestMessage srm = new() { clientId = nu.clientId, isShared = !nu.isShared };
            MainWindow.instance.roomManager.PackAndSend<ShareRequestMessage>(PacketType.ShareRequestMessage, srm);
        }
    }

    public class ShareTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return "Stop sharing";
            }
            else
            {
                return "Share";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
