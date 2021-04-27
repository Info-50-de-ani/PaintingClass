﻿using System;
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
using Microsoft.Win32;
using System.IO;
using PaintingClass.PaintTools.Interfaces;
using PaintingClass.Resources;

namespace PaintingClass.Tabs
{
    /// <summary>
    /// Tab care contine codul necesar pentru a face o tabla interactiva
    /// TODO: tabla are o marime fixa deocamdata dar ar fi ideal daca ar suporta orice rezolutie, mentinand aspectul de 16:9
    /// TODO: NU ESTE TERMINATA
    /// </summary>

    public partial class MyWhiteboard : UserControl
    {
        #region Events
        /// <summary>
        /// se produce atunci cand cineva selecteaza un tool
        /// </summary>
        Action<PaintTool> OnToolSelect = (tool) => { };

        /// <summary>
        /// se produce atunci cand Userul schimba font-sizeul la text
        /// </summary>
        public Action<double> OnFontSizeChanged = (fontSize) => { };

		#endregion

		private PaintTool _selectedTool;
        public PaintTool selectedTool {
            get { return _selectedTool; } 
            set {
                _selectedTool = value;
                OnToolSelect(_selectedTool);
            } 
        }//click stanga o sa foloseasca unealta selectata

        public SolidColorBrush globalBrush = Brushes.Black;
        public double globalBrushThickness = 0.3;
        private double _globalFontSize;
        public double globalFontSize {
			set
			{
                if(value != _globalFontSize)
				{
                    _globalFontSize = value;
                    OnFontSizeChanged(value);
				}
			}
            get => _globalFontSize;
        }

        //lista uneltelor incarcate
        List<PaintTool> tools = new List<PaintTool>();

        /// <summary>
        /// Are valoare de ViewBox.ActualHeight/WindowHeight
        /// </summary>
        public double ViewBoxToWindowSizeHeightRatio { get; private set; }

        /// <summary>
        /// Contine toate TextBoxurile facute de <see cref="TextTool"/> pentru a le 
        /// updata dimensiunea dinamic cu usurinta
        /// </summary>
        public List<TextToolResize> textToolResizeCollection = new List<TextToolResize>();
        public List<FormulaToolResize> formulaToolResizeCollection = new List<FormulaToolResize>();

        //daca suntem in procesul de a scrie pe tabla
        bool isDrawing;

