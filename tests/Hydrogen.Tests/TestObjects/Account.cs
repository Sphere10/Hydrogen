using System;
using Hydrogen.ObjectSpaces;

namespace Hydrogen.Tests;

public class Account {

	public string Name { get; set; }

	public decimal Quantity { get; set; }

	public long UniqueNumber { get; set; }	

	public Identity Identity { get; set; }

}
