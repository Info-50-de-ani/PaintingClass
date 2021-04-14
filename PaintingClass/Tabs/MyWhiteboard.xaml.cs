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
using System.Threading;
using System.Text.Json;
using PaintingClass.Networking;
using System.Windows.Media.Animation;

namespace PaintingClass.Tabs
{
    /// <summary>
    /// Tab care contine codul necesar pentru a face o tabla interactiva
    /// TODO: tabla are o marime fixa deocamdata dar ar fi ideal daca ar suporta orice rezolutie, mentinand aspectul de 16:9
    /// TODO: NU ESTE TERMINATA
    /// </summary>
   
    public partial class MyWhiteboard : UserControl
    {
        PaintTool selectedTool;//click stanga o sa foloseasca unealta selectata
        public SolidColorBrush globalBrush = Brushes.Black;

        //lista uneltelor incarcate
        List<PaintTool> tools = new List<PaintTool>();

        //daca suntem in procesul de a scrie pe tabla
        bool drawing;

        //constructorul
        public MyWhiteboard()
        {
            InitializeComponent();

            //instantam uneltele
            foreach (Type type in paintToolTypes)
            {
                PaintTool tool = (PaintTool)Activator.CreateInstance(type);
                tool.owner  =  this;
                tool.whiteboard = whiteboard;
                tool.myWhiteboardCanvas = myWhiteboardCanvas;
                tools.Add(tool);
            }

            //le sortam folosind proprietate priority
            tools.Sort((a,b) => { return a.priority.CompareTo(b.priority); } );

            //generaza controalele pt ToolBar
            foreach (PaintTool tool in tools)
            {
                Control toolControl = tool.GetControl();
                //incapsulam controlul intr-un buton
                Button button = new Button();
                button.Content = toolControl;
                //adaugam butonul la toolbar
                toolbar.Children.Add(button);
                //cand butonul este apasat o sa selecteze unealta corecta
                button.Click+=(sender,e) => selectedTool = tools[toolbar.Children.IndexOf(sender as UIElement) ];
            }

            //selecteaza prima unealta
            if (tools.Count>0)
            selectedTool = tools[0];

            //adauga eventuri
            whiteboard.MouseLeftButtonDown += (sender,args) =>
            {
                if (drawing) return;
                drawing = true;
                selectedTool.MouseDown(whiteboard.NormalizePosition(args.GetPosition(whiteboard)));
            };

            whiteboard.MouseMove += (sender, args) =>
            {
                if (!drawing) return;
                selectedTool.MouseDrag(whiteboard.NormalizePosition(args.GetPosition(whiteboard)));
            };

            whiteboard.MouseLeave += (sender, args) =>
            {
                if (!drawing) return;
                drawing = false;
                selectedTool.MouseUp();
            };

            whiteboard.MouseLeftButtonUp += (sender,args) =>
            {
                if (!drawing) return;
                drawing = false;
                selectedTool.MouseUp();
            };

        }

        //contine toate tipurile de PaintTool obitnute prin reflexie
        static Type[] paintToolTypes;

        //constructor static, ruleaza o singura data, folosit pentru reflexie
        static MyWhiteboard()
        {
            //extrage toate clasele non-abstracte care mostenesc din PaintTool
            //folosind reflexie si Linq dar nu e necesar sa intelegi acest cod acum
            paintToolTypes = (from type
                              in typeof(MyWhiteboard).Assembly.GetTypes()
                              where type.IsAbstract == false && type.IsSubclassOf(typeof(PaintTool))
                              select type).ToArray();
        }

        #region Event Handlere butoane

        /// <summary>
        /// Animatie pentru meniul de culori
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorMenu_Button_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation panelAnimation = new DoubleAnimation()
            {
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300)),
                DecelerationRatio=0.9f
            };
            
            // matricea normala(care face obiectul sa arate normal) de scaling arata asa 
            // 1 0
            // 0 1 
            // Matricea cand este ascuns meniul arata asa 
            // 0 0 
            // 0 1 
            // coloana 1 reprezinta i(final) in functie de i(linia 1) si j(linia 2)
            // coloana 2 reprezinta j(final) in functie de i(linia 1) si j(linia 2)
            // scaleX reprezinta nr din coloana 1 linia 1

            //Daca este collapsed panelul... 
            if(ColorPanel.RenderTransform.Value.M11 == 0)
		    {
                // animam la matricea normala
                panelAnimation.From = 0;
                panelAnimation.To = 1;
            }
            else
			{
                // amnimam la matricea care ascunde meniul
                panelAnimation.From = 1;
                panelAnimation.To = 0;
            }
            Storyboard.SetTargetProperty(panelAnimation, new PropertyPath("(Grid.RenderTransform).(ScaleTransform.ScaleX)"));
            Storyboard.SetTarget(panelAnimation, ColorPanel);

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(panelAnimation);
            storyboard.Begin(this);

        }

        private void Grosime_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Marime_Font_Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Color_Buton_Click(object sender, RoutedEventArgs e)
        {
            var buton = (Button)sender;

            switch ((string)buton.Tag)
            {
                case "Red":
                    globalBrush = Brushes.Red;
                    break;
                case "Yellow":
                    globalBrush = Brushes.Yellow;
                    break;
                case "Green":
                    globalBrush = Brushes.Green;
                    break;
                case "Black":
                    globalBrush = Brushes.Black;
                    break;
            }

            ColorMenu_Button_Click(null,new RoutedEventArgs());
        }

        #endregion
    }
}
