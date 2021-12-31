
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
	/// Aligns the vertical center of uppercase characters by shrinking padding.
	/// </summary>
	UppercaseCenterShrink,

	/// <summary>
	/// Aligns the vertical center of uppercase characters by expanding padding.
	/// </summary>
	UppercaseCenterExpand,

	/// <summary>
	/// Aligns the vertical center of extent by shrinking padding.
	/// </summary>
	ExtentCenterShrink,

	/// <summary>
	/// Aligns the vertical center of extent by expanding padding.
	/// </summary>
	ExtentCenterExpand
}