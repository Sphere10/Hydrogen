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
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class ParallelClusteredStreamsTests {

	[Test]
	public void OpenedStreamKeepsLockAfterAccessScopeClosed( [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, 3, policy: policy, autoLoad: true);
		var scope = streams.EnterAccessScope();
		var danglingStream = streams.Add();
		scope.Dispose();
		Assert.That(streams.IsLocked, Is.True);
		danglingStream.Dispose();
		Assert.That(streams.IsLocked, Is.False);
	}

	[Test]
	public void ParallelAdd_Then_ParallelRead_WithoutErrors([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy, [Values(0, 1, 11, 7777)] int itemCount) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);

		// add a bunch of strings in parallel
		Parallel.For(0, itemCount, i => { streams.AddBytes($"Hello World! - {i}".ToByteArray(Encoding.UTF8)); });

		// check total count is correct
		Assert.That(streams.Count, Is.EqualTo(itemCount));

		// read all the strings out in parallel
		var dictionary = new SynchronizedDictionary<int, string>();
		Parallel.For(0,
			itemCount,
			i => {
				var originalString = Encoding.UTF8.GetString(streams.ReadAll(i));
				var originalIndex = int.Parse(originalString.Substring(originalString.LastIndexOf('-') + 1));
				dictionary[originalIndex] = originalString;
			});


		// verify all strings
		for (var i = 0; i < itemCount; i++) {
			Assert.That(dictionary[i], Is.EqualTo($"Hello World! - {i}"));
		}
	}


	[Test]
	public void ParallelInsert_Then_ParallelRead_WithoutErrors([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy, [Values(0, 1, 11, 555)] int itemCount) {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);

		// add a bunch of strings in parallel
		Parallel.For(0L,
			itemCount,
			i => {
				var streamIndex = rng.Next(0, (int)streams.Count + 1);
				streams.InsertBytes(streamIndex, $"Hello World! - {i}".ToByteArray(Encoding.UTF8));
			});

		// check total count is correct
		Assert.That(streams.Count, Is.EqualTo(itemCount));

		// read all the strings out in parallel
		var dict = new System.Collections.Generic.Dictionary<int,int>();
		var dictionary = new SynchronizedDictionary<int, string>();
		Parallel.For(0,
			itemCount,
			i => {
				var originalString = Encoding.UTF8.GetString(streams.ReadAll(i));
				var originalIndex = int.Parse(originalString.Substring(originalString.LastIndexOf('-') + 1));
				dictionary[originalIndex] = originalString;
			});


		// verify all strings
		for (var i = 0; i < itemCount; i++) {
			Assert.That(dictionary[i], Is.EqualTo($"Hello World! - {i}"));
		}
	}


	[Test]
	public void ParallelRemove_Then_ParallelRead_WithoutErrors([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy, [Values(0, 1, 11, 333)] int itemCount) {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		var deleted = new SynchronizedList<bool>();

		// create initial values
		for (var i = 0; i < itemCount; i++)
			streams.AddBytes($"Hello World! - {i}".ToByteArray(Encoding.UTF8));

		// remove half in parallel 
		Parallel.For(0,
			itemCount,
			i => {
				if (i % 2 == 1) {
					// you can't track what's being deleted in this test due to race-conditions in the test
					// i.e. two threads go to delete item at index 0 same time, both think they're deleting same thing, they're not
					var streamIndex = 0; // rng.Next(0, streams.Count); 
					streams.Remove(streamIndex);
					deleted.Add(true);
				}
			});

		// check total count is correct
		Assert.That(streams.Count, Is.EqualTo(itemCount - deleted.Count));

		// read all the strings out in parallel
		var dictionary = new SynchronizedDictionary<int, string>();
		Parallel.For(0,
			streams.Count,
			i => {
				var originalString = Encoding.UTF8.GetString(streams.ReadAll(i));
				var originalIndex = int.Parse(originalString.Substring(originalString.LastIndexOf('-') + 1));
				dictionary[originalIndex] = originalString;
			});

		Assert.That(streams.Count, Is.EqualTo(itemCount - deleted.Count));

		// verify all strings
		foreach (var item in dictionary)
			Assert.That(item.Value.StartsWith($"Hello World! - {item.Key}"));

	}


	[Test]
	public void AllOps_WithoutErrors([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy, [Values(0, 1, 11, 444)] int itemCount) {
		// Note: insert at index 0 is O(N), so we do less of them
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);

		var updated = new SynchronizedList<int>();
		var deleted = new SynchronizedList<bool>();
		// insert a bunch of strings in parallel
		Parallel.For(0,
			itemCount,
			i => {

				if (i % 2 == 1)
					streams.InsertBytes(0, $"Hello World! - {i}".ToByteArray(Encoding.UTF8));
				else
					streams.AddBytes($"Hello World! - {i}".ToByteArray(Encoding.UTF8));

				if (streams.Count > 0) {
					// pick a random index to update or delete
					// you can't track what's being deleted in this test due to race-conditions in the test
					// i.e. two threads go to delete item at index 0 same time, both think they're deleting same thing, they're not
					var streamIndex = 0; //rng.Next(0, streams.Count); 
					var originalString = Encoding.UTF8.GetString(streams.ReadAll(streamIndex));
					var originalIndex = int.Parse(originalString.Substring(originalString.LastIndexOf('-') + 1));

					// if logical index was odd delete, if even or 0 update
					if (originalIndex % 2 == 1) {
						streams.Remove(0);
						deleted.Add(true);
					} else {
						updated.Add(originalIndex);
						streams.UpdateBytes(0, $"{originalString} - {originalIndex}".ToByteArray(Encoding.UTF8));
					}
				}

			});

		// read all the strings out in parallel
		var dictionary = new SynchronizedDictionary<int, string>();
		Parallel.For(0,
			streams.Count,
			i => {
				var originalString = Encoding.UTF8.GetString(streams.ReadAll(i));
				var originalIndex = int.Parse(originalString.Substring(originalString.LastIndexOf('-') + 1));
				dictionary[originalIndex] = originalString;
			});


		// check count
		Assert.That(streams.Count, Is.EqualTo(itemCount - deleted.Count));

		// verify all strings
		foreach (var item in dictionary) {
			Assert.That(item.Value.StartsWith($"Hello World! - {item.Key}"));
			Assert.That(item.Value.EndsWith($"- {item.Key}"));
		}

	}


}
