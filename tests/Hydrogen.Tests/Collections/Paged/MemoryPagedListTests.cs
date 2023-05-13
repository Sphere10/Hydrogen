// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using NUnit.Framework.Constraints;
using Hydrogen;
using Hydrogen.NUnit;

namespace Hydrogen.Tests {

    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
    public class MemoryPagedListTests {


        [Test]
        public void SinglePage([Values(1, 17)] int itemSize) {
            var pageSize = 1 * itemSize;
            using (var collection = new MemoryPagedList<int>(pageSize, 1*pageSize, itemSize)) {
                collection.Add(10);
                // Check page
				Assert.AreEqual(1, collection.Pages.Count());
                Assert.AreEqual(0, collection.Pages[0].Number);
				Assert.AreEqual(pageSize, collection.Pages[0].MaxSize);
                Assert.AreEqual(0, collection.Pages[0].StartIndex);
                Assert.AreEqual(1, collection.Pages[0].Count);
				Assert.AreEqual(0, collection.Pages[0].EndIndex);
				Assert.AreEqual(pageSize, collection.Pages[0].Size);
                Assert.IsTrue(collection.Pages[0].Dirty);

                // Check value
                Assert.AreEqual(10, collection[0]);
                Assert.AreEqual(1, collection.Count);
				Assert.AreEqual(1 * itemSize, collection.CalculateTotalSize());
            }
        }


        [Test]
		public void TwoPages([Values(1, 17)] int itemSize) {
			var pageSize = 1 * itemSize;
            using (var collection = new MemoryPagedList<int>(1 * itemSize, 1*itemSize, itemSize)) {
				collection.Add(10);

                // Check Page 1
				Assert.AreEqual(1, collection.Pages.Count());
				Assert.IsTrue(collection.Pages[0].State == PageState.Loaded);
                Assert.AreEqual(0, collection.Pages[0].Number);
				Assert.AreEqual(pageSize, collection.Pages[0].MaxSize);
				Assert.AreEqual(0, collection.Pages[0].StartIndex);
				Assert.AreEqual(1, collection.Pages[0].Count);
				Assert.AreEqual(0, collection.Pages[0].EndIndex);
				Assert.AreEqual(pageSize, collection.Pages[0].Size);
                Assert.IsTrue(collection.Pages[0].Dirty);

                // Add new page
                collection.Add(20);

                // Check pages 1 & 2
				Assert.AreEqual(2, collection.Pages.Count());
				Assert.IsTrue(collection.Pages[0].State == PageState.Unloaded);
				Assert.AreEqual(0, collection.Pages[0].Number);
				Assert.AreEqual(pageSize, collection.Pages[0].MaxSize);
				Assert.AreEqual(0, collection.Pages[0].StartIndex);
				Assert.AreEqual(1, collection.Pages[0].Count);
				Assert.AreEqual(0, collection.Pages[0].EndIndex);
				Assert.AreEqual(pageSize, collection.Pages[0].Size);


                Assert.IsTrue(collection.Pages[1].State == PageState.Loaded);
				Assert.AreEqual(1, collection.Pages[1].Number);
				Assert.AreEqual(pageSize, collection.Pages[1].MaxSize);
                Assert.AreEqual(1, collection.Pages[1].StartIndex);
				Assert.AreEqual(1, collection.Pages[1].Count);
				Assert.AreEqual(1, collection.Pages[1].EndIndex);
				Assert.AreEqual(pageSize, collection.Pages[1].Size);
                Assert.IsTrue(collection.Pages[1].Dirty);

                // Check values
                Assert.AreEqual(10, collection[0]);
                Assert.AreEqual(20, collection[1]);

                // Check size
				Assert.AreEqual(collection.Pages[0].Size, itemSize);
				Assert.AreEqual(collection.Pages[1].Size, itemSize);
                Assert.AreEqual(collection.CalculateTotalSize(), 2 * itemSize);
            }
        }


