using System;

namespace Sphere10.Framework {
	[Flags]
	public enum CommandLineArgOptions {
		SingleDash = 1,
		DoubleDash = 2,
		ForwardSlash = 4,
		CaseSensitive = 8,
		PrintHelpOnH = 16,
		PrintHelpOnHelp = 32
	}
}
