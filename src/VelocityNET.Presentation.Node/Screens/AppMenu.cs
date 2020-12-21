using System.ComponentModel;

namespace VelocityNET.Presentation.Node {

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
	}

}
