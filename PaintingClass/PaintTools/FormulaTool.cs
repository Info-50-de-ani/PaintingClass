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
using Exversion.Analytics;
using Analytics;
using WpfMath.Controls;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.Threading;

namespace PaintingClass.PaintTools
{
	class FormulaTool : PaintTool
	{
		#region Constants

		private const string defaultMessage = "Scrie formula aici.";

		#endregion

		#region Properities & fields

		public override int priority => 3;
		private static AnalyticsTeXConverter converter;
		private static Translator translator;

		#endregion

		#region Constructors

		static FormulaTool()
		{
			translator = new Translator();
			converter = new AnalyticsTeXConverter();
			// umplem FormulPanel
		}

		#endregion

		#region Public Methods

		public override Control GetControl()
		{
			Label label = new Label();
			label.Content = "Formula";
			return label;
		}

		#endregion

		#region Static Methods
		/// <summary>
		/// Umple gridul cu operatori pentru a fi folositi la scrierea formulelor
		/// </summary>
		public static void FillFormulaPanel()
		{
			const string Background = "#FFE66D";
			const string BorderBrush = "#FFA96C";
			string operators = "•×√≡≈≠><≥≤¬|&←→↔∂∫ΔΣΠ";
			var formulaPanel = MainWindow.instance.myWhiteboardInstance.FormulaPanel;
			int cnt = 0;
			for (int i = 0; i < formulaPanel.RowDefinitions.Count; i++)
			{
				for (int j = 0; j < formulaPanel.ColumnDefinitions.Count; j++, cnt++)
				{
					Border border = new Border()
					{
						Margin = new Thickness(5),
						Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Background)),
						BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BorderBrush)),
						BorderThickness = new Thickness(4),
						CornerRadius = new CornerRadius(20),
						Effect = new DropShadowEffect() { BlurRadius = 20, Opacity = 0.6, ShadowDepth = 2, Color = Colors.Black },
					};
					TextBlock tb = new TextBlock()
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						FontSize = 35,
					};
					if (operators[cnt] == '|')
						tb.Text = operators[cnt].ToString() + " " + operators[cnt].ToString();
					else
						tb.Text = operators[cnt].ToString();
					border.Child = tb;
					border.MouseDown += FormulaButtonClick;
					formulaPanel.Children.Add(border);
					Grid.SetRow(border, i);
					Grid.SetColumn(border, j);
				}
			}
		}
		#endregion

		#region Private Methods


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
				MinHeight = 100,
				MinWidth = 50,
				FontSize = 16,
				TextWrapping = TextWrapping.Wrap,
				Foreground = Brushes.Gray,
				AcceptsReturn = true,
			};
			// setam eventuri pentru textbox 
			tb.GotKeyboardFocus += Tb_GotKeyboardFocus;
			tb.LostKeyboardFocus += Tb_LostKeyboardFocus;
			tb.KeyDown += Tb_KeyDown;
			tb.TextChanged += Tb_TextChanged;

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

			canv.Width = 0;
			canv.Height = 0;
			canv.VerticalAlignment = VerticalAlignment.Bottom;
			canv.HorizontalAlignment = HorizontalAlignment.Left;
			canv.Margin = new Thickness(-15, 0, 0, 0);
			grid.Children.Add(canv);


			return grid;
		}

		#endregion

		#region EventHandlers

		private static void FormulaButtonClick(object sender, MouseButtonEventArgs e)
		{
			var tb = Keyboard.FocusedElement as TextBox;
			if (tb == null)
				return;
			char op = ((TextBlock)((Border)sender).Child).Text[0];
			if (op == '|')
				tb.Text = tb.Text.Insert(tb.CaretIndex, "||");
			else
				tb.Text = tb.Text.Insert(tb.CaretIndex, op.ToString());
		}

		/// <summary>
		/// Se produce atunci cand acest tool este selectat
		/// </summary>
		public void SelectToolEventHandler(PaintTool tool)
		{
			if (!(tool is FormulaTool))
			{
				if (owner.FormulaPanelParentGrid.Visibility == Visibility.Hidden)
					return;
				else
				{
					Storyboard sb = new Storyboard() {Duration= new Duration(new TimeSpan(0, 0, 0, 0, 300)) };
					DoubleAnimation an = new DoubleAnimation()
					{
						Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300)),
						From = 1,
						To = 0
					};
					Storyboard.SetTargetProperty(an, new PropertyPath("(Grid.Opacity)"));
					Storyboard.SetTarget(an, owner.FormulaPanelParentGrid);
					sb.Completed += (sender, e) =>
					{
						owner.FormulaPanelParentGrid.Visibility = Visibility.Hidden;
					};
					sb.Children.Add(an);
					sb.Begin(owner);
					return;
				}
			}

				if (owner.FormulaPanelParentGrid.Visibility == Visibility.Visible)
					return;
			{ 
				owner.FormulaPanelParentGrid.Visibility = Visibility.Visible;
				Storyboard storyboard = new Storyboard();
				DoubleAnimation animation = new DoubleAnimation()
				{
					Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300)),
					From = 0,
					To = 1
				};
				Storyboard.SetTargetProperty(animation, new PropertyPath("(Grid.Opacity)"));
				Storyboard.SetTarget(animation, owner.FormulaPanelParentGrid);

				storyboard.Children.Add(animation);
				storyboard.Begin(owner);
				return;
			}
		}

		private void CloseIcon_MouseDown(object sender, MouseButtonEventArgs e)
		{
			myWhiteboardCanvas.Children.Remove((StackPanel)((Grid)((Canvas)((Image)sender).Parent).Parent).Parent);
		}

		public override void MouseDown(Point position)
		{
			var grid = GetResizableTextboxGrid(defaultMessage);


			StackPanel stackPanel = new StackPanel();
			stackPanel.MouseEnter += StackPanel_MouseEnter;

			// cream un nou formula control si il adaugam in stackpanel
			// formula control functioneaza asemenea unui textbox doar ca accepta doar LaTeX
			FormulaControl formulaControl = new FormulaControl();
			formulaControl.Margin = new Thickness(10);

			stackPanel.Children.Add(grid);
			stackPanel.Children.Add(formulaControl);

			myWhiteboardCanvas.Children.Add(stackPanel);
			position = MainWindow.instance.myWhiteboardInstance.whiteboard.DenormalizePosition(position);
			Canvas.SetTop(stackPanel, position.Y * myWhiteboardCanvas.ActualHeight);
			Canvas.SetLeft(stackPanel, position.X * myWhiteboardCanvas.ActualWidth);
		}


		private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
		{
			((StackPanel)sender).Children[0].Visibility = Visibility.Visible;
		}

		private void Tb_TextChanged(object sender, TextChangedEventArgs e)
		{
			string crudeText = ((TextBox)sender).Text;
			FormulaControl formulaControl = ((StackPanel)((Grid)((TextBox)sender).Parent).Parent).Children.OfType<FormulaControl>().First();
			try
			{
				if (translator.CheckSyntax(crudeText) && crudeText.Length > 0)
				{
					// convertim in LaTeX
					string texf = converter.Convert(crudeText);
					formulaControl.Formula = texf;
				}
			}
			catch (Exception ex)
			{
				formulaControl.Formula = ex.Message;
			}
		}

		#region Typing related
		private void Tb_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				((TextBox)sender).Visibility = Visibility.Collapsed;
			}
		}

		private void Tb_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			TextBox tb = (TextBox)sender;
			if (tb.Text == "")
			{
				tb.Foreground = Brushes.Gray;
				tb.Text = defaultMessage;
			}
			else
			{
				bool IsSyntaxOk = true;
				string texf;
				try
				{
					if (translator.CheckSyntax(tb.Text) && tb.Text.Length > 0)
						texf = converter.Convert(tb.Text);
				}
				catch (Exception ex)
				{
					IsSyntaxOk = false;
				}
				if (IsSyntaxOk)
					((Grid)tb.Parent).Visibility = Visibility.Collapsed;
			}
		}

		private void Tb_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			TextBox tb = (TextBox)sender;
			if (tb.Text == defaultMessage)
			{
				tb.Foreground = Brushes.Black;
				tb.Text = "";
			}
		}

		#endregion
	}
	#endregion
}
