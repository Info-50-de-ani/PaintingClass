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
using System.Reflection;
using PaintingClass.PaintTools;
using PaintingClass.Networking;
using Microsoft.Win32;
using System.IO;

namespace PaintingClass.PaintTools
{
    /// <summary>
    /// Folosit pentru a afisa imagini pe tabla 
    /// si pentru ale trimite
    /// </summary>
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

        /// <summary>
        /// Este true atunci cand este inca in stadiul de mutare a pozei
        /// cand acesta apasa pe langa poza se va face false insa daca apasa pe poza 
        /// acesta ramane true
        /// </summary>
		bool isMoving;

        /// <summary>
        /// Deschide un open file dialog atunci cand userul apasa pe tabla cu toolul acesta selectat
        /// </summary>
        /// <param name="position"></param>
		public override void MouseDown(Point position)
		{
            if (image != null) /// see <see cref="isMoving"/>
                if ((position.X > image.Rect.X && position.X < image.Rect.X + image.Rect.Width) && (position.Y > image.Rect.Y && position.Y < image.Rect.Y + image.Rect.Height))
                {
                    isMoving = true;
                    mouseOffset = new Point(image.Rect.X - position.X, image.Rect.Y - position.Y);
                }
                else isMoving = false;

            // daca userul nu mai este in stadiul de mutare a pozei ...
            if (mouseOffset.X == 0 && mouseOffset.Y == 0 && !isMoving)
			{
                // daca imaginea exista 
                if(image!=null )
				{
                    // poza va fi trimisa la server 
                    image.Freeze();
                    MainWindow.instance.roomManager.PackAndSend(PaintingClassCommon.PacketType.WhiteboardMessage, MessageUtils.SerialzieDrawing(image));
                    image = null;
                    bmp = null;
                    mouseOffset = new Point(0, 0);
                    size = new Size(0, 0);
                    return;
				}

                // altfel se va deschide un dialog prin care userul poate alege
                // o noua poza
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image files(*.bmp, *.jpg, *.png) | *.bmp; *.jpg; *.png" ;
                dialog.ShowDialog();
                if(File.Exists(dialog.FileName) == true)
			    {
                    try
				    {
                        bmp = new BitmapImage(new Uri(dialog.FileName));
				    }
                    catch // daca este vreo eroare informam userul
				    {
                        MessageBox.Show("Fisierul nu are un format valid","Eroare");
                        return;
				    }
			    }

                // daca bitmapul a fost fct cu succes il afisam pe tabla 
                if(bmp != null)
			    {
                    size = new Size(defaultImageSize / bmp.Width * bmp.Height / 9f * 16f, defaultImageSize) ;
                    var rect = new Rect(position, size);
                    image = new ImageDrawing(bmp,rect);
                    whiteboard.collection.Add(image);
			    }
            }
        }

        /// <summary>
        /// Cand userul face drag and drop in aplicatie cu o poza
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnDropEventHandler(object sender, DragEventArgs e)
		{
            // normalizam pozitia
            Point position = owner.whiteboard.TransformPosition(e.GetPosition(owner.whiteboard));

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Count() > 1)
                return;

            var ext = Path.GetExtension(files[0]);
            
            if (!(ext == ".png" || ext == ".bmp" || ext == ".jpg") || !File.Exists(files[0]))
                return;
            // selectam toolul de imagine 
            owner.selectedTool = this;

            // daca inainte a mai fost o imagine o trimitem 
            if(image != null)
			{
                image.Freeze();
                MainWindow.instance.roomManager.PackAndSend(PaintingClassCommon.PacketType.WhiteboardMessage, MessageUtils.SerialzieDrawing(image));
                image = null;
                bmp = null;
            }

            bmp = new BitmapImage(new Uri(files[0]));
            // pentru a lasa userul sa mute poza dupa ce i-a dat drop
            isMoving = true;

            // introducem noua imagine in tabla
            size = new Size(defaultImageSize / bmp.Width * bmp.Height / 9f * 16f, defaultImageSize);
            var rect = new Rect(position, size);
            image = new ImageDrawing(bmp, rect);
            whiteboard.collection.Add(image);
        }

        public void SelectToolEventHandler(PaintTool tool)
		{
            // dam unsubscribe la event atunci cand toolul este deselectat
            if(tool is ImageTool)
			{
                owner.whiteboard.MouseMove += Whiteboard_MouseMove;
            }
            else owner.whiteboard.MouseMove -= Whiteboard_MouseMove;

        }

        /// <summary>
        /// Folosit pentru a da display la cursorul pt size 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
