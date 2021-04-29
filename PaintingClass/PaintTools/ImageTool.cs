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
using System.Diagnostics;
using PaintingClass.PaintTools.Interfaces;
using System.IO.Packaging;
using System.Data;
using System.Windows.Interop;

namespace PaintingClass.PaintTools
{
    /// <summary>
    /// Folosit pentru a afisa imagini pe tabla 
    /// si pentru ale trimite
    /// </summary>
	public class ImageTool : PaintTool, IToolSelected
    {
        const int defaultImageSize = 50;

        public override int priority => 3;

        public override Control GetControl()
        {
            var cc = new ContentControl() { Height = 40 };
            Image image = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Tools/image.png")) };
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
            cc.Content = image;
            return cc;
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
        bool isMoving = false;
        private int DragDropImagesCnt = 0;
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

            GridSplitter gsVert = new GridSplitter() { Height = 9, HorizontalAlignment = HorizontalAlignment.Stretch, Background = Brushes.Gray };
            GridSplitter gsHorz = new GridSplitter() { Width = 9, HorizontalAlignment = HorizontalAlignment.Stretch, Background = Brushes.Gray };
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

        public void ImageResizer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (image != null)
            {
                Point offset = TextTool.CalculateOffset(owner);
                Canvas canvas = resizeGrid.Children.OfType<Canvas>().First();
                canvas.Height = image.Rect.Height / Whiteboard.sizeY * owner.myWhiteboardViewBox.ActualHeight;
                canvas.Width = image.Rect.Width / Whiteboard.sizeX * owner.myWhiteboardViewBox.ActualWidth;
                var gridPos = whiteboard.DenormalizePosition(image.Rect.TopLeft);
                
                Canvas.SetTop(resizeGrid, gridPos.Y * owner.myWhiteboardViewBox.ActualHeight * owner.myWhiteboardViewBox.RenderTransform.Value.M22 + offset.Y);
                Canvas.SetLeft(resizeGrid, gridPos.X * owner.myWhiteboardViewBox.ActualWidth * owner.myWhiteboardViewBox.RenderTransform.Value.M11 + offset.X);
                resizeGrid.RenderTransform = owner.myWhiteboardViewBox.RenderTransform;

                size = new Size(resizeGrid.ColumnDefinitions[0].Width.Value / owner.myWhiteboardViewBox.ActualWidth * Whiteboard.sizeX, resizeGrid.RowDefinitions[0].Height.Value / owner.myWhiteboardViewBox.ActualHeight * Whiteboard.sizeY);
                image.Rect = new Rect(image.Rect.TopLeft, size);

            }
        }

