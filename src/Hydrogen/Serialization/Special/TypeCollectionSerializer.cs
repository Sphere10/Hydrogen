// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public sealed class TypeCollectionSerializer : ProjectedSerializer<IEnumerable<(Type, Void)>, IEnumerable<Type>> {
	
	public TypeCollectionSerializer()
		: this(SizeDescriptorStrategy.UseCVarInt) {
	}

	public TypeCollectionSerializer(SizeDescriptorStrategy sizeDescriptorStrategy) 
		: base(new TaggedTypeCollectionSerializer<Void>(VoidSerializer.Instance, sizeDescriptorStrategy), taggedTypes => taggedTypes.Select(x => x.Item1), types => types.Select(type => (type, Void.Value))) {
	}
}