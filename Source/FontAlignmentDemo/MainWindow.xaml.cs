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

namespace FontAlignmentDemo
{
	public partial class MainWindow : Window
	{
		#region Font Family

		public FontFamily MainFontFamily
		{
			get { return (FontFamily)GetValue(MainFontFamilyProperty); }
			set { SetValue(MainFontFamilyProperty, value); }
		}
		public static readonly DependencyProperty MainFontFamilyProperty =
			DependencyProperty.Register("MainFontFamily", typeof(FontFamily), typeof(MainWindow), new PropertyMetadata(default(FontFamily)));

		private void PrepareFontFamily()
		{
			this.FontFamilyComboBox.ItemsSource = Fonts.SystemFontFamilies.Select(x => x.Source).ToArray();

			if (TryFindFontFamily("Tahoma", out FontFamily? buffer))
			{
				MainFontFamily = buffer!;
				this.FontFamilyComboBox.SelectedItem = buffer?.Source;
			}

			this.FontFamilyComboBox.SelectionChanged += (_, _) =>
			{
				var fontFamilyName = this.FontFamilyComboBox.SelectedItem?.ToString();

				if (TryFindFontFamily(fontFamilyName, out FontFamily? buffer))
				{
					MainFontFamily = buffer!;

					FontAlignment.FontAlignment.AdjustPaddings();
				}
			};

			static bool TryFindFontFamily(string? fontFamilyName, out FontFamily? fontFamily)
			{
				fontFamily = Fonts.SystemFontFamilies.FirstOrDefault(x => x.Source == fontFamilyName);
				return (fontFamily is not null);
			}
		}

		#endregion

		public MainWindow()
		{
			InitializeComponent();

			PrepareFontFamily();
		}
	}
}
