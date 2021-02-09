using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sphere10.Framework;
using Terminal.Gui;

namespace Sphere10.Hydrogen.Node.UI {
	public static class Dialogs {


		public static bool SelectEnum<T>(string title, string description, T currentSelection, out T selection) where T : struct, Enum {
			var enumValues = Enum.GetValues<T>();
			var currIndex = Array.IndexOf(enumValues, currentSelection);
			Guard.Argument(currIndex >= 0, nameof(currentSelection), "Not a value of enumeration");
			var datasource = new ListDataSource<T>(enumValues, x => Enum.GetName(x), x => Tools.Enums.GetDescription(x));
			var dlg = new ListDialog<T>(title, description, datasource, currIndex);
			Application.Run(dlg);
			if (!dlg.Cancelled) {
				selection = dlg.SelectedValue;
				return true;
			}
			selection = default;
			return false;
		}

		public static void Error(string title, string message) {
			MessageBox.ErrorQuery(title, message);
		}

		public static void Exception(Exception error) {
			Error("Error", error.ToDisplayString());
		}

	}
}
