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
	public partial class FormPasswordbox : UserControl, INotifyPropertyChanged
	{

		#region Public Event 

		public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
		public event RoutedEventHandler PasswordChanged= (sender,e) => { };

		#endregion

		#region 

		private bool? _IsSyntaxCorrect = null;
		
		#endregion

		#region Public Properties
		public bool HasText { get; set; }
		public FormPasswordbox instance { get; private set; }
		public double minimumWidth { get; set; } = 100;
		public string defaultText { get; set; } = "Scrie aici";
		public bool? isSyntaxCorrect {
			get => _IsSyntaxCorrect;
			set
			{
				if(_IsSyntaxCorrect != value)
				{
					_IsSyntaxCorrect = value;
					PropertyChanged(this, new PropertyChangedEventArgs(nameof(instance)));
				}
			}
		
		} 
		public Color CorrectAnsColor { get; set; } = (Color)ColorConverter.ConvertFromString("#00FF00");
		public Color WrongAnsColor { get; set; } = Colors.Red;
		public Color DefaultAnsColor { get; set; } = Colors.Gray;
		public double CornerRadius { get; set; } = 10;
		public int MaxLength { get; set; }
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
			PasswordChanged += (sender, e) => { };
		}

		private void MainPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			PasswordChanged(sender, e);
		}
	}
}
