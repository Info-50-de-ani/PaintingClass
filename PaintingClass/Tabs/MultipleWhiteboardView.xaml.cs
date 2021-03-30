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
using PaintingClass.Networking;
using static PaintingClass.Networking.RoomManager;

namespace PaintingClass.Tabs
{
    /// <summary>
    /// Interaction logic for MultipleWhiteboardView.xaml
    /// </summary>
    public partial class MultipleWhiteboardView : UserControl
    {

        /// <summary>
        /// trebuie adaugata o noua clasa ce cotine si informatia despre table
        /// </summary>
        public Dictionary<int, NetworkUser> userList;
        public MultipleWhiteboardView()
        {
            InitializeComponent();
            userList = MainWindow.instance.roomManager.userList;
            UpdateParticipantCounter();
            TB_RoomCode.Text= MainWindow.userData.roomId.ToString();
            TB_BrowserLink.Text = $"{App.costumUrl}://{MainWindow.userData.roomId}";
        }
        void UpdateParticipantCounter()
        {
            int cntConnected = 0;
            int cntSharing = 0;
            foreach (var user in userList)
            {
                if (user.Value.isConnected)
                    cntConnected++;
                if (user.Value.isShared)
                    cntSharing++;
            }
            TB_particpants.Text = $"Connected: {cntConnected}";
            TB_sharing.Text =$"Sharing: {cntSharing}";
        }
    }
}
