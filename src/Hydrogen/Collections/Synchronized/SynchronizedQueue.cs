//-----------------------------------------------------------------------
// <copyright file="SynchronizedQueue.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

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


