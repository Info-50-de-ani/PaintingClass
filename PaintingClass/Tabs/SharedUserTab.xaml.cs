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
using PaintingClass.Networking;

namespace PaintingClass.Tabs
{
    /// <summary>
    /// Interaction logic for SharedUserTab.xaml
    /// </summary>
    public partial class SharedUserTab : UserControl
    {
        public SharedUserTab(NetworkUser nu)
        {
            InitializeComponent();
            networkuserWhiteboard.DataContext = nu;
        }
    }
}
