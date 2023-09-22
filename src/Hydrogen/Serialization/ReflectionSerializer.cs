// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Hydrogen.FastReflection;
using Hydrogen.Mapping;

namespace Hydrogen;

/// <summary>
/// A serializer for any object. It will intelligently serialize it's member fields/properties in a recursive manner and supports
/// circular references between objects.
/// </summary>
public class ReflectionSerializer<T> : CompositeSerializer<T> {


	public ReflectionSerializer(Func<T> activator, MemberSerializationBinding[] memberBindings) : base(activator, memberBindings) {
	}


	private static MemberSerializationBinding[] CreateBindings(SerializerFactory factory, out Func<T> activator) {

	}
}
