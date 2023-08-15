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

namespace Hydrogen;

public interface IClusteredStorage : ILoadable, ISynchronizedObject {

	event EventHandlerEx<ClusteredStreamRecord> RecordCreated;
	event EventHandlerEx<long, ClusteredStreamRecord> RecordAdded;
	event EventHandlerEx<long, ClusteredStreamRecord> RecordInserted;
	event EventHandlerEx<long, ClusteredStreamRecord> RecordUpdated;
	event EventHandlerEx<(long, ClusteredStreamRecord), (long, ClusteredStreamRecord)> RecordSwapped;
	event EventHandlerEx<long, long> RecordSizeChanged;
	event EventHandlerEx<long> RecordRemoved;

	Stream RootStream { get; }

	ClusteredStorageHeader Header { get; }

	ClusteredStoragePolicy Policy { get; }

	long Count { get; }

	IReadOnlyList<ClusteredStreamRecord> Records { get; }

	IClusterMap ClusterMap { get; }

	Endianness Endianness { get; }

	ClusteredStreamRecord GetRecord(long index);

	ClusteredStreamScope Add();

	ClusteredStreamScope OpenRead(long index);

	ClusteredStreamScope OpenWrite(long index);

	void Remove(long index);

	ClusteredStreamScope Insert(long index);

	void Swap(long first, long second);

	void Clear();

	ClusteredStreamScope EnterSaveItemScope<TItem>(long index, TItem item, IItemSerializer<TItem> serializer, ListOperationType operationType);

	ClusteredStreamScope EnterLoadItemScope<TItem>(long index, IItemSerializer<TItem> serializer, out TItem item);

}


public static class IClusteredStorageExtensions {

	public static bool IsNull(this IClusteredStorage storage, long index) {
		return storage.GetRecord(index).Traits.HasFlag(ClusteredStreamTraits.IsNull);
	}


	public static byte[] ReadAll(this IClusteredStorage storage, long index) {
		using var scope = storage.OpenRead(index);
		return scope.Stream.ReadAll();
	}

	public static void AddBytes(this IClusteredStorage storage, ReadOnlySpan<byte> bytes) {
		using var scope = storage.Add();
		scope.Stream.Write(bytes);
	}

	public static void UpdateBytes(this IClusteredStorage storage, long index, ReadOnlySpan<byte> bytes) {
		using var scope = storage.OpenWrite(index);
		scope.Stream.SetLength(0);
		scope.Stream.Write(bytes);
	}

	public static void AppendBytes(this IClusteredStorage storage, long index, ReadOnlySpan<byte> bytes) {
		using var scope = storage.OpenWrite(index);
		scope.Stream.Seek(scope.Stream.Length, SeekOrigin.Current);
		scope.Stream.Write(bytes);
	}

	public static void InsertBytes(this IClusteredStorage storage, long index, ReadOnlySpan<byte> bytes) {
		using var scope = storage.Insert(index);
		if (bytes != null) {
			scope.Stream.Seek(scope.Stream.Length, SeekOrigin.Current);
			scope.Stream.Write(bytes);
		}
	}

	public static void SaveItem<TItem>(this IClusteredStorage storage, long index, TItem item, IItemSerializer<TItem> serializer, ListOperationType operationType) {
		using var scope = storage.EnterSaveItemScope(index, item, serializer, operationType);
	}

	public static TItem LoadItem<TItem>(this IClusteredStorage storage, long index, IItemSerializer<TItem> serializer) {
		using var scope = storage.EnterLoadItemScope(index, serializer, out var item);
		return item;
	}
}
