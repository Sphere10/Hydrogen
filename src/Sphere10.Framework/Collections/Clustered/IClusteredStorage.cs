using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {

	public interface IClusteredStorage {

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

		void SaveItem<TItem>(int index, TItem item, IItemSerializer<TItem> serializer, ListOperationType operationType);

		TItem LoadItem<TItem>(int index, IItemSerializer<TItem> serializer);

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
	}


}
