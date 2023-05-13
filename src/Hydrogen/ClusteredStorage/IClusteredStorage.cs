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

namespace Hydrogen {

	public interface IClusteredStorage {

		event EventHandlerEx<ClusteredStreamRecord> RecordCreated;
		event EventHandlerEx<int, ClusteredStreamRecord> RecordAdded;
		event EventHandlerEx<int, ClusteredStreamRecord> RecordInserted;
		event EventHandlerEx<int, ClusteredStreamRecord> RecordUpdated;
		event EventHandlerEx<int> RecordRemoved;

		ClusteredStorageHeader Header { get; }

		ClusteredStoragePolicy Policy { get; }

		int Count { get; }

		IReadOnlyList<ClusteredStreamRecord> Records { get; }

		Endianness Endianness { get; }

		ClusteredStreamRecord GetRecord(int index);


		ClusteredStreamScope Add();

		ClusteredStreamScope Open(int index);

		void Remove(int index);

		ClusteredStreamScope Insert(int index);

		void Swap(int first, int second);

		void Clear();

		ClusteredStreamScope EnterSaveItemScope<TItem>(int index, TItem item, IItemSerializer<TItem> serializer, ListOperationType operationType);

		ClusteredStreamScope EnterLoadItemScope<TItem>(int index, IItemSerializer<TItem> serializer, out TItem item);

	}

	public static class IClusteredStorageExtensions {

		public static bool IsNull(this IClusteredStorage storage, int index) 
			=> storage.GetRecord(index).Traits.HasFlag(ClusteredStreamTraits.IsNull);
		

		public static byte[] ReadAll(this IClusteredStorage storage, int index) {
			using var scope = storage.Open(index);
			return scope.Stream.ReadAll();
		}

		public static void AddBytes(this IClusteredStorage storage, ReadOnlySpan<byte> bytes) {
			using var scope = storage.Add();
			scope.Stream.Write(bytes);
		}

		public static void UpdateBytes(this IClusteredStorage storage, int index, ReadOnlySpan<byte> bytes) {
			using var scope = storage.Open(index);
			scope.Stream.SetLength(0);
			scope.Stream.Write(bytes);
		}

		public static void AppendBytes(this IClusteredStorage storage, int index, ReadOnlySpan<byte> bytes) {
			using var scope = storage.Open(index);
			scope.Stream.Seek(scope.Stream.Length, SeekOrigin.Current);
			scope.Stream.Write(bytes);
		}

		public static void InsertBytes(this IClusteredStorage storage, int index, ReadOnlySpan<byte> bytes) {
			using var scope = storage.Insert(index);
			if (bytes != null) {
				scope.Stream.Seek(scope.Stream.Length, SeekOrigin.Current);
				scope.Stream.Write(bytes);
			}
		}

		public static void SaveItem<TItem>(this IClusteredStorage storage, int index, TItem item, IItemSerializer<TItem> serializer, ListOperationType operationType) {
			using var scope = storage.EnterSaveItemScope(index, item, serializer, operationType);
		}

		public static TItem LoadItem<TItem>(this IClusteredStorage storage, int index, IItemSerializer<TItem> serializer) {
			using var scope = storage.EnterLoadItemScope(index, serializer, out var item);
			return item;
		}
	}

}
