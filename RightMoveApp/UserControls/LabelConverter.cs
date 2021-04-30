using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace RightMoveApp.UserControls
{
	public class LabelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var count = int.Parse(value.ToString());
			return $"Count: {count}";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// not needed
			return value;
		}
	}
}
