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

            // adaugam GeometryDrawing la tabla
            //whiteboard.collection.Add(geometryDrawing);

            //adaugam niste simple chenare 
            whiteboard.collection.Add(new GeometryDrawing(null, new Pen(Brushes.Black, 1), new RectangleGeometry(new Rect(new Point(10, 10), new Point( 20, 20)))));
            whiteboard.collection.Add(new GeometryDrawing(null, new Pen(Brushes.Blue, 1), new RectangleGeometry(new Rect( new Point(40, 50), new Point(60, 70)))));
            whiteboard.collection.Add(new GeometryDrawing(null, new Pen(Brushes.Green, 1), new RectangleGeometry(new Rect(new Point(80, 70), new Point(120, 130)))));

            //un GeometryGroup contine mai multe geometrii
            GeometryGroup geometryGroup = new GeometryGroup();

            //adaugam doua elipse (geometrii)
            geometryGroup.Children.Add(
                new EllipseGeometry(new Point(70, 30), 20, 5)
                );
            geometryGroup.Children.Add(
                new EllipseGeometry(new Point(70, 30), 5, 20)
                );

            // un GeometryDrawing contine un Geometry, un Brush (umplutura geometriei) si un Pen(conturul geometriei)
            GeometryDrawing geometryDrawing = new GeometryDrawing();

            geometryDrawing.Geometry = geometryGroup;
            geometryDrawing.Pen = new Pen(Brushes.Black, 1);

            // folosim un gradient
            geometryDrawing.Brush =
                new LinearGradientBrush(
                    Colors.Blue,
                    Color.FromRgb(204, 204, 255),
                    new Point(0, 0),
                    new Point(1, 1));

            //adaugam desenul
            whiteboard.collection.Add(geometryDrawing);
        }

        private void AddTab_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.instance.AddTab(new TestTab(),"Test Tab");
        }

        private void RemoteTab_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.instance.RemoveTab(this);
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (whiteboard.collection.Count > 0)
                whiteboard.collection.RemoveAt(whiteboard.collection.Count - 1);
        }
    }
}
