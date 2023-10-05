using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.Tests;

public class ComplexObject {

	public object ObjectProperty { get; set; }

	public ComplexObject RecursiveProperty { get; set; }

	public TestObject TestProperty { get; set; }

	public CrudAction? NullableEnumProperty { get; set; }

	public IList<ComplexObject> ManyRecursiveProperty { get; set; }

	public override string ToString() => 
		$"""
		[ComplexObject]
			{nameof(ObjectProperty)}: {(ReferenceEquals(ObjectProperty, this) ? "this" : ObjectProperty?.ToString())}, 
			{nameof(RecursiveProperty)}: {RecursiveProperty},
			{nameof(TestProperty)}: '{TestProperty}',
			{nameof(ManyRecursiveProperty)}: {(ManyRecursiveProperty is not null ? ManyRecursiveProperty.Select(x => x.ToString()).ToDelimittedString(", ") : "NULL") }
		""";
}