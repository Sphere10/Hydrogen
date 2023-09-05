// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

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
