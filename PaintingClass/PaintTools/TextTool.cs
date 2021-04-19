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
using System.Globalization;
using System.IO;
using System.Windows.Media.Animation;

namespace PaintingClass.PaintTools
{

	public class TextBoxResize 
	{

		public TextBoxResize(Point position,Size absSize)
		{
			this.absSize = absSize;
			this.position = position;
		}

		#region private Properties

		bool isUpdating;
		
		#endregion

		#region Public Properties
		/// <summary>
		/// pozitia textboxului functie de coordonatele tablei (0-100,0-100)
		/// </summary>
		public Point position { get; set; }

		public double FontSize { get; set; }

		/// <summary>
		/// relativ cu marimea ecranului in pixeli 
		/// </summary>
		public Size absSize { get; set; }

		/// <summary>
		/// gridul care contine text boxul (si gridsplitterul )
		/// </summary>
		public Grid textBoxGrid { get; set; }

		/// <summary>
		/// Actual textbox
		/// </summary>
		public TextBox tb { get; set; }

		/// <summary>
		/// Parent Canvas
		/// </summary>
		public Canvas myWhiteboardCanvas { get; set; }

		/// <summary>
		/// ViewBoxul in care sta myWhiteboard
		/// </summary>
		public Viewbox myWhiteboardViewBox { get; set; }

		/// <summary>
		/// Contine instanta clasei MyWhiteboard
		/// </summary>
		public MyWhiteboard owner { get; set; }

		#endregion

		#region EventsHandlers

		public void UpdateTextBoxSize(object sender, SizeChangedEventArgs e)
		{
			Point offset = TextTool.CalculateOffset(owner);
			var positionDenormalized = MainWindow.instance.myWhiteboard.whiteboard.DenormalizePosition(position);
			Canvas.SetTop(textBoxGrid, offset.Y + positionDenormalized.Y * myWhiteboardViewBox.RenderSize.Height);
			Canvas.SetLeft(textBoxGrid, offset.X + positionDenormalized.X * myWhiteboardViewBox.RenderSize.Width);

			isUpdating = true;
			textBoxGrid.RowDefinitions[0].Height = new GridLength(absSize.Height * myWhiteboardViewBox.RenderSize.Height);
			textBoxGrid.ColumnDefinitions[0].Width = new GridLength(absSize.Width * myWhiteboardViewBox.RenderSize.Width);

			tb.FontSize = FontSize * myWhiteboardViewBox.ActualWidth;
		}

		public void TextBoxMessage_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (isUpdating)
			{
				isUpdating = false;
				return;
			}
			absSize = new Size(textBoxGrid.ActualWidth / myWhiteboardViewBox.RenderSize.Width, textBoxGrid.ActualHeight / myWhiteboardViewBox.RenderSize.Height);
		}

		#endregion
	}

	class TextTool : PaintTool
	{
		public override int priority => 3;

		public override Control GetControl()
		{
			Label label = new Label();
			label.Content = "Text";
			return label;
		}

		#region Constants 
		public const int defaultTextBoxSize = 100;
		public const int defaultFontSize = 16;
		#endregion

		/// <summary>
		/// Calculeaza offsetul dintre viewbox si whiteboard
		/// </summary>
		/// <returns>un <see cref="Point"/> cu Point.X = offsetX si Point.Y = offsetY</returns>
		public static Point CalculateOffset(MyWhiteboard owner)
		{
			if (owner.myWhiteboardCanvas.ActualHeight == owner.myWhiteboardViewBox.ActualHeight)
			{ // avem offset doar in width
				return new Point((owner.myWhiteboardCanvas.ActualWidth - owner.myWhiteboardViewBox.ActualWidth) / 2, 0);
			}
			else if (owner.myWhiteboardCanvas.ActualWidth == owner.myWhiteboardViewBox.ActualWidth)
			{ // avem offset doar in height 
				return new Point(0, (owner.myWhiteboardCanvas.ActualHeight - owner.myWhiteboardViewBox.ActualHeight) / 2);
			}
			else
				return new Point((owner.myWhiteboardCanvas.ActualWidth - owner.myWhiteboardViewBox.ActualWidth) / 2, (owner.myWhiteboardCanvas.ActualHeight - owner.myWhiteboardViewBox.ActualHeight) / 2);
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
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(9) });
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0) });

			grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
			grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(9) });
			grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0) });
			// cream un textbox 
			TextBox tb = new TextBox()
			{
				Background = Brushes.Transparent,
				Text = defaultText,
				MinHeight = defaultTextBoxSize,
				MinWidth = defaultTextBoxSize,
				TextWrapping = TextWrapping.Wrap,
				Foreground = Brushes.Gray,
				AcceptsReturn = true,
				FontFamily = new FontFamily("Arial"),
				FontSize = 12,
			};
			tb.GotKeyboardFocus += Tb_GotKeyboardFocus;
			tb.LostKeyboardFocus += Tb_LostKeyboardFocus;
			tb.MouseRightButtonDown += (sender, e) => { };
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
			TextBox tb = grid.Children.OfType<TextBox>().First();
			TextBoxResize tbMsg = new TextBoxResize(position,new Size(defaultTextBoxSize / owner.myWhiteboardViewBox.ActualWidth, defaultTextBoxSize / owner.myWhiteboardViewBox.ActualHeight))
			{
				owner = owner,
				tb = tb,
				FontSize = defaultFontSize / owner.myWhiteboardViewBox.ActualWidth,
				textBoxGrid = grid,
				myWhiteboardCanvas = owner.myWhiteboardCanvas,
				myWhiteboardViewBox = owner.myWhiteboardViewBox
			};

			owner.myWhiteboardGrid.SizeChanged += tbMsg.UpdateTextBoxSize;
			grid.Children.OfType<TextBox>().First().SizeChanged += tbMsg.TextBoxMessage_SizeChanged;

			position = MainWindow.instance.myWhiteboard.whiteboard.DenormalizePosition(position);

			owner.textBoxMessages.Add(tbMsg);

			Point offset = CalculateOffset(owner);

			Canvas.SetTop(grid, offset.Y + position.Y * owner.myWhiteboardViewBox.RenderSize.Height);
			Canvas.SetLeft(grid, offset.X + position.X * owner.myWhiteboardViewBox.RenderSize.Width);
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
