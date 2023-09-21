// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
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

	public override long CalculateSize(TItem item) 
		=> base.CalculateSize(item ?? _nullSubstitution);

	public override TItem DeserializeInternal(long byteSize, EndianBinaryReader reader) {
		var item = base.DeserializeInternal(byteSize, reader);
		return _equalityComparer.Equals(item, _nullSubstitution) ? default : item;
	}

	public override void SerializeInternal(TItem item, EndianBinaryWriter writer) 
		=> base.SerializeInternal(item ?? _nullSubstitution, writer);
}
