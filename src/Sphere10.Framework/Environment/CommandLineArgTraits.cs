using System;

namespace Sphere10.Framework {
	[Flags]
	public enum CommandLineArgTraits {
		Optional = 1,  // must occur 0..1 times
		Mandatory = 2,  // must occur 1 time
		Multiple = 4,   // if optional, can occur 0..N times, if mandatory can occur 1..N times
	}
}
