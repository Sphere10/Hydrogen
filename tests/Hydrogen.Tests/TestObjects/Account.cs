using Hydrogen.ObjectSpaces;

namespace Hydrogen.Tests;

public class Account {

	[UniqueProperty]
	public string Name { get; set; }

	public decimal Quantity { get; set; }

	public Identity Identity { get; set; }

}
