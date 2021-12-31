using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace FontAlignmentFinder
{
	public class DistanceZoomConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if ((values is { Length: 2 }) &&
				(values[0] is double distance and >= 0D) &&
				(values[1] is double zoomRatio and >= 1D))
			{
				return distance * zoomRatio;
			}
			return DependencyProperty.UnsetValue;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}