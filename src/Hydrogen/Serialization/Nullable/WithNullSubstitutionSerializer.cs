// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

internal sealed class WithNullSubstitutionSerializer<TItem> : ItemSerializerDecorator<TItem> {

	private readonly TItem _nullSubstitution;
	private readonly IEqualityComparer<TItem> _equalityComparer;

	public WithNullSubstitutionSerializer(IItemSerializer<TItem> internalHasher, TItem nullSubstitution, IEqualityComparer<TItem> equalityComparer = null)
		: base(internalHasher) {
		_nullSubstitution = nullSubstitution;
		_equalityComparer = equalityComparer ?? EqualityComparer<TItem>.Default;
	}

	public override bool SupportsNull => true;

	public override long CalculateSize(SerializationContext context, TItem item) 
		=> base.CalculateSize(context, item ?? _nullSubstitution);

	public override TItem Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var item = base.Deserialize(reader, context);
		return _equalityComparer.Equals(item, _nullSubstitution) ? default : item;
	}

	public override void Serialize(TItem item, EndianBinaryWriter writer, SerializationContext context) 
		=> base.Serialize(item ?? _nullSubstitution, writer, context);
}
