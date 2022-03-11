using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using Sphere10.Framework.NUnit;
using Tools;

namespace Sphere10.Framework.Tests {


	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class TransactionalDictionaryTests : StreamPersistedTestsBase {

		[Test]
		public void IntegrationTests([Values(23)] int maxItems) => DoIntegrationTests(maxItems, 30);

#if DEBUG
		[Test]
		public void IntegrationTests_Heavy() => DoIntegrationTests(250, 250);
#endif

		private void DoIntegrationTests(int maxItems, int iterations) {
			var keyGens = 0;
			using (CreateDictionary(out var clusteredDictionary)) {
				AssertEx.DictionaryIntegrationTest(
					clusteredDictionary,
					maxItems,
					(rng) => ($"{keyGens++}_{rng.NextString(0, 100)}", new TestObject(rng)),
					iterations: iterations,
					valueComparer: new TestObjectComparer()
				);
			}
		}


		protected IDisposable CreateDictionary(out TransactionalDictionary<string, TestObject> clusteredDictionary)
			=> CreateDictionary(new StringSerializer(Encoding.UTF8), new TestObjectSerializer(), EqualityComparer<string>.Default, out clusteredDictionary);

		protected IDisposable CreateDictionary<TKey, TValue>( IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IEqualityComparer<TKey> keyComparer, out TransactionalDictionary<TKey, TValue> clusteredDictionary) {
			var file = Tools.FileSystem.GenerateTempFilename();
			var dir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var disposable1 = Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file)));
			var disposable2 = Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir)));
			clusteredDictionary = new TransactionalDictionary<TKey, TValue>(file, dir, keySerializer, valueSerializer, null, keyComparer);
			return new Disposables(disposable1, disposable2);
		}

	}

}
