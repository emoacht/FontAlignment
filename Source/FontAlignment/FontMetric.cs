using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FontAlignment;

/// <summary>
/// Font metric information
/// </summary>
public class FontMetric
{
	/// <summary>
	/// Distance from Top to Ascent (the topmost drawn pixel).
	/// </summary>
	public double Ascent { get; }

	/// <summary>
	/// Distance from Top to Uppercase (uppercase alphabetic charactors).
	/// This position matches cap height.
	/// </summary>
	public double Uppercase { get; }

	/// <summary>
	/// Distance from Top to Lowercase (lowercase alphabetic charactors).
	/// This position matches x-height.
	/// </summary>
	public double Lowercase { get; }

	/// <summary>
	/// Distance from Top to Baseline.
	/// </summary>
	public double Baseline { get; }

	/// <summary>
	/// Distance from Top to Descent (the bottommost drawn pixel).
	/// </summary>
	public double Descent { get; }

	/// <summary>
	/// Distance from Top to Bottom.
	/// </summary>
	public double Bottom { get; }

	/// <summary>
	/// Distance from Ascent to Descent.
	/// This is equals to <see cref="FormattedText.Extent"/>.
	/// </summary>
	public double Extent { get; }

	/// <summary>
	/// Offset length from vertical center of font to vertical center of uppercase charactors.
	/// This is equal to the difference between top space (from Top to Uppercase) and bottom
	/// space (from Baseline to Bottom) divided by 2.
	/// </summary>
	public double UppercaseCenterOffset { get; }

	/// <summary>
	/// Offset length from vertical center of font to vertical center of Extent.
	/// This is equal to the difference between top space (from Top to Ascent) and buttom scape
	/// (from Descent to Bottom) divided by 2.
	/// </summary>
	public double ExtentCenterOffset { get; }

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="formattedText">FormattedText</param>
	/// <param name="typeface">Font typeface</param>
	/// <param name="emSize">Font size</param>
	public FontMetric(FormattedText formattedText, Typeface typeface, double emSize)
	{
		Extent = formattedText.Extent;
		Bottom = formattedText.Height;
		Baseline = formattedText.Baseline;

		Descent = Bottom + formattedText.OverhangAfter;
		Ascent = Descent - Extent;

		Uppercase = Baseline - (typeface.CapsHeight * emSize);
		Lowercase = Baseline - (typeface.XHeight * emSize);

		UppercaseCenterOffset = (Uppercase - (Bottom - Baseline)) / 2D;
		ExtentCenterOffset = (Ascent - (Bottom - Descent)) / 2D;
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="text">Text</param>
	/// <param name="typeface">Font typeface</param>
	/// <param name="emSize">Font size</param>
	/// <param name="pixelsPerDip">Pixels per DIP</param>
	public FontMetric(string text, Typeface typeface, double emSize, double pixelsPerDip) :
		this(GetFormattedText(text, typeface, emSize, pixelsPerDip), typeface, emSize)
	{ }

	private static FormattedText GetFormattedText(string text, Typeface typeface, double emSize, double pixelsPerDip)
	{
		return new FormattedText(
			text,
			CultureInfo.InvariantCulture,
			FlowDirection.LeftToRight,
			typeface,
			emSize,
			Brushes.Black,
			pixelsPerDip);
	}
}

/// <summary>
/// Helper methods for <see cref="FontMetric"/>
/// </summary>
public static class FontMetricHelper
{
	/// <summary>
	/// Creates FontMetric instance from <see cref="TextBlock"/>.
	/// </summary>
	public static FontMetric Create(TextBlock textBlock) => Create(textBlock, textBlock.Text);

	/// <summary>
	/// Creates FontMetric instance from <see cref="TextBlock"/>.
	/// </summary>
	public static FontMetric Create(TextBlock textBlock, string? text)
	{
		var dpi = GetDpi(textBlock, nameof(textBlock));

		var typeface = new Typeface(textBlock.FontFamily,
									textBlock.FontStyle,
									textBlock.FontWeight,
									textBlock.FontStretch);

		return new FontMetric(GetFirstLine(text), typeface, textBlock.FontSize, dpi.PixelsPerDip);
	}

	/// <summary>
	/// Creates FontMetric instance from <see cref="TextBox"/>.
	/// </summary>
	public static FontMetric Create(TextBox textBox) => Create(textBox, textBox.Text);

	/// <summary>
	/// Creates FontMetric instance from <see cref="Control"/>.
	/// </summary>
	public static FontMetric Create(Control control, string? text)
	{
		var dpi = GetDpi(control, nameof(control));

		var typeface = new Typeface(control.FontFamily,
									control.FontStyle,
									control.FontWeight,
									control.FontStretch);

		return new FontMetric(GetFirstLine(text), typeface, control.FontSize, dpi.PixelsPerDip);
	}

	private static DpiScale GetDpi(Visual visual, string parameterName)
	{
		if (visual is null)
			throw new ArgumentNullException(parameterName);

		// If Visual is not loaded, the system DPI will be returned.
		var dpi = VisualTreeHelper.GetDpi(visual);
		return (dpi.PixelsPerDip > 0)
			? dpi
			: throw new InvalidOperationException("DPI is invalid.");
	}

	private static string GetFirstLine(string? source)
	{
		return source?.Split(new[] { "\r\n", "\r", "\n" }, 2, StringSplitOptions.RemoveEmptyEntries)
			.FirstOrDefault() ?? string.Empty;
	}
}