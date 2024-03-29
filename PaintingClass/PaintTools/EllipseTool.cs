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
using PaintingClass.Networking;

namespace PaintingClass.PaintTools
{
    class EllipseTool : PaintTool
    {
        public override int priority => 4;

        public override Control GetControl()
        {
            var cc = new ContentControl() { Height = 40 };
            Image image = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Tools/circle.png")) };
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
            cc.Content = image;
            cc.ToolTip = "Tine SHIFT apasat pentru a desena un cerc";
            return cc;
        }


        GeometryDrawing drawing;
        EllipseGeometry ellipse;
        Point initialPos;
        public override void MouseDown(Point position)
        {
            initialPos = position;
            ellipse = new EllipseGeometry(new Rect(position, position));
            drawing = new GeometryDrawing(null, new Pen(this.owner.globalBrush, owner.globalBrushThickness), ellipse);
            whiteboard.drawingCollection.Add(drawing);
        }


        public override void MouseDrag(Point position)
        {
            //cerc
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                double normx = position.X - initialPos.X;
                double normy = (double)whiteboard.Height / whiteboard.Width * (position.Y - initialPos.Y);
                if (Math.Abs(normy) > Math.Abs(normx))
                {//x
                    if (initialPos.Y - position.Y < 0)
                    {
                        ellipse.Center = new Point(initialPos.X + (position.X - initialPos.X) / 2, (initialPos.Y + whiteboard.Width / whiteboard.Height * Math.Abs(position.X - initialPos.X) / 2));
                        ellipse.RadiusX = (position.X - initialPos.X) / 2;
                        ellipse.RadiusY = ellipse.RadiusX * whiteboard.Width / whiteboard.Height;
                    }
                    else
                    {
                        ellipse.Center = new Point(initialPos.X + (position.X - initialPos.X) / 2, (initialPos.Y - whiteboard.Width / whiteboard.Height * Math.Abs(position.X - initialPos.X) / 2));
                        ellipse.RadiusX = (position.X - initialPos.X) / 2;
                        ellipse.RadiusY = ellipse.RadiusX * whiteboard.Width / whiteboard.Height;
                    }
                }
                else
                {//y
                    if (initialPos.X - position.X < 0)
                    {
                        ellipse.Center = new Point( (initialPos.X + whiteboard.Height / whiteboard.Width * Math.Abs(position.Y - initialPos.Y) / 2), initialPos.Y + (position.Y - initialPos.Y) / 2);
                        ellipse.RadiusY = (position.Y - initialPos.Y) / 2;
                        ellipse.RadiusX = ellipse.RadiusY * whiteboard.Height / whiteboard.Width;
                    }
                    else
                    {
                        ellipse.Center = new Point((initialPos.X - whiteboard.Height / whiteboard.Width * Math.Abs(position.Y - initialPos.Y) / 2), initialPos.Y + (position.Y - initialPos.Y) / 2);
                        ellipse.RadiusY = (position.Y - initialPos.Y) / 2;
                        ellipse.RadiusX = ellipse.RadiusY * whiteboard.Height / whiteboard.Width;
                    }
                }
            }
            else // elipsa
            {
                ellipse.Center = new Point(initialPos.X + (position.X - initialPos.X) / 2, initialPos.Y + (position.Y - initialPos.Y) / 2);
                ellipse.RadiusX = (position.X - initialPos.X) / 2;
                ellipse.RadiusY = (position.Y - initialPos.Y) / 2;
            }
        }
        public override void MouseUp()
        {
            ellipse.Freeze();//extra performanta
            MessageUtils.SendNewDrawing(drawing, whiteboard.drawingCollection.Count - 1);
            ellipse = null;
        }
    }
}

