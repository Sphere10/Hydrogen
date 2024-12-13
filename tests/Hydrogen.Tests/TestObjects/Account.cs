using System.Collections;
using Hydrogen.ObjectSpaces;

namespace Hydrogen.Tests;

[EqualityComparer(typeof(AccountEqualityComparer))]
public class Account {

	[Identity]
	public string Name { get; set; }

	public decimal Quantity { get; set; }

	[UniqueIndex]
	public long UniqueNumber { get; set; }	

	public Identity Identity { get; set; }

	[Transient]
	public bool Dirty { get; set; }

}