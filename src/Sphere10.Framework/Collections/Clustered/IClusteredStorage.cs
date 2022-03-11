using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {

	public interface IClusteredStorage {

		public event EventHandlerEx<ClusteredStorageRecord> RecordCreated;
		ClusteredStoragePolicy Policy { get; }

		ClusteredStorageHeader Header { get; }

		IReadOnlyList<ClusteredStorageRecord> Records { get; }

		int Count { get; }

		bool IsNull(int index);

		ClusteredStorageScope Add();

		ClusteredStorageScope Open(int index);

		void Remove(int index);

		ClusteredStorageScope Insert(int index);

		void Swap(int first, int second);

		void Clear();

		ClusteredStorageScope EnterSaveItemScope<TItem>(int index, TItem item, IItemSerializer<TItem> serializer, ListOperationType operationType);

		ClusteredStorageScope EnterLoadItemScope<TItem>(int index, IItemSerializer<TItem> serializer, out TItem item);

		ClusteredStorageRecord GetRecord(int index);

		void UpdateRecord(int index, ClusteredStorageRecord record);

	}

	public static class IClusteredStorageExtensions {
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
