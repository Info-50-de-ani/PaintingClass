using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PaintingClass.Tabs
{
    /// <summary>
    /// Interaction logic for MultipleWhiteboardView.xaml
    /// </summary>
    public partial class TeachersTab : UserControl
    {
        //in viitor valoarea ar trb calculata
        const int whiteboardsPerLine = 4;

        /// <summary>
        /// trebuie adaugata o noua clasa ce cotine si informatia despre table
        /// </summary>
        public TeachersTab()
        {
            InitializeComponent();
            inviteLink.Text = $"{Networking.Constants.customProtocol}://{MainWindow.userData.roomId}";
            MainWindow.instance.roomManager.onUserListUpdate += () =>
            {
                //probleme de multithreading
                if (rootItemsControl.Dispatcher.CheckAccess())
                    UpdateItemsSource();
                else
                    rootItemsControl.Dispatcher.InvokeAsync(UpdateItemsSource);
            };

            // PT TEST
            //for (int i=0;i<=20;i++)
            //{
            //    MainWindow.instance.roomManager.userList.Add(100 + i, new NetworkUser { name = $"user{i}" });
            //}
            //MainWindow.instance.roomManager.onUserListUpdate?.Invoke();
        }

        void UpdateItemsSource()
        {
            ObservableCollection<ObservableCollection<NetworkUser>> list = new();
            rootItemsControl.ItemsSource = list;
            int i = whiteboardsPerLine;

            foreach (NetworkUser networkUser in MainWindow.instance.roomManager.userList.Values)
            {
                if (i == whiteboardsPerLine)
                {
                    ObservableCollection<NetworkUser> nl = new();
                    nl.Add(networkUser);
                    list.Add(nl);
                    i = 1;
                }
                else
                {
                    list.Last().Add(networkUser);
                    i++;
                }
            }
        }
    }
}
