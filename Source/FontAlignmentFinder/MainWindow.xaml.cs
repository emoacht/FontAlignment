using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using FontAlignment;

namespace FontAlignmentFinder
{
	public partial class MainWindow : Window
	{
		#region Font Family

		private void PrepareFontFamily(string defaultFontName)
		{
			this.FontFamilyComboBox.ItemsSource = FontHelper.AvailableFontNames;

			if (FontHelper.TryGetFontFamily(defaultFontName, out FontFamily? buffer))
			{
				this.MainTextBox.FontFamily = buffer;
				this.FontFamilyComboBox.SelectedItem = defaultFontName;
			}

			this.FontFamilyComboBox.SelectionChanged += (_, _) =>
			{
				var fontName = this.FontFamilyComboBox.SelectedItem?.ToString();
				if (string.IsNullOrEmpty(fontName))
					return;

				if (FontHelper.TryGetFontFamily(fontName, out FontFamily? buffer))
				{
					this.MainTextBox.FontFamily = buffer;

					ReflectFontMetric(this.MainTextBox);
				}
			};
		}

		#endregion

		#region Font Style

		private void PrepareFontStyle()
		{
			this.FontStyleComboBox.ItemsSource = new[] { FontStyles.Normal, FontStyles.Italic }.ToArray();

			this.FontStyleComboBox.SelectedItem = FontStyles.Normal;

			this.FontStyleComboBox.SelectionChanged += (_, _) =>
			{
				this.MainTextBox.FontStyle = (FontStyle)this.FontStyleComboBox.SelectedItem;

				ReflectFontMetric(this.MainTextBox);
			};
		}

		#endregion

		#region Font Weight

		private static readonly Lazy<PropertyInfo[]> _fontWeightsProperties = new(() =>
			typeof(FontWeights).GetProperties(BindingFlags.Public | BindingFlags.Static)
			.Where(x => x.PropertyType == typeof(FontWeight))
			.ToArray());

		private void PrepareFontWeight()
		{
			this.FontWeightComboBox.ItemsSource = _fontWeightsProperties.Value.Select(x => x.Name).ToArray();

			this.FontWeightComboBox.SelectedItem = nameof(FontWeights.Regular);

			this.FontWeightComboBox.SelectionChanged += (_, _) =>
			{
				var fontWeightName = this.FontWeightComboBox.SelectedItem as string;
				var fontWeightProperty = _fontWeightsProperties.Value.Single(x => x.Name == fontWeightName);
				this.MainTextBox.FontWeight = (FontWeight)fontWeightProperty.GetValue(null)!;

				ReflectFontMetric(this.MainTextBox);
			};
		}

		#endregion

		#region Font Size

		private void PrepareFontSize()
		{
			this.FontSizeComboBox.ItemsSource = Enumerable.Range(10, (300 - 10 + 1)) // From 10 to 300
				.ToArray();

			this.FontSizeComboBox.SelectedItem = 100;

			this.FontSizeComboBox.SelectionChanged += (_, _) =>
			{
				this.MainTextBox.FontSize = (int)this.FontSizeComboBox.SelectedItem;

				ReflectFontMetric(this.MainTextBox);
			};
		}

		#endregion

		#region Zoom

		public double ZoomRatio
		{
			get { return (double)GetValue(ZoomRatioProperty); }
			set { SetValue(ZoomRatioProperty, value); }
		}
		public static readonly DependencyProperty ZoomRatioProperty =
			DependencyProperty.Register("ZoomRatio", typeof(double), typeof(MainWindow), new PropertyMetadata(1D));

		private void PrepareZoom()
		{
			this.ZoomComboBox.ItemsSource = Enumerable.Range(10, (100 - 10 + 1)) // From 10 to 100
				.Select(x => ConvertToPercentage(x / 10D))
				.ToArray();

			ZoomRatio = 1.6;
			this.ZoomComboBox.SelectedItem = ConvertToPercentage(ZoomRatio);

			this.ZoomComboBox.SelectionChanged += (_, _) =>
			{
				ZoomRatio = ConvertFromPercentage(this.ZoomComboBox.SelectedItem?.ToString());
			};

			static string ConvertToPercentage(double ratio) => $"{Math.Round(ratio * 100D)} %";

			static double ConvertFromPercentage(string? percentage) =>
				double.TryParse(percentage?.TrimEnd('%'), out double buffer) ? (buffer / 100D) : 1D;
		}

		#endregion

		public MainWindow()
		{
			InitializeComponent();

			PrepareFontFamily("Times New Roman");
			PrepareFontStyle();
			PrepareFontWeight();
			PrepareFontSize();
			PrepareZoom();

			this.Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			ReflectFontMetric(this.MainTextBox);

			this.MainTextBox.TextChanged += (_, _) =>
			{
				ReflectFontMetric(this.MainTextBox);
			};
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);

			Keyboard.ClearFocus();
		}

		#region Metric

		public double Ascent
		{
			get { return (double)GetValue(AscentProperty); }
			set { SetValue(AscentProperty, value); }
		}
		public static readonly DependencyProperty AscentProperty =
			DependencyProperty.Register("Ascent", typeof(double), typeof(MainWindow), new PropertyMetadata(0D));

