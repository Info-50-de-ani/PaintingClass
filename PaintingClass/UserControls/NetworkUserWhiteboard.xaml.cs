using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
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

            DataContextChanged += (sender, args) =>
            {
                nu = args.NewValue as NetworkUser;
                if (nu != null)
                {
                    //
                }
            };

            IsVisibleChanged += (sender, e) =>
            {
                if (nu==null || whiteboardSlot.Child!=null || (bool)e.NewValue == false || (bool)e.OldValue == true) return;

                //fur copilul!
                if (nu.whiteboard.Parent!=null)
                ((Viewbox)nu.whiteboard.Parent).Child = null;
                whiteboardSlot.Child = nu.whiteboard;
            };
        }

        private void shareButton_Click(object sender, RoutedEventArgs e)
        {
            if (nu==null) return;
            ShareRequestMessage srm = new() { clientId = nu.clientId, isShared = !nu.isShared };
            MainWindow.instance.roomManager.SendMessage(Packet.Pack(PacketType.ShareRequestMessage, JsonSerializer.Serialize(srm)));
        }

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
            whiteboardSlot.Measure(new Size(whiteboardSlot.ActualWidth, whiteboardSlot.ActualHeight));
            whiteboardSlot.Arrange(new Rect(new Point(whiteboardSlotBorder.BorderThickness.Left, whiteboardSlotBorder.BorderThickness.Top),new Size(whiteboardSlot.ActualWidth, whiteboardSlot.ActualHeight)));
            whiteboardSlot.RenderTransform = new ScaleTransform(1080d/ whiteboardSlot.ActualWidth, 608d/ whiteboardSlot.ActualHeight);
            whiteboardSlot.UpdateLayout();
            RenderTargetBitmap bmp = new RenderTargetBitmap(1080, 608, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(whiteboardSlot);
            Clipboard.SetImage(bmp);
            whiteboardSlot.RenderTransform = new ScaleTransform(1, 1);
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
