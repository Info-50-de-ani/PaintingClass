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

namespace PaintingClass
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // o sa avem mereu o singura instanta a MainWindow pe care o accesam folosind aceasta variabila statica
        public static MainWindow instance;
        
        // trebuie sa folosim ObservableCollection<> in loc de List<> ca sa evitam bug-uri de UI
        ObservableCollection<TabItem> tabs = new ObservableCollection<TabItem>();

        public MainWindow()
        {
            InitializeComponent();
            instance = this;
            tabControl.ItemsSource = tabs;

            // maximizam window-ul cand se deschide
            WindowState = WindowState.Maximized;

            //pt testing
            AddTab(new TestTab(),"test tab");
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
