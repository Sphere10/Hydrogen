using System;

namespace Hydrogen;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public abstract class IndexAttributeBase : Attribute {
	public string IndexName { get; set; } = null;

	public IndexNullPolicy NullPolicy { get; set; } = IndexNullPolicy.IgnoreNull;
}
