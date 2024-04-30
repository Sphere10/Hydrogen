using System;

namespace Hydrogen;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class IdentityAttribute : Attribute {
	public string IndexName { get; set; } = null;
}