        [Test]
		public void SizeByCount_Integration([Values(1, 17)] int itemSize) {
			var deletes = 0;
			var created = 0;
			var loads = 0;
			var unloads = 0;
            using (var collection = new MemoryPagedList<string>(2 * itemSize, 2*(2*itemSize), itemSize)) {
                collection.PageCreated += (o, page) => created++;
				collection.PageDeleted += (o, page) => deletes++;
                collection.PageLoaded += (o, page) => loads++;
                collection.PageUnloaded += (o, page) => unloads++;
				Assert.AreEqual(0, created);
                Assert.AreEqual(0, loads);
                Assert.AreEqual(0, unloads);
				Assert.AreEqual(collection.Count, 0);
				Assert.AreEqual(collection.Pages.Count, 0);
				AssertEx.HasLoadedPages(collection, new int[0]);

                collection.Add("page1");
				Assert.AreEqual(1, created);
                Assert.AreEqual(1, loads);
				Assert.AreEqual(0, unloads);
				Assert.AreEqual(collection.Count, 1);
				Assert.AreEqual(collection.Pages.Count, 1);
				AssertEx.HasLoadedPages(collection, 0);

                collection.Add("page1.1");
				Assert.AreEqual(1, created);
                Assert.AreEqual(1, loads);
				Assert.AreEqual(0, unloads);
				Assert.AreEqual(collection.Count, 2);
                Assert.AreEqual(collection.Pages.Count, 1);
				AssertEx.HasLoadedPages(collection, 0);

                collection.Add("page2");
				Assert.AreEqual(2, created);
                Assert.AreEqual(2, loads);
				Assert.AreEqual(0, unloads);
				Assert.AreEqual(collection.Count, 3);
                Assert.AreEqual(collection.Pages.Count, 2);
				AssertEx.HasLoadedPages(collection, 0, 1);

                collection.Add("page2.2");
				Assert.AreEqual(2, created);
                Assert.AreEqual(2, loads);
				Assert.AreEqual(0, unloads);
				Assert.AreEqual(collection.Count, 4);
                Assert.AreEqual(collection.Pages.Count, 2);
				AssertEx.HasLoadedPages(collection, 0, 1);

                // should be two pages open
                collection.Add("page3");
				Assert.AreEqual(3, created);
                Assert.AreEqual(3, loads);
				Assert.AreEqual(1, unloads);
				Assert.AreEqual(collection.Pages.Count, 3);
				AssertEx.HasLoadedPages(collection, 1, 2);

                collection.Add("page3.3");
				Assert.AreEqual(3, created);
                Assert.AreEqual(3, loads);
				Assert.AreEqual(1, unloads);
				Assert.AreEqual(collection.Pages.Count, 3);
				AssertEx.HasLoadedPages(collection, 1, 2);

                // read from page[2] a few times to increase demand ticker
                var xxx = collection[5] + collection[5];

                collection.Add("page4");
				Assert.AreEqual(4, created);
				Assert.AreEqual(4, loads);
				Assert.AreEqual(2, unloads);
                Assert.AreEqual(collection.Pages.Count, 4);
				AssertEx.HasLoadedPages(collection, 2, 3);

                // read from page[3] several times to increase demand ticker
                xxx = collection[6] + collection[6] + collection[6] + collection[6] + collection[6] + collection[6] + collection[6] + collection[6];

                var item = collection[0];
				Assert.AreEqual(4, created);
				Assert.AreEqual(5, loads);
				Assert.AreEqual(3, unloads);
				Assert.AreEqual(collection.Pages.Count, 4);
                Assert.AreEqual(item, "page1");
				AssertEx.HasLoadedPages(collection, 3, 0);

                item = collection[1];
				Assert.AreEqual(4, created);
				Assert.AreEqual(5, loads);
				Assert.AreEqual(3, unloads);
				Assert.AreEqual(collection.Pages.Count, 4);
				Assert.AreEqual(item, "page1.1");
				AssertEx.HasLoadedPages(collection, 3, 0);

                item = collection[2];
				Assert.AreEqual(4, created);
				Assert.AreEqual(6, loads);
				Assert.AreEqual(4, unloads);
				Assert.AreEqual(collection.Pages.Count, 4);
				Assert.AreEqual(item, "page2");
				AssertEx.HasLoadedPages(collection, 3, 1);

                item = collection[3];
				Assert.AreEqual(4, created);
				Assert.AreEqual(6, loads);
				Assert.AreEqual(4, unloads);
				Assert.AreEqual(collection.Pages.Count, 4);
				Assert.AreEqual(item, "page2.2");
				AssertEx.HasLoadedPages(collection, 3, 1);

				item = collection[4];
				Assert.AreEqual(4, created);
				Assert.AreEqual(7, loads);
				Assert.AreEqual(5, unloads);
				Assert.AreEqual(collection.Pages.Count, 4);
				Assert.AreEqual(item, "page3");
				AssertEx.HasLoadedPages(collection, 3, 2);

				item = collection[5];
				Assert.AreEqual(4, created);
				Assert.AreEqual(7, loads);
				Assert.AreEqual(5, unloads);
				Assert.AreEqual(collection.Pages.Count, 4);
				Assert.AreEqual(item, "page3.3");
				AssertEx.HasLoadedPages(collection, 3, 2);

				item = collection[6];
				Assert.AreEqual(4, created);
				Assert.AreEqual(7, loads);
				Assert.AreEqual(5, unloads);
				Assert.AreEqual(collection.Pages.Count, 4);
				Assert.AreEqual(item, "page4");
				AssertEx.HasLoadedPages(collection, 3, 2);

                // Remove an illegal subrange
				Assert.Throws<ArgumentOutOfRangeException>(()=> collection.RemoveRange(3, 3));

                // remove some items
                collection.RemoveRange(3, 4);
				Assert.AreEqual(collection.Pages.Count, 2);
				Assert.AreEqual(collection.Count, 3);
                Assert.AreEqual(collection.Last(), "page2");
				Assert.AreEqual(2, deletes);
            }

        }

