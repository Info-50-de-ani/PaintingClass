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
    /// <summary>
    /// de desenat dreptunghiuri
    /// </summary>
    class RectangleTool : PaintTool
    {

        public override int priority => 3;

        public override Control GetControl()
        {
            var cc = new ContentControl() { Height = 40 };
            Image image = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Tools/square.png")) };
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
            cc.Content = image;
            return cc;
        }

        GeometryDrawing geometryDrawing;
        RectangleGeometry rectangle;
        Point initialPos;
        public override void MouseDown(Point position)
        {
            initialPos = position;
            rectangle = new RectangleGeometry(new Rect(position,position));
            geometryDrawing = new GeometryDrawing(null, new Pen(owner.globalBrush, owner.globalBrushThickness), rectangle);
            whiteboard.drawingCollection.Add(geometryDrawing);
        }

        public override void MouseDrag(Point position)
        {
            //patrat
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                double normx = position.X - initialPos.X;
                double normy = (double)whiteboard.Height / whiteboard.Width*(position.Y - initialPos.Y);
                if (Math.Abs(normy) > Math.Abs(normx)) 
                {//x
                    if (initialPos.Y - position.Y < 0)
                        rectangle.Rect = new Rect(initialPos, new Point(position.X, (initialPos.Y + (double)whiteboard.Width / whiteboard.Height * Math.Abs(position.X - initialPos.X))));
                    else
                        rectangle.Rect = new Rect(initialPos, new Point(position.X, (initialPos.Y - (double)whiteboard.Width / whiteboard.Height * Math.Abs(position.X - initialPos.X))));
                }
                else
                {//y
                    if (initialPos.X - position.X < 0)
                        rectangle.Rect = new Rect(initialPos, new Point(initialPos.X + (double)whiteboard.Height / whiteboard.Width * Math.Abs(position.Y - initialPos.Y), position.Y));
                    else
                        rectangle.Rect = new Rect(initialPos, new Point(initialPos.X - (double)whiteboard.Height / whiteboard.Width * Math.Abs(position.Y - initialPos.Y), position.Y));
                }
            }
            else // dreptunghi
                rectangle.Rect = new Rect(initialPos, position);
        }

        public override void MouseUp()
        {
            rectangle.Freeze();//extra performanta
            MessageUtils.SendNewDrawing(geometryDrawing, whiteboard.drawingCollection.Count - 1);
            rectangle = null;
        }
    }
}
