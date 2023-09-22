﻿// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class NullableObjectSerializer<T> : ItemSerializerDecorator<T> {

	public NullableObjectSerializer(IItemSerializer<T> valueSerializer, bool preserveConstantLength = false) 
		: base(new BoxedNullableSerializer<T>(valueSerializer,preserveConstantLength ).AsProjection(x => x.HasValue ? x.Value : default, x => x is not null ? new BoxedNullable<T>(x) : new BoxedNullable<T>())) {
	}

	public override bool SupportsNull => true;
}
