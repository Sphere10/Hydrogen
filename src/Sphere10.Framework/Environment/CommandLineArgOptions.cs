using System;

namespace Sphere10.Framework {
	[Flags]
	public enum CommandLineArgOptions {
		SingleDash,
		DoubleDash,
		ForwardSlash,
		CaseSensitive,
		PrintHelpOnH,
		PrintHelpOnHelp,
	}
}
