// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// Tracks and maintains a list of of spare indices in an <see cref="ObjectContainer"/> so they can be re-used in
/// subsequent operations. This is needed by a <see cref="StreamMappedRecyclableList{TItem}"/>
/// </summary>
internal class RecyclableIndexIndex : MetaDataObserverBase {
	
	private IStack<long> _freeIndexStack;

	public RecyclableIndexIndex(ObjectContainer objectContainer, long reservedStreamIndex) 
		: base(objectContainer, reservedStreamIndex) {
		_freeIndexStack = null;
	}

	public IStack<long> Stack { 
		get {
			CheckAttached();
			return _freeIndexStack;
		}
	}

	protected override void AttachInternal() {
		// the free index stack is an in-stream stack at all times (no memory overhead)
		_freeIndexStack = new StreamPagedList<long>(
			PrimitiveSerializer<long>.Instance,
			Stream,
			Endianness.LittleEndian,
			false,
			true
		).AsStack();
	}

	protected override void DetachInternal() {
		_freeIndexStack = null;
	}

	protected override void OnReaped(long index) {
		Stack.Push(index);
	}
}
