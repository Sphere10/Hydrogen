using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {


	public abstract class SetTestsBase  {


		[Test]
		public void AddN([Values(0, 1, 2, 10, 111)] int N) {
			var rng = new Random(31337);
			using (CreateSet<string>(null, out var set)) {
				for (var i = 0; i < N; i++) 
					set.Add($"test{i}");

				Assert.That(set.Count, Is.EqualTo(N));
			}
		}


		[Test]
		public void ContainsN([Values(0, 1, 2, 10, 111)] int N) {
			var rng = new Random(31337);
			using (CreateSet<string>(null, out var set)) {
				for (var i = 0; i < N; i++)
					set.Add($"test{i}");

				for (var i = 0; i < N; i++)
					Assert.That(set.Contains($"test{i}"));
			}
		}



		protected abstract IDisposable CreateSet<TValue>(IEqualityComparer<TValue> comparer, out ISet<TValue> set);

	}

}
