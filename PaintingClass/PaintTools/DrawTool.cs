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
using PaintingClass.Networking;

namespace PaintingClass.PaintTools
{
    /// <summary>
    /// Unealta exemplu
    /// </summary>
    class DrawTool : PaintTool
    {
        public override int priority => 0;

        public override Control GetControl()
        {
            Label label = new Label();
            label.Content = "Draw";
            return label;
        }


        GeometryDrawing drawing;
        PathFigure figure;
        PolyBezierSegment poliBezierSegment;
        public int pointCounter = 0;

        /// <summary>
        /// Incepe sa deseneze o noua linie
        /// </summary>
        /// <param name="position"></param>
        public override void MouseDown(Point position)
        {
            // un nou path figure pentru a adauga segemnte
            figure = new PathFigure();
            figure.StartPoint = position;
            // resetam point counter
            pointCounter = 1;

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            
            // array gol pentru a putea initializa cu contructorul ce 
            // ia doua argumente
            Point[] startPos = new Point[] { position };
            // adaugam un singur segment si apoi ii adaugam puncte
            poliBezierSegment = new PolyBezierSegment(startPos,true);
            figure.Segments.Add(poliBezierSegment);

            // punem totul in drawing collection 
            drawing = new GeometryDrawing();
            drawing.Pen = new Pen(owner.globalBrush,0.3);
            drawing.Geometry = geometry;
            whiteboard.collection.Add(drawing);
        }

        /// <summary>
        /// Adaugam puncte la bezier din 5 in 5 puncte
        /// </summary>
        /// <param name="position"></param>
        public override void MouseDrag(Point position)
        {
            pointCounter++;
            if (pointCounter % 3 == 0)
                poliBezierSegment.Points.Add(position);
        }

        /// <summary>
        /// Finalizam editatul segmentului si il trimitem la server
        /// </summary>
        public override void MouseUp()
        {
            drawing.Freeze();//extra performanta
            MainWindow.instance.roomManager.PackAndSend(PaintingClassCommon.PacketType.WhiteboardMessage,MessageUtils.SerialzieDrawing(drawing) );
            drawing = null;
            figure = null;
            poliBezierSegment = null;    
        }
    }
}