        //constructorul
        public MyWhiteboard()
        {
            InitializeComponent();
            
            #region Constants
            void ViewBoxToWindowSizeHeightRationSetter(object sender, SizeChangedEventArgs e)
            {
                ViewBoxToWindowSizeHeightRatio = myWhiteboardViewBox.ActualHeight / SystemParameters.PrimaryScreenHeight;
                myWhiteboardViewBox.SizeChanged -= ViewBoxToWindowSizeHeightRationSetter;
            }
            /// seteaza constanta <see cref="ViewBoxToWindowSizeHeightRation"/>
            myWhiteboardViewBox.SizeChanged += ViewBoxToWindowSizeHeightRationSetter;
            #endregion

            //instantam uneltele
            foreach (Type type in paintToolTypes)
            {
                PaintTool tool = (PaintTool)Activator.CreateInstance(type);
                tool.owner  =  this;
                tool.whiteboard = whiteboard;
                tool.myWhiteboardCanvas = myWhiteboardCanvas;
                tool.myWhiteboardGrid = myWhiteboardGrid;
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
                if (tool is IToolSelected)
                    OnToolSelect += ((IToolSelected)tool).SelectToolEventHandler;
                if (tool is ImageTool)
                {
                    ImageTool t = (ImageTool)tool;
                    this.Drop += t.OnDropEventHandler;
                }
            }

            //selecteaza prima unealta
            if (tools.Count>0)
            selectedTool = tools[0];

            //adauga eventuri
            whiteboard.MouseLeftButtonDown += (sender,args) =>
            {
                if (isDrawing) return;
                isDrawing = true;
                selectedTool.MouseDown(whiteboard.TransformPosition(args.GetPosition(whiteboard)));
            };

            whiteboard.MouseMove += (sender, args) =>
            {
                if (!isDrawing) return;
                selectedTool.MouseDrag(whiteboard.TransformPosition(args.GetPosition(whiteboard)));
            };

            whiteboard.MouseLeave += (sender, args) =>
            {
                if (!isDrawing) return;
                isDrawing = false;
                selectedTool.MouseUp();
            };

            whiteboard.MouseLeftButtonUp += (sender,args) =>
            {
                if (!isDrawing) return;
                isDrawing = false;
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

        #region Pdf Viewer

        private void ClosePdfButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation()
            {
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300)),
                To = 0,
                From = 1
            };
            Storyboard sb = new Storyboard() { Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300)) };
            sb.Completed += (sender, e) => { PdfViewerGrid.Visibility = Visibility.Hidden; };
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("(Grid.Opacity)"));
            Storyboard.SetTarget(opacityAnimation, PdfViewerGrid);
            sb.Children.Add(opacityAnimation);
            sb.Begin(this);
        }

		public void ShowPdfViewer()
		{
            PdfViewerGrid.Visibility = Visibility.Visible;
            DoubleAnimation opacityAnimation = new DoubleAnimation()
            {
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300)),
                To = 1,
                From = 0
            };
            Storyboard sb = new Storyboard() { Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300)) };
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("(Grid.Opacity)"));
            Storyboard.SetTarget(opacityAnimation, PdfViewerGrid);
            sb.Children.Add(opacityAnimation);
            sb.Begin(this);
        }

        private void OpenNewPdfButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPdfViewer();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PDf files(*.pdf) | *.pdf";
            dialog.ShowDialog();
            if (File.Exists(dialog.FileName) == true)
            {
                try
                {
                    pdfViewer.SetPdfTo(dialog.FileName);
                }
                catch // daca este vreo eroare informam userul
                {
                    MessageBox.Show("Fisierul nu are un format valid", "Eroare");
                    pdfViewer.isEmpty = true;
                    return;
                }
            }
        }

		#endregion

		#region Panel Animations

        private void CloseOpenPanels()
		{
            if (ColorPanel.RenderTransform.Value.M11 == 1)
                SlidingAnimation_Grid(ColorPanel);
            if (ThicknessPanel.RenderTransform.Value.M11 == 1)
                SlidingAnimation_Grid(ThicknessPanel);
            if (FontSizePanel.RenderTransform.Value.M11 == 1)
                SlidingAnimation_Grid(FontSizePanel);
		}

        /// <summary>
        /// Animeaza un grid pus ca parametru
        /// la a doua chemare va face animatia invers
        /// </summary>
        /// <param name="grid"></param>
		private void SlidingAnimation_Grid(Grid grid)
		{

            DoubleAnimation panelAnimation = new DoubleAnimation()
            {
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300)),
                DecelerationRatio = 0.9f
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
            if (grid.RenderTransform.Value.M11 == 0)
            {
                // animam la matricea normala
                panelAnimation.To = 1;
            }
            else
            {
                // amnimam la matricea care ascunde meniul
                panelAnimation.To = 0;
            }
            Storyboard.SetTargetProperty(panelAnimation, new PropertyPath("(Grid.RenderTransform).(ScaleTransform.ScaleX)"));
            Storyboard.SetTarget(panelAnimation, grid);

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(panelAnimation);
            storyboard.Begin(this);
        }

        /// <summary>
        /// Animatie pentru meniul de culori
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorMenu_Button_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenPanels();
            SlidingAnimation_Grid(ColorPanel);
        }

        private void BrushThicknessMenu_Button_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenPanels();
            SlidingAnimation_Grid(ThicknessPanel);
        }
		private void FontSizeMenu_Button_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenPanels();
            SlidingAnimation_Grid(FontSizePanel);
        }

		#endregion

		#region Settings Panels 

        private void Color_Button_Click(object sender, RoutedEventArgs e)
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

            CloseOpenPanels();
        }

        private void BrushThickness_Button_Click(object sender, RoutedEventArgs e)
        {
            var buton = (Button)sender;

            switch ((string)buton.Tag)
            {
                case "Subtire":
                    globalBrushThickness = 0.1;
                    break;
                case "Normal":
                    globalBrushThickness = 0.3;
                    break;
                case "Gros":
                    globalBrushThickness = 0.5;
                    break;
                case "Foarte gros":
                    globalBrushThickness = 0.8;
                    break;
            }

            CloseOpenPanels();
        }

        private void FontSize_Button_Click(object sender, RoutedEventArgs e)
		{
            var buton = (Button)sender;

            switch ((string)buton.Tag)
            {
                case "Mic":
                    globalFontSize = 12;
                    break;
                case "Mediu":
                    globalFontSize = 14;
                    break;
                case "Mare":
                    globalFontSize = 16;
                    break;
                case "Foarte Mare":
                    globalFontSize = 20;
                    break;
            }

            CloseOpenPanels();
        }

		private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
		{
            string s = MyWhiteboardUtils.InvertHex(e.NewValue.ToString());
            globalBrush = new SolidColorBrush((Color)e.NewValue);
            TB_Alege_Culoare.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(s));
            colorPicker.IsOpen = false;

            CloseOpenPanels();
        }

        #endregion
		#endregion
	}
}
