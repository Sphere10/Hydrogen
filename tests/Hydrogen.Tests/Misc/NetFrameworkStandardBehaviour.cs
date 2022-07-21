//-----------------------------------------------------------------------
// <copyright file="UrlToolTests.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime.Interop;
using NUnit.Framework;

namespace Hydrogen.Tests {

    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    public class NetFrameworkStandardBehaviour {

        [Test]
        public void DictionaryAddDoesntUpdate() {
            var dictionary = new Dictionary<string, object>();
            dictionary.Add("alpha", 1);
            Assert.That(() => dictionary.Add("alpha", 2), Throws.Exception);
        }

        [Test]
        public void ParallelForEachPropagatesException() {

            Assert.That(ParallelCode, Throws.TypeOf<AggregateException>());

            void ParallelCode() {
                Parallel.For(1, 100, x => {
                    if (x == 50)
                        throw new SoftwareException();
                });
            }
        }

        [Test]
        public void ListGetRangeNotSupportOverflow() {
            var list = new List<int>();
            list.AddRange(new[] { 1, 2, 3 });
            Assert.That(() => list.GetRange(1, 3), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public void ListRemoveRangeNotSupportOverflow() {
            var list = new List<int>();
            list.AddRange(new[] { 1, 2, 3 });
            Assert.That(() => list.RemoveRange(1, 3), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public void ListInsertRangeThrowsOnNull() {
            var list = new List<int>();
            Assert.That(() => list.InsertRange(0, null), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public void FileMode_Open_DoesNotTruncate() {
            // append 100b to a file
            // open a file stream overwrite
            // append 50b
            // close stream
            // check file is 100b

            var rng = new Random(31337);
            var file = Tools.FileSystem.GetTempFileName(true);
            var _ = Tools.Scope.DeleteFileOnDispose(file);
            Tools.FileSystem.AppendAllBytes(file, rng.NextBytes(100));
            Assert.That(Tools.FileSystem.GetFileSize(file), Is.EqualTo(100));
            using (var stream = File.Open(file, FileMode.Open, FileAccess.Write))
                stream.Write(rng.NextBytes(50));
            Assert.That(Tools.FileSystem.GetFileSize(file), Is.EqualTo(100));
        }

        [Test]
        public void FileMode_Truncate() {
            // append 100b to a file
            // open a file stream overwrite
            // append 50b
            // close stream
            // check file is 50b

            var rng = new Random(31337);
            var file = Tools.FileSystem.GetTempFileName(true);
            var _ = Tools.Scope.DeleteFileOnDispose(file);
            Tools.FileSystem.AppendAllBytes(file, rng.NextBytes(100));
            Assert.That(Tools.FileSystem.GetFileSize(file), Is.EqualTo(100));
            using (var stream = File.Open(file, FileMode.Truncate, FileAccess.Write))
                stream.Write(rng.NextBytes(50));
            Assert.That(Tools.FileSystem.GetFileSize(file), Is.EqualTo(50));
        }

        [Test]
        public void FileOpenWrite_DoesNotTruncate() {
            // append 100b to a file
            // open a file stream overwrite
            // append 50b
            // close stream
            // check file is 100b

            var rng = new Random(31337);
            var file = Tools.FileSystem.GetTempFileName(true);
            var _ = Tools.Scope.DeleteFileOnDispose(file);
            Tools.FileSystem.AppendAllBytes(file, rng.NextBytes(100));
            Assert.That(Tools.FileSystem.GetFileSize(file), Is.EqualTo(100));
            using (var stream = File.OpenWrite(file))
                stream.Write(rng.NextBytes(50));
            Assert.That(Tools.FileSystem.GetFileSize(file), Is.EqualTo(100));
        }

        [Test]
        public void StreamAllowsTipCursor() {
            var rng = new Random(31337);
            using Stream stream = new MemoryStream();
            Assert.That(stream.Length, Is.EqualTo(0));
            Assert.That(stream.Position, Is.EqualTo(0));
            stream.Write(rng.NextBytes(100));
            Assert.That(stream.Position, Is.EqualTo(100));
        }

        [Test]
        public void WhenAllDoesntAbandonAfterSingleFailure() {
            var rng = new Random(31337);
            using Stream stream = new MemoryStream();

            var ran1 = false;
            var ran2 = false;

            async Task Task1() {
                await Task.Delay(100);
                ran1 = true;
                Guard.Ensure(false, "Exception");
            };

            async Task Task2() {
                while (true) {
                    await Task.Delay(100);
                }
                ran2 = true;
            }

            Assert.That(() => Task.WhenAll(Task1(), Task2()).WithTimeout(250), Throws.InstanceOf<TaskCanceledException>());
            Assert.That(ran1, Is.True);
            Assert.That(ran2, Is.False);
        }

        [Test]
        public void WhenAllDoesntThrowAfterSingleFailure() {
            var rng = new Random(31337);
            using Stream stream = new MemoryStream();

            var ran1 = false;
            var ran2 = false;

            async Task Task1() {
                await Task.Delay(100);
                ran1 = true;
                Guard.Ensure(false, "Exception");
            };

            async Task Task2() {
                await Task.Delay(150);
                ran2 = true;
            }

            Assert.That(() => Task.WhenAll(Task1(), Task2()).WithTimeout(250), Throws.Nothing);
            Assert.That(ran1, Is.True);
            Assert.That(ran2, Is.True);
        }

        [Test]
        public void WhenAllDoesntThrowAfterAllFailure() {
            var rng = new Random(31337);
            using Stream stream = new MemoryStream();

            var ran1 = false;
            var ran2 = false;
            async Task Task1() {
                await Task.Delay(100);
                ran1 = true;
                Guard.Ensure(false, "Exception");
            }

            async Task Task2() {
                await Task.Delay(100);
                ran2 = true;
                Guard.Ensure(false, "Exception");
            }

            Assert.That(() => Task.WhenAll(Task1(), Task2()).WithTimeout(250), Throws.Nothing);
            Assert.That(ran1, Is.True);
            Assert.That(ran2, Is.True);
        }

        [Test]
        public async Task WhenAnyAbandonsAfterSingleFailure() {
            var rng = new Random(31337);
            using Stream stream = new MemoryStream();

            var task1 = Task.Factory.StartNew(() => {
                Thread.Sleep(100);
                Guard.Ensure(false, "Exception");
            });
            var task2 = Task.Factory.StartNew(() => {
                while (true) {
                    Thread.Sleep(100);
                }
            });

            await Task.WhenAny(task1, task2);
            Assert.That(task1.Exception, Is.Not.Null);
            Assert.That(task1.Exception.InnerExceptions.Count, Is.EqualTo(1));
            Assert.That(task1.Exception.InnerExceptions[0], Is.TypeOf<InvalidOperationException>());
            Assert.That(task2.Exception, Is.Null);

        }

        [Test]
        public void OutOfBoundsIntCastDoesntThrow() {
            var x = (long)int.MaxValue + 1;
            Assert.That(() => (int)x, Throws.Nothing);
        }

        [Test]
        public async Task AsyncTaskNotIgnoringExceptionsDoesThrow() {
            try {
                await SomeTaskAsync();
            } catch {
                return;
            }
            Assert.That(false, Is.True);

            async Task SomeTaskAsync() {
                throw new InvalidOperationException("Should never be seen");
            }
        }

        [Test]
        public async Task AsyncTaskIgnoringExceptionsDoesntThrow() {
            await SomeTaskAsync().IgnoringExceptions();
            Assert.That(true, Is.True);

            async Task SomeTaskAsync() {
                throw new InvalidOperationException("Should never be seen");
            }
        }


    }
}
