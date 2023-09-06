// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

internal class ObjectContainerFreeIndexStore : MetaDataProviderBase, IMetaDataStack {
	
	private IStack<long> _freeIndexStack;
	public ObjectContainerFreeIndexStore(ObjectContainer objectContainer, long reservedStreamIndex, long offset) 
		: base(objectContainer, reservedStreamIndex, offset) {
		_freeIndexStack = null;
		objectContainer.PostItemOperation += ObjectContainer_PostItemOperation;
	}

	private void ObjectContainer_PostItemOperation(object source, long index, object item, ObjectContainerOperationType operationType) {
		// When an object is reaped, we remember the index so it can be re-used later
		if (operationType == ObjectContainerOperationType.Reap) {
			Stack.Push(index);
		}
	}

	public IStack<long> Stack { 
		get {
			CheckAttached();
			return _freeIndexStack;
		}
	}

	protected override void OnLoaded() {
		// the free index stack is an in-stream stack at all times (no memory overhead)
		_freeIndexStack = new StreamPagedList<long>(
			PrimitiveSerializer<long>.Instance,
			Stream,
			Endianness.LittleEndian,
			false,
			true
		).AsStack();
	}
}
