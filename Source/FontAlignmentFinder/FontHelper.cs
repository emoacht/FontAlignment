using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FontAlignmentFinder
{
	internal static class FontHelper
	{
		public static string[] AvailableFontNames => ExternalFontNames.Concat(SystemFontNames).ToArray();

		public static bool TryGetFontFamily(string fontName, out FontFamily? fontFamily)
		{
			if (!string.IsNullOrWhiteSpace(fontName))
			{
				if (TryGetExternalFontFamily(fontName, out fontFamily))
					return true;

				if (TryGetSystemFontFamily(fontName, out fontFamily))
					return true;
			}
			fontFamily = null;
			return false;
		}

		#region External font

		public static Environment.SpecialFolder SpecialFolder { get; set; }

		public static IEnumerable<string> ExternalFontNames => _externalFontPairs.Value.Keys.AsEnumerable();

		private static readonly Lazy<Dictionary<string, string>> _externalFontPairs = new(() =>
			EnumerateExternalFontPairs()
				.GroupBy(x => x.fontName)
				.Select(x => x.First())
				.ToDictionary(x => x.fontName, x => x.fontFolderPath));

		private static IEnumerable<(string fontName, string fontFolderPath)> EnumerateExternalFontPairs()
		{
			var folderPath = Environment.GetFolderPath(SpecialFolder);
			if (string.IsNullOrEmpty(folderPath))
				return Enumerable.Empty<(string, string)>();

			return EnumerateExternalFontPairs(folderPath);
		}

		private static IEnumerable<(string fontName, string fontFolderPath)> EnumerateExternalFontPairs(string folderPath)
		{
			foreach (var fontFilePath in Directory.EnumerateFiles(folderPath, "*.ttf", SearchOption.AllDirectories))
			{
				var fontFolderPath = Path.GetDirectoryName(fontFilePath);
				if (string.IsNullOrEmpty(fontFolderPath))
					continue;

				using var collection = new System.Drawing.Text.PrivateFontCollection();
				collection.AddFontFile(fontFilePath);

				foreach (System.Drawing.FontFamily fontFamily in collection.Families)
					yield return (fontFamily.Name, fontFolderPath);
			}
		}

		private static bool TryGetExternalFontFamily(string fontName, out FontFamily? fontFamily)
		{
			if (_externalFontPairs.Value.TryGetValue(fontName, out string? fontFolderPath))
			{
				fontFamily = new FontFamily($"file:///{fontFolderPath}/#{fontName}");
				return true;
			}
			fontFamily = default;
			return false;
		}

		#endregion

		#region System font

		public static IEnumerable<string> SystemFontNames => Fonts.SystemFontFamilies.Select(x => x.Source);

		private static bool TryGetSystemFontFamily(string fontName, out FontFamily? fontFamily)
		{
			fontFamily = Fonts.SystemFontFamilies.FirstOrDefault(x => x.Source == fontName);
			return (fontFamily is not null);
		}

		#endregion
	}
}