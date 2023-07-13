// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

/// <summary>
/// A Serializer that works for base-level objects by delegating actual serialization to registered concrete-level serializers. 
/// </summary>
/// <typeparam name="TBase">The type of object which is serialized/deserialized</typeparam>
public interface IFactorySerializer<TBase> : IItemSerializer<TBase> {

	IEnumerable<Type> RegisteredTypes { get; }

	public void RegisterSerializer<TConcrete>(ushort typeCode, IItemSerializer<TConcrete> concreteSerializer) where TConcrete : TBase;

	ushort GetTypeCode<TConcrete>(TConcrete item) where TConcrete : TBase => GetTypeCode(item.GetType());

	ushort GetTypeCode(Type type);

	ushort GenerateTypeCode();

}
