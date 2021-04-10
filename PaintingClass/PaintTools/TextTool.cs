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
using PaintingClass.Tabs;

namespace PaintingClass.PaintTools
{
    class TextTool : PaintTool
    {
        public override int priority => 3;

        public override Control GetControl()
        {
            Label label = new Label();
            label.Content = "Text";
            return label;
        }
        
        /// <summary>
        /// Returneaza un grid ce contine textboxul cu marime editabila de catre 
        /// utilizator
        /// </summary>
        /// <returns></returns>
        private Grid GetResizableTextboxGrid(string defaultText)
		{
			// cream grid pt functia de resize
			Grid grid = new Grid();
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5) });
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0) });

			grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
			grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5) });
			grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0) });
			// cream un textbox 
			TextBox tb = new TextBox()
			{
				Background = Brushes.Transparent,
				Text = defaultText,
				MinHeight = 100,
				MinWidth = 100,
				TextWrapping = TextWrapping.Wrap,
				Foreground = Brushes.Gray,
				AcceptsReturn = true,
			};
			tb.GotKeyboardFocus += Tb_GotKeyboardFocus;
			tb.LostKeyboardFocus += Tb_LostKeyboardFocus;

			// setam coloana si randul
			grid.Children.Add(tb);
			Grid.SetRow(tb, 0);
			Grid.SetColumn(tb, 0);

			// Grid splitter vertical 
			GridSplitter gsVert = new GridSplitter();
			gsVert.Width = 5;
			gsVert.HorizontalAlignment = HorizontalAlignment.Stretch;
			grid.Children.Add(gsVert);
			Grid.SetColumn(gsVert, 1);
			Grid.SetRow(gsVert, 0);

			// Grid splitter orizontal
			GridSplitter gsHorz = new GridSplitter();
			gsHorz.Height = 5;
			gsHorz.HorizontalAlignment = HorizontalAlignment.Stretch;
			grid.Children.Add(gsHorz);
			Grid.SetColumn(gsHorz, 0);
			Grid.SetRow(gsHorz, 1);

			Image closeIcon = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Tabs/Images/DeleteIcon.png")), Width = 15, Height = 15 };
			RenderOptions.SetBitmapScalingMode(closeIcon, BitmapScalingMode.Fant);
			closeIcon.MouseDown += CloseIcon_MouseDown;
			Canvas canv = new Canvas();
			canv.Children.Add(closeIcon);

			grid.Children.Add(canv);
			Grid.SetColumn(canv, 1);
			Grid.SetRow(canv, 1);
			return grid;
		}

		#region Events
		/// <summary>
		/// are loc cand userul da click pe tabla
		/// </summary>
		/// <param name="position"></param>
		public override void MouseDown(Point position)
        {
			var grid = GetResizableTextboxGrid("Scrie aici");
			myWhiteboardCanvas.Children.Add(grid);
            position = MainWindow.instance.myWhiteboardInstance.whiteboard.DenormalizePosition(position);
            Canvas.SetTop(grid, Canvas.GetTop(whiteboard)+ position.Y);
            Canvas.SetLeft(grid, Canvas.GetLeft(whiteboard) + position.X);
        }

		private void CloseIcon_MouseDown(object sender, MouseButtonEventArgs e)
		{
            myWhiteboardCanvas.Children.Remove(((Grid)((Canvas)((Image)sender).Parent).Parent));
		}

		private void Tb_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == "")
                    {
                tb.Foreground = Brushes.Gray;
                tb.Text = "Scrie aici";
            }
        }

        private void Tb_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == "Scrie aici")
            {
                tb.Foreground = Brushes.Black;
                tb.Text = "";
            }
        }

        public override void MouseDrag(Point position)
        {

        }

        public override void MouseUp()
        {
            // TODO Transmitere la server
        }

		#endregion
	}
}
