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
using Microsoft.Win32;
using System.IO;

namespace PaintingClass.PaintTools
{
	public class ImageTool : PaintTool
	{
        const int defaultImageSize = 50;

        public override int priority => 3;

        public override Control GetControl()
        {
            Label label = new Label();
            label.Content = "Image";
            return label;
        }

	
		ImageDrawing image;
        BitmapImage bmp;
        Point mouseOffset;
        Size size;
		bool isMoving;

		public override void MouseDown(Point position)
		{
            if (image != null)
                if ((position.X > image.Rect.X && position.X < image.Rect.X + image.Rect.Width) && (position.Y > image.Rect.Y && position.Y < image.Rect.Y + image.Rect.Height))
                {
                    isMoving = true;
                    mouseOffset = new Point(image.Rect.X - position.X, image.Rect.Y - position.Y);
                }
                else isMoving = false;

            if (mouseOffset.X == 0 && mouseOffset.Y == 0 && !isMoving)
			{
                if(image!=null )
				{
                    image.Freeze();
                    MainWindow.instance.roomManager.PackAndSend(PaintingClassCommon.PacketType.WhiteboardMessage, MessageUtils.SerialzieDrawing(image));
                    image = null;
                    bmp = null;
                    mouseOffset = new Point(0, 0);
                    size = new Size(0, 0);
                    return;
				}

                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image files(*.bmp, *.jpg, *.png) | *.bmp; *.jpg; *.png" ;
                dialog.ShowDialog();
                if(File.Exists(dialog.FileName) == true)
			    {
                    try
				    {
                        bmp = new BitmapImage(new Uri(dialog.FileName));
				    }
                    catch
				    {
                        MessageBox.Show("Fisierul nu are un format valid","Eroare");
				    }
			    }
                if(bmp != null)
			    {
                    size = new Size(defaultImageSize / bmp.Width * bmp.Height / 9f * 16f, defaultImageSize) ;
                    var rect = new Rect(position, size);
                    image = new ImageDrawing(bmp,rect);
                    whiteboard.collection.Add(image);
			    }
            }
        }

        public void SelectToolEventHandler(PaintTool tool)
		{
            if(tool is ImageTool)
			{
                owner.whiteboard.MouseMove += Whiteboard_MouseMove;
            }
            else owner.whiteboard.MouseMove -= Whiteboard_MouseMove;
        }

		private void Whiteboard_MouseMove(object sender, MouseEventArgs e)
		{
            Point position = owner.whiteboard.TransformPosition(e.GetPosition(owner.whiteboard));
            Window.GetWindow(myWhiteboardGrid).Title = $"x:{position.X} y:{position.Y}";
            
            if(image!=null)
            if ((position.X > image.Rect.X && position.X < image.Rect.X + image.Rect.Width) && (position.Y > image.Rect.Y && position.Y < image.Rect.Y + image.Rect.Height) )
            {
                Mouse.OverrideCursor = Cursors.SizeAll;               
            }
            else 
			{
                Mouse.OverrideCursor = Cursors.Arrow;
                mouseOffset = new Point(0,0);
            }
        }

		public override void MouseDrag(Point position)
        {
            if ((position.X > image.Rect.X && position.X < image.Rect.X + image.Rect.Width) && (position.Y > image.Rect.Y && position.Y < image.Rect.Y + image.Rect.Height))
			{
                if(image != null)
				{
                    image.Rect = new Rect(new Point(position.X + mouseOffset.X, position.Y + mouseOffset.Y), size);
                }
            }          
        }
        public override void MouseUp()
        {

        }
    }
}
