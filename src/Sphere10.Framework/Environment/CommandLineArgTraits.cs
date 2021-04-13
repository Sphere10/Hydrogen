using System;

namespace Sphere10.Framework {
	[Flags]
	public enum CommandLineArgTraits {
		Optional,   // must occur 0..1 times
		Mandatory,  // must occur 1 time
		Multiple,   // if optional, can occur 0..N times, if mandatory can occur 1..N times
	}
}
