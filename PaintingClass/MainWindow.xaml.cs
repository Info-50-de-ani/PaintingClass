﻿using System;
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
using System.ComponentModel;
using PaintingClass.Tabs;
using PaintingClass.Networking;
using PaintingClass.PaintTools;
using PaintingClassCommon;
using System.Text.Json;

namespace PaintingClass
{
    public class UserData
    {
        //https://docs.google.com/document/d/1h2vGUe8lYZHYZIrdqsPBZkuaF7gZo-0efA72nLhFwVs/edit
        
        //ales the user
        public string name;
        
        //generat de client
        public int clientID;
        
        //0 inseamna ca nu e profesor
        public int profToken=0;

        //roomId
        public int roomId;
        public bool isTeacher { get => profToken != 0; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		// o sa avem mereu o singura instanta a MainWindow pe care o accesam folosind aceasta variabila statica
		public static MainWindow instance;
        public static UserData userData;
        public RoomManager roomManager;
        public Action<bool,MainWindow> OnConnect;
        public Action OnPaste;
        //can be null
        public TeachersTab teachersTab;
        public MyWhiteboard myWhiteboard;

        // trebuie sa folosim ObservableCollection<> in loc de List<> ca sa evitam bug-uri de UI
        ObservableCollection<TabItem> tabs = new ObservableCollection<TabItem>();

        public MainWindow(UserData data, Action<bool, MainWindow> OnConnectEventHandler = null)
        {
			//UI
			InitializeComponent();
            OnConnect = OnConnectEventHandler;
            this.DataContext = this;
            instance = this;
            tabControl.ItemsSource = tabs;
            // maximizam window-ul cand se deschide
            WindowState = WindowState.Maximized;

            //pagina de login va genera UserData si il va trimite prin constructor
            userData = data;
            if (userData == null)
            {
                TestInit();
            }
            else
            {
                Init();
            }

        }


        /// <summary>
        /// Pt testare, ruleaza cand userData este null
        /// </summary>
        void TestInit()
        {
            myWhiteboard = new MyWhiteboard();
            AddTab(myWhiteboard, "Tabla mea");
            AddTab(new TestTab(), "test tab");
        }

        /// <summary>
        /// Init, am incercat sa folosesc programare asincrona ca sa o invat putin
        /// </summary>
        async void Init()
        {
            PleaseWait pw = new PleaseWait();
            AddTab(pw,"");
            
            if (userData.roomId==0)
            {
                if (!userData.isTeacher)
                    throw new Exception("roomId not set but can't create a new room because user is not a teacher");

                while (userData.roomId == 0)
                {
                    //cream o noua incapere
                    userData.roomId = await CreateRoom.SendRequest(userData.profToken);

                    if (userData.roomId == 0)
                        System.Diagnostics.Trace.WriteLine("roomId nu poate fi 0, incercam din nou");
                }
            }

            Clipboard.SetText(userData.roomId.ToString());
            roomManager = new RoomManager(userData);
            Closing+=(sender,args)=> roomManager?.ws.Close();
            OnConnect?.Invoke(roomManager.ws.IsAlive, this);
            roomManager.onUserListUpdate += UserListUpdate;
            roomManager.onUserListUpdate();//reparam bug

            RemoveTab(pw);

            if (userData.isTeacher)
            {
                teachersTab = new();
                AddTab(teachersTab, "Tablele Elevilor");
            }
            myWhiteboard = new();
            AddTab(myWhiteboard, "Tabla mea");

            // afisare lista participanti 
            MainTabControl.Visibility = Visibility.Visible;
        }

        void UserListUpdate()
        {
            Dispatcher.Invoke(() =>
            {
                UserListBox.ItemsSource = new ObservableCollection<NetworkUser>(
                    from nu in roomManager.userList.Values
                    where nu.isConnected
                    select nu);
            });
        }


        public void AddTab(UserControl tabuc, string title)
        {
            TabItem ti = new TabItem();
            ti.Header = title;
            ti.Content = tabuc;

            tabs.Add(ti);

            //daca este primul tab adaugat atunci il selectam noi ca utilizatorul sa nu trebuiasca
            if (tabs.Count == 1) tabControl.SelectedIndex = 0;
        }

        public void RemoveTab(UserControl tabuc)
        {
            for (int i=0;i<tabs.Count;i++)
            {
                if (tabs[i].Content==tabuc)
                {
                    tabs.RemoveAt(i);
                    break;
                }
            }
        }

        private void Kick_MouseDown(object sender, RoutedEventArgs e)
        {
            if (userData.isTeacher==false)
            {
                MessageBox.Show("Nu ai voie!","PaintingClass",MessageBoxButton.OK);
                return;
            }

            NetworkUser nu = (sender as FrameworkElement)?.DataContext as NetworkUser;
            if (nu!=null)
            {
                string request = JsonSerializer.Serialize(new KickRequestMessage { clientId = nu.clientId });
                MainWindow.instance.roomManager.SendMessage(Packet.Pack(PacketType.KickRequestMessage,request));
            }
        }
	}
}
