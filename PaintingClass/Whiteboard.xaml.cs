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
using System.Text.Json;

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

       
        //aka List<Drawing>

        public DrawingCollection collection { get => group.Children; set => group.Children = value; }
        public const double sizeX=100,sizeY=100;

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
        public Point NormalizePosition(Point p)
        {
            return new Point(p.X/Width*sizeX,p.Y/ Height * sizeY);
        }
    }
}
