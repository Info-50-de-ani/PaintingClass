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

namespace PaintingClass.UserControls
{
    /// <summary>
    /// Interaction logic for LimitedLengthTextBox.xaml
    /// </summary>
    public partial class LimitedLengthTextBox : UserControl
    {
        public int MaxTextLength { get; set; } = 0;
        public LimitedLengthTextBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
