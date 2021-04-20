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

		#region Fields 

		ImageDrawing image;
        BitmapImage bmp;
        
        /// <summary>
        /// Contine doua gridSplittere cu ajutorul carora userul poate da resize la imagine.
        /// Important: Este instanta o singura data in constructor 
        /// </summary>
        Grid resizeGrid;
        
        Point mouseOffset;
        Size size;

        /// <summary>
        /// Este true atunci cand este inca in stadiul de mutare a pozei
        /// cand acesta apasa pe langa poza se va face false insa daca apasa pe poza 
        /// acesta ramane true
        /// </summary>
        bool isMoving;

        #endregion

        #region private Methods 

        /// <summary>
        /// Folosit pentru a da resize la imagine 
        /// </summary>
        /// <param name="size">dimensiunea normalizata in pixeli fata de tabla </param>
        /// <param name="img">image la care trebuie dat resize</param>
        /// <returns></returns>
        private Grid GetImageResizer()
        {

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(9) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(9) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0) });

            GridSplitter gsVert = new GridSplitter() { Height = 9, HorizontalAlignment = HorizontalAlignment.Stretch,Background = Brushes.Gray};
            GridSplitter gsHorz = new GridSplitter() { Width = 9, HorizontalAlignment = HorizontalAlignment.Stretch ,Background = Brushes.Gray};
            grid.SizeChanged += ImageResizer_SizeChanged;

            Canvas dummy = new Canvas() { MinWidth = 100, MinHeight = 100, };
            Grid.SetRow(dummy, 0);
            Grid.SetColumn(dummy, 0);

            Grid.SetRow(gsVert, 1);
            Grid.SetColumn(gsVert, 0);

            Grid.SetRow(gsHorz, 0);
            Grid.SetColumn(gsHorz, 1);

            grid.Children.Add(gsVert);
            grid.Children.Add(gsHorz);
            grid.Children.Add(dummy);

            return grid;
        }

		private void ImageResizer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (image != null)
            {
                Point offset = TextTool.CalculateOffset(owner);
                Canvas canvas = resizeGrid.Children.OfType<Canvas>().First();
                canvas.Height = image.Rect.Height / 100 * owner.myWhiteboardViewBox.ActualHeight;
                canvas.Width = image.Rect.Width / 100 * owner.myWhiteboardViewBox.ActualWidth;
                var gridPos = whiteboard.DenormalizePosition(image.Rect.TopLeft);
                Canvas.SetTop(resizeGrid,gridPos.Y * owner.myWhiteboardViewBox.ActualHeight + offset.Y);
                Canvas.SetLeft(resizeGrid,gridPos.X*owner.myWhiteboardViewBox.ActualWidth + offset.X);

                size = new Size(resizeGrid.ColumnDefinitions[0].Width.Value / owner.myWhiteboardViewBox.ActualWidth * 100, resizeGrid.RowDefinitions[0].Height.Value / owner.myWhiteboardViewBox.ActualHeight * 100);
                image.Rect = new Rect(image.Rect.TopLeft,size );

            }
        }

        #endregion

        #region Event Handlers 

        /// <summary>
        /// Are loc cand un tool este selectat
        /// </summary>
        /// <param name="tool"></param>
        public void SelectToolEventHandler(PaintTool tool)
		{
            // dam unsubscribe la event atunci cand toolul este deselectat
            if(tool is ImageTool)
			{
                if(resizeGrid==null) 
				{
					// are loc numai odata 
					owner.myWhiteboardGrid.MouseRightButtonDown += Whiteboard_RightClick;
					owner.myWhiteboardGrid.SizeChanged += MyWhiteboardViewBox_SizeChanged;
                    resizeGrid = GetImageResizer();
                    owner.myWhiteboardCanvas.Children.Add(resizeGrid);
                    resizeGrid.Visibility = Visibility.Hidden;
                }
                owner.whiteboard.MouseMove += Whiteboard_MouseMove;
            }
            else owner.whiteboard.MouseMove -= Whiteboard_MouseMove;

        }

		#region Insert Image

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
                    resizeGrid.Visibility = Visibility.Hidden;
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
                    resizeGrid.Visibility = Visibility.Visible;
                    size = new Size(defaultImageSize / bmp.Height * bmp.Width /16*9f, defaultImageSize) ;
                    var rect = new Rect(position, size);
                    image = new ImageDrawing(bmp,rect);
                    whiteboard.collection.Add(image);
                    resizeGrid.ColumnDefinitions[0].Width = new GridLength(size.Width / 100 * owner.myWhiteboardViewBox.ActualWidth);
                    resizeGrid.RowDefinitions[0].Height = new GridLength(size.Height/100*owner.myWhiteboardViewBox.ActualHeight);
                    ImageResizer_SizeChanged(null,null);
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
            resizeGrid.Visibility = Visibility.Visible;
            size = new Size(defaultImageSize / bmp.Height * bmp.Width / 16 * 9f, defaultImageSize);
            var rect = new Rect(position, size);
            image = new ImageDrawing(bmp, rect);
            whiteboard.collection.Add(image);
            resizeGrid.ColumnDefinitions[0].Width = new GridLength(bmp.Width);
            resizeGrid.RowDefinitions[0].Height = new GridLength(bmp.Height);
            ImageResizer_SizeChanged(null, null);
        }

		#endregion

		/// <summary>
		/// Context menu pentru imagine 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Whiteboard_RightClick(object sender, MouseButtonEventArgs e)
		{
            Point position = whiteboard.TransformPosition(e.GetPosition(whiteboard));
            if (image != null)
				if ((position.X > image.Rect.X && position.X < image.Rect.X + image.Rect.Width) && (position.Y > image.Rect.Y && position.Y < image.Rect.Y + image.Rect.Height))
				{
                    var cm = new ContextMenu();
                    var mi = new MenuItem() { Header = "Delete" };
                    mi.Click += (sender, e) =>
                    {
                        // delete the foto
                        isMoving = false;
                        whiteboard.collection.Remove(image);
                        image = null;
                        bmp = null;
                        mouseOffset = new Point(0, 0);
                        size = new Size(0, 0);
                        resizeGrid.Visibility = Visibility.Hidden;
                        Mouse.OverrideCursor = null;
                    };
                    cm.Items.Add(mi);
                    cm.PlacementTarget = whiteboard;
                    cm.IsOpen = true;
				}
            
		}

		#region Movement

		/// <summary>
		/// Se produce atunci cand windowul isi ia resize
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MyWhiteboardViewBox_SizeChanged(object sender, SizeChangedEventArgs e)
		{
            // updatam de fiecre data cand windowul este resized 
            resizeGrid.ColumnDefinitions[0].Width = new GridLength(size.Width / 100 * owner.myWhiteboardViewBox.ActualWidth);
            resizeGrid.RowDefinitions[0].Height = new GridLength(size.Height / 100 * owner.myWhiteboardViewBox.ActualHeight);
            ImageResizer_SizeChanged(null, null);
        }

		/// <summary>
		/// Folosit pentru a da display la cursorul pt size 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Whiteboard_MouseMove(object sender, MouseEventArgs e)
		{
            Point position = whiteboard.TransformPosition(e.GetPosition(whiteboard));
            
            if(image!=null)
            if ((position.X > image.Rect.X && position.X < image.Rect.X + image.Rect.Width) && (position.Y > image.Rect.Y && position.Y < image.Rect.Y + image.Rect.Height) )
            {
                Mouse.OverrideCursor = Cursors.SizeAll;               
            }
            else 
			{
                Mouse.OverrideCursor = null;
                mouseOffset = new Point(0,0);
            }
        }

        /// <summary>
        /// Folosit pentru a modifica pozitia imagini cand userul da 
        /// ii drag
        /// </summary>
        /// <param name="position"></param>
		public override void MouseDrag(Point position)
        {
            if(image != null)
            if ((position.X > image.Rect.X && position.X < image.Rect.X + image.Rect.Width) && (position.Y > image.Rect.Y && position.Y < image.Rect.Y + image.Rect.Height))
			{
                image.Rect = new Rect(new Point(position.X + mouseOffset.X, position.Y + mouseOffset.Y), size);
                ImageResizer_SizeChanged(null, null);
            }          
        }

		#endregion

		#endregion

	}
}
