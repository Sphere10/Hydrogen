//-----------------------------------------------------------------------
// <copyright file="ScopeContextTest.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Sphere10.Framework.Tests {

    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
    public class ScopeContextTest {


        [Test]
        public void TestNested_None_None() {
            Assert.False(ExceptionOccured(ScopeContextPolicy.None, ScopeContextPolicy.None));
        }

        [Test]
        public void TestNested_None_MustBeNested() {
            Assert.False(ExceptionOccured(ScopeContextPolicy.None, ScopeContextPolicy.MustBeNested));
        }

        [Test]
        public void TestNested_None_MustBeRoot() {
            Assert.True(ExceptionOccured(ScopeContextPolicy.None, ScopeContextPolicy.MustBeRoot));
        }

        [Test]
        public void TestNested_MustBeNested_None() {
            Assert.True(ExceptionOccured(ScopeContextPolicy.MustBeNested, ScopeContextPolicy.None));
        }

        [Test]
        public void TestNested_MustBeNested_MustBeNested() {
            Assert.True(ExceptionOccured(ScopeContextPolicy.MustBeNested, ScopeContextPolicy.MustBeNested));
        }

        [Test]
        public void TestNested_MustBeNested_MustBeRoot() {
            Assert.True(ExceptionOccured(ScopeContextPolicy.MustBeNested, ScopeContextPolicy.MustBeRoot));
        }

        [Test]
        public void TestNested_MustBeRoot_None() {
            Assert.False(ExceptionOccured(ScopeContextPolicy.MustBeRoot, ScopeContextPolicy.None));
        }

        [Test]
        public void TestNested_MustBeRoot_MustBeNested() {
            Assert.False(ExceptionOccured(ScopeContextPolicy.MustBeRoot, ScopeContextPolicy.MustBeNested));
        }


        [Test]
        public void TestNested_MustBeRoot_MustBeRoot() {
            Assert.True(ExceptionOccured(ScopeContextPolicy.MustBeRoot, ScopeContextPolicy.MustBeRoot));
        }

        [Test]
        public void MultiThreaded_0() {
            Assert.IsTrue(Enumerable.Range(1, 10).All(x => !ExceptionOccured(ScopeContextPolicy.MustBeRoot, ScopeContextPolicy.MustBeNested, Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100))));
        }


        [Test]
        public void MultiThreaded_1() {
            var task1 = new Task<bool>(() => Enumerable.Range(1, 10).All(x => !ExceptionOccured(ScopeContextPolicy.MustBeRoot, ScopeContextPolicy.MustBeNested, Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100))));
            var task2 = new Task<bool>(() => Enumerable.Range(1, 10).All(x => !ExceptionOccured(ScopeContextPolicy.MustBeRoot, ScopeContextPolicy.MustBeNested, Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100))));
            task1.Start();
            task2.Start();
            Task.WaitAll(task1, task2);
            Assert.IsTrue(task1.Result);
            Assert.IsTrue(task2.Result);
        }

        [Test]
        public void MultiThreaded_2() {
            var task1 = new Task<bool>(() => !ExceptionOccured(ScopeContextPolicy.MustBeRoot, ScopeContextPolicy.MustBeNested, 1000, 0, 0));
            var task2 = new Task<bool>(() => !ExceptionOccured(ScopeContextPolicy.MustBeRoot, ScopeContextPolicy.MustBeNested, 0, 0, 0));
            task1.Start();
            task2.Start();
            Task.WaitAll(task1, task2);
            Assert.IsTrue(task1.Result);
            Assert.IsTrue(task2.Result);
        }


        [Test]
        public void TestNested_None_None_Async() {
            Assert.False(ExceptionOccuredAsync(ScopeContextPolicy.None, ScopeContextPolicy.None));
        }

        [Test]
        public void TestNested_None_MustBeNested_Async() {
            Assert.False(ExceptionOccuredAsync(ScopeContextPolicy.None, ScopeContextPolicy.MustBeNested));
        }

        [Test]
        public void TestNested_None_MustBeRoot_Async() {
            Assert.True(ExceptionOccuredAsync(ScopeContextPolicy.None, ScopeContextPolicy.MustBeRoot));
        }

        [Test]
        public void TestNested_MustBeNested_None_Async() {
            Assert.True(ExceptionOccuredAsync(ScopeContextPolicy.MustBeNested, ScopeContextPolicy.None));
        }

        [Test]
        public void TestNested_MustBeNested_MustBeNested_Async() {
            Assert.True(ExceptionOccuredAsync(ScopeContextPolicy.MustBeNested, ScopeContextPolicy.MustBeNested));
        }

        [Test]
        public void TestNested_MustBeNested_MustBeRoot_Async() {
            Assert.True(ExceptionOccuredAsync(ScopeContextPolicy.MustBeNested, ScopeContextPolicy.MustBeRoot));
        }

        [Test]
        public void TestNested_MustBeRoot_None_Async() {
            Assert.False(ExceptionOccuredAsync(ScopeContextPolicy.MustBeRoot, ScopeContextPolicy.None));
        }

        [Test]
        public void TestNested_MustBeRoot_MustBeNested_Async() {
            Assert.False(ExceptionOccuredAsync(ScopeContextPolicy.MustBeRoot, ScopeContextPolicy.MustBeNested));
        }


        [Test]
        public void TestNested_MustBeRoot_MustBeRoot_Async() {
            Assert.True(ExceptionOccuredAsync(ScopeContextPolicy.MustBeRoot, ScopeContextPolicy.MustBeRoot));
        }


        [Test]
        public void MultiThreaded_0_Async() {
            Assert.IsTrue(Enumerable.Range(1, 10).All(x => !ExceptionOccuredAsync(ScopeContextPolicy.MustBeRoot, ScopeContextPolicy.MustBeNested, Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100))));
        }


        [Test]
        public void MultiThreaded_1_Async() {
            var task1 = new Task<bool>(() => Enumerable.Range(1, 10).All(x => !ExceptionOccuredAsync(ScopeContextPolicy.MustBeRoot, ScopeContextPolicy.MustBeNested, Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100))));
            var task2 = new Task<bool>(() => Enumerable.Range(1, 10).All(x => !ExceptionOccuredAsync(ScopeContextPolicy.MustBeRoot, ScopeContextPolicy.MustBeNested, Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(0, 100))));
            task1.Start();
            task2.Start();
            Task.WaitAll(task1, task2);
            Assert.IsTrue(task1.Result);
            Assert.IsTrue(task2.Result);
        }

        [Test]
        public void MultiThreaded_2_Async() {
            var task1 = new Task<bool>(() => !ExceptionOccuredAsync(ScopeContextPolicy.MustBeRoot, ScopeContextPolicy.MustBeNested, 1000, 0, 0));
            var task2 = new Task<bool>(() => !ExceptionOccuredAsync(ScopeContextPolicy.MustBeRoot, ScopeContextPolicy.MustBeNested, 0, 0, 0));
            task1.Start();
            task2.Start();
            Task.WaitAll(task1, task2);
            Assert.IsTrue(task1.Result);
            Assert.IsTrue(task2.Result);
        }


        
        private bool ExceptionOccured(ScopeContextPolicy rootPolicy, ScopeContextPolicy childPolicy, int delay1 = 0, int delay2 = 0, int delay3 = 0) {
            try {
                using (new ScopeContextDemo(rootPolicy)) {
                    System.Threading.Thread.Sleep(delay1);
                    using (new ScopeContextDemo(childPolicy)) {
                        System.Threading.Thread.Sleep(delay2);
                        using (new ScopeContextDemo(childPolicy)) {
                            System.Threading.Thread.Sleep(delay3);
                        }
                    }
                }
            } catch (Exception error) {
                return true;
            }
            return false;
        }

        private bool ExceptionOccuredAsync(ScopeContextPolicy rootPolicy, ScopeContextPolicy childPolicy, int delay1 = 0, int delay2 = 0, int delay3 = 0) {
            try {
                AsyncTest(Tuple.Create(rootPolicy, delay1), Tuple.Create(childPolicy, delay2), Tuple.Create(ScopeContextPolicy.None, delay3)).Wait();
            } catch (Exception error) {
                return true;
            }
            return false;
        }



        private async Task AsyncTest(params Tuple<ScopeContextPolicy, int>[] policies) {
            if (policies.Any()) {
                var head = policies.First();
                using (new ScopeContextDemo(head.Item1)) {
                    System.Threading.Thread.Sleep(head.Item2);
                    var tail = policies.Skip(1).ToArray();
                    await AsyncTest(tail);
                }
            }
        }


        public class ScopeContextDemo : ScopeContext<ScopeContextDemo> {
            

            public ScopeContextDemo(ScopeContextPolicy policy)
                : base("ScopedContextDemo", policy) {
            }


            protected override void OnScopeEnd(ScopeContextDemo rootScope, bool inException) {
            }
        }
    }

}
