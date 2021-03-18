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
                tool.whiteboard = whiteboard;
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
    }
}