        [Test]
        public void SizeByFunc_Integration() {
			var deletes = 0;
            var created = 0;
			var loads = 0;
			var unloads = 0;
            using (var collection = new MemoryPagedList<string>(3, 2*3, str => str.Length)) {
                collection.PageCreated += (o, page) => created++;
				collection.PageDeleted += (o, page) => deletes++;
                collection.PageLoaded += (o, page) => loads++;
                collection.PageUnloaded += (o, page) => unloads++;
                Assert.AreEqual(0, created);
                Assert.AreEqual(0, loads);
                Assert.AreEqual(0, unloads);
                Assert.AreEqual(collection.Count, 0);
                Assert.AreEqual(collection.Pages.Count(), 0);
				AssertEx.HasLoadedPages(collection, new int[0]);

                // page 1
                collection.Add("01");
                Assert.AreEqual(1, created);
                Assert.AreEqual(1, loads);
                Assert.AreEqual(0, unloads);
                Assert.AreEqual(collection.Count, 1);
                Assert.AreEqual(collection.Pages.Count(), 1);
                Assert.AreEqual(collection.Pages[0].Size, 2);
				AssertEx.HasLoadedPages(collection, 0);

                collection.Add("2");
                Assert.AreEqual(1, created);
                Assert.AreEqual(1, loads);
                Assert.AreEqual(0, unloads);
                Assert.AreEqual(collection.Count, 2);
                Assert.AreEqual(collection.Pages.Count(), 1);
				Assert.AreEqual(collection.Pages[0].Size, 3);
                AssertEx.HasLoadedPages(collection, 0);

                // page 2
                collection.Add("34");
                Assert.AreEqual(2, created);
                Assert.AreEqual(2, loads);
                Assert.AreEqual(0, unloads);
                Assert.AreEqual(collection.Count, 3);
                Assert.AreEqual(collection.Pages.Count(), 2);
				Assert.AreEqual(collection.Pages[1].Size, 2);
                AssertEx.HasLoadedPages(collection, 0, 1);
                // read this page[1] few times to bump ticker
                var xxx = collection[2] + collection[2];

                // page 3
                collection.Add("56");
                Assert.AreEqual(3, created);
                Assert.AreEqual(3, loads);
                Assert.AreEqual(1, unloads);
                Assert.AreEqual(collection.Pages.Count(), 3);
				Assert.AreEqual(collection.Pages[2].Size, 2);
                AssertEx.HasLoadedPages(collection, 1, 2);
                // read this page[2] many times to bump ticker up
                xxx = collection[3] + collection[3] + collection[3] + collection[3];

                var item = collection[0];
                Assert.AreEqual(3, created);
                Assert.AreEqual(4, loads);
                Assert.AreEqual(2, unloads);
                Assert.AreEqual(collection.Pages.Count(), 3);
                Assert.AreEqual(item, "01");
				AssertEx.HasLoadedPages(collection, 2, 0);

                item = collection[1];
                Assert.AreEqual(3, created);
                Assert.AreEqual(4, loads);
                Assert.AreEqual(2, unloads);
                Assert.AreEqual(collection.Pages.Count(), 3);
                Assert.AreEqual(item, "2");
				AssertEx.HasLoadedPages(collection, 2, 0);

                item = collection[2];
                Assert.AreEqual(3, created);
                Assert.AreEqual(5, loads);
                Assert.AreEqual(3, unloads);
                Assert.AreEqual(collection.Pages.Count(), 3);
                Assert.AreEqual(item, "34");
				AssertEx.HasLoadedPages(collection, 2, 1);

                item = collection[3];
                Assert.AreEqual(3, created);
                Assert.AreEqual(5, loads);
                Assert.AreEqual(3, unloads);
                Assert.AreEqual(collection.Pages.Count(), 3);
                Assert.AreEqual(item, "56");
				AssertEx.HasLoadedPages(collection, 2, 1);

				// Remove an illegal subrange
				Assert.Throws<ArgumentOutOfRangeException>(() => collection.RemoveRange(1, 2));

				// remove some items
				collection.RemoveRange(1, 3);
				Assert.AreEqual(collection.Pages.Count(), 1);
				Assert.AreEqual(collection.Count, 1);
				Assert.AreEqual(collection[0], "01");
				Assert.AreEqual(2, deletes);
			}
        }

