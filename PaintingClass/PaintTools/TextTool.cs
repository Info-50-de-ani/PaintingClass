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
using System.Windows.Markup;

namespace PaintingClass.PaintTools
{
	/// <summary>
	/// Fiecare textbox de pe <see cref="MyWhiteboard"/> va avea o instanta a acestei clase 
	/// ea este folosita pentru a da resize acestuia in cazul in care userul da resize la <see cref="MainWindow"/>
	/// </summary>
	public class TextToolResize
	{
		/// <summary>
		/// Indexul din userControl collection
		/// </summary>
		public int index { get; set; }

		public TextToolResize(Point position,Size absSize)
		{
			this.absSize = absSize;
			this.absPosition = position;
		}

		#region Public Properties
		/// <summary>
		/// pozitia textboxului functie de coordonatele tablei (0-100,0-100)
		/// </summary>
		public Point absPosition { get; set; }

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
		/// Contine instanta clasei MyWhiteboard
		/// </summary>
		public MyWhiteboard owner { get; set; }

		#endregion

		#region EventsHandlers

		/// <summary>
		/// Este produs atunci cand windowul este resized si are rolul de a updata marimea si pozitie textboxului
		/// relativ cu tabla
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void UpdateTextBoxSize(object sender, SizeChangedEventArgs e)
		{
			Point offset = TextTool.CalculateOffset(owner);
			var positionDenormalized = MainWindow.instance.myWhiteboard.whiteboard.DenormalizePosition(absPosition);
			Canvas.SetTop(textBoxGrid, offset.Y + positionDenormalized.Y * owner.myWhiteboardViewBox.RenderSize.Height);
			Canvas.SetLeft(textBoxGrid, offset.X + positionDenormalized.X * owner.myWhiteboardViewBox.RenderSize.Width);
			textBoxGrid.RenderTransform = new ScaleTransform(owner.myWhiteboardViewBox.ActualWidth / SystemParameters.PrimaryScreenWidth * 1d / (owner.ViewBoxToWindowSizeHeightRatio * SystemParameters.PrimaryScreenHeight / SystemParameters.PrimaryScreenWidth * 16d / 9d), owner.myWhiteboardViewBox.ActualHeight / SystemParameters.PrimaryScreenHeight * 1d / owner.ViewBoxToWindowSizeHeightRatio);
		}
		
		/// <summary>
		/// Este chemat atunci cand Userul selecteaza un font size 
		/// </summary>
		public void UpdateTextBoxFontsize(double fontSize)
		{
			var tb = textBoxGrid.Children.OfType<TextBox>().First();
			if (tb.IsKeyboardFocused)
			{
				tb.FontSize = fontSize;
			}
		}

		/// <summary>
		/// se updateaza cand userul da zoom
		/// </summary>
		public void OnTranformChanged()
		{
			Point offset = TextTool.CalculateOffset(owner);
			Canvas.SetTop(textBoxGrid, offset.Y + absPosition.Y/Whiteboard.sizeY * owner.myWhiteboardViewBox.RenderSize.Height * owner.myWhiteboardViewBox.RenderTransform.Value.M22);
			Canvas.SetLeft(textBoxGrid, offset.X + absPosition.X/Whiteboard.sizeX * owner.myWhiteboardViewBox.RenderSize.Width * owner.myWhiteboardViewBox.RenderTransform.Value.M11);
			textBoxGrid.RenderTransform = owner.myWhiteboardViewBox.RenderTransform;
		}
		#endregion
	}

	class TextTool : PaintTool
	{
		public override int priority => 3;

		public override Control GetControl()
		{
			var cc = new ContentControl() { Height = 40 };
			Image image = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Tools/text.png")) };
			RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
			cc.Content = image;
			return cc;
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
				Name = nameof(TextToolResize),
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
			TextToolResize tbResize = new TextToolResize(position,new Size(defaultTextBoxSize / owner.myWhiteboardViewBox.ActualWidth, defaultTextBoxSize / owner.myWhiteboardViewBox.ActualHeight))
			{
				owner = owner,
				tb = tb,
				textBoxGrid = grid,
				index = myWhiteboardCanvas.Children.Count-1
			};

			grid.RenderTransform = owner.myWhiteboardViewBox.RenderTransform;
			owner.OnTransformChanged += tbResize.OnTranformChanged;
			owner.OnFontSizeChanged += tbResize.UpdateTextBoxFontsize;
			owner.myWhiteboardGrid.SizeChanged += tbResize.UpdateTextBoxSize;

			owner.textToolResizeCollection.Add(tbResize);
			tb.TextChanged += (sender,e) => { TextChangedHandler(tbResize); };

			Point offset = CalculateOffset(owner);
			Canvas.SetTop(grid,offset.Y+ position.Y / Whiteboard.sizeY * owner.myWhiteboardViewBox.ActualHeight * owner.myWhiteboardViewBox.RenderTransform.Value.M22);
			Canvas.SetLeft(grid,offset.X+ position.X/  Whiteboard.sizeX * owner.myWhiteboardViewBox.ActualWidth * owner.myWhiteboardViewBox.RenderTransform.Value.M11);
		}

		/// <summary>
		/// Are loc cand textul dintr-un TextBox este modificat
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextChangedHandler(TextToolResize textResize)
		{
			TextBox tb = (TextBox)XamlReader.Parse(XamlWriter.Save(textResize.tb));
			tb.Height = textResize.textBoxGrid.RowDefinitions[0].ActualHeight;
			tb.Width = textResize.textBoxGrid.ColumnDefinitions[0].ActualWidth;
			MessageUtils.SendNewUserControl(new MessageUtils.UserControlWBMessage(textResize.absPosition,tb), whiteboard.userControlCollection.Count - 1);
		}

		#region Typing and Closing Related
		private void CloseIcon_MouseDown(object sender, MouseButtonEventArgs e)
		{
			//todo de facut astfel incat indexul sa nu fie afectat
			var x = myWhiteboardCanvas.Children.OfType<object>().ToList();
			MessageBox.Show(x.Count.ToString());
			myWhiteboardCanvas.Children.Remove(((Grid)((Canvas)((Image)sender).Parent).Parent));
			x = myWhiteboardCanvas.Children.OfType<object>().ToList();
			MessageBox.Show(x.Count.ToString());
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

		#endregion

		#endregion
	}
}