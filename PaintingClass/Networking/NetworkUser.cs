using System.Windows.Media;
using System.ComponentModel;
using PaintingClassCommon;
using PaintingClass.Tabs;

namespace PaintingClass.Networking
{
    //tine informatii legate de alt utilizator
    public class NetworkUser : INotifyPropertyChanged
    {
        int _clientId;
        string _name;
        bool _isConnected;
        DrawingCollection _collection;
        bool _isShared;

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
        // WARNING: Ca sal accesezi foloseste collection.Dispatcher.Invoke()
        public DrawingCollection collection
        {
            get => _collection;
            set
            {
                _collection = value;
                OnPropertyChanged(nameof(collection));
            }
        }
        public bool isShared
        {
            get => _isShared;
            set
            {
                if (_isShared == value)
                    return;
                if (value==true)
                {
                    var tab = new SharedUserTab(this);
                }
                else
                {

                }
                _isShared = value;
                OnPropertyChanged(nameof(isShared));
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
            App.Current.Dispatcher.Invoke(() => _collection = new());
        }
    }
    
}