        /// <summary>
        /// Verifica daca este vreo imagine netrimisa si o trimite 
        /// </summary>
        /// <returns>Daca a fost vreo imagine netrimisa returneaza True, altfel False</returns>
        private bool CheckIfLastImageIsNotSent()
		{
            if (image != null)
            {
                // poza va fi trimisa la server 
                image.Freeze();
                MessageUtils.SendNewDrawing(image, whiteboard.drawingCollection.Count - 1);
                image = null;
                bmp = null;
                mouseOffset = new Point(0, 0);
                size = new Size(0, 0);
                resizeGrid.Visibility = Visibility.Hidden;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Converteste Bmp-ul in image si il afiseaza pe tabla
        /// </summary>
        private void DisplayBmp(Point position)
		{
            isMoving = true;
            resizeGrid.Visibility = Visibility.Visible;
            size = new Size(defaultImageSize / bmp.Height * bmp.Width / 16 * 9f, defaultImageSize);
            var rect = new Rect(position, size);
            image = new ImageDrawing(bmp, rect);
            whiteboard.drawingCollection.Add(image);
            resizeGrid.RenderTransform = owner.myWhiteboardViewBox.RenderTransform;
            resizeGrid.ColumnDefinitions[0].Width = new GridLength(size.Width / Whiteboard.sizeX * owner.myWhiteboardViewBox.ActualWidth);
            resizeGrid.RowDefinitions[0].Height = new GridLength(size.Height / Whiteboard.sizeY * owner.myWhiteboardViewBox.ActualHeight);
            ImageResizer_SizeChanged(null, null);
        }

        #endregion

        #region Event Handlers 

        /// <summary>
        /// Are loc cand un tool este selectat
        /// </summary>
        /// <param name="tool"></param>
        public void SelectToolEventHandler(PaintTool tool)
        {
            if (resizeGrid == null)
            {
                owner.myWhiteboardGrid.MouseRightButtonDown += Whiteboard_RightClick;
                owner.myWhiteboardGrid.SizeChanged += MyWhiteboardViewBox_SizeChanged;
                resizeGrid = GetImageResizer();
                owner.myWhiteboardCanvas.Children.Add(resizeGrid);
                resizeGrid.Visibility = Visibility.Hidden;
                owner.whiteboard.MouseMove += Whiteboard_MouseMove;
            }
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
                if (CheckIfLastImageIsNotSent())
                    return;

                // altfel se va deschide un dialog prin care userul poate alege
                // o noua poza
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image files(*.bmp, *.jpg, *.png) | *.bmp; *.jpg; *.png";
                dialog.ShowDialog();
                if (File.Exists(dialog.FileName) == true)
                {
                    try
                    {
                        bmp = new BitmapImage(new Uri(dialog.FileName));
                    }
                    catch // daca este vreo eroare informam userul
                    {
                        MessageBox.Show("Fisierul nu are un format valid", "Eroare");
                        return;
                    }
                }

                // daca bitmapul a fost fct cu succes il afisam pe tabla 
                if (bmp != null)
                {
                    DisplayBmp(position);
                }
            }
        }


        Uri Create_Memory_Resource_Uri(MemoryStream ms)
        {
            if(DragDropImagesCnt == 0 && Directory.Exists(Directory.GetCurrentDirectory() + "\\ImageCache"))
                Directory.Delete(Directory.GetCurrentDirectory() + "\\ImageCache",true);
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\ImageCache");
            string path = Directory.GetCurrentDirectory() + $"\\ImageCache\\ImageTransfer{DragDropImagesCnt++}";
            using FileStream file = File.OpenWrite(path);
            file.Write(ms.ToArray());
            return new Uri(path);
        }

        /// <summary>
        /// Cand userul face drag and drop in aplicatie cu o poza
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnDropEventHandler(object sender, DragEventArgs e)
        {
            CheckIfLastImageIsNotSent();
            // normalizam pozitia
            Point position = whiteboard.TransformPosition(e.GetPosition(owner.whiteboard));
            owner.selectedTool = this;
            MemoryStream ms;
            // daca imaginea este prim ita ca un fisier bitmap
            if ((ms = (MemoryStream)e.Data.GetData("Object")) != null)
            {
                isMoving = true;
                resizeGrid.Visibility = Visibility.Visible;
                ms.Position = 0;
                byte[] arr = ms.ToArray();
                bmp = new BitmapImage(Create_Memory_Resource_Uri(ms));
                DisplayBmp(position);
            }
            else
            { // daca imaginea este primita ca un path spre file 
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Count() > 1)
                    return;

                var ext = Path.GetExtension(files[0]);

                if (ext == ".pdf" && File.Exists(files[0]))
                {
                    owner.ShowPdfViewer();
                    owner.pdfViewer.SetPdfTo(files[0]);
                }

                if (!(ext == ".png" || ext == ".bmp" || ext == ".jpg") || !File.Exists(files[0]))
                    return;

                // daca inainte a mai fost o imagine o trimitem 
                if (image != null)
                {
                    image.Freeze();
                    MessageUtils.SendNewDrawing(image, whiteboard.drawingCollection.Count - 1); 
                    image = null;
                    bmp = null;
                }

                bmp = new BitmapImage(new Uri(files[0]));

                DisplayBmp(position);
            }
        }

        public void OnPasteEventHandler(object sender, KeyEventArgs e)
		{
            if(e.Key == Key.V && Keyboard.IsKeyDown(Key.LeftCtrl))
			{
                CheckIfLastImageIsNotSent();
                BitmapSource src = ((InteropBitmap)Clipboard.GetImage());
                if (src == null)
                    return;
                owner.selectedTool = this;
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(src));
                MemoryStream ms = new MemoryStream();
                encoder.Save(ms);
                isMoving = true;
                resizeGrid.Visibility = Visibility.Visible;
                bmp = new BitmapImage(Create_Memory_Resource_Uri(ms));
                DisplayBmp(new Point(0, 0));
            }
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
                        whiteboard.drawingCollection.Remove(image);
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
            resizeGrid.ColumnDefinitions[0].Width = new GridLength(size.Width / Whiteboard.sizeX * owner.myWhiteboardViewBox.ActualWidth);
            resizeGrid.RowDefinitions[0].Height = new GridLength(size.Height / Whiteboard.sizeY * owner.myWhiteboardViewBox.ActualHeight);
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

            if (image != null && owner.selectedTool == this)
                if ((position.X > image.Rect.X && position.X < image.Rect.X + image.Rect.Width) && (position.Y > image.Rect.Y && position.Y < image.Rect.Y + image.Rect.Height))
                {
                    Mouse.OverrideCursor = Cursors.SizeAll;
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    mouseOffset = new Point(0, 0);
                }
        }

        /// <summary>
        /// Folosit pentru a modifica pozitia imagini cand userul da 
        /// ii drag
        /// </summary>
        /// <param name="position"></param>
		public override void MouseDrag(Point position)
        {
            if (image != null)
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
