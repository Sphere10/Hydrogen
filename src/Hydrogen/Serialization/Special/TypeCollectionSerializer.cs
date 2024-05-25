// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
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