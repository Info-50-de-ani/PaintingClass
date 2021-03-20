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
using PaintingClass.Tabs;
using PaintingClass.Networking;

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

        public UserData userData;
        public bool connected=false; // daca conextiunea initiala cu serverul a fost facuta
        public RoomManager networkManager;// face conexiunea cu server-ul
        
        // trebuie sa folosim ObservableCollection<> in loc de List<> ca sa evitam bug-uri de UI
        ObservableCollection<TabItem> tabs = new ObservableCollection<TabItem>();

        public MainWindow(UserData data)
        {
            //UI
            InitializeComponent();
            instance = this;
            tabControl.ItemsSource = tabs;
            // maximizam window-ul cand se deschide
            WindowState = WindowState.Maximized;

            //pagina de login va genera UserData si il va trimite prin constructor
            userData = data;

            if (userData==null)
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
        public void TestInit()
        {
            AddTab(new MyWhiteboard(), "Tabla mea");
            AddTab(new TestTab(), "test tab");
            AddTab(new TestUI(), "test ui");
        }

        /// <summary>
        /// Init
        /// </summary>
        public async void Init()
        {
            PleaseWait pw = new PleaseWait();
            AddTab(pw,"");

            //todo: conectare la server
            await Task.Delay(1000);
            networkManager = new RoomManager(userData);

            RemoveTab(pw);

            AddTab( new MyWhiteboard(),"Tabla mea");
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
    }
}
