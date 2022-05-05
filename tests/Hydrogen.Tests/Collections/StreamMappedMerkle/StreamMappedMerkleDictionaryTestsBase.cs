using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using NUnit.Framework;
using Hydrogen.NUnit;
using Tools;

namespace Hydrogen.Tests {
	
	public abstract class StreamMappedMerkleDictionaryTestsBase : StreamPersistedCollectionTestsBase {
		protected abstract IDisposable CreateDictionary(CHF chf, out StreamMappedMerkleDictionary<string, TestObject> merkleDictionary);

		[Test]
		public void AddNothing([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
			var rng = new Random(31337);
			using (CreateDictionary(chf, out var clusteredDictionary)) {
				Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public void AddOne([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(chf, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				Assert.That(clusteredDictionary.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void ReuseRecord([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(chf, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				clusteredDictionary.Remove(key);
				clusteredDictionary.Add(key, new TestObject(rng));
				Assert.That(clusteredDictionary.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void ContainsKey([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(chf, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				Assert.That(clusteredDictionary.ContainsKey(key), Is.True);
			}
		}

		[Test]
		public void DoesNotContainKeyAfterRemove([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(chf, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				clusteredDictionary.Remove(key);
				Assert.That(clusteredDictionary.ContainsKey(key), Is.False);
			}
		}

		[Test]
		public void ContainsKeyValuePair([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(chf, out var clusteredDictionary)) {
				var value = new TestObject(rng);
				var kvp = KeyValuePair.Create(key, value);
				clusteredDictionary.Add(kvp);
				Assert.That(clusteredDictionary.Contains(kvp), Is.True);
			}
		}

		[Test]
		public void DoesNotContainKeyValuePair_SameKeyDifferentValue([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(chf, out var clusteredDictionary)) {
				var value = new TestObject(rng);
				var kvp = KeyValuePair.Create(key, value);
				clusteredDictionary.Add(kvp);
				value.A += "1";
				Assert.That(clusteredDictionary.Contains(kvp), Is.False);
			}
		}

		[Test]
		public void RemoveByKey([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(chf, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				clusteredDictionary.Remove(key);
				Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public void RemoveByKeyValuePair([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values("alpha", "Unicode😊😊😊", "")] string key, [Values(23)] int maxItems) {
			var rng = new Random(31337);
			using (CreateDictionary(chf, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				clusteredDictionary.Remove(key);
				Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
			}
		}

#if DEBUG
		[Test]
		public void IntegrationTests_Heavy([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values(250)] int maxItems) {
			var keyGens = 0;
			using (CreateDictionary(chf, out var clusteredDictionary)) {
				AssertEx.DictionaryIntegrationTest(
					clusteredDictionary,
					maxItems,
					(rng) => ($"{keyGens++}_{rng.NextString(0, 100)}", new TestObject(rng)),
					iterations: 250,
					valueComparer: new TestObjectComparer()
				);
			}
		}
#endif

		[Test]
		public void IntegrationTests([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
			const int maxItems = 100;
			var keyGens = 0;
			using (CreateDictionary(chf, out var clusteredDictionary)) {
				AssertEx.DictionaryIntegrationTest(
					clusteredDictionary,
					maxItems,
					(rng) => ($"{keyGens++}_{rng.NextString(0, 100)}", new TestObject(rng)),
					iterations: 10,
					valueComparer: new TestObjectComparer(),
					endOfIterTest: () => {
						// Manually test the merkle root
						var itemSerializer = new TestObjectSerializer();
						var itemHasher = new ItemHasher<TestObject>(chf, itemSerializer, Endianness.LittleEndian).WithNullHash(chf);
						var itemHashes = Enumerable.Range(0, clusteredDictionary.Storage.Count - clusteredDictionary.Storage.Header.ReservedRecords).Select(i => {
							var item = clusteredDictionary.ReadValue(i);
							return itemHasher.Hash(item);
						}).ToArray(); ;
						Assert.That(clusteredDictionary.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(itemHashes, chf)));
					}
				);
			}
		}

	}

}
