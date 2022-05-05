using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen {

	/// <summary>
	/// This class can be used to enqueue items from multiple threads for processing
	/// in sequenced fashion within a single thread. For example, incoming messages from
	/// many threads can be processed one-at-a-time in arrival sequence using this class without
	/// any blocking on the enqueue.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ProcessingQueue<T> {
		public const int DefaultMaxMessages = 1000000;
		public EventHandlerEx<T> Succeeded;
		public EventHandlerEx<T> Failed;
		public EventHandlerEx<T, Exception> Errors;

		private readonly SynchronizedQueue<T> _queue;
		private readonly Func<T, Result> _processor;
		private readonly int _maxMessages;
		private bool _isProcessing;
		private bool _enabled;

		public ProcessingQueue(Action<T> processor, int maxMessages = DefaultMaxMessages)
			: this(new SynchronizedQueue<T>(), processor, maxMessages) {
		}

		public ProcessingQueue(SynchronizedQueue<T> queue, Action<T> processor, int maxMessages = DefaultMaxMessages)
			: this(queue, (item) => { processor(item); return true; }, maxMessages) {
		}

		public ProcessingQueue(Func<T, bool> processor, int maxMessages = DefaultMaxMessages)
			: this(new SynchronizedQueue<T>(), (item) => { processor(item); return true; }, maxMessages) {
		}

		public ProcessingQueue(SynchronizedQueue<T> queue, Func<T, bool> processor, int maxMessages = DefaultMaxMessages) 
			: this(queue, (item) => processor(item) ? Result.Valid : Result.Error($"Failed to process item: {item?.ToString() ?? "<null>"}"), maxMessages) {
		}

		public ProcessingQueue(Func<T, Result> processor, int maxMessages = DefaultMaxMessages)
			: this(new SynchronizedQueue<T>(), processor, maxMessages) {
		}

		public ProcessingQueue(SynchronizedQueue<T> queue, Func<T, Result> processor, int maxMessages = DefaultMaxMessages) {
			_queue = queue;
			_processor = processor;
			_maxMessages = maxMessages;
			_enabled = false;
			_isProcessing = false;
		}

		public bool Enabled {
			get => _enabled;
			set {
				if (_enabled == value)
					return;

				using (_queue.EnterWriteScope()) {
					_enabled = value;
					StartProcessing();
				}
			}
		}

		public void Enqueue(T item) {
			using (_queue.EnterWriteScope()) {
				if (_queue.Count > _maxMessages)
					throw new InvalidOperationException("Queue full");
				_queue.Add(item);
				if (_enabled)
					StartProcessing();
			}
		}

		private void StartProcessing() {
			Guard.Ensure(_enabled == true);
			using (_queue.EnterWriteScope()) {
				if (_isProcessing)
					return;
				if (_queue.Count > 0) {
					_isProcessing = true;
					Tools.Threads.QueueAction(ProcessQueue);
				}
			}
		}

		private void ProcessQueue() {
			bool hasMore;
			do {
				// Gather items in list for processing outside the lock scope
				var toProcess = new Queue<T>();
				using (_queue.EnterWriteScope()) {
					if (!_queue.Any()) {
						_isProcessing = false;
						return;
					}
					foreach (var action in _queue)
						toProcess.Enqueue(action);
					_queue.Clear();
				}

				// Items are processed sequentially without locking queue
				foreach (var item in toProcess) {
					try {
						var result = _processor(item);
						if (result.Success)
							Succeeded?.InvokeAsync(item);
						else
							Failed?.InvokeAsync(item);
					} catch (Exception error) {
						Errors?.InvokeAsync(item, error);
					}
				}
				hasMore = _queue.Count > 0;
				if (!hasMore) {
					// if loop is going to exit, set _isProcessing false here to avoid
					// race-condition outside of loop
					using (_queue.EnterWriteScope()) {
						_isProcessing = false;
					}
				}
			} while (hasMore);

		}
	}
}
