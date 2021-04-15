using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace PaintingClass.Login
{
	/// <summary>
	/// Interaction logic for FormPasswordbox.xaml
	/// </summary>
	[AddINotifyPropertyChangedInterface]
	public partial class FormPasswordbox : UserControl, INotifyPropertyChanged
	{

		#region Public Event 

		public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

		#endregion

		#region Public Properties
		public bool HasText { get; set; }
		public FormPasswordbox instance { get; private set; }
		public double minimumWidth { get; set; } = 100;
		public string defaultText { get; set; } = "Scrie aici";
		public bool? isSyntaxCorrect { get; set; } = false;
		public Color CorrectAnsColor { get; set; } = (Color)ColorConverter.ConvertFromString("#00FF00");
		public Color WrongAnsColor { get; set; } = Colors.Red;
		public Color DefaultAnsColor { get; set; } = Colors.Gray;
		public double CornerRadius { get; set; } = 10;
		public string Password 
		{
			get
			{
				return MainPasswordBox.Password; 
			}
		}
		#endregion

		public FormPasswordbox()
		{
			InitializeComponent();
			DataContext = this;
			instance = this;
		}

	}
}
