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

namespace PaintingClass.PaintTools
{
	class FormulaTool : PaintTool
	{
		public override int priority => 3;

		public override Control GetControl()
		{
			Label label = new Label();
			label.Content = "Formula";
			return label;
		}
		
		private static AnalyticsTeXConverter converter;
		private static Translator translator;

		private FormulaControl formulaControl;
		private TextBox tb;

		public override void MouseDown(Point position)
		{
			#region TextBox  
			tb = new TextBox();
			tb.Background = Brushes.Transparent;
			tb.Text = "Scrie formula aici";
			tb.Width = 100;
			tb.Height = 100;
			tb.TextWrapping = TextWrapping.Wrap;
			tb.Foreground = Brushes.Gray;
			tb.KeyDown += Tb_KeyDown;
			tb.TextChanged += Tb_TextChanged;
			tb.GotKeyboardFocus += Tb_GotKeyboardFocus;
			tb.LostKeyboardFocus += Tb_LostKeyboardFocus;
			#endregion

			StackPanel stackPanel = new StackPanel();
			stackPanel.MouseEnter += StackPanel_MouseEnter;
			stackPanel.MouseLeave += StackPanel_MouseLeave;
			
			// cream un nou formula control si il adaugam in stackpanel
			// formula control functioneaza asemenea unui textbox doar ca accepta doar LaTeX
			formulaControl = new FormulaControl();
			formulaControl.Margin = new Thickness(10);
			stackPanel.Children.Add(tb);
			stackPanel.Children.Add(formulaControl);
			myWhiteboardCanvas.Children.Add(stackPanel);
			position = MainWindow.instance.myWhiteboardInstance.whiteboard.DenormalizePosition(position);
			Canvas.SetTop(stackPanel, Canvas.GetTop(whiteboard) + position.Y);
			Canvas.SetLeft(stackPanel, Canvas.GetLeft(whiteboard) + position.X);
		}

		private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
		{
			bool IsSyntaxOk = true;
			TextBox tb = ((TextBox)((StackPanel)sender).Children[0]);
			string texf;
			try
			{
				if (translator.CheckSyntax(tb.Text) && tb.Text.Length > 0)
					texf = converter.Convert(tb.Text);
			}
			catch(Exception ex)
			{
				IsSyntaxOk = false;
			}
 			if (IsSyntaxOk && tb.Text.Length > 0 && !tb.IsKeyboardFocused)
 				((StackPanel)sender).Children[0].Visibility = Visibility.Collapsed;
		}

		private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
		{
			((StackPanel)sender).Children[0].Visibility = Visibility.Visible;
		}

		private void Tb_TextChanged(object sender, TextChangedEventArgs e)
		{
			string crudeText = ((TextBox)sender).Text;
			try
			{
				if (translator.CheckSyntax(crudeText) && crudeText.Length>0)
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
				tb.Text = "Scrie formula aici";
			}
		}

		private void Tb_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			TextBox tb = (TextBox)sender;
			if (tb.Text == "Scrie formula aici")
			{
				tb.Foreground = Brushes.Black;
				tb.Text = "";
			}
		}
	#endregion

		static FormulaTool()
		{
			translator = new Translator();
			converter = new AnalyticsTeXConverter();
		}
	}
}
