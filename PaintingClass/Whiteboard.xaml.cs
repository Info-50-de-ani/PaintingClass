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
using System.Drawing;
using System.Collections.ObjectModel;

namespace PaintingClass
{
    /// <summary>
    /// Tabla simpla
    /// Foloseste collection pt a adauga si scadea drawing-uri
    /// Foloseste proprietatea Background (mostenita de la Control) pt a seta un Background
    /// </summary>
    public partial class Whiteboard : UserControl
    {
        //aka List<Drawing>
        public DrawingCollection collection { get => group.Children;}
        DrawingGroup group;

        //constructor principal
        public Whiteboard()
        {
            InitializeComponent();

            group = new DrawingGroup();

            //folosim un DrawingImage pentru a renderiza DrawingGroup pe <Image>
            image.Source = new DrawingImage(group);
            image.Stretch = Stretch.None;

        }
    }
}
