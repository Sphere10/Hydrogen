// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using Hydrogen.NUnit;
using Tools;

namespace Hydrogen.Tests {


	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class TransactionalDictionaryTests : StreamPersistedCollectionTestsBase {

		[Test]
		public void AddNothing([ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy) {
			var rng = new Random(31337);
			using (Create(policy, out var dictionary)) {
				Assert.That(dictionary.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public void AddOne([ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (Create(policy, out var dictionary)) {
				dictionary.Add(key, new TestObject(rng));
				Assert.That(dictionary.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void ReuseRecord([ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (Create(policy, out var dictionary)) {
				dictionary.Add(key, new TestObject(rng));
				dictionary.Remove(key);
				dictionary.Add(key, new TestObject(rng));
				Assert.That(dictionary.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void ContainsKey([ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (Create(policy, out var dictionary)) {
				dictionary.Add(key, new TestObject(rng));
				Assert.That(dictionary.ContainsKey(key), Is.True);
			}
		}

		[Test]
		public void DoesNotContainKeyAfterRemove([ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (Create(policy, out var dictionary)) {
				dictionary.Add(key, new TestObject(rng));
				dictionary.Remove(key);
				Assert.That(dictionary.ContainsKey(key), Is.False);
			}
		}

		[Test]
		public void ContainsKeyValuePair([ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (Create(policy, out var dictionary)) {
				var value = new TestObject(rng);
				var kvp = KeyValuePair.Create(key, value);
				dictionary.Add(kvp);
				Assert.That(dictionary.Contains(kvp), Is.True);
			}
		}

		[Test]
		public void DoesNotContainKeyValuePair_SameKeyDifferentValue([ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (Create(policy, out var dictionary)) {
				var value = new TestObject(rng);
				var kvp = KeyValuePair.Create(key, value);
				dictionary.Add(kvp);
				value.A += "1";
				Assert.That(dictionary.Contains(kvp), Is.False);
			}
		}

		[Test]
		public void RemoveByKey([ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (Create(policy, out var dictionary)) {
				dictionary.Add(key, new TestObject(rng));
				dictionary.Remove(key);
				Assert.That(dictionary.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public void RemoveByKeyValuePair([ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (Create(policy, out var dictionary)) {
				dictionary.Add(key, new TestObject(rng));
				dictionary.Remove(key);
				Assert.That(dictionary.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public void IntegrationTests([ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values(23)] int maxItems) => DoIntegrationTests(policy, maxItems, 30);

#if DEBUG
		[Test]
		public void IntegrationTests_Heavy([ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy) => DoIntegrationTests(policy, 250, 250);
#endif

		private void DoIntegrationTests([ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, int maxItems, int iterations) {
			var keyGens = 0;
			using (Create(policy, out var dictionary)) {
				AssertEx.DictionaryIntegrationTest(
					dictionary,
					maxItems,
					(rng) => ($"{keyGens++}_{rng.NextString(0, 100)}", new TestObject(rng)),
					iterations: iterations,
					valueComparer: new TestObjectComparer()
				);
			}
		}


		protected IDisposable Create(ClusteredStoragePolicy policy, out TransactionalDictionary<string, TestObject> dictionary)
			=> Create(new StringSerializer(Encoding.UTF8), new TestObjectSerializer(), EqualityComparer<string>.Default, new TestObjectComparer(), policy, out dictionary);

		protected IDisposable Create<TKey, TValue>( IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer, ClusteredStoragePolicy policy, out TransactionalDictionary<TKey, TValue> clustered) {
			var file = Tools.FileSystem.GenerateTempFilename();
			var dir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var disposable1 = Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file)));
			var disposable2 = Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir)));
			clustered = new TransactionalDictionary<TKey, TValue>(file, dir, keySerializer, valueSerializer, null, keyComparer, valueComparer, policy: policy | ClusteredStoragePolicy.TrackChecksums);
			return new Disposables(disposable1, disposable2);
		}

	}

}
