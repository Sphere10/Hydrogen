﻿// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// Tracks and maintains a list of of spare indices in an <see cref="ObjectStream"/> so they can be re-used in
/// subsequent operations. This is needed by a <see cref="StreamMappedRecyclableList{TItem}"/>
/// </summary>
internal class RecyclableIndexIndex : IndexBase<long, StackBasedMetaDataStore<long>> {

	public RecyclableIndexIndex(ObjectStream objectStream, long reservedStreamIndex) 
		: base(objectStream, new StackBasedMetaDataStore<long>(objectStream.Streams, reservedStreamIndex, PrimitiveSerializer<long>.Instance)) {
	}

	public IStack<long> Stack => KeyStore.Stack;

	protected override void OnInserted(object item, long index) {
		throw new InvalidOperationException($"A {typeof(RecyclableIndexIndex).ToStringCS()} cannot be used on a objectStream which inserts items. Items can only be reaped when using this index");
	}

	protected override void OnRemoved(long index) {
		throw new InvalidOperationException($"A {typeof(RecyclableIndexIndex).ToStringCS()} cannot be used on a objectStream which removes items. Items can only be reaped when using this index");
	}

	protected override void OnReaped(long index) {
		Stack.Push(index);
	}
}
