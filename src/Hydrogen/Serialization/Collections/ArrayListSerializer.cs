// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections;

namespace Hydrogen;

public class ArrayListSerializer : CollectionSerializerBase<ArrayList, object> {

	public ArrayListSerializer(SerializerFactory factory, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) 
		: base(factory.GetSerializer<object>(), sizeDescriptorStrategy) {
	}

	protected override long GetLength(ArrayList collection) => collection.Count;

	protected override ArrayList Activate(long capacity) => new(checked((int)capacity));

	protected override void SetItem(ArrayList collection, long index, object item) {
		Guard.Ensure(collection.Count == index, "Unexpected index");
		collection.Add(item);
	}

}
