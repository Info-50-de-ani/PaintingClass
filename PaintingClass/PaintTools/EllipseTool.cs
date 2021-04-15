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
using PaintingClass.Networking;

namespace PaintingClass.PaintTools
{
    class EllipseTool : PaintTool
    {
        public override int priority => 3;

        public override Control GetControl()
        {
            Label label = new Label();
            label.Content = "Circle";
            return label;
        }


        GeometryDrawing geometryDrawing;
        EllipseGeometry ellipse;
        Point initialPos;
        bool isControlPressed = false;
        public override void MouseDown(Point position)
        {
            Window.GetWindow(whiteboard).KeyDown += Whiteboard_KeyDown;
            Window.GetWindow(whiteboard).KeyUp += Whiteboard_KeyUp;
            initialPos = position;
            ellipse = new EllipseGeometry(new Rect(position, position));
            geometryDrawing = new GeometryDrawing(null, new Pen(this.owner.globalBrush, 0.3), ellipse);
            whiteboard.collection.Add(geometryDrawing);
        }

        private void Whiteboard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
                isControlPressed = true;
        }

        private void Whiteboard_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
                isControlPressed = false;
        }

        public override void MouseDrag(Point position)
        {
            //cerc
            if (isControlPressed)
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
            MainWindow.instance.roomManager.PackAndSend(PaintingClassCommon.PacketType.WhiteboardMessage, MessageUtils.SerialzieDrawing(geometryDrawing));
            ellipse.Freeze();//extra performanta
            ellipse = null;
        }
    }
}