        [Test]
        public void IterateLazilyLoadedPages() {
            using (var collection = new MemoryPagedList<string>(3, 1*3, str => str.Length)) {
                // page 1
                collection.Add("0");
                collection.Add("1");
                collection.Add("2");
                // page 2
                collection.Add("34");
                // page 3
                collection.Add("56");
                AssertEx.HasLoadedPages(collection, 2);

				var loads = new List<int>();
				var unloads = new List<int>();
				collection.PageLoaded += (o, page) => loads.Add(page.Number);
				collection.PageUnloaded += (o, page) => unloads.Add(page.Number);
                foreach(var item in collection.WithDescriptions()) {
                    // ensure lazily loads
                    switch(item.Index) {
                        case 0:
							Assert.AreEqual(1, loads.Count);
                            break;
						case 1:
							Assert.AreEqual(1, loads.Count);
                            break;
						case 2:
							Assert.AreEqual(2, loads.Count);
                            break;
						case 3:
							Assert.AreEqual(3, loads.Count);
							break;
						case 4:
							Assert.AreEqual(3, loads.Count);
							break;

                    }
				}

                Assert.AreEqual(3, loads.Count);
				Assert.AreEqual(3, unloads.Count);

                Assert.AreEqual(loads[0], 0);
				Assert.AreEqual(loads[1], 1);
				Assert.AreEqual(loads[2], 2);

				Assert.AreEqual(unloads[0], 2);
				Assert.AreEqual(unloads[1], 0);
				Assert.AreEqual(unloads[2], 1);

            }
        }

		[Test]
		public void ItemTooLargeException() {
            using (var collection = new MemoryPagedList<string>(3, 1*3, str => str.Length)) {
                collection.Add("012");
				collection.Add("3");
				collection.Add("4");
				collection.Add("5");
				collection.Add("67");
				collection.Add("89");
                Assert.That(() => collection.Add("0123"), Throws.InstanceOf<InvalidOperationException>());
            }				
		}

        [Test]
        public void TestSinglePage() {
            using (var collection = new MemoryPagedList<string>(100, 1*100, str => str.Length*sizeof (char))) {

                collection.Add("01234567890123456789012345678901234567890123456789");

                var pages = collection.Pages.ToArray();
                Assert.AreEqual(1, pages.Length);
                Assert.AreEqual(0, pages[0].StartIndex);
                Assert.AreEqual(0, pages[0].EndIndex);
                Assert.AreEqual(1, pages[0].Count);
                Assert.AreEqual(100, pages[0].Size);
            }
        }

        [Test]
        public void TestSinglePage2() {
            using (var collection = new MemoryPagedList<string>(100, 1*100, str => str.Length*sizeof (char))) {
                collection.Add("0123456789");
                collection.Add("0123456789");
                collection.Add("0123456789");
                collection.Add("0123456789");
                collection.Add("0123456789");


                Assert.AreEqual(5, collection.Count);
                var pages = collection.Pages.ToArray();
                Assert.AreEqual(1, pages.Length);

                // Page 0
                Assert.AreEqual(0, pages[0].Number);

                Assert.AreEqual(0, pages[0].StartIndex);
                Assert.AreEqual(4, pages[0].EndIndex);
                Assert.AreEqual(5, pages[0].Count);
                Assert.AreEqual(100, pages[0].Size);
            }
        }


