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
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Markup;
using System.Diagnostics;
using PaintingClassCommon;
using PaintingClass.Networking;
using System.Text.Json;
using static PaintingClass.Networking.MessageUtils;

namespace PaintingClass
{
    /// <summary>
    /// Tabla simpla, deoarece foloseste Drawing ar trebui sa fie destul de rapida si eficienta
    /// 
    /// Foloseste collection pt a adauga si scoate drawing-uri
    /// Foloseste proprietatea Background (mostenita de la Control) pt a seta un Background
    /// 
    /// toate Drawing-urile din collection trebuie sa incapa in acest chenar
    ///   ⇩ (0,0)
    ///   ◸   ◹
    ///            
    ///   ◺   ◿
    ///        ⇧ (size,size)
    /// </summary>
    public partial class Whiteboard : UserControl
    {
        public const double sizeX=100,sizeY=100;

        //aka List<Drawing>
        public DrawingCollection drawingCollection { get => group.Children; set => group.Children = value; }
        
        // lista de userControale
        public UIElementCollection userControlCollection { get => canvas.Children; }

        DrawingGroup mainGroup;
        DrawingGroup group;

        //constructor principal
        public Whiteboard()
        {
            InitializeComponent();

            mainGroup = new DrawingGroup();
            group = new DrawingGroup();
            var clipGeometry = new RectangleGeometry(new Rect(0, 0, sizeX, sizeY));

            //ne asiguram ca mainGroup este la dimensiunea corecta, un bodge cam stupid dar merge
            //TODO: poate poti sa gasesti o metoda prin care sa eviti asta
            mainGroup.Children.Add(new GeometryDrawing(null, new Pen(Brushes.Red,0) , clipGeometry) );

            //inseram group in mainGroup    
            mainGroup.Children.Add(group);

            //ignoram tot ce iese din chenar
            group.ClipGeometry = clipGeometry;

            //folosim un DrawingImage pentru a renderiza DrawingGroup pe <Image>
            image.Source = new DrawingImage(mainGroup);
            image.Stretch = Stretch.Fill;//de la aspect 1:1 la 16:9 (sau alt aspect daca schimbi dimensiunea tablei)
        }

        // transforma un punct in spatiu XAML in spatiul corect
        public Point TransformPosition(Point p)
        {
            return new Point(p.X/ActualWidth*sizeX,p.Y/ ActualHeight * sizeY);
        }

        public Point DenormalizePosition(Point p)
        {   
            return new Point(p.X / sizeX , p.Y/ sizeY );
        }

        /// <summary>
        /// returneaza true daca sa aplicat cu success WBItemMessage-ul
        /// </summary>
        public bool ApplyWBItem(WBItemMessage msg)
        {
            switch (msg.type)
            {
                case WBItemMessage.ContentType.drawing:
                    return ApplyDrawing(msg);
                case WBItemMessage.ContentType.userControl:
                    return ApplyUserControl(msg);
                case WBItemMessage.ContentType.clearAll:
                    return ClearWhiteboard();
            }
            return false;
        }

		private bool ApplyUserControl(WBItemMessage msg)
		{
            UserControlWBMessage ucMsg  = null;

            if (msg.op != WBItemMessage.Operation.delete)
            {
                ucMsg = JsonSerializer.Deserialize<UserControlWBMessage>(msg.content);
                if (ucMsg == null)
                {
                    Trace.WriteLine("JsonSerializer returned null from WBItemMessage.content");
                    return false;
                }
            }

            switch (msg.op)
            {
                // TODO Rezolvat indexarea la UserControale
                case WBItemMessage.Operation.add:
                    //if (msg.contentIndex != userControlCollection.Count)
                    //{
                    //    Trace.WriteLine("WBItemMessage.contentIndex has the wrong value!");
                    //    return false;
                    //}
                    userControlCollection.Add(ucMsg.Deserialize(this));
                    return true;
                case WBItemMessage.Operation.edit:
                    //if (msg.contentIndex < 0 || msg.contentIndex >= userControlCollection.Count)
                    //{
                    //    Trace.WriteLine("WBItemMessage.contentIndex has the wrong value!");
                    //    return false;
                    //}
                    //TODO
                    break;
                case WBItemMessage.Operation.delete:
                    //if (msg.contentIndex < 0 && msg.contentIndex >= userControlCollection.Count)
                    //{
                    //    Trace.WriteLine("WBItemMessage.contentIndex has the wrong value!");
                    //    return false;
                    //}
                    userControlCollection[msg.contentIndex] = null;
                    break;
            }
            return false;
        }

		public bool ClearWhiteboard()
        {
            drawingCollection.Clear();
            return true;
        }

        //todo de deletat
		private void UserControl_MouseMove(object sender, MouseEventArgs e)
		{
            Point p = e.GetPosition(canvas);
            p.X /= canvas.ActualWidth;
            p.Y /= canvas.ActualHeight;
            Size whiteboardSizePix;
            {
                Vector psd = PointToScreen(new Point(ActualWidth, ActualHeight)) - PointToScreen(new Point(0, 0));
                whiteboardSizePix = new Size(psd.X, psd.Y);
            }
            p.X *= whiteboardSizePix.Width;
            p.Y *= whiteboardSizePix.Height;

            Window.GetWindow(this).Title = p.ToString();
        }

		bool ApplyDrawing(WBItemMessage msg)
        {
            Drawing drawing=null;
            
            if (msg.op != WBItemMessage.Operation.delete)
            {
                drawing = XamlReader.Parse(msg.content) as Drawing;
                if (drawing==null)
                {
                    Trace.WriteLine("XAML parser returned null from WBItemMessage.content");
                    return false;
                }
            }    

            switch (msg.op)
            {
                case WBItemMessage.Operation.add:
                    if (msg.contentIndex!=drawingCollection.Count)
                    {
                        Trace.WriteLine("WBItemMessage.contentIndex has the wrong value!");
                        return false;
                    }
                    drawingCollection.Add(drawing);
                    return true;
                case WBItemMessage.Operation.edit:
                    if (msg.contentIndex<0 || msg.contentIndex>=drawingCollection.Count)
                    {
                        Trace.WriteLine("WBItemMessage.contentIndex has the wrong value!");
                        return false;
                    }
                    drawingCollection[msg.contentIndex] = drawing;
                    break;
                case WBItemMessage.Operation.delete:
                    if (msg.contentIndex < 0 && msg.contentIndex >= drawingCollection.Count)
                    {
                        Trace.WriteLine("WBItemMessage.contentIndex has the wrong value!");
                        return false;
                    }
                    drawingCollection[msg.contentIndex] = null;
                    break;
            }
            return false;
        }
    }
}
