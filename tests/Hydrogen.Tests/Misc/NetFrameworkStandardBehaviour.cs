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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class NetFrameworkStandardBehaviour {


	[Test]
	public void TypedArraysAreDifferentTypes() {
		var byteArrType = typeof(byte[]);
		var intArrType = typeof(int[]);
		Assert.That(byteArrType.IsArray, Is.Not.EqualTo(intArrType));
	}


	[Test]
	public void TypedArrayIsArray() {
		var type = typeof(int[]);
		Assert.That(type.IsArray, Is.True);
	}

	[Test]
	public void ArrayTypeIsNotArray() {
		var arrayType = typeof(Array);
		Assert.That(arrayType.IsArray, Is.False);
	}

	[Test]
	public void TypedArrayIsNotArrayType() {
		var type = typeof(int[]);
		var arrayType = typeof(Array);
		Assert.That(type, Is.Not.EqualTo(arrayType));
	}

	[Test]
	public void TypedArrayHasArrayBaseType() {
		var type = typeof(int[]);
		var arrayType = typeof(Array);
		Assert.That(type.BaseType, Is.EqualTo(arrayType));
		Assert.That(arrayType.BaseType, Is.EqualTo(typeof(object)));
	}

	
	[Test]
	public void TypedArraysAreNotGenericTypes() {
		var type = typeof(int[]);
		var listType = typeof(List<int>);
		

		// Generic arrays are not generic types in .NET (unlike List<>)
		Assert.That(type.IsGenericType, Is.False);
		Assert.That(listType.IsGenericType, Is.True);
	}
	
	[Test]
	public void ConstructedGenericTypesAreReused() {
		var first = typeof(List<>).MakeGenericType(typeof(int));
		var second = typeof(List<int>);
		Assert.That(first, Is.EqualTo(second));
	}
	
	[Test]
	public void ConstructType() {
		Type openType = typeof(KeyValuePairSerializer<,>);
		Type[] typeArgs = { typeof(int), typeof(float) };
		var genericTypeDefinition = openType.MakeGenericType(typeArgs);
		var constructedGenericType = Activator.CreateInstance(genericTypeDefinition,new object[] { new PrimitiveSerializer<int>(), new PrimitiveSerializer<float>(), SizeDescriptorStrategy.UseCVarInt });
		Assert.That(constructedGenericType, Is.TypeOf(typeof(KeyValuePairSerializer<int, float>)));
	}

	[Test]
	public void ResolveIoC() {
		var serviceCollection = new ServiceCollection();
		serviceCollection.AddTransient(typeof(ICollection<>), typeof(ExtendedList<>));
		var serviceProvider = serviceCollection.BuildServiceProvider();
		var x = serviceProvider.GetService<ICollection<int>>();
		Assert.That(x, Is.TypeOf<ExtendedList<int>>());
	}

	[Test] 
	public void ReadByteAtEndOfStreamReturnsNegOne() {
		var memoryStream = new MemoryStream();
		var x = memoryStream.ReadByte();
		Assert.That(x, Is.EqualTo(-1));
	}

	[Test]
	public void DelegateEquality() {

		var x = SomeFunc;
		var y = SomeFunc;

		// delegates pointers are not reference equal
		Assert.That(ReferenceEquals(x, y), Is.False);

		// delegates pointers are not reference equal
		Assert.That(Equals(x, y), Is.True);

		void SomeFunc(int x, int y) {
		}
	}



	[Test]
	public void MemoryStream_PositionBehaviour() {
		// used as a basis to test BoundedStream
		var memoryStream = new MemoryStream();
		Assert.That(memoryStream.Position, Is.EqualTo(0));
		Assert.That(memoryStream.Length, Is.EqualTo(0));

		memoryStream.WriteByte(0);
		Assert.That(memoryStream.Position, Is.EqualTo(1));
		Assert.That(memoryStream.Length, Is.EqualTo(1));

		memoryStream.WriteByte(0);
		Assert.That(memoryStream.Position, Is.EqualTo(2));
		Assert.That(memoryStream.Length, Is.EqualTo(2));

		memoryStream.Seek(0, SeekOrigin.End);
		Assert.That(memoryStream.Position, Is.EqualTo(2));

		memoryStream = new MemoryStream(memoryStream.ToArray());
		memoryStream.Seek(0, SeekOrigin.End);
		Assert.That(memoryStream.Position, Is.EqualTo(2));
		Assert.That(() => memoryStream.WriteByte(3), Throws.Exception);
	}

	[Test]
	public void MemoryStream_SeekBehaviour() {
		// used as a basis to test BoundedStream
		var memoryStream = new MemoryStream(new byte[] { });

		// Allowed to set position past positive boundary
		Assert.That( () => memoryStream.Position = 1, Throws.Nothing);
		Assert.That(memoryStream.Position, Is.EqualTo(1));

		// Allowed to seek beyond past positive boundary
		Assert.That( () => memoryStream.Seek(11, SeekOrigin.Begin), Throws.Nothing);
		Assert.That(memoryStream.Position, Is.EqualTo(11));

		// Allowed to seek beyond past positive boundary
		Assert.That( () => memoryStream.Seek(11, SeekOrigin.Begin), Throws.Nothing);
		Assert.That(memoryStream.Position, Is.EqualTo(11));

		// Throws when breaching negative boundary
		Assert.That( () => memoryStream.Position = -1, Throws.Exception);
		Assert.That( () => memoryStream.Seek(-1, SeekOrigin.Begin), Throws.Exception);
		
		// Special: Allowed to seek beyond past positive boundary
		memoryStream = new MemoryStream(new byte[] { 1 });
		Assert.That( () => memoryStream.Seek(-1, SeekOrigin.End), Throws.Nothing);
		Assert.That(memoryStream.Position, Is.EqualTo(0));
	}



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
			Parallel.For(1,
				100,
				x => {
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
		}

		;

		async Task Task2() {
			while (true) {
				await Task.Delay(100);
			}
			ran2 = true;
		}

		Assert.That(() => Task.WhenAll(Task1(), Task2()).WithTimeout(1000), Throws.InstanceOf<TaskCanceledException>());
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
		}

		;

		async Task Task2() {
			await Task.Delay(200);
			ran2 = true;
		}

		Assert.That(() => Task.WhenAll(Task1(), Task2()).WithTimeout(1000), Throws.Nothing);
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

		Assert.That(() => Task.WhenAll(Task1(), Task2()).WithTimeout(1000), Throws.Nothing);
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

	[Test]
	public void ObjectInitializersExecutedAfterConstructor() {
		int? actual = null;
		var testClass = new TestClass(v => actual = v) { Property = 1 };
		Assert.That(actual, Is.EqualTo(0));
		Assert.That(testClass.Property, Is.EqualTo(1));
	}

	private class TestClass {
		public TestClass(Action<int> valueFetcher) {
			valueFetcher(Property);
		}
		public int Property { get; init; } = 0;
	}
}
