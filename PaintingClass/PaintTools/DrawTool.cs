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
            var cc = new ContentControl() { Height = 40 };
            Image image = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Tools/pen.png")) };
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
            cc.Content = image;
            cc.ToolTip = "Tine apasat click stanga pentru a scrie si click dreapta pentru a sterge";
            return cc;
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
            if(Mouse.LeftButton == MouseButtonState.Pressed)
			{// daca dam draw normal
                drawing.Pen = new Pen(owner.globalBrush, owner.globalBrushThickness);
			}
            else
			{// dam erase cand userul apasa click dreapta si punem un cerc sa aratam cat dam erase 
                drawing.Pen = new Pen(Brushes.White, owner.globalBrushThickness*4);
            }
            drawing.Geometry = geometry;
            whiteboard.drawingCollection.Add(drawing);
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
            MessageUtils.SendNewDrawing(drawing, whiteboard.drawingCollection.Count-1);
            drawing = null;
            figure = null;
        }
    }
}
