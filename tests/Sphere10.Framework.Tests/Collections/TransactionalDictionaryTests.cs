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
	public class TransactionalTests : StreamPersistedTestsBase {

		[Test]
		public void AddNothing([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy) {
			var rng = new Random(31337);
			using (Create(out var dictionary)) {
				Assert.That(dictionary.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public void AddOne([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (Create(out var dictionary)) {
				dictionary.Add(key, new TestObject(rng));
				Assert.That(dictionary.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void ReuseRecord([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (Create(out var dictionary)) {
				dictionary.Add(key, new TestObject(rng));
				dictionary.Remove(key);
				dictionary.Add(key, new TestObject(rng));
				Assert.That(dictionary.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void ContainsKey([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (Create(out var dictionary)) {
				dictionary.Add(key, new TestObject(rng));
				Assert.That(dictionary.ContainsKey(key), Is.True);
			}
		}

		[Test]
		public void DoesNotContainKeyAfterRemove([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (Create(out var dictionary)) {
				dictionary.Add(key, new TestObject(rng));
				dictionary.Remove(key);
				Assert.That(dictionary.ContainsKey(key), Is.False);
			}
		}

		[Test]
		public void ContainsKeyValuePair([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (Create(out var dictionary)) {
				var value = new TestObject(rng);
				var kvp = KeyValuePair.Create(key, value);
				dictionary.Add(kvp);
				Assert.That(dictionary.Contains(kvp), Is.True);
			}
		}

		[Test]
		public void DoesNotContainKeyValuePair_SameKeyDifferentValue([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (Create(out var dictionary)) {
				var value = new TestObject(rng);
				var kvp = KeyValuePair.Create(key, value);
				dictionary.Add(kvp);
				value.A += "1";
				Assert.That(dictionary.Contains(kvp), Is.False);
			}
		}

		[Test]
		public void RemoveByKey([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (Create(out var dictionary)) {
				dictionary.Add(key, new TestObject(rng));
				dictionary.Remove(key);
				Assert.That(dictionary.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public void RemoveByKeyValuePair([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (Create(out var dictionary)) {
				dictionary.Add(key, new TestObject(rng));
				dictionary.Remove(key);
				Assert.That(dictionary.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public void IntegrationTests([Values(23)] int maxItems) => DoIntegrationTests(maxItems, 30);

#if DEBUG
		[Test]
		public void IntegrationTests_Heavy() => DoIntegrationTests(250, 250);
#endif

		private void DoIntegrationTests(int maxItems, int iterations) {
			var keyGens = 0;
			using (Create(out var dictionary)) {
				AssertEx.DictionaryIntegrationTest(
					dictionary,
					maxItems,
					(rng) => ($"{keyGens++}_{rng.NextString(0, 100)}", new TestObject(rng)),
					iterations: iterations,
					valueComparer: new TestObjectComparer()
				);
			}
		}


		protected IDisposable Create(out TransactionalDictionary<string, TestObject> dictionary)
			=> Create(new StringSerializer(Encoding.UTF8), new TestObjectSerializer(), EqualityComparer<string>.Default, new TestObjectComparer(), out dictionary);

		protected IDisposable Create<TKey, TValue>( IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer, out TransactionalDictionary<TKey, TValue> clustered) {
			var file = Tools.FileSystem.GenerateTempFilename();
			var dir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var disposable1 = Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file)));
			var disposable2 = Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir)));
			clustered = new TransactionalDictionary<TKey, TValue>(file, dir, keySerializer, valueSerializer, null, keyComparer, valueComparer);
			return new Disposables(disposable1, disposable2);
		}

	}

}
