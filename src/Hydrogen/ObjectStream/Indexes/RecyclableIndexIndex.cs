// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// Tracks and maintains a list of of spare indices in an <see cref="ObjectStream"/> so they can be re-used in
/// subsequent operations. This is needed by a <see cref="StreamMappedRecyclableList{TItem}"/>
/// </summary>
internal class RecyclableIndexIndex : IndexBase<StackStorageAttachment<long>> {

	public RecyclableIndexIndex(ObjectStream objectStream, string indexName) 
		: base(objectStream, new StackStorageAttachment<long>(objectStream.Streams, indexName, PrimitiveSerializer<long>.Instance)) {
	}

	protected override void OnInserted(object item, long index) {
		throw new NotSupportedException($"{typeof(RecyclableIndexIndex).ToStringCS()}'s cannot be used when inserting items into an {nameof(ObjectStream)}");
	}

	protected override void OnRemoved(long index) {
		throw new NotSupportedException($"{typeof(RecyclableIndexIndex).ToStringCS()}'s cannot be used when removing items into an {nameof(ObjectStream)}");
	}

	protected override void OnReaped(long index) {
		Store.Push(index);
	}

	protected override void OnContainerClearing() {
		CheckAttached();
		Store.Clear();
		Store.Detach();
	}

	protected override void OnContainerCleared() {
		CheckDetached();
		Store.Attach();
	}
}
