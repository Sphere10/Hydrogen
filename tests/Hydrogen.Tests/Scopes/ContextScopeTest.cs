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

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class ContextScopeTest {


	[Test]
	public void TestNested_None_None() {
		Assert.False(ExceptionOccured(ContextScopePolicy.None, ContextScopePolicy.None));
	}

	[Test]
	public void TestNested_None_MustBeNested() {
		Assert.False(ExceptionOccured(ContextScopePolicy.None, ContextScopePolicy.MustBeNested));
	}

	[Test]
	public void TestNested_None_MustBeRoot() {
		Assert.True(ExceptionOccured(ContextScopePolicy.None, ContextScopePolicy.MustBeRoot));
	}

	[Test]
	public void TestNested_MustBeNested_None() {
		Assert.True(ExceptionOccured(ContextScopePolicy.MustBeNested, ContextScopePolicy.None));
	}

	[Test]
	public void TestNested_MustBeNested_MustBeNested() {
		Assert.True(ExceptionOccured(ContextScopePolicy.MustBeNested, ContextScopePolicy.MustBeNested));
	}

	[Test]
	public void TestNested_MustBeNested_MustBeRoot() {
		Assert.True(ExceptionOccured(ContextScopePolicy.MustBeNested, ContextScopePolicy.MustBeRoot));
	}

	[Test]
	public void TestNested_MustBeRoot_None() {
		Assert.False(ExceptionOccured(ContextScopePolicy.MustBeRoot, ContextScopePolicy.None));
	}

	[Test]
	public void TestNested_MustBeRoot_MustBeNested() {
		Assert.False(ExceptionOccured(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested));
	}


	[Test]
	public void TestNested_MustBeRoot_MustBeRoot() {
		Assert.True(ExceptionOccured(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeRoot));
	}

	[Test]
	public void MultiThreaded_0() {
		Assert.IsTrue(Enumerable.Range(1, 10).All(x => !ExceptionOccured(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100))));
	}


	[Test]
	public void MultiThreaded_1() {
		var task1 = new Task<bool>(() => Enumerable.Range(1, 10).All(x => !ExceptionOccured(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100))));
		var task2 = new Task<bool>(() => Enumerable.Range(1, 10).All(x => !ExceptionOccured(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100))));
		task1.Start();
		task2.Start();
		Task.WaitAll(task1, task2);
		Assert.IsTrue(task1.Result);
		Assert.IsTrue(task2.Result);
	}

	[Test]
	public void MultiThreaded_2() {
		var task1 = new Task<bool>(() => !ExceptionOccured(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, 1000, 0, 0));
		var task2 = new Task<bool>(() => !ExceptionOccured(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, 0, 0, 0));
		task1.Start();
		task2.Start();
		Task.WaitAll(task1, task2);
		Assert.IsTrue(task1.Result);
		Assert.IsTrue(task2.Result);
	}


	[Test]
	public void TestNested_None_None_Async() {
		Assert.False(ExceptionOccuredAsync(ContextScopePolicy.None, ContextScopePolicy.None));
	}

	[Test]
	public async Task TestNested_None_None_Async2() {
		Assert.False(await ExceptionOccuredAsync2(ContextScopePolicy.None, ContextScopePolicy.None));
	}

	[Test]
	public void TestNested_None_MustBeNested_Async() {
		Assert.False(ExceptionOccuredAsync(ContextScopePolicy.None, ContextScopePolicy.MustBeNested));
	}

	[Test]
	public async Task TestNested_None_MustBeNested_Async2() {
		Assert.False(await ExceptionOccuredAsync2(ContextScopePolicy.None, ContextScopePolicy.MustBeNested));
	}

	[Test]
	public void TestNested_None_MustBeRoot_Async() {
		Assert.True(ExceptionOccuredAsync(ContextScopePolicy.None, ContextScopePolicy.MustBeRoot));
	}

	[Test]
	public async Task TestNested_None_MustBeRoot_Async2() {
		Assert.True(await ExceptionOccuredAsync2(ContextScopePolicy.None, ContextScopePolicy.MustBeRoot));
	}

	[Test]
	public void TestNested_MustBeNested_None_Async() {
		Assert.True(ExceptionOccuredAsync(ContextScopePolicy.MustBeNested, ContextScopePolicy.None));
	}

	[Test]
	public async Task TestNested_MustBeNested_None_Async2() {
		Assert.True(await ExceptionOccuredAsync2(ContextScopePolicy.MustBeNested, ContextScopePolicy.None));
	}

	[Test]
	public void TestNested_MustBeNested_MustBeNested_Async() {
		Assert.True(ExceptionOccuredAsync(ContextScopePolicy.MustBeNested, ContextScopePolicy.MustBeNested));
	}

	[Test]
	public async Task TestNested_MustBeNested_MustBeNested_Async2() {
		Assert.True(await ExceptionOccuredAsync2(ContextScopePolicy.MustBeNested, ContextScopePolicy.MustBeNested));
	}

	[Test]
	public void TestNested_MustBeNested_MustBeRoot_Async() {
		Assert.True(ExceptionOccuredAsync(ContextScopePolicy.MustBeNested, ContextScopePolicy.MustBeRoot));
	}

	[Test]
	public async Task TestNested_MustBeNested_MustBeRoot_Async2() {
		Assert.True(await ExceptionOccuredAsync2(ContextScopePolicy.MustBeNested, ContextScopePolicy.MustBeRoot));
	}

	[Test]
	public void TestNested_MustBeRoot_None_Async() {
		Assert.False(ExceptionOccuredAsync(ContextScopePolicy.MustBeRoot, ContextScopePolicy.None));
	}

	[Test]
	public async Task TestNested_MustBeRoot_None_Async2() {
		Assert.False(await ExceptionOccuredAsync2(ContextScopePolicy.MustBeRoot, ContextScopePolicy.None));
	}

	[Test]
	public void TestNested_MustBeRoot_MustBeNested_Async() {
		Assert.False(ExceptionOccuredAsync(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested));
	}

	[Test]
	public async Task TestNested_MustBeRoot_MustBeNested_Async2() {
		Assert.False(await ExceptionOccuredAsync2(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested));
	}


	[Test]
	public void TestNested_MustBeRoot_MustBeRoot_Async() {
		Assert.True(ExceptionOccuredAsync(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeRoot));
	}


	[Test]
	public async Task TestNested_MustBeRoot_MustBeRoot_Async2() {
		Assert.True(await ExceptionOccuredAsync2(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeRoot));
	}

	[Test]
	public void MultiThreaded_0_Async() {
		Assert.IsTrue(Enumerable.Range(1, 10).All(x => !ExceptionOccuredAsync(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100))));
	}

	//[Test]
	//public async Task MultiThreaded_0_Async2() {
	//    Assert.IsTrue(AsyncEnumerable.Range(1, 10).AllAsync(async x => await !ExceptionOccuredAsync2(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100))

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
		Assert.IsTrue(task1.Result);
		Assert.IsTrue(task2.Result);
	}

	[Test]
	public void MultiThreaded_2_Async() {
		var task1 = new Task<bool>(() => !ExceptionOccuredAsync(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, 1000, 0, 0));
		var task2 = new Task<bool>(() => !ExceptionOccuredAsync(ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested, 0, 0, 0));
		task1.Start();
		task2.Start();
		Task.WaitAll(task1, task2);
		Assert.IsTrue(task1.Result);
		Assert.IsTrue(task2.Result);
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
