﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PaintingClass
{
	// Helper class pentru a face attached properties mai repede si usor 
	// mai multe detalii gasiti in videoul acesta https://www.youtube.com/watch?v=trR5RQGEteM&list=PLrW43fNmjaQVYF4zgsD0oL9Iv6u23PI6M&index=6&ab_channel=AngelSix si documentatia oficiala https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/attached-properties-overview?view=netframeworkdesktop-4.8
	/// <summary>
	/// A base attached property to replace the vanilla WPF attached property
	/// </summary>
	/// <typeparam name="Parent">The parent class to be the attached property</typeparam>
	/// <typeparam name="Property">The type of this attached property</typeparam>
	public abstract class BaseAttachedProperty<Parent, Property>
		where Parent : BaseAttachedProperty<Parent, Property>, new()
	{
		#region Public Events

		/// <summary>
		/// Fired when the value changes
		/// </summary>
		public event Action<DependencyObject, DependencyPropertyChangedEventArgs> ValueChanged = (sender, e) => { };

		#endregion

		#region Public Properties

		/// <summary>
		/// A singleton instance of our parent class
		/// </summary>
		public static Parent Instance { get; private set; } = new Parent();

		#endregion

		#region Attached Property Definitions

		/// <summary>
		/// The attached property for this class
		/// </summary>
		public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached("Value", typeof(Property), typeof(BaseAttachedProperty<Parent, Property>), new UIPropertyMetadata(new PropertyChangedCallback(OnValuePropertyChanged)));

		/// <summary>
		/// The callback event when the <see cref="ValueProperty"/> is changed
		/// </summary>
		/// <param name="d">The UI element that had it's property changed</param>
		/// <param name="e">The arguments for the event</param>
		private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			// Call the parent function
			Instance.OnValueChanged(d, e);

			// Call event listeners
			Instance.ValueChanged(d, e);
		}

		/// <summary>
		/// Gets the attached property
		/// </summary>
		/// <param name="d">The element to get the property from</param>
		/// <returns></returns>
		public static Property GetValue(DependencyObject d) => (Property)d.GetValue(ValueProperty);

		/// <summary>
		/// Sets the attached property
		/// </summary>
		/// <param name="d">The element to get the property from</param>
		/// <param name="value">The value to set the property to</param>
		public static void SetValue(DependencyObject d, Property value) => d.SetValue(ValueProperty, value);

		#endregion

		#region Event Methods

		/// <summary>
		/// The method that is called when any attached property of this type is changed
		/// </summary>
		/// <param name="sender">The UI element that this property was changed for</param>
		/// <param name="e">The arguments for this event</param>
		public virtual void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }

		#endregion
	}
}
