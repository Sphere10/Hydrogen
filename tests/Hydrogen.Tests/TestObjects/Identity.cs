using Hydrogen.ObjectSpaces;

namespace Hydrogen.Tests;

public class Identity {

	public DSS DSS { get; set; }

	[UniqueProperty]
	public byte[] Key { get; set; }
}
