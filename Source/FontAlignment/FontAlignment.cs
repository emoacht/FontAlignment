using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace FontAlignment;

/// <summary>
/// Attached properties for font alignment
/// </summary>
public static class FontAlignment
{
	private class Item
	{
		public FontAlignmentMethod Method;
		public Thickness BasePadding;
		public string? Text;

		public static IReadOnlyDictionary<FrameworkElement, Item> Elements => _elements;
		private static readonly Dictionary<FrameworkElement, Item> _elements = new();

		public static Item GetItem(FrameworkElement element)
		{
			if (!_elements.TryGetValue(element, out var item))
			{
				_elements[element] = item = new Item();
				element.Unloaded += OnElementUnloaded;
			}
			return item;
		}

		private static void OnElementUnloaded(object sender, RoutedEventArgs e)
		{
			if (sender is not FrameworkElement element)
				return;

			element.Loaded -= OnElementLoaded;
			element.Unloaded -= OnElementUnloaded;
			_elements.Remove(element);
		}
	}

	/// <summary>
	/// Gets method for font alignment.
	/// </summary>
	public static FontAlignmentMethod GetMethod(DependencyObject obj)
	{
		return (FontAlignmentMethod)obj.GetValue(MethodProperty);
	}
	/// <summary>
	/// Sets method for font alignment.
	/// </summary>
	public static void SetMethod(DependencyObject obj, FontAlignmentMethod value)
	{
		obj.SetValue(MethodProperty, value);
	}
	/// <summary>
	/// Method for font alignment
	/// </summary>
	public static readonly DependencyProperty MethodProperty =
		DependencyProperty.RegisterAttached(
			"Method",
			typeof(FontAlignmentMethod),
			typeof(FontAlignment),
			new PropertyMetadata(
				FontAlignmentMethod.None,
				(d, e) =>
				{
					if (d is not FrameworkElement element)
						return;

					var item = Item.GetItem(element);
					item.Method = (FontAlignmentMethod)e.NewValue;
				}));

	/// <summary>
	/// Gets padding to be used as a basis for font alignment.
	/// </summary>
	public static Thickness GetBasePadding(DependencyObject obj)
	{
		return (Thickness)obj.GetValue(BasePaddingProperty);
	}
	/// <summary>
	/// Sets padding to be used as a basis for font alignment.
	/// </summary>
	public static void SetBasePadding(DependencyObject obj, Thickness value)
	{
		obj.SetValue(BasePaddingProperty, value);
	}
	/// <summary>
	/// Padding to be used as a basis for font alignment
	/// </summary>
	public static readonly DependencyProperty BasePaddingProperty =
		DependencyProperty.RegisterAttached(
			"BasePadding",
			typeof(Thickness),
			typeof(FontAlignment),
			new PropertyMetadata(
				default(Thickness),
				(d, e) =>
				{
					if (d is not FrameworkElement element)
						return;

					var item = Item.GetItem(element);
					item.BasePadding = (Thickness)e.NewValue;
				}));

	/// <summary>
	/// Gets text to be reflected to font alignment.
	/// </summary>
	public static string GetText(DependencyObject obj)
	{
		return (string)obj.GetValue(TextProperty);
	}
	/// <summary>
	/// Sets text to be reflected to font alignment.
	/// </summary>
	public static void SetText(DependencyObject obj, string value)
	{
		obj.SetValue(TextProperty, value);
	}
	/// <summary>
	/// Text to be reflected to font alignment
	/// </summary>
	public static readonly DependencyProperty TextProperty =
		DependencyProperty.RegisterAttached(
			"Text",
			typeof(string),
			typeof(FontAlignment),
			new PropertyMetadata(
				null,
				(d, e) =>
				{
					if (d is not FrameworkElement element)
						return;

					var item = Item.GetItem(element);
					item.Text = (string)e.NewValue;

					if (!element.IsLoaded)
					{
						element.Loaded += OnElementLoaded;
					}
					else
					{
						AdjustPadding(element, item);
					}
				}));

	private static void OnElementLoaded(object sender, RoutedEventArgs e)
	{
		if (sender is not FrameworkElement element)
			return;

		element.Loaded -= OnElementLoaded;

		var item = Item.GetItem(element);
		if (string.IsNullOrWhiteSpace(item.Text))
			return;

		AdjustPadding(element, item);
	}

	private static void AdjustPadding(FrameworkElement element, Item item)
	{
		switch (element)
		{
			case TextBlock textBlock:
				{
					var metric = FontMetricHelper.Create(textBlock, item.Text);
					textBlock.Padding = GetPadding(item, metric);
				}
				break;

			case Control control:
				{
					var metric = FontMetricHelper.Create(control, item.Text);
					control.Padding = GetPadding(item, metric);
				}
				break;
		}

		static Thickness GetPadding(Item item, FontMetric metric)
		{
			var (top, bottom) = (item.Method, metric.UppercaseCenterOffset, metric.ExtentCenterOffset) switch
			{
				(FontAlignmentMethod.UppercaseCenterShrink, < 0, _) => (0D, (metric.UppercaseCenterOffset * 2D)),
				(FontAlignmentMethod.UppercaseCenterShrink, > 0, _) => ((metric.UppercaseCenterOffset * -2D), 0D),
				(FontAlignmentMethod.UppercaseCenterExpand, < 0, _) => ((metric.UppercaseCenterOffset * -2D), 0D),
				(FontAlignmentMethod.UppercaseCenterExpand, > 0, _) => (0D, (metric.UppercaseCenterOffset * 2D)),
				(FontAlignmentMethod.ExtentCenterShrink, _, < 0) => (0D, (metric.ExtentCenterOffset * 2D)),
				(FontAlignmentMethod.ExtentCenterShrink, _, > 0) => ((metric.ExtentCenterOffset * -2D), 0D),
				(FontAlignmentMethod.ExtentCenterExpand, _, < 0) => ((metric.ExtentCenterOffset * -2D), 0D),
				(FontAlignmentMethod.ExtentCenterExpand, _, > 0) => (0D, (metric.ExtentCenterOffset * 2D)),
				_ => (0, 0)
			};

			if ((top, bottom) == (0, 0))
				return item.BasePadding;

			Debug.WriteLine($"Padding Top:{top} Bottom:{bottom}");

			return new Thickness(
				item.BasePadding.Left,
				item.BasePadding.Top + top,
				item.BasePadding.Right,
				item.BasePadding.Bottom + bottom);
		}
	}

	/// <summary>
	/// Adjusts paddings of all elements immediately.
	/// </summary>
	public static void AdjustPadding()
	{
		// Deconstruct method for KeyValuePair is not available on .NET Framework as is.
		foreach (var pair in Item.Elements)
			AdjustPadding(pair.Key, pair.Value);
	}
}