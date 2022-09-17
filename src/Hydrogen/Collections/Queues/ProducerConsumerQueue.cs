//-----------------------------------------------------------------------
// <copyright file="ProducerConsumerQueue.cs" company="Sphere 10 Software">
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Hydrogen {
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

		public ProducerConsumerQueue(long maxCapacity) : this(UnitSizeEstimator, maxCapacity) {
		}

		public ProducerConsumerQueue(Func<T, long> sizeEstimator, long maxCapacity) : this(sizeEstimator, maxCapacity, (maxCapacity / 10).ClipTo(1, maxCapacity)) {
		}

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

		public virtual bool HasFinishedProducing {
			get {
				lock (_threadLock)
					return _finishedProducing;
			}
		}

		public virtual bool HasFinishedConsuming {
			get {
				lock (_threadLock)
					return _finishedConsuming;
			}
		}

		/// <summary>
		/// If the queue has items ready for consumption
		/// </summary>
		public virtual bool IsConsumable {
			get {
				lock (_threadLock)
					// note: If production has finished but items remain unconsumed, then it's consumable
					return !(_finishedProducing && _itemQueue.Count == 0);
			}
		}

		public virtual int Count {
			get {
				lock (_threadLock)
					return _itemQueue.Count;
			}
		}

		public virtual long MaxCapacity {
			get {
				lock (_threadLock)
					return _maxCapacity;
			}
		}

		public virtual long MaxPutSize => _maxPutSize;

		public virtual long CurrentSize {
			get {
				lock (_threadLock)
					return _currentLevel;
			}
		}

		public virtual double AvailableCapacity {
			get {
				lock (_threadLock)
					return 1 - _currentLevel / (double)_maxCapacity;
			}
		}

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

		public virtual T Take() {
			var results = TakeMany(1);
			if (results.Length == 0)
				return default;
			return results[0];
		}

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

		public virtual void FinishedProducing() {
			//lock (_threadLock)
			_finishedProducing = true;
			_sharedAccess.Pulse();
		}

		public virtual void FinishedConsuming() {
			//lock(_threadLock) 
			_finishedConsuming = true;

			_sharedAccess.Pulse();
		}

		private static long UnitSizeEstimator(T item) {
			return 1;
		}

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
}
