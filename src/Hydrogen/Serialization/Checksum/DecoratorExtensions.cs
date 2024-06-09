// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Reflection;

namespace Hydrogen;

/// <summary>
/// Implements various extensions for activating decorators, wrappers, adapters.
/// </summary>
public static partial class DecoratorExtensions {

	internal static MethodInfo SerializerCastMethod;

	static DecoratorExtensions() {
		//SerializerCastMethod =
		//	typeof(DecoratorExtensions)
		//	.GetMethods(BindingFlags.Static | BindingFlags.Public)
		//	.Single(m => m.Name == nameof(AsCasted) && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 2 && m.ReturnType.IsSubtypeOfGenericType(typeof(IItemSerializer<>)));
			
	}

	public static IItemChecksummer<T> WithSubstitution<T>(this IItemChecksummer<T> serializer, int reservedValue, int substitutionValue)
		=> new WithSubstitutionChecksummer<T>(serializer, reservedValue, substitutionValue);
	
	public static IItemSerializer<TTo> AsProjection<TFrom, TTo>(this IItemSerializer<TFrom> serializer, Func<TFrom, TTo> projection, Func<TTo, TFrom> inverseProjection) 
		=> new ProjectedSerializer<TFrom, TTo>(serializer, projection, inverseProjection);

	//public static IItemSerializer<TTo> AsCasted<TFrom, TTo>(this IItemSerializer<TFrom> serializer) where TTo : TFrom
	//	=> AsProjection(serializer, x => (TTo)(object)x, x => x);

	//public static IItemSerializer<object> AsPacked<TItem>(this IItemSerializer<TItem> serializer) => PackedSerializer.Pack(serializer);
	
}
