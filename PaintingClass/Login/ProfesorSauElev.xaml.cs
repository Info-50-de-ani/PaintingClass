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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PaintingClass.Login
{
    /// <summary>
    /// Interaction logic for ProfesorSauElev.xaml
    /// </summary>
    public partial class ProfesorSauElev : Page
    {
        Frame CurrentFrame;
        public ProfesorSauElev(Frame frame)
        {
            InitializeComponent();
            CurrentFrame = frame;
			this.Loaded += OnLoadedSecundar;
			this.Loaded += OnLoadedMinutar;
            StartWindow.FadeAnimateElement(this, new Duration(new TimeSpan(0, 0, 0, 0, 400)), false);
        }

		private void OnLoadedMinutar(object sender, RoutedEventArgs e)
		{
            var minute = DateTime.Now.Minute;
            DoubleAnimation panelAnimation = new DoubleAnimation()
            {
                Duration = new Duration(new TimeSpan(0, 1, 0, 0, 0)),
                From = 180 + 6 * minute,
                To = (180 + 6 * minute) + 360,
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTargetProperty(panelAnimation, new PropertyPath("(Rectangle.RenderTransform).(RotateTransform.Angle)"));
            Storyboard.SetTarget(panelAnimation, Minutar);

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(panelAnimation);
            storyboard.Begin(this);
        }

		private void OnLoadedSecundar(object sender, RoutedEventArgs e)
		{
            var second = DateTime.Now.Second;
            DoubleAnimation panelAnimation = new DoubleAnimation()
            {
                Duration = new Duration(new TimeSpan(0, 0, 1, 0, 0)),
                From = 180 + 6 * second,
                To = (180 + 6 * second) + 360,
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTargetProperty(panelAnimation, new PropertyPath("(Rectangle.RenderTransform).(RotateTransform.Angle)"));
            Storyboard.SetTarget(panelAnimation, Secundar);

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(panelAnimation);
            storyboard.Begin(this);
        }

		private void BT_Elev_Click(object sender, RoutedEventArgs e)
        {
            CurrentFrame.Content = new ElevJoinRoom(CurrentFrame);
        }

        private void BT_Profesor_Click(object sender, RoutedEventArgs e)
        {
            CurrentFrame.Content = new LoginSauRegister(CurrentFrame);
        }

	}
}
