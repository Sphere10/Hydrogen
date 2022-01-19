using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {

	public interface IStreamStorage { 
		int Count { get; }

		Stream Add();

		Stream Open(int index);

		void Remove(int index);

		Stream Insert(int index);

		void Swap(int first, int second);

		void Clear();

		public sealed byte[] ReadAll(int index)
			=> Open(index).ReadAllAndDispose();

		public sealed void AddBytes(ReadOnlySpan<byte> bytes) {
			using var stream = Add();
			stream.Write(bytes);
		}

		public sealed void UpdateBytes(int index, ReadOnlySpan<byte> bytes) {
			using var stream = Open(index);
			stream.SetLength(0);
			stream.Write(bytes);
		}

		public sealed void AppendBytes(int index, ReadOnlySpan<byte> bytes) {
			using var stream = Open(index);
			stream.Seek(stream.Length, SeekOrigin.Current);
			stream.Write(bytes);
		}

		public sealed void InsertBytes(int index, ReadOnlySpan<byte> bytes) {
			using var stream = Insert(index);
			if (bytes != null) {
				stream.Seek(stream.Length, SeekOrigin.Current);
				stream.Write(bytes);
			}
		}

	}

	public interface IStreamStorage<out THeader, TRecord> : IStreamStorage
		where THeader : IStreamStorageHeader
		where TRecord : IStreamRecord {

		THeader Header { get; }

		IReadOnlyList<TRecord> Records { get; }

		internal void UpdateRecord(int index, IStreamRecord record);

	}


}
