//using System;
//using System.IO;
//using NUnit.Framework;

//namespace Sphere10.Framework.Tests {
//	public abstract class ClusteredListTestsBase : ClusteredCollectionTestsBase {

//		[Test]
//		public void EmptyListings() {
//			using (CreateList(out var clusteredList)) {
//				Assert.That(() => clusteredList.InternalExtendedList.Listings, Throws.Nothing);
//				Assert.That(clusteredList.InternalExtendedList.Listings.Count, Is.EqualTo(0));
//			}
//		}

//		[Test]
//		public void AddOneTest() {
//			var rng = new Random(31337);
//			using (CreateList(out var clusteredList)) {
//				var obj = new TestObject(rng);
//				clusteredList.Add(obj);
//				Assert.That(clusteredList.Count, Is.EqualTo(1));
//				Assert.That(clusteredList[0], Is.EqualTo(obj).Using(new TestObjectComparer()));
//			}
//		}

//		[Test]
//		public void AddOneRepeat([Values(100)] int iterations) {
//			var rng = new Random(31337);
//			using (CreateList(out var clusteredList)) {
//				for (var i = 0; i < iterations; i++) {
//					var obj = new TestObject(rng);
//					clusteredList.Add(obj);
//					Assert.That(clusteredList.Count, Is.EqualTo(i+1));
//					Assert.That(clusteredList[i], Is.EqualTo(obj).Using(new TestObjectComparer()));
//				}
//			}
//		}


//		protected abstract IDisposable CreateList(out ClusteredList<TestObject> clusteredList);
//	}
//}
