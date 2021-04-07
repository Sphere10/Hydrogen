using System;
using Sphere10.Framework;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Usage {
	public class QueueTransactionDemo {
		public const string TempDir = "C:/temp/tests";

		private ILocalQueue _queue1;
		private ILocalQueue _queue2;

		public void PopFrom1PushTo2() {
			// multi-threaded
			// multi-instance
			if (Queue1.Count == 0)
				throw new InvalidOperationException("Nothing in Queue1");

			using (var txnScope = new FileTransactionScope(TempDir, true, ScopeContextPolicy.None)) {
				txnScope.EnlistFile(Queue1);
				txnScope.EnlistFile(Queue2);
				var poppedItem = Queue1[^1];
				Queue2.Add(poppedItem);
				Queue1.RemoveAt(^1);
				txnScope.Commit();   // if crashes here, will be fixed on app restart when calling ProcessDanglingTransactions(TempDir)
			}
		}

		public ILocalQueue Queue1 {
			get {
				if (_queue1 == null) {
					var configDTO = new QueueConfigDto(); // TODO


					_queue1 = new LocalQueue(configDTO);
				}
				return _queue1;
			}
		}

		public ILocalQueue Queue2 {
			get {
				if (_queue2 == null) {
					var configDTO = new QueueConfigDto(); // TODO
					_queue2 = new LocalQueue(configDTO);
				}
				return _queue2;
			}
		}
	}
}
