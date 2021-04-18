using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using PropertyChanged;
namespace PaintingClass.Login
{
	/// <summary>
	/// Interaction logic for FormTextbox.xaml
	/// </summary>
	public partial class FormTextbox : UserControl,INotifyPropertyChanged
	{
		#region Events 

		public event TextChangedEventHandler TextChanged;
		public event PropertyChangedEventHandler PropertyChanged = (sender,e) =>{};

		#endregion


		#region Private Properties

		private bool? _isSyntaxCorrect = null;


		#endregion

		#region Public Properties

		public FormTextbox instance { get; private set; }
		public string Text { get; set; } = "";
		public double minimumWidth { get; set; } = 100;
		public string defaultText { get; set; } = "Scrie aici";
		public bool? isSyntaxCorrect
		{
			get { return _isSyntaxCorrect; } 
			set {	
				if(_isSyntaxCorrect!=value)
				{
					_isSyntaxCorrect = value; 
					PropertyChanged(this, new PropertyChangedEventArgs(nameof(instance))); 
				} 
			}
		} 
		public Color CorrectAnsColor { get; set; } = (Color)ColorConverter.ConvertFromString("#00FF00");
		public Color WrongAnsColor { get; set; } = Colors.Red;
		public Color DefaultAnsColor { get; set; } = Colors.Gray;
		public double CornerRadius { get; set; } = 10;
		public int MaxLength { get; set; }
		#endregion

		public FormTextbox()
		{
			InitializeComponent();
			DataContext = this;
			instance = this;
			TextChanged += (sender,e) => { };
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextChanged(sender,e);
		}
	}

	#region Value Converters 
	public class BoolToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(value is FormPasswordbox)
			{
				var inst = (FormPasswordbox)value;
				if (inst.isSyntaxCorrect == null)
					return inst.DefaultAnsColor;
				else if ((bool)inst.isSyntaxCorrect)
					return inst.CorrectAnsColor;
				else
					return inst.WrongAnsColor;
			}

			var instance = ((FormTextbox)value);
			if (instance.isSyntaxCorrect == null)
				return instance.DefaultAnsColor;
			else if ((bool)instance.isSyntaxCorrect)
				return instance.CorrectAnsColor;
			else
				return instance.WrongAnsColor;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class MultiplyConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			double result = 1.0;
			for (int i = 0; i < values.Length; i++)
			{
				if (values[i] is double)
					result *= (double)values[i];
			}

			return result;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new Exception("Not implemented");
		}
	}
	#endregion
}

