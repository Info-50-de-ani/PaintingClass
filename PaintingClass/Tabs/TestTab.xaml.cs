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

            // un GeometryGroup contine mai multe geometrii
            GeometryGroup ellipses = new GeometryGroup();

            //adaugam patru elipse
            ellipses.Children.Add(
                new EllipseGeometry(new Point(0, 0), 45, 20)
                );
            ellipses.Children.Add(
                new EllipseGeometry(new Point(0, 0), 20, 45)
                );

            ellipses.Children.Add(
                new EllipseGeometry(new Point(50, 50), 45, 20)
                );
            ellipses.Children.Add(
                new EllipseGeometry(new Point(50, 50), 20, 45)
                );



            // un GeometryDrawing contine un Geometry, un Brush (umplutura geometriei) si un Pen(conturul geometriei)
            GeometryDrawing geometryDrawing = new GeometryDrawing();

            geometryDrawing.Geometry = ellipses;
            geometryDrawing.Pen = new Pen(Brushes.Black,2);

            // folosim un gradient
            geometryDrawing.Brush =
                new LinearGradientBrush(
                    Colors.Blue,
                    Color.FromRgb(204, 204, 255),
                    new Point(0, 0),
                    new Point(1, 1));

            // adaugam GeometryDrawing la tabla
            whiteboard.collection.Add(geometryDrawing);

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
