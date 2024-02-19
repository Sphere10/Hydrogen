// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.ObjectSpaces;

public abstract class ContainerObserverBase  {

	protected ContainerObserverBase(ObjectStream objectStream) {
		objectStream.PreItemOperation += OnPreItemOperation;
		objectStream.PostItemOperation += OnPostItemOperation;
		objectStream.Clearing += OnContainerClearing;
		objectStream.Cleared += OnContainerCleared;
	}

	protected virtual void OnAdding(object item, long index) {
	}

	protected virtual void OnAdded(object item, long index) {
	}

	protected virtual void OnInserting(object item, long index) {
	}

	protected virtual void OnInserted(object item, long index) {
	}

	protected virtual void OnUpdating(object item, long index) {
	}

	protected virtual void OnUpdated(object item, long index) {
	}

	protected virtual void OnRemoving(long index) {
	}

	protected virtual void OnRemoved(long index) {
	}

	protected virtual void OnReaping(long index) {
	}

	protected virtual void OnReaped(long index) {
	}

	protected virtual void OnPreItemOperation(long index, object item, ObjectStreamOperationType operationType) {
		switch (operationType) {
			case ObjectStreamOperationType.Read:
				break;
			case ObjectStreamOperationType.Add:
				OnAdding(item, index);
				break;
			case ObjectStreamOperationType.Insert:
				OnInserting(item, index);
				break;
			case ObjectStreamOperationType.Update:
				OnUpdating(item, index);
				break;
			case ObjectStreamOperationType.Remove:
				OnRemoving(index);
				break;
			case ObjectStreamOperationType.Reap:
				OnReaping(index);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null);
		}
	}

	protected virtual void OnPostItemOperation(long index, object item, ObjectStreamOperationType operationType) {
		switch (operationType) {
			case ObjectStreamOperationType.Read:
				break;
			case ObjectStreamOperationType.Add:
				OnAdded(item, index);
				break;
			case ObjectStreamOperationType.Insert:
				OnInserted(item, index);
				break;
			case ObjectStreamOperationType.Update:
				OnUpdated(item, index);
				break;
			case ObjectStreamOperationType.Remove:
				OnRemoved(index);
				break;
			case ObjectStreamOperationType.Reap:
				OnReaped(index);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null);
		}
	}

	protected virtual void OnContainerClearing() {
	}

	protected virtual void OnContainerCleared() {
	}

}
