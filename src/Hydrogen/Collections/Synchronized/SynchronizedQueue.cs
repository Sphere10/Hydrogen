// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Hydrogen;

public class SynchronizedQueue<T> : SynchronizedCollection<T, IQueue<T>> {

	public SynchronizedQueue()
		: this(new Queue<T>()) {
	}

	public SynchronizedQueue(Queue<T> internalQueue) 
		: this(new QueueAdapter<T>(internalQueue)){
	}

	public SynchronizedQueue(IQueue<T> internalCollection) 
		: base(internalCollection) {
	}

	#region Methods

	public T Peek() {
		using (EnterReadScope())
			return  InternalCollection.Peek();
	}

	public void Enqueue(T value) {
		using (EnterWriteScope())
			InternalCollection.Enqueue(value);
	}

	public T Dequeue() {
		using (EnterWriteScope())
			return InternalCollection.Dequeue();
	}

	#endregion
};


