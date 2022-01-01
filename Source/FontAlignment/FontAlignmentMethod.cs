
namespace FontAlignment;

/// <summary>
/// Method for font alignment
/// </summary>
public enum FontAlignmentMethod
{
	/// <summary>
	/// Dose nothing.
	/// </summary>
	None = 0,

	/// <summary>
	/// Aligns the vertical center of Uppercase characters by shrinking padding.
	/// </summary>
	UppercaseCenterShrink,

	/// <summary>
	/// Aligns the vertical center of Uppercase characters by expanding padding.
	/// </summary>
	UppercaseCenterExpand,

	/// <summary>
	/// Aligns the vertical center of Extent by shrinking padding.
	/// </summary>
	ExtentCenterShrink,

	/// <summary>
	/// Aligns the vertical center of Extent by expanding padding.
	/// </summary>
	ExtentCenterExpand
}