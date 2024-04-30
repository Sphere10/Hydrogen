using System;
using System.Collections.Generic;

namespace Hydrogen;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class EqualityComparerAttribute(Type type) : Attribute {

	public Type EqualityComparerType { get; } = type;

}
