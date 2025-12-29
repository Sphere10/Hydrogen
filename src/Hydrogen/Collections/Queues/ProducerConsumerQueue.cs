// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Hydrogen;

/// <summary>
/// Thread-safe producer/consumer queue with capacity tracking.
/// </summary>
/// <typeparam name="T">Item type.</typeparam>
/// <remarks>
/// See tests/Hydrogen.Tests/Collections/Queue/ProducerConsumerQueueTest.cs for usage patterns.
/// </remarks>
public class ProducerConsumerQueue<T> : SyncDisposable {
	private readonly object _threadLock;
	private readonly Func<T, long> _sizeEstimator;
	private readonly long _maxCapacity;
	private readonly long _maxPutSize;
	private long _currentLevel;
	private readonly Queue<Tuple<T, long>> _itemQueue;
	private bool _finishedProducing;
	private bool _finishedConsuming;
	private readonly ProducerConsumerLock _sharedAccess;

	/// <summary>
	/// Initializes the queue with unit item sizes.
	/// </summary>
	/// <param name="maxCapacity">The maximum capacity.</param>
	public ProducerConsumerQueue(long maxCapacity) : this(UnitSizeEstimator, maxCapacity) {
	}

	/// <summary>
	/// Initializes the queue with a size estimator and capacity.
	/// </summary>
	/// <param name="sizeEstimator">The size estimator for items.</param>
	/// <param name="maxCapacity">The maximum capacity.</param>
	public ProducerConsumerQueue(Func<T, long> sizeEstimator, long maxCapacity) : this(sizeEstimator, maxCapacity, (maxCapacity / 10).ClipTo(1, maxCapacity)) {
	}

	/// <summary>
	/// Initializes the queue with a size estimator, capacity, and max batch size.
	/// </summary>
	/// <param name="sizeEstimator">The size estimator for items.</param>
	/// <param name="maxCapacity">The maximum capacity.</param>
	/// <param name="maxPutSize">The maximum batch size accepted by <see cref="PutMany"/>.</param>
	public ProducerConsumerQueue(Func<T, long> sizeEstimator, long maxCapacity, long maxPutSize) {
		if (maxPutSize < 1) throw new ArgumentOutOfRangeException(nameof(maxPutSize), maxPutSize, "Must be greater than 1 and less than maxCapacity");
		if (maxPutSize > maxCapacity) throw new ArgumentOutOfRangeException(nameof(maxPutSize), maxPutSize, "Must be greater than 1 and less than maxCapacity");
		_sizeEstimator = sizeEstimator;
		_currentLevel = 0;
		_maxCapacity = maxCapacity;
		_maxPutSize = maxPutSize;
		_threadLock = new object();
		_itemQueue = new Queue<Tuple<T, long>>();
		_finishedConsuming = false;
		_finishedProducing = false;
		_sharedAccess = new ProducerConsumerLock(_threadLock);
	}

	/// <summary>
	/// Indicates whether producers have signaled completion.
	/// </summary>
	public virtual bool HasFinishedProducing {
		get {
			lock (_threadLock)
				return _finishedProducing;
		}
	}

	/// <summary>
	/// Indicates whether consumers have signaled completion.
	/// </summary>
	public virtual bool HasFinishedConsuming {
		get {
			lock (_threadLock)
				return _finishedConsuming;
		}
	}

	/// <summary>
	/// True when the queue can be consumed (items exist or production not finished).
	/// </summary>
	public virtual bool IsConsumable {
		get {
			lock (_threadLock)
				// note: If production has finished but items remain unconsumed, then it's consumable
				return !(_finishedProducing && _itemQueue.Count == 0);
		}
	}

	/// <summary>
	/// Gets the number of queued items.
	/// </summary>
	public virtual int Count {
		get {
			lock (_threadLock)
				return _itemQueue.Count;
		}
	}

	/// <summary>
	/// Gets the maximum total capacity for queued items.
	/// </summary>
	public virtual long MaxCapacity {
		get {
			lock (_threadLock)
				return _maxCapacity;
		}
	}

	/// <summary>
	/// Gets the maximum batch size accepted by <see cref="PutMany"/>.
	/// </summary>
	public virtual long MaxPutSize => _maxPutSize;

	/// <summary>
	/// Gets the current capacity used by queued items.
	/// </summary>
	public virtual long CurrentSize {
		get {
			lock (_threadLock)
				return _currentLevel;
		}
	}

	/// <summary>
	/// Gets the remaining capacity as a fraction of the maximum.
	/// </summary>
	public virtual double AvailableCapacity {
		get {
			lock (_threadLock)
				return 1 - _currentLevel / (double)_maxCapacity;
		}
	}

	/// <summary>
	/// Enqueues items in batches, blocking until capacity is available.
	/// </summary>
	/// <param name="items">The items to enqueue.</param>
	public virtual void PutMany(IEnumerable<T> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		items
			.Select(i => Tuple.Create(i, _sizeEstimator(i)))
			.PartitionBySize(i => (int)i.Item2, (int)MaxPutSize)
			.ForEach(PutManyInternal);
	}

