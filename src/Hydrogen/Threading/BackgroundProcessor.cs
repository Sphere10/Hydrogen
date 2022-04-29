//-----------------------------------------------------------------------
// <copyright file="BackgroundProcessor.cs" company="Sphere 10 Software">
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
using System.IO;
using System.Linq;

namespace Hydrogen {


	public class BackgroundProcessor {

		private readonly SynchronizedCollection<Action> _queue;
		private readonly Action<Exception> _errorHandler;

		public BackgroundProcessor() : this(new List<Action>()) {	
		}

		public BackgroundProcessor(ICollection<Action> queue, Action<Exception> errorEventHandler = null) {
			_queue = new SynchronizedCollection<Action>( queue );
			_errorHandler = errorEventHandler ?? (_ => { });
		}


		public void QueueForExecution(Action action) {
			using(_queue.EnterWriteScope()) {
				_queue.Add(action);
				if (_queue.Count == 1)
					Tools.Threads.QueueAction(RunActions);
			}
		}

		private void RunActions() {
			bool hasMore;
			do {
				var actionQueue = new Queue<Action>();
				using (_queue.EnterWriteScope()) {
					if (!_queue.Any())
						return;
					foreach(var action in _queue)
						actionQueue.Enqueue(action);
					_queue.Clear();
				}

				foreach (var action in actionQueue) {
					try {
						action();
					} catch (Exception error) {
						_errorHandler(error);
					}
				}

				using (_queue.EnterReadScope()) {
					hasMore = _queue.Count > 0;
				}
			} while (hasMore);
		}
	}
}
