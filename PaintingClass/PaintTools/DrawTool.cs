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

        /// <summary>
        /// Incepe sa deseneze o noua linie
        /// </summary>
        /// <param name="position"></param>
        public override void MouseDown(Point position)
        {
            // un nou path figure pentru a adauga segemnte
            figure = new PathFigure();
            figure.StartPoint = position;


            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            
            // punem totul in drawing collection 
            drawing = new GeometryDrawing();
            drawing.Pen = new Pen(owner.globalBrush, owner.thickness);
            drawing.Geometry = geometry;
            whiteboard.collection.Add(drawing);
        }

        /// <summary>
        /// cand userul misca mousul adaugam segmente 
        /// </summary>
        /// <param name="position"></param>
        public override void MouseDrag(Point position)
        {
            /// Setam proprietatea IsSmoothJoin la true si astfel 
            /// cand unghiul este prea mic nu vor mai aparea aberatii 
            figure.Segments.Add(new LineSegment(position, true) { IsSmoothJoin=true });
        }

        /// <summary>
        /// Finalizam editatul pathGeometry si il trimitem la server
        /// </summary>
        public override void MouseUp()
        {
            drawing.Freeze();//extra performanta
            MessageUtils.SendNewDrawing(drawing, whiteboard.collection.Count-1);
            drawing = null;
            figure = null;
        }
    }
}