	private void PutManyInternal(IEnumerable<Tuple<T, long>> items) {
		Guard.Against(_finishedProducing, "ProducerConsumerQueue is closed for production");
		var itemsArr = items as Tuple<T, long>[] ?? items.ToArray();
		if (itemsArr.Length == 0)
			return;
		var batchSize = itemsArr.Sum(i => i.Item2).ClipTo(0, long.MaxValue);
		Guard.Against(batchSize > _maxCapacity, "ProducerConsumerQueue has insufficient capacity for your items");
		using (_sharedAccess.ProducerBlockUntil(() => _maxCapacity - Volatile.Read(ref _currentLevel) >= batchSize || Volatile.Read(ref _finishedConsuming))) {
			Guard.Against(_maxCapacity - Volatile.Read(ref _currentLevel) < batchSize && Volatile.Read(ref _finishedConsuming), "ProducerConsumerQueue stopped consuming whilst attempting to produce data into queue.");
			foreach (var item in itemsArr)
				_itemQueue.Enqueue(item);
			Volatile.Write(ref _currentLevel, Volatile.Read(ref _currentLevel) + batchSize);
		}
	}

	/// <summary>
	/// Takes a single item, or default if none available and production is finished.
	/// </summary>
	public virtual T Take() {
		var results = TakeMany(1);
		if (results.Length == 0)
			return default;
		return results[0];
	}

	/// <summary>
	/// Takes up to a maximum number of items.
	/// </summary>
	/// <param name="maxItems">The maximum number of items to take.</param>
	public virtual T[] TakeMany(int maxItems) {
		if (maxItems < 0) throw new ArgumentOutOfRangeException(nameof(maxItems));
		if (_finishedConsuming) throw new InvalidOperationException("ProducerConsumerQueue is closed for consumption");

		if (maxItems == 0)
			return Array.Empty<T>();

		using (_sharedAccess.ConsumerBlockUntil(() => Interlocked.Read(ref _currentLevel) > 0 || Volatile.Read(ref _finishedProducing))) {
			var takeAmount = _itemQueue.Count.ClipTo(0, maxItems);
			var takeResults = new T[takeAmount];
			for (var i = 0; i < takeAmount; i++) {
				var item = _itemQueue.Dequeue();
				takeResults[i] = item.Item1;
				Volatile.Write(ref _currentLevel, Interlocked.Read(ref _currentLevel) - item.Item2);
			}
			return takeResults;
		}
	}

	/// <summary>
	/// Takes items up to a maximum total size.
	/// </summary>
	/// <param name="maxSize">The maximum total size to take.</param>
	public virtual T[] TakeBySize(int maxSize) {
		if (maxSize < 0) throw new ArgumentOutOfRangeException(nameof(maxSize));
		if (_finishedConsuming) throw new InvalidOperationException("ProducerConsumerQueue is closed for consumption");

		if (maxSize == 0)
			return Array.Empty<T>();

		using (_sharedAccess.ConsumerBlockUntil(() => Interlocked.Read(ref _currentLevel) > 0 || Volatile.Read(ref _finishedProducing))) {
			var takeList = new List<T>();
			var takenAmount = 0L;
			while (takenAmount < maxSize) {
				// If no more items, stop taking
				if (_itemQueue.Count == 0)
					break;

				// If next item won't fit, stop taking
				if (takenAmount + _itemQueue.Peek().Item2 > maxSize)
					break;

				// Take next item and update state keeping fields
				var nextItem = _itemQueue.Dequeue();
				takeList.Add(nextItem.Item1);
				takenAmount += nextItem.Item2;
				Volatile.Write(ref _currentLevel, Volatile.Read(ref _currentLevel) - nextItem.Item2);
			}
			return takeList.ToArray();
		}

	}

	/// <summary>
	/// Takes all currently queued items.
	/// </summary>
	public virtual T[] TakeAll() {
		Guard.Against(_finishedConsuming, "ProducerConsumerQueue is closed for consumption");

		using (_sharedAccess.ConsumerBlockUntil(() => Volatile.Read(ref _currentLevel) > 0 || Volatile.Read(ref _finishedProducing))) {
			var takeList = new List<T>();
			var takenAmount = 0L;
			while (_itemQueue.Count > 0) {
				// Take next item and update state keeping fields
				var nextItem = _itemQueue.Dequeue();
				takeList.Add(nextItem.Item1);
				takenAmount += nextItem.Item2;
				Volatile.Write(ref _currentLevel, Volatile.Read(ref _currentLevel) - nextItem.Item2);
			}
			return takeList.ToArray();
		}
	}

	/// <summary>
	/// Signals that producers are finished.
	/// </summary>
	public virtual void FinishedProducing() {
		//lock (_threadLock)
		_finishedProducing = true;
		_sharedAccess.Pulse();
	}

	/// <summary>
	/// Signals that consumers are finished.
	/// </summary>
	public virtual void FinishedConsuming() {
		//lock(_threadLock) 
		_finishedConsuming = true;

		_sharedAccess.Pulse();
	}

	private static long UnitSizeEstimator(T item) {
		return 1;
	}

	/// <summary>
	/// Releases managed resources and marks the queue as finished.
	/// </summary>
	protected override void FreeManagedResources() {
		lock (_threadLock) {
			_sharedAccess.Dispose();
			// NOTE: when debugging SemaphoreSlim bug, disable below 2 lines to prevent threads doing stuff after this is disposed by unit test
			_finishedConsuming = true;
			_finishedProducing = true;
			_itemQueue.Clear();
		}
	}


}
