using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {

	public interface IStreamStorage { 
		int Count { get; }

		bool IsNull(int index);

		Stream Add();

		Stream Open(int index);

		void Remove(int index);

		Stream Insert(int index);

		void Swap(int first, int second);

		void Clear();

		void SaveItem<TItem>(int index, TItem item, IItemSerializer<TItem> serializer, ListOperationType operationType);

		TItem LoadItem<TItem>(int index, IItemSerializer<TItem> serializer);

	}

	public interface IStreamStorage<out THeader, TRecord> : IStreamStorage
		where THeader : IStreamStorageHeader
		where TRecord : IStreamRecord {

		THeader Header { get; }

		IReadOnlyList<TRecord> Records { get; }

		Stream Open(int index, out TRecord record);

		internal void UpdateRecord(int index, IStreamRecord record);

	}


	public static class IStreamStorageExtensions {
		public static byte[] ReadAll(this IStreamStorage storage, int index)
			=> storage.Open(index).ReadAllAndDispose();

		public static void AddBytes(this IStreamStorage storage, ReadOnlySpan<byte> bytes) {
			using var stream = storage.Add();
			stream.Write(bytes);
		}

		public static void UpdateBytes(this IStreamStorage storage, int index, ReadOnlySpan<byte> bytes) {
			using var stream = storage.Open(index);
			stream.SetLength(0);
			stream.Write(bytes);
		}

		public static void AppendBytes(this IStreamStorage storage, int index, ReadOnlySpan<byte> bytes) {
			using var stream = storage.Open(index);
			stream.Seek(stream.Length, SeekOrigin.Current);
			stream.Write(bytes);
		}

		public static void InsertBytes(this IStreamStorage storage, int index, ReadOnlySpan<byte> bytes) {
			using var stream = storage.Insert(index);
			if (bytes != null) {
				stream.Seek(stream.Length, SeekOrigin.Current);
				stream.Write(bytes);
			}
		}
	}


}
