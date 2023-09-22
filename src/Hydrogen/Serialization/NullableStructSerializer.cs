// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


namespace Hydrogen;

public class NullableStructSerializer<T> : ItemSerializerDecorator<T?> where T : struct {

	public NullableStructSerializer(IItemSerializer<T> valueSerializer, bool preserveConstantLength = false) 
		: base(new BoxedNullableSerializer<T>(valueSerializer, preserveConstantLength ).AsProjection(x => x.HasValue ? new T?(x.Value) : null, x => x.HasValue ? new BoxedNullable<T>(x.Value) : new BoxedNullable<T>())) {
	}

	public static NullableStructSerializer<T> Instance { get; } = new(PrimitiveSerializer<T>.Instance);

	public override bool SupportsNull => true;
}