        [Test]
        public void TestTwoPages1() {
            var pageLoads = new List<int>();
            var pageUnloads = new List<int>();
            using (var collection = new MemoryPagedList<string>(100, 1*100, str => str.Length*sizeof (char))) {
                collection.PageLoaded += (largeCollection, page) => pageLoads.Add(page.Number);
                collection.PageUnloaded += (largeCollection, page) => pageUnloads.Add(page.Number);
                collection.Add("01234567890123456789012345678901234567890123456789");
                collection.Add("0123456789012345678901234567890123456789012345678");

                Assert.AreEqual(2, collection.Count);
                var pages = collection.Pages.ToArray();
                Assert.AreEqual(2, pages.Length);

                // Page 0
                Assert.AreEqual(0, pages[0].Number);
                Assert.AreEqual(0, pages[0].StartIndex);
                Assert.AreEqual(0, pages[0].EndIndex);
                Assert.AreEqual(1, pages[0].Count);
                Assert.AreEqual(100, pages[0].Size);

                // Page 1
                Assert.AreEqual(1, pages[1].Number);
                Assert.AreEqual(1, pages[1].StartIndex);
                Assert.AreEqual(1, pages[1].EndIndex);
                Assert.AreEqual(1, pages[1].Count);
                Assert.AreEqual(98, pages[1].Size);

                // Page Swaps
                Assert.AreEqual(2, pageLoads.Count);
                Assert.AreEqual(1, pageUnloads.Count);

                Assert.AreEqual(0, pageUnloads[0]);
            }
        }

        [Test]
        public void TestTwoPages2() {
            var pageLoads = new List<int>();
            var pageUnloads = new List<int>();
            using (var collection = new MemoryPagedList<string>(100, 1*100, str => str.Length * sizeof(char))) {
                collection.PageLoaded += (largeCollection, page) => pageLoads.Add(page.Number);
                collection.PageUnloaded += (largeCollection, page) => pageUnloads.Add(page.Number);
                collection.Add("0123456789");
                collection.Add("0123456789");
                collection.Add("0123456789");
                collection.Add("0123456789");
                collection.Add("0123456789");
                collection.Add("0123456789");
                collection.Add("0123456789");
                collection.Add("0123456789");
                collection.Add("0123456789");
                collection.Add("012345678");

                Assert.AreEqual(10, collection.Count);
                var pages = collection.Pages.ToArray();
                Assert.AreEqual(2, pages.Length);

                // Page 0
                Assert.AreEqual(0, pages[0].Number);
                Assert.AreEqual(0, pages[0].StartIndex);
                Assert.AreEqual(4, pages[0].EndIndex);
                Assert.AreEqual(5, pages[0].Count);
                Assert.AreEqual(100, pages[0].Size);

                // Page 1
                Assert.AreEqual(1, pages[1].Number);
                Assert.AreEqual(5, pages[1].StartIndex);
                Assert.AreEqual(9, pages[1].EndIndex);
                Assert.AreEqual(5, pages[1].Count);
                Assert.AreEqual(98, pages[1].Size);


                // Page Swaps
                Assert.AreEqual(2, pageLoads.Count);
                Assert.AreEqual(1, pageUnloads.Count);
                Assert.AreEqual(0, pageUnloads[0]);
            }
        }
    
        [Test]
        public void TestEmpty() {
            using (var collection = new MemoryPagedList<string>(40, 1*40, str => str.Length * sizeof(char))) {
                Assert.AreEqual(0, collection.Pages.Count());
                Assert.AreEqual(0, collection.Count);
            }
        }

        [Test]
        public void TestEmptyItems() {
            using (var collection = new MemoryPagedList<string>(1, 1*1, str => str.Length * sizeof(char))) {
                collection.Add("");
                collection.Add("");
                collection.Add("");
                Assert.AreEqual(1, collection.Pages.Count());
                Assert.AreEqual(3, collection.Count);
            }
        }

        [Test]
        public void TestIteratorEmpty() {
            using (var collection = new MemoryPagedList<string>(40, 1*40, str => str.Length*sizeof (char))) {
                foreach (var item in collection) {
                    var xxx = 1;
                }
            }
        }


		[Test]
		public void TestIteratorThrowsWhenCollectionChanged_1() {
			using (var collection = new MemoryPagedList<string>(40, 1*40, str => str.Length * sizeof(char))) {
				collection.AddRange("10");
				var thrown = false;
				try {
					foreach (var item in collection) 
						collection.Add("20");
				} catch (Exception error) {
					thrown = true;
				}
				Assert.IsTrue(thrown, "Exception was not thrown");
			}
		}


        [Test]
        public void TestIteratorThrowsWhenCollectionChanged_2() {
            using (var collection = new MemoryPagedList<string>(40, 1*40, str => str.Length*sizeof (char))) {
                collection.AddRange("10", "20");
                var thrown = false;
                try {
                    foreach (var item in collection) 
                        collection.Add("20");
                } catch (Exception error) {
                    thrown = true;
                }
                Assert.IsTrue(thrown, "Exception was not thrown");
            }
        }