		public double Uppercase
		{
			get { return (double)GetValue(UppercaseProperty); }
			set { SetValue(UppercaseProperty, value); }
		}
		public static readonly DependencyProperty UppercaseProperty =
			DependencyProperty.Register("Uppercase", typeof(double), typeof(MainWindow), new PropertyMetadata(0D));

		public double Lowercase
		{
			get { return (double)GetValue(LowercaseProperty); }
			set { SetValue(LowercaseProperty, value); }
		}
		public static readonly DependencyProperty LowercaseProperty =
			DependencyProperty.Register("Lowercase", typeof(double), typeof(MainWindow), new PropertyMetadata(0D));

		public double Baseline
		{
			get { return (double)GetValue(BaselineProperty); }
			set { SetValue(BaselineProperty, value); }
		}
		public static readonly DependencyProperty BaselineProperty =
			DependencyProperty.Register("Baseline", typeof(double), typeof(MainWindow), new PropertyMetadata(0D));

		public double Descent
		{
			get { return (double)GetValue(DescentProperty); }
			set { SetValue(DescentProperty, value); }
		}
		public static readonly DependencyProperty DescentProperty =
			DependencyProperty.Register("Descent", typeof(double), typeof(MainWindow), new PropertyMetadata(0D));

		public double Bottom
		{
			get { return (double)GetValue(BottomProperty); }
			set { SetValue(BottomProperty, value); }
		}
		public static readonly DependencyProperty BottomProperty =
			DependencyProperty.Register("Bottom", typeof(double), typeof(MainWindow), new PropertyMetadata(0D));

		public double FontCenter
		{
			get { return (double)GetValue(FontCenterProperty); }
			set { SetValue(FontCenterProperty, value); }
		}
		public static readonly DependencyProperty FontCenterProperty =
			DependencyProperty.Register("FontCenter", typeof(double), typeof(MainWindow), new PropertyMetadata(0D));

		public double UppercaseCenter
		{
			get { return (double)GetValue(UppercaseCenterProperty); }
			set { SetValue(UppercaseCenterProperty, value); }
		}
		public static readonly DependencyProperty UppercaseCenterProperty =
			DependencyProperty.Register("UppercaseCenter", typeof(double), typeof(MainWindow), new PropertyMetadata(0D));

		public double ExtentCenter
		{
			get { return (double)GetValue(ExtentCenterProperty); }
			set { SetValue(ExtentCenterProperty, value); }
		}
		public static readonly DependencyProperty ExtentCenterProperty =
			DependencyProperty.Register("ExtentCenter", typeof(double), typeof(MainWindow), new PropertyMetadata(0D));

		public bool? OffsetState
		{
			get { return (bool?)GetValue(OffsetStateProperty); }
			set { SetValue(OffsetStateProperty, value); }
		}
		public static readonly DependencyProperty OffsetStateProperty =
			DependencyProperty.Register("OffsetState", typeof(bool?), typeof(MainWindow),
				new PropertyMetadata(false, (d, e) =>
				{
					var instance = (MainWindow)d;
					instance.ReflectFontMetric(instance.MainTextBox);
				}));

		public double Offset
		{
			get { return (double)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		public static readonly DependencyProperty OffsetProperty =
			DependencyProperty.Register("Offset", typeof(double), typeof(MainWindow), new PropertyMetadata(0D));

		public Thickness OffsetPadding
		{
			get { return (Thickness)GetValue(OffsetPaddingProperty); }
			set { SetValue(OffsetPaddingProperty, value); }
		}
		public static readonly DependencyProperty OffsetPaddingProperty =
			DependencyProperty.Register("OffsetPadding", typeof(Thickness), typeof(MainWindow), new PropertyMetadata(default(Thickness)));

		private void ReflectFontMetric(TextBox textBox)
		{
			var metric = FontMetricHelper.Create(textBox);

			Ascent = metric.Ascent;
			Uppercase = metric.Uppercase;
			Lowercase = metric.Lowercase;
			Baseline = metric.Baseline;
			Descent = metric.Descent;
			Bottom = metric.Bottom;
			FontCenter = metric.Bottom / 2D;
			UppercaseCenter = (metric.Uppercase + metric.Baseline) / 2D;
			ExtentCenter = Descent - metric.Extent / 2D;

			(Offset, OffsetPadding) = (OffsetState, metric.UppercaseCenterOffset, metric.ExtentCenterOffset) switch
			{
				(false, < 0, _) => (metric.UppercaseCenterOffset, new Thickness(0, (metric.UppercaseCenterOffset * -2D), 0, 0)),
				(false, > 0, _) => (metric.UppercaseCenterOffset, new Thickness(0, 0, 0, (metric.UppercaseCenterOffset * 2D))),
				(true, _, < 0) => (metric.ExtentCenterOffset, new Thickness(0, (metric.ExtentCenterOffset * -2D), 0, 0)),
				(true, _, > 0) => (metric.ExtentCenterOffset, new Thickness(0, 0, 0, (metric.ExtentCenterOffset * 2D))),
				_ => (0, new Thickness(0))
			};
		}

		#endregion
	}
}