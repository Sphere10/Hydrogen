//// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
//// Author: Herman Schoenfeld
////
//// Distributed under the MIT software license, see the accompanying file
//// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
////
//// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using NUnit.Framework;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Hydrogen.Tests {

//	[TestFixture]
//	[Parallelizable(ParallelScope.Children)]
//	public class ProducerConsumerLockTests {

//		[Test]
//		public void Single() {
//			var result = 0;
//			using var @lock = new ProducerConsumerLock();
//			using (@lock.BlockUntil(ProducerConsumerType.Consumer, () => true)) {
//				result = 1;
//			}
//			ClassicAssert.AreEqual(1, result);
//		}

//		[Test]
//		public async Task WaitForeverBug_1() {
//			var result = 0;
//			using var conditionalLock = new ProducerConsumerLock();
//			var greenLight = false;
//			Task.Factory.StartNew(() => {
//				Thread.Sleep(250);
//				conditionalLock.Dispose();  // the disposing will unblock the waiting forever
//			});

//			var tasks = Tools.Collection.Generate(
//				() =>  Task.Factory.StartNew( () =>{
//						using (conditionalLock.BlockUntil(ProducerConsumerType.Consumer, () => false)) {
//							Interlocked.Increment(ref result);
//						}
//				})
//			).Take(2).ToArray();

//			await Task.WhenAll(tasks).IgnoringExceptions();

//			// All tasks finished, otherwise would run forever
//		}


//		[Test]
//		public async Task DeadlockBug() {
//			var @lock = new ProducerConsumerLock();
//			var read = Task.Factory.StartNew(async () => {
//				using (@lock.BlockUntil(ProducerConsumerType.Consumer, () => false)) ;
//			}).Unwrap();
//			var write =Task.Factory.StartNew(async () => {
//				using (@lock.BlockUntil(ProducerConsumerType.Producer, () => false)) ;
//			}).Unwrap();

//			await Task.WhenAny(read, write).WithTimeout(TimeSpan.FromMilliseconds(100));

//			Assert.That(read.Exception != null || write.Exception != null, Is.True);
//			var exception = read.Exception ?? write.Exception;
//			Assert.That(exception.InnerExceptions.Count, Is.EqualTo(1));
//			Assert.That(exception.InnerExceptions[0], Is.TypeOf<InvalidOperationException>());
//		}

//		[Test]
//		public void TwoThreads() {
//			int state = 0;
//			var @lock = new ProducerConsumerLock();
//			var result = new List<int>();
//			var start = false;

//			void ConcurrentAction(int x, Func<bool> pred) {
//				using (@lock.BlockUntil(ProducerConsumerType.Consumer, pred)) {
//					result.Add(x);
//				}
//			}

//			var thread1 = new Thread(() => ConcurrentAction(1, () => Volatile.Read(ref start)));
//			var thread2 = new Thread(() => ConcurrentAction(2, () => true));

//			thread1.Start();
//			Thread.Sleep(1);
//			thread2.Start();
//			Thread.Sleep(100);

//			Volatile.Write(ref start, true);
//			@lock.Pulse();
//			Thread.Sleep(200);
//			ClassicAssert.AreEqual(new int[] { 1, 2 }, result.ToArray());
//		}


//		[Test]
//		public void PulseInMiddle() {
//			var @lock = new ProducerConsumerLock();
//			var result = new List<int>();
//			var start = false;

//			void ConcurrentAction(int x, Func<bool> pred) {
//				using (@lock.BlockUntil(ProducerConsumerType.Consumer, pred)) {
//					Thread.Sleep(x * 10);
//					result.Add(x);
//					if (x == 2)
//						Volatile.Write(ref start, false);
//				}
//			}

//			var thread1 = new Thread(() => ConcurrentAction(1, () => Volatile.Read(ref start)));
//			var thread2 = new Thread(() => ConcurrentAction(2, () => true));
//			var thread3 = new Thread(() => ConcurrentAction(3, () => Volatile.Read(ref start)));
//			var thread4 = new Thread(() => ConcurrentAction(4, () => true));

//			thread1.Start();
//			Thread.Sleep(1);
//			thread2.Start();
//			Thread.Sleep(1);
//			thread3.Start();
//			Thread.Sleep(1);
//			thread4.Start();
//			Thread.Sleep(1);

//			// Everything should be blocked
//			Thread.Sleep(100);
//			ClassicAssert.AreEqual(Enumerable.Empty<int>(), result);

//			// Allow first two threads in
//			Volatile.Write(ref start, true);
//			@lock.Pulse();
//			Thread.Sleep(100);
//			ClassicAssert.AreEqual(new[] { 1, 2 }, result.ToArray());

//			// Allow last two threads in
//			Volatile.Write(ref start, true);
//			@lock.Pulse();
//			Thread.Sleep(100);
//			ClassicAssert.AreEqual(new[] { 1, 2, 3, 4 }, result.ToArray());

//		}


//		[Test]
//		public void MultipleThreads() {
//			int state = 0;
//			var @lock = new ProducerConsumerLock();
//			var result = new SynchronizedList<int>();
//			var start = false;

//			void ConcurrentAction(int x, Func<bool> pred) {
//				using (@lock.BlockUntil(ProducerConsumerType.Consumer, pred)) {
//					Thread.Sleep(x);
//					result.Add(x);
//				}
//			}

//			var threads = new Thread[100];
//			for (var i = 0; i < 100; i++) {
//				if (i == 0) {
//					var i1 = i;
//					threads[i] = new Thread(() => ConcurrentAction(i1 + 1, () => Volatile.Read(ref start)));
//				} else {
//					var i1 = i;
//					threads[i] = new Thread(() => ConcurrentAction(i1 + 1, () => true));
//				}
//			}

//			foreach (var thread in threads) {
//				thread.Start();
//				Thread.Sleep(1);
//			}

//			Thread.Sleep(100);
//			ClassicAssert.AreEqual(Enumerable.Empty<int>(), result);


//			Volatile.Write(ref start, true);
//			@lock.Pulse();
//			while (result.Count < 100) {
//				Thread.Sleep(100);
//			}
//			Tools.NUnit.Print(result);
//			ClassicAssert.AreEqual(Enumerable.Range(1, 100), result);
//		}
//	}

//}