		[Test]
		public void TestIteratorThrowsWhenCollectionChanged_3() {
			using (var collection = new MemoryPagedList<string>(40, 1*40, str => str.Length * sizeof(char))) {
				collection.AddRange("10", "20", "30");
				try {
					foreach (var item in collection) {
						collection.Add("50");
					}
				} catch (Exception error) {
				}


                var list = new List<string>();
				foreach (var item in collection) {
                    list.Add(item);
				}
                Assert.AreEqual("10", list[0]);
				Assert.AreEqual("20", list[1]);
				Assert.AreEqual("30", list[2]);

            }
        }

        [Test]
        public void TestRandomAccess() {
            using (var collection = new MemoryPagedList<string>(50000, 1*50000, str => str.Length*sizeof (char))) {
                collection.PageLoaded += (largeCollection, page) => {
                    //System.Console.WriteLine("Page Loaded: {0}\t\t{1}", page.Number, ((MemoryPagedListBase<string>)largeCollection).Pages.Count());
                };
                collection.PageUnloaded += (largeCollection, page) => {
                    //System.Console.WriteLine("Page Unloaded: {0}\t\t{1}", page.Number, ((MemoryPagedListBase<string>)largeCollection).Pages.Count());
                };
                for (var i = 0; i < 10000; i++) {
                    collection.Add(Tools.Text.GenerateRandomString(Tools.Maths.RNG.Next(0, 100)));
                }

                Assert.AreEqual(10000, collection.Count);

                for (var i = 0; i < 300; i++) {
                    var str = collection[Tools.Maths.RNG.Next(0, 10000 - 1)];
                }
            }
        }


        [Test]
        public void TestGrowWhilstRandomAccess() {
            using (var collection = new MemoryPagedList<string>(5000, 1*5000, str => str.Length*sizeof (char))) {
                collection.PageLoaded += (largeCollection, page) => {
                    //System.Console.WriteLine("Page Loaded: {0}\t\t{1}", page.Number, ((MemoryPagedListBase<string>)largeCollection).Pages.Count());
                };
                collection.PageUnloaded += (largeCollection, page) => {
                    //System.Console.WriteLine("Page Unloaded: {0}\t\t{1}", page.Number, ((MemoryPagedListBase<string>)largeCollection).Pages.Count());
                };


                for (var i = 0; i < 100; i++) {
                    collection.Add(Tools.Text.GenerateRandomString(Tools.Maths.RNG.Next(100, 1000)));
                    for (var j = 0; j < 3; j++) {
                        var str = collection[Tools.Maths.RNG.Next(0, collection.Count - 1)];
                    }
                }
            }
        }

        [Test]
        public void TestLinq() {
            using (var collection = new MemoryPagedList<string>(50000, 1*50000, str => str.Length*sizeof (char))) {
                collection.PageLoaded += (largeCollection, page) => {
                    //System.Console.WriteLine("Page Loaded: {0}\t\t{1}", page.Number, ((MemoryPagedListBase<string>)largeCollection).Pages.Count());
                };
                collection.PageUnloaded += (largeCollection, page) => {
                    //System.Console.WriteLine("Page Unloaded: {0}\t\t{1}", page.Number, ((MemoryPagedListBase<string>)largeCollection).Pages.Count());
                };

                for (int i = 0; i < 100000; i++) {
                    collection.Add(i.ToString());
                }
                var testCollection = collection
                    .Where(s => s.StartsWith("1"))
                    .Union(collection.Where(s => s.StartsWith("2")))
                    .Reverse();

                foreach (var val in testCollection) {
                    Assert.IsTrue(val.StartsWith("1") || val.StartsWith("2"));
                }

            }
        }


		[Test]
		[Sequential]
		public void IntegrationTests(
		[Values(1, 10, 57, 173, 1111)] int maxCapacity,
		[Values(1, 1, 3, 31, 13)] int pageSize,
		[Values(1, 1, 7, 2, 19)] int maxOpenPages) {
			using (var list = new MemoryPagedList<byte>(pageSize, maxOpenPages * pageSize, sizeof(byte))) {
				AssertEx.ListIntegrationTest<byte>(list, maxCapacity, (rng, i) => rng.NextBytes(i), mutateFromEndOnly: true);
			}
		}
	}

}
