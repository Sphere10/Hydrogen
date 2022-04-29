using System.Text.RegularExpressions;

namespace Sphere10.Hydrogen.Presentation2.UI.Controls.BlazorGrid.Classes {
	public static class Utility {

		public static bool IsNullOrEmpty(this string text) {
			return string.IsNullOrEmpty(text);
		}
		public static string AddSpacesForCamelCase(string text) {
			if (text.IsNullOrEmpty()) return string.Empty;

			return Regex.Replace(text, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");
		}
	}
}