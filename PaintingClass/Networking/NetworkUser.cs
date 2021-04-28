using System.Windows.Media;
using System.ComponentModel;
using PaintingClassCommon;
using PaintingClass.Tabs;
using PaintingClass.UserControls;
using System.Windows;

namespace PaintingClass.Networking
{
    //tine informatii legate de alt utilizator
    public class NetworkUser : INotifyPropertyChanged
    {
        int _clientId;
        string _name;
        bool _isConnected;
        bool _isShared;
        SharedUserTab sharedUserTab;

        int _wbItemIndex;
        Whiteboard _whiteboard;

        #region properties
        public int clientId
        {
            get => _clientId;
            set
            {
                _clientId = value;
                OnPropertyChanged(nameof(clientId));
            }
        }
        public string name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(name));
            }
        }
        public bool isConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                OnPropertyChanged(nameof(isConnected));
            }
        }
        public bool isShared
        {
            get => _isShared;
            set
            {
                if (_isShared == value)
                    return;
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (value==true)
                    {
                        if (sharedUserTab==null)
                        {
                            sharedUserTab = new SharedUserTab(this);
                            MainWindow.instance.AddTab(sharedUserTab,$"Tabla lui {name}");
                        }
                    }
                    else
                    {
                        if (sharedUserTab!=null)
                        {
                            MainWindow.instance.RemoveTab(sharedUserTab);
                            sharedUserTab = null;
                        }
                    }
                });
                _isShared = value;
                OnPropertyChanged(nameof(isShared));
            }
        }

        public int wbItemIndex
        {
            get => _wbItemIndex;
            set
            {
                _wbItemIndex = value;
                OnPropertyChanged(nameof(wbItemIndex));
            }
        }
        // foloseste App.Current.Dispatcher.Invoke pentru a folosi obiect-ul
        public Whiteboard whiteboard
        {
            get => _whiteboard;
            set
            {
                _whiteboard = value;
                OnPropertyChanged(nameof(whiteboard));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion properties

        public NetworkUser()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                whiteboard = new();
                whiteboard.Background = Brushes.White;
            });
        }
    }
    
}
