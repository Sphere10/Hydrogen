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
//using System.Text;
//using Hydrogen.NUnit;
//using NUnit.Framework;
//using static Hydrogen.Tests.StreamPersistedCollectionTestsBase;

//namespace Hydrogen.Tests;

//[Parallelizable]
//public class BTreeTests {

//	[Test]
//	public void IntegrationTests([Values(2,3,4,5,6,7,8,9,10,11,100,256)] int order) => DoIntegrationTests(order, 500, 500);

//	private void DoIntegrationTests([Values(2,5,11,100)] int order, int maxItems, int iterations) {
//		var keyGens = 0;
//		using (Create(order, out var dictionary)) {
//			AssertEx.DictionaryIntegrationTest(
//				dictionary,
//				maxItems,
//				(rng) => ($"{keyGens++}_{rng.NextString(0, 100)}", new TestObject(rng)),
//				iterations: iterations,
//				valueComparer: new TestObjectEqualityComparer()
//			);
//		}
//	}

//	protected IDisposable Create(int order, out BTree<string, TestObject> dictionary) {
//		dictionary = new BTree<string, TestObject>(order);
//		return new Disposables();
//	}
		

//}
