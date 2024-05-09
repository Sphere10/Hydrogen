// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class ContextScopeTest {

	[Test]
	public void TestNested_None_None() {
		ClassicAssert.IsFalse(ExceptionOccured(ContextScopePolicy.None, ContextScopePolicy.None));
	}

	[Test]
	public void TestNested_None_MustBeNested() {
		ClassicAssert.IsFalse(ExceptionOccured(ContextScopePolicy.None, ContextScopePolicy.MustBeNested));
	}

	[Test]
	public void TestNested_None_MustBeRoot() {
		ClassicAssert.IsTrue(ExceptionOccured(ContextScopePolicy.None, ContextScopePolicy.MustBeRoot));
	}

	[Test]
	public void TestNested_MustBeNested_None() {
		ClassicAssert.IsTrue(ExceptionOccured(ContextScopePolicy.MustBeNested, ContextScopePolicy.None));
	}

	[Test]
	public void TestNested_MustBeNested_MustBeNested() {
		ClassicAssert.IsTrue(ExceptionOccured(ContextScopePolicy.MustBeNested, ContextScopePolicy.MustBeNested));
	}

	[Test]
	public void TestNested_MustBeNested_MustBeRoot() {
		ClassicAssert.IsTrue(ExceptionOccured(ContextScopePolicy.MustBeNested, ContextScopePolicy.MustBeRoot));
	}

	[Test]
	public void TestNested_MustBeRoot_None() {
		ClassicAssert.IsFalse(ExceptionOccured(ContextScopePolicy.MustBeRoot, ContextScopePolicy.None));
	}

	[Test]
	public void TestNested_MustBeRoot_MustBeNested() {
		ClassicAssert.IsFalse(ExceptionOccured(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested));
	}

	[Test]
	public void TestNested_MustBeRoot_MustBeRoot() {
		ClassicAssert.IsTrue(ExceptionOccured(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeRoot));
	}

	[Test]
	public void MultiThreaded_0() {
		ClassicAssert.IsTrue(Enumerable.Range(1, 10).All(x => !ExceptionOccured(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100))));
	}

	[Test]
	public void MultiThreaded_1() {
		var task1 = new Task<bool>(() => Enumerable.Range(1, 10).All(x => !ExceptionOccured(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100))));
		var task2 = new Task<bool>(() => Enumerable.Range(1, 10).All(x => !ExceptionOccured(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100))));
		task1.Start();
		task2.Start();
		Task.WaitAll(task1, task2);
		ClassicAssert.IsTrue(task1.Result);
		ClassicAssert.IsTrue(task2.Result);
	}

	[Test]
	public void MultiThreaded_2() {
		var task1 = new Task<bool>(() => !ExceptionOccured(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, 1000, 0, 0));
		var task2 = new Task<bool>(() => !ExceptionOccured(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, 0, 0, 0));
		task1.Start();
		task2.Start();
		Task.WaitAll(task1, task2);
		ClassicAssert.IsTrue(task1.Result);
		ClassicAssert.IsTrue(task2.Result);
	}

	[Test]
	public void TestNested_None_None_Async() {
		ClassicAssert.IsFalse(ExceptionOccuredAsync(ContextScopePolicy.None, ContextScopePolicy.None));
	}

	[Test]
	public async Task TestNested_None_None_Async2() {
		ClassicAssert.IsFalse(await ExceptionOccuredAsync2(ContextScopePolicy.None, ContextScopePolicy.None));
	}

	[Test]
	public void TestNested_None_MustBeNested_Async() {
		ClassicAssert.IsFalse(ExceptionOccuredAsync(ContextScopePolicy.None, ContextScopePolicy.MustBeNested));
	}

	[Test]
	public async Task TestNested_None_MustBeNested_Async2() {
		ClassicAssert.IsFalse(await ExceptionOccuredAsync2(ContextScopePolicy.None, ContextScopePolicy.MustBeNested));
	}

	[Test]
	public void TestNested_None_MustBeRoot_Async() {
		ClassicAssert.IsTrue(ExceptionOccuredAsync(ContextScopePolicy.None, ContextScopePolicy.MustBeRoot));
	}

	[Test]
	public async Task TestNested_None_MustBeRoot_Async2() {
		ClassicAssert.IsTrue(await ExceptionOccuredAsync2(ContextScopePolicy.None, ContextScopePolicy.MustBeRoot));
	}

	[Test]
	public void TestNested_MustBeNested_None_Async() {
		ClassicAssert.IsTrue(ExceptionOccuredAsync(ContextScopePolicy.MustBeNested, ContextScopePolicy.None));
	}

	[Test]
	public async Task TestNested_MustBeNested_None_Async2() {
		ClassicAssert.IsTrue(await ExceptionOccuredAsync2(ContextScopePolicy.MustBeNested, ContextScopePolicy.None));
	}

	[Test]
	public void TestNested_MustBeNested_MustBeNested_Async() {
		ClassicAssert.IsTrue(ExceptionOccuredAsync(ContextScopePolicy.MustBeNested, ContextScopePolicy.MustBeNested));
	}

	[Test]
	public async Task TestNested_MustBeNested_MustBeNested_Async2() {
		ClassicAssert.IsTrue(await ExceptionOccuredAsync2(ContextScopePolicy.MustBeNested, ContextScopePolicy.MustBeNested));
	}

	[Test]
	public void TestNested_MustBeNested_MustBeRoot_Async() {
		ClassicAssert.IsTrue(ExceptionOccuredAsync(ContextScopePolicy.MustBeNested, ContextScopePolicy.MustBeRoot));
	}

	[Test]
	public async Task TestNested_MustBeNested_MustBeRoot_Async2() {
		ClassicAssert.IsTrue(await ExceptionOccuredAsync2(ContextScopePolicy.MustBeNested, ContextScopePolicy.MustBeRoot));
	}

	[Test]
	public void TestNested_MustBeRoot_None_Async() {
		ClassicAssert.IsFalse(ExceptionOccuredAsync(ContextScopePolicy.MustBeRoot, ContextScopePolicy.None));
	}

	[Test]
	public async Task TestNested_MustBeRoot_None_Async2() {
		ClassicAssert.IsFalse(await ExceptionOccuredAsync2(ContextScopePolicy.MustBeRoot, ContextScopePolicy.None));
	}

	[Test]
	public void TestNested_MustBeRoot_MustBeNested_Async() {
		ClassicAssert.IsFalse(ExceptionOccuredAsync(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested));
	}

	[Test]
	public async Task TestNested_MustBeRoot_MustBeNested_Async2() {
		ClassicAssert.IsFalse(await ExceptionOccuredAsync2(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested));
	}


	[Test]
	public void TestNested_MustBeRoot_MustBeRoot_Async() {
		ClassicAssert.IsTrue(ExceptionOccuredAsync(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeRoot));
	}


	[Test]
	public async Task TestNested_MustBeRoot_MustBeRoot_Async2() {
		ClassicAssert.IsTrue(await ExceptionOccuredAsync2(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeRoot));
	}

	[Test]
	public void MultiThreaded_0_Async() {
		ClassicAssert.IsTrue(Enumerable.Range(1, 10).All(x => !ExceptionOccuredAsync(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100))));
	}

	//[Test]
	//public async Task MultiThreaded_0_Async2() {
	//    ClassicAssert.IsTrue(AsyncEnumerable.Range(1, 10).AllAsync(async x => await !ExceptionOccuredAsync2(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100))

	//    ));
	//}

	[Test]
	public void MultiThreaded_1_Async() {
		var task1 = new Task<bool>(() =>
			Enumerable.Range(1, 10).All(x => !ExceptionOccuredAsync(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100))));
		var task2 = new Task<bool>(() =>
			Enumerable.Range(1, 10).All(x => !ExceptionOccuredAsync(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100))));
		task1.Start();
		task2.Start();
		Task.WaitAll(task1, task2);
		ClassicAssert.IsTrue(task1.Result);
		ClassicAssert.IsTrue(task2.Result);
	}

	[Test]
	public void MultiThreaded_2_Async() {
		var task1 = new Task<bool>(() => !ExceptionOccuredAsync(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, 1000, 0, 0));
		var task2 = new Task<bool>(() => !ExceptionOccuredAsync(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, 0, 0, 0));
		task1.Start();
		task2.Start();
		Task.WaitAll(task1, task2);
		ClassicAssert.IsTrue(task1.Result);
		ClassicAssert.IsTrue(task2.Result);
	}


	private bool ExceptionOccured(ContextScopePolicy rootPolicy, ContextScopePolicy childPolicy, int delay1 = 0, int delay2 = 0, int delay3 = 0) {
		try {
			using (new ContextScopeDemo(rootPolicy)) {
				System.Threading.Thread.Sleep(delay1);
				using (new ContextScopeDemo(childPolicy)) {
					System.Threading.Thread.Sleep(delay2);
					using (new ContextScopeDemo(childPolicy)) {
						System.Threading.Thread.Sleep(delay3);
					}
				}
			}
		} catch (Exception error) {
			return true;
		}
		return false;
	}

	private bool ExceptionOccuredAsync(ContextScopePolicy rootPolicy, ContextScopePolicy childPolicy, int delay1 = 0, int delay2 = 0, int delay3 = 0) {
		try {
			AsyncTest(Tuple.Create(rootPolicy, delay1), Tuple.Create(childPolicy, delay2), Tuple.Create(ContextScopePolicy.None, delay3)).Wait();
		} catch (Exception error) {
			return true;
		}
		return false;
	}

	private async Task<bool> ExceptionOccuredAsync2(ContextScopePolicy rootPolicy, ContextScopePolicy childPolicy, int delay1 = 0, int delay2 = 0, int delay3 = 0) {
		try {
			await AsyncTest2(Tuple.Create(rootPolicy, delay1), Tuple.Create(childPolicy, delay2), Tuple.Create(ContextScopePolicy.None, delay3));
		} catch (Exception error) {
			return true;
		}
		return false;
	}

	private async Task AsyncTest(params Tuple<ContextScopePolicy, int>[] policies) {
		if (policies.Any()) {
			var head = policies.First();
			using (new ContextScopeDemo(head.Item1)) {
				System.Threading.Thread.Sleep(head.Item2);
				var tail = policies.Skip(1).ToArray();
				await AsyncTest(tail);
			}
		}
	}

	private async Task AsyncTest2(params Tuple<ContextScopePolicy, int>[] policies) {
		if (policies.Any()) {
			var head = policies.First();
			await using (new ContextScopeDemo(head.Item1)) {
				await Task.Delay(head.Item2);
				var tail = policies.Skip(1).ToArray();
				await AsyncTest2(tail);
			}
		}
	}

	public class ContextScopeDemo : SyncContextScope {

		public ContextScopeDemo(ContextScopePolicy policy)
			: base(policy, "ContextScopeDemo") {
		}

		protected override void OnScopeEndInternal() {
		}

		protected override void OnContextEnd() {
		}
	}
}
