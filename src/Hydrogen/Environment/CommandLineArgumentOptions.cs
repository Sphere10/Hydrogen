using System;

namespace Sphere10.Framework {
	[Flags]
	public enum CommandLineArgumentOptions {
		SingleDash = 1 << 0,
		DoubleDash = 1 << 1,
		ForwardSlash = 1 << 2,
		CaseSensitive = 1 << 3,
		PrintHelpOnH = 1 << 4,
		PrintHelpOnHelp = 1 << 5,
		Default = DoubleDash | PrintHelpOnH | PrintHelpOnHelp
	}
}
