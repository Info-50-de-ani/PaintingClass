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
        public override int priority => -1;

        public override Control GetControl()
        {
            Label label = new Label();
            label.Content = "Text";
            return label;
        }

        TextBox tb;

        public override void MouseDown(Point position)
        {
            tb = new TextBox();
            tb.Background = Brushes.Transparent;
            tb.Text = "Scrie aici";
            tb.Width = 100;
            tb.Height = 100;
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Foreground = Brushes.Gray;
            tb.AcceptsReturn = true;
            //tb.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            tb.KeyDown += Tb_KeyDown;
            tb.GotKeyboardFocus += Tb_GotKeyboardFocus;
            tb.LostKeyboardFocus += Tb_LostKeyboardFocus;
            myWhiteboardCanvas.Children.Add(tb);
            position = MainWindow.instance.myWhiteboardInstance.whiteboard.DenormalizePosition(position);
            //MessageBox.Show($"{position.X} {position.Y}");
            Canvas.SetTop(tb, Canvas.GetTop(whiteboard)+ position.Y);
            Canvas.SetLeft(tb, Canvas.GetLeft(whiteboard) + position.X);
            //MessageBox.Show($"{position.X + Canvas.GetTop(whiteboard)} {Canvas.GetLeft(whiteboard)+position.Y}");
        }

        private void Tb_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                tb.Text = tb.Text + '\n';
            }
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
            // todo sa fie transmisa prin glyph
        }
    }
}
