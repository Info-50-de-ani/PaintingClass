﻿using System;
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

namespace PaintingClass.Tabs
{
    /// <summary>
    /// Interaction logic for TestTab.xaml
    /// </summary>
    public partial class TestTab : UserControl
    {
        public TestTab()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.instance.AddTab(new TestTab(),"Test Tab");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow.instance.RemoveTab(this);
        }
    }
}
