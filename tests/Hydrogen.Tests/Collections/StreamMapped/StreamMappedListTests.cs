﻿// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
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
using NUnit.Framework;
using Hydrogen.NUnit;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class StreamMappedListTests : StreamPersistedCollectionTestsBase {

	private IDisposable CreateList(StreamContainerPolicy policy, bool useChecksumIndex, out IStreamMappedList<TestObject> clusteredList) {
		var stream = new MemoryStream();
		clusteredList = StreamMappedFactory.CreateList(stream, 32, new TestObjectSerializer(), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<TestObject>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
		if (clusteredList.RequiresLoad)
			clusteredList.Load();
		return new Disposables(clusteredList, stream);
	}

	[Test]
	public void HasReservedRecord([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		using var scope = CreateList(policy, useChecksumIndex, out var list);
		Assert.That(list.ObjectContainer.StreamContainer.Count, Is.EqualTo(useChecksumIndex ? 1 : 0));
	}

	[Test]
	public void AddOneTest([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		var rng = new Random(31337);
		using (CreateList(policy, useChecksumIndex, out var clusteredList)) {
			var obj = new TestObject(rng);
			clusteredList.Add(obj);
			var item = clusteredList.Read(0);
			Assert.That(clusteredList.Count, Is.EqualTo(1));
			Assert.That(clusteredList[0], Is.EqualTo(obj).Using(new TestObjectEqualityComparer()));
		}
	}

	[Test]
	public void AddOneRepeat([Values(100)] int iterations, [StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		var rng = new Random(31337);
		using (CreateList(policy, useChecksumIndex, out var clusteredList)) {
			for (var i = 0; i < iterations; i++) {
				var obj = new TestObject(rng);
				clusteredList.Add(obj);
				Assert.That(clusteredList.Count, Is.EqualTo(i + 1));
				Assert.That(clusteredList[i], Is.EqualTo(obj).Using(new TestObjectEqualityComparer()));
			}
		}
	}

	[Test]
	public void ConstructorArgumentsAreGuarded([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		Assert.Throws<ArgumentNullException>(() => StreamMappedFactory.CreateList<int>(null, 1, new PrimitiveSerializer<int>(), reservedStreams: 1, policy: policy));
		Assert.DoesNotThrow(() => StreamMappedFactory.CreateList<int>(new MemoryStream(), 1, null, reservedStreams: 1, policy: policy));
		Assert.Throws<ArgumentOutOfRangeException>(() => StreamMappedFactory.CreateList<int>(new MemoryStream(), 0, null, reservedStreams: 1, policy: policy));
	}

	[Test]
	public void ReadRange([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		var random = new Random(31337);
		string[] inputs = Enumerable.Range(0, random.Next(5, 10)).Select(x => random.NextString(1, 100)).ToArray();
		using var stream = new MemoryStream();
		using var list = StreamMappedFactory.CreateList<string>(stream, 32, new StringSerializer(Encoding.UTF8), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<string>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
		if (list.RequiresLoad)
			list.Load();
		list.AddRange(inputs);

		var read = list
			.ReadRange(0, 3)
			.ToArray();

		Assert.AreEqual(inputs[0], read[0]);
		Assert.AreEqual(inputs[1], read[1]);
		Assert.AreEqual(inputs[2], read[2]);
	}

	[Test]
	public void ReadRangeInvalidArguments([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		using var stream = new MemoryStream();
		using var list = StreamMappedFactory.CreateList<int>(stream, 1, new PrimitiveSerializer<int>(), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<int>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
		if (list.RequiresLoad)
			list.Load();
		list.AddRange(999, 1000, 1001, 1002);
		if (list.RequiresLoad)
			list.Load();

		Assert.Throws<ArgumentOutOfRangeException>(() => _ = list.ReadRange(-1, 1).ToList());
		Assert.Throws<ArgumentOutOfRangeException>(() => _ = list.ReadRange(0, 5).ToList());
	}

	[Test]
	public void ReadRangeEmpty([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		using var stream = new MemoryStream();
		using var list = StreamMappedFactory.CreateList<int>(stream, 1, new PrimitiveSerializer<int>(), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<int>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
		if (list.RequiresLoad)
			list.Load();

		list.AddRange(999, 1000, 1001, 1002);
		Assert.IsEmpty(list.ReadRange(0, 0));

		list.Clear();
		Assert.IsEmpty(list.ReadRange(0, 0));
	}

	[Test]
	public void AddRangeEmptyNullStrings([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		using var stream = new MemoryStream();
		using var list = StreamMappedFactory.CreateList<string>(stream, 32, new StringSerializer(Encoding.UTF8), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<string>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
		if (list.RequiresLoad)
			list.Load();
		string[] input = { string.Empty, null, string.Empty, null };
		list.AddRange(input);
		Assert.That(list.Count, Is.EqualTo(4));
		Assert.That(list, Is.EqualTo(input));
	}

	[Test]
	public void IndexOf_NullEmptyStrings([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		using var stream = new MemoryStream();
		using var list = StreamMappedFactory.CreateList<string>(stream, 32, new StringSerializer(Encoding.UTF8), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<string>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
		if (list.RequiresLoad)
			list.Load();
		string[] input = { string.Empty, null, string.Empty, null };
		list.AddRange(input);
		Assert.That(list.IndexOf(null), Is.EqualTo(1));
		list[1] = "z";
		Assert.That(list.IndexOf(null), Is.EqualTo(3));
		Assert.That(list.IndexOf(string.Empty), Is.EqualTo(0));
		list[0] = "z";
		Assert.That(list.IndexOf(string.Empty), Is.EqualTo(2));
	}

	[Test]
	public void AddRangeNullEmptyCollections([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		using var stream = new MemoryStream();
		using var list = StreamMappedFactory.CreateList<string>(stream, 32, new StringSerializer(Encoding.UTF8), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<string>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
		if (list.RequiresLoad)
			list.Load();
		Assert.Throws<ArgumentNullException>(() => list.AddRange(null));
		Assert.DoesNotThrow(() => list.AddRange(new string[0]));
	}

	[Test]
	public void UpdateRange([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		var random = new Random(31337);
		string[] inputs = Enumerable.Range(0, random.Next(1, 100)).Select(x => random.NextString(1, 100)).ToArray();
		using var stream = new MemoryStream();
		using var list = StreamMappedFactory.CreateList<string>(stream, 32, new StringSerializer(Encoding.UTF8), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<string>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
		if (list.RequiresLoad)
			list.Load();
		list.AddRange(inputs);

		string update = random.NextString(0, 100);
		list.UpdateRange(0, new[] { update });
		var value = list.Read(0);
		Assert.AreEqual(update, value);
		Assert.AreEqual(inputs.Length, list.Count);
	}

	[Test]
	public void UpdateRangeInvalidArguments([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		using var stream = new MemoryStream();
		using var list = StreamMappedFactory.CreateList<int>(stream, 32, new PrimitiveSerializer<int>(), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<int>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
		if (list.RequiresLoad)
			list.Load();

		list.AddRange(999, 1000, 1001, 1002);
		list.UpdateRange(0, new[] { 998 });
		int read = list[0];

		Assert.AreEqual(998, read);
		Assert.AreEqual(4, list.Count);
	}

	[Test]
	public void RemoveRange([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		var random = new Random(31337);
		string[] inputs = Enumerable.Range(0, random.Next(1, 100)).Select(x => random.NextString(1, 100)).ToArray();
		using var stream = new MemoryStream();
		using var list = StreamMappedFactory.CreateList<string>(stream, 32, new StringSerializer(Encoding.UTF8), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<string>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
		if (list.RequiresLoad)
			list.Load();

		list.AddRange(inputs);
		list.RemoveRange(0, 1);

		Assert.AreEqual(inputs[1], list[0]);
		Assert.AreEqual(inputs.Length - 1, list.Count);
	}

	[Test]
	public void RemoveRangeInvalidArguments([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		using var stream = new MemoryStream();
		using var list = StreamMappedFactory.CreateList<int>(stream, 1, new PrimitiveSerializer<int>(), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<int>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
		if (list.RequiresLoad)
			list.Load();

		list.Add(999);

		Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveRange(1, 1));
		Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveRange(-1, 1));
		Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveRange(0, -1));

		Assert.DoesNotThrow(() => list.RemoveRange(0, 0));
	}

	[Test]
	public void IndexOf([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		var random = new Random(31337);
		string[] inputs = Enumerable.Range(1, 10).Select(x => random.NextString(1, 100)).ToArray();
		using var stream = new MemoryStream();
		using var list = StreamMappedFactory.CreateList<string>(stream, 32, new StringSerializer(Encoding.UTF8), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<string>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
		if (list.RequiresLoad)
			list.Load();
		list.AddRange(inputs);

		IEnumerable<long> indexes = list.IndexOfRange(inputs[..5]);

		Assert.AreEqual(new[] { 0, 1, 2, 3, 4 }, indexes);
	}

	[Test]
	public void IndexOf_Bug_1() {
		var random = new Random(31337);
		string[] inputs = Enumerable.Range(1, 10).Select(x => random.NextString(1, 100)).ToArray();
		using var stream = new MemoryStream();
		using var list = StreamMappedFactory.CreateList<string>(stream, 32, new StringSerializer(Encoding.UTF8),  itemChecksummer: new ObjectHashCodeChecksummer<string>(), reservedStreams: 1, autoLoad: true);
		list.Add("1");
		var index = list.IndexOf("1");
		Assert.That(index, Is.EqualTo(0));
	}

	[Test]
	public void IndexOfInvalidArguments([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		using var stream = new MemoryStream();
		using var list = StreamMappedFactory.CreateList<int>(stream, 1, new PrimitiveSerializer<int>(), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<int>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
		if (list.RequiresLoad)
			list.Load();
		list.AddRange(999, 1000, 1001, 1002);

		Assert.Throws<ArgumentNullException>(() => list.IndexOfRange(null));
		Assert.DoesNotThrow(() => list.IndexOfRange(Array.Empty<int>()));
	}

	[Test]
	public void Count([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		var random = new Random(31337);
		string[] inputs = Enumerable.Range(0, random.Next(1, 100)).Select(x => random.NextString(1, 100)).ToArray();
		using var stream = new MemoryStream();
		using var list = StreamMappedFactory.CreateList<string>(stream, 32, new StringSerializer(Encoding.UTF8), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<string>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
		if (list.RequiresLoad)
			list.Load();
		Assert.AreEqual(0, list.Count);
		list.AddRange(inputs);

		Assert.AreEqual(inputs.Length, list.Count);
	}

	[Test]
	public void InsertRange([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		var random = new Random(31337);
		string[] inputs = Enumerable.Range(1, random.Next(1, 5)).Select(x => random.NextString(1, 5)).ToArray();
		using var stream = new MemoryStream();
		using var list = StreamMappedFactory.CreateList<string>(stream, 32, new StringSerializer(Encoding.UTF8), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<string>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
		if (list.RequiresLoad)
			list.Load();

		list.AddRange(inputs);
		list.InsertRange(0, new[] { random.NextString(1, 100) });

		Assert.AreEqual(inputs.Length + 1, list.Count);
		Assert.AreEqual(inputs[0], list[1]);
	}

	[Test]
	public void InsertRangeInvalidArguments([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		using var stream = new MemoryStream();
		using var list = StreamMappedFactory.CreateList<int>(stream, 1, new PrimitiveSerializer<int>(), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<int>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
		if (list.RequiresLoad)
			list.Load();

		list.AddRange(999, 1000, 1001, 1002);

		Assert.Throws<ArgumentNullException>(() => list.InsertRange(0, null));
		Assert.Throws<ArgumentOutOfRangeException>(() => list.InsertRange(5, new int[] { 1 }));
		Assert.Throws<ArgumentOutOfRangeException>(() => list.InsertRange(-1, new int[] { 1 }));
		Assert.DoesNotThrow(() => list.InsertRange(0, Array.Empty<int>()));
	}

	[Test]
	public void Clear([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		var random = new Random(31337);
		string[] inputs = Enumerable.Range(0, random.Next(1, 100)).Select(x => random.NextString(1, 100)).ToArray();
		using var stream = new MemoryStream();
		using var list = StreamMappedFactory.CreateList<string>(stream, 32, new StringSerializer(Encoding.UTF8), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<string>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
		if (list.RequiresLoad)
			list.Load();
		list.AddRange(inputs);
		list.Clear();
		Assert.AreEqual(0, list.Count);

		list.AddRange(inputs);
		Assert.AreEqual(inputs, list);
	}

	[Test]
	public void IntegrationTests_String([StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		using var stream = new MemoryStream();
		using var list = StreamMappedFactory.CreateList<string>(stream, 32, new StringSerializer(Encoding.UTF8), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<string>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
		if (list.RequiresLoad)
			list.Load();

		AssertEx.ListIntegrationTest(list,
			100,
			(rng, i) => Enumerable.Range(0, i)
				.Select(x => rng.NextString(1, 100))
				.ToArray());
	}

	[Test]
	public void LoadAndUseExistingStream([Values(1, 100)] int iterations, [StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		var random = new Random(31337 + iterations);
		var input = Enumerable.Range(0, random.Next(1, 100)).Select(x => random.NextString(0, 100)).ToArray();
		var fileName = Tools.FileSystem.GetTempFileName(true);
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
			using (var fileStream = new FileStream(fileName, FileMode.Open)) {
				using var list = StreamMappedFactory.CreateList<string>(fileStream, 32, new StringSerializer(Encoding.UTF8), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<string>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
				if (list.RequiresLoad)
					list.Load();
				list.AddRange(input);
			}

			using (var fileStream = new FileStream(fileName, FileMode.Open)) {
				using var list = StreamMappedFactory.CreateList<string>(fileStream, 32, new StringSerializer(Encoding.UTF8), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<string>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
				if (list.RequiresLoad)
					list.Load();
				Assert.AreEqual(input.Length, list.Count);
				Assert.AreEqual(input, list);

				var secondInput = Enumerable.Range(0, random.Next(1, 100)).Select(x => random.NextString(1, 100)).ToArray();
				list.AddRange(secondInput);
				Assert.AreEqual(input.Concat(secondInput), list.ReadRange(0, list.Count));
				list.RemoveRange(0, list.Count);
				Assert.IsEmpty(list);
			}
		}
	}

	[Test]
	public void BugCase([Values(100)] int iterations, [Values(StreamContainerPolicy.Default)] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		var random = new Random(31337 + iterations);
		var input = Enumerable.Range(0, random.Next(1, 100)).Select(x => random.NextString(0, 100)).ToArray();
		var fileName = Tools.FileSystem.GetTempFileName(true);
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
			using (var fileStream = new FileStream(fileName, FileMode.Open)) {
				using var list = StreamMappedFactory.CreateList<string>(fileStream, 32, new StringSerializer(Encoding.UTF8), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<string>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy, autoLoad: true);
				list.AddRange(input);
				var secondInput = Enumerable.Range(0, random.Next(1, 100)).Select(x => random.NextString(1, 100)).ToArray();
				list.AddRange(secondInput);
				list.RemoveRange(0, list.Count);
			}
		}
	}

	[Test]
	public void IntegrationTests([StreamContainerStorageTypeValues] StorageType storage, [StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values] bool useChecksumIndex) {
		var rng = new Random(31337);
		using (CreateStream(storage, 5000, out Stream stream)) {
			using var list = StreamMappedFactory.CreateList<TestObject>(stream, 100, new TestObjectSerializer(), new TestObjectEqualityComparer(), itemChecksummer: useChecksumIndex ? new ObjectHashCodeChecksummer<TestObject>() : null, reservedStreams: useChecksumIndex ? 1 : 0, policy: policy);
			if (list.RequiresLoad)
				list.Load();

			AssertEx.ListIntegrationTest(list,
				100,
				(rng, i) => Enumerable.Range(0, i).Select(x => new TestObject(rng)).ToArray(),
				false,
				10,
				itemComparer: new TestObjectEqualityComparer()
			);
		}
	}

}
