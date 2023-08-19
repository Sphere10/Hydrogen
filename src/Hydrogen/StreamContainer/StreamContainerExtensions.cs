// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;

namespace Hydrogen;

internal static class StreamContainerExtensions {

	public static bool IsNull(this StreamContainer streams, long index) {
		using var _ = streams.EnterAccessScope();
		return streams.GetStreamDescriptor(index).Traits.HasFlag(ClusteredStreamTraits.Null);
	}

	public static byte[] ReadAll(this StreamContainer streams, long index) {
		using var _ = streams.EnterAccessScope();
		using var stream = streams.OpenWrite(index);
		return stream.ReadAll();
	}

	public static void AddBytes(this StreamContainer streams, ReadOnlySpan<byte> bytes) {
		using var _ = streams.EnterAccessScope();
		using var stream = streams.Add();
		stream.Write(bytes);
	}

	public static void UpdateBytes(this StreamContainer streams, long index, ReadOnlySpan<byte> bytes) {
		using var _ = streams.EnterAccessScope();
		using var stream = streams.OpenWrite(index);
		stream.SetLength(0);
		stream.Write(bytes);
	}

	public static void AppendBytes(this StreamContainer streams, long index, ReadOnlySpan<byte> bytes) {
		using var _ = streams.EnterAccessScope();
		using var stream = streams.OpenWrite(index);
		stream.Seek(stream.Length, SeekOrigin.Current);
		stream.Write(bytes);
	}

	public static void InsertBytes(this StreamContainer streams, long index, ReadOnlySpan<byte> bytes) {
		using var _ = streams.EnterAccessScope();
		using var stream = streams.Insert(index);
		if (bytes != null) {
			stream.Seek(stream.Length, SeekOrigin.Current);
			stream.Write(bytes);
		}
	}

	public static void SaveItem<TItem>(this StreamContainer streams, long index, TItem item, IItemSerializer<TItem> serializer, ListOperationType operationType, bool preAllocateSpaceOptimization) {
		using var _ = streams.EnterAccessScope();
		using var stream = streams.SaveItemAndReturnStream(index, item, serializer, operationType, preAllocateSpaceOptimization);
	}

	public static TItem LoadItem<TItem>(this StreamContainer streams, long index, IItemSerializer<TItem> serializer, bool preAllocateSpaceOptimization) {
		using var _ = streams.EnterAccessScope();
		using var stream = streams.LoadItemAndReturnStream(index, serializer, out var item, preAllocateSpaceOptimization);
		return item;
	}

	internal static ClusteredStream SaveItemAndReturnStream<TItem>(this StreamContainer streams, long index, TItem item, IItemSerializer<TItem> serializer, ListOperationType operationType, bool preAllocateSpaceOptimization) {
		// initialized and reentrancy checks done by one of below called methods
		var stream = operationType switch {
			ListOperationType.Add => streams.Add(),
			ListOperationType.Update => streams.OpenWrite(index),
			ListOperationType.Insert => streams.Insert(index),
			_ => throw new ArgumentException($@"List operation type '{operationType}' not supported", nameof(operationType)),
		};
		try {
			using var writer = new EndianBinaryWriter(EndianBitConverter.For(streams.Endianness), stream);
			if (item != null) {
				stream.IsNull = false;
				if (preAllocateSpaceOptimization) {
					// pre-setting the stream length before serialization improves performance since it avoids
					// re-allocating fragmented stream on individual properties of the serialized item
					var expectedSize = serializer.CalculateSize(item);
					stream.SetLength(expectedSize);
					serializer.Serialize(item, writer);
				} else {
					var byteLength = serializer.Serialize(item, writer);
					stream.SetLength(byteLength);
				}

			} else {
				stream.IsNull = true;
				stream.SetLength(0); // open descriptor will save when closed
			}
			return stream;
		} catch {
			// need to dispose explicitly if not returned
			stream.Dispose();
			throw;
		}
	}

	internal static ClusteredStream LoadItemAndReturnStream<TItem>(this StreamContainer streams, long index, IItemSerializer<TItem> serializer, out TItem item, bool preAllocateSpaceOptimization) {
		// initialized and reentrancy checks done by Open
		var stream = streams.OpenWrite(index);
		try {
			if (!stream.IsNull) {
				using var reader = new EndianBinaryReader(EndianBitConverter.For(streams.Endianness), stream);
				item = serializer.Deserialize(stream.Length, reader);
			} else item = default;
			return stream;
		} catch {
			// need to dispose explicitly if not returned
			stream.Dispose();
			throw;
		}
	}

}
