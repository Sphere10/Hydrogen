using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Hydrogen;

/// <summary>
/// Implements various extensions for activating decorators, wrappers, adapters.
/// </summary>
public static partial class DecoratorExtensions {

	public static IItemChecksummer<T> WithSubstitution<T>(this IItemChecksummer<T> serializer, int reservedValue, int substitutionValue)
		=> new WithSubstitutionChecksummer<T>(serializer, reservedValue, substitutionValue);
	
	public static IItemSerializer<TTo> AsProjection<TFrom, TTo>(this IItemSerializer<TFrom> serializer, Func<TFrom, TTo> projection, Func<TTo, TFrom> inverseProjection) 
		=> new ProjectedSerializer<TFrom, TTo>(serializer, projection, inverseProjection);

	public static PackedSerializer AsPacked<TItem>(this IItemSerializer<TItem> serializer) => PackedSerializer.Pack(serializer);
	
}
