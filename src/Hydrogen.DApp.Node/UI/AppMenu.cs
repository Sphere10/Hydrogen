using System.ComponentModel;

namespace Hydrogen.DApp.Node.UI {

	public enum AppMenu {
		[Description("_File")]
		File,

		[Description("_Wallet")]
		Wallet,

		[Description("_Settings")]
		Settings,

		[Description("_Explorer")]
		Explorer,

		[Description("D_iagnostic")]
		Diagnostic,

		[Description("_Development")]
		Development,

		[Description("_WebBrowser")]
		WebBrowser,
	}

}
