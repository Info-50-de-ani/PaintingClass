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
using System.Reflection;
using PaintingClass.PaintTools;

namespace PaintingClass.PaintTools
{
    /// <summary>
    /// Unealta exemplu
    /// </summary>
    class SimpleTool : PaintTool
    {
        public override int priority => throw new NotImplementedException();

        public override Control GetControl()
        {
            Label label = new Label();
            label.Content = "test";
            return label;
        }


        GeometryDrawing drawing;
        PathFigure figure;

        public override void MouseDown(Point position)
        {
            figure = new PathFigure();
            figure.StartPoint = position;

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            
            drawing = new GeometryDrawing();
            drawing.Pen = new Pen(Brushes.Black,3);
            drawing.Geometry = geometry;
            whiteboard.collection.Add(drawing);
        }

        public override void MouseDrag(Point position)
        {
            figure.Segments.Add(new LineSegment(position,true));
        }

        public override void MouseUp()
        {
            drawing.Freeze();//extra performanta
            drawing = null;
            figure = null;
        }
    }
}
