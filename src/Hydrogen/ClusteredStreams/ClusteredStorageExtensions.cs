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

internal static class ClusteredStorageExtensions {

	public static bool IsNull(this ClusteredStorage storage, long index) {
		using var _ = storage.EnterAccessScope();
		return storage.GetRecord(index).Traits.HasFlag(ClusteredStreamTraits.IsNull);
	}


	public static byte[] ReadAll(this ClusteredStorage storage, long index) {
		using var _ = storage.EnterAccessScope();
		using var scope = storage.OpenWrite(index);
		return scope.Stream.ReadAll();
	}

	public static void AddBytes(this ClusteredStorage storage, ReadOnlySpan<byte> bytes) {
		using var _ = storage.EnterAccessScope();
		using var scope = storage.Add();
		scope.Stream.Write(bytes);
	}

	public static void UpdateBytes(this ClusteredStorage storage, long index, ReadOnlySpan<byte> bytes) {
		using var _ = storage.EnterAccessScope();
		using var scope = storage.OpenWrite(index);
		scope.Stream.SetLength(0);
		scope.Stream.Write(bytes);
	}

	public static void AppendBytes(this ClusteredStorage storage, long index, ReadOnlySpan<byte> bytes) {
		using var _ = storage.EnterAccessScope();
		using var scope = storage.OpenWrite(index);
		scope.Stream.Seek(scope.Stream.Length, SeekOrigin.Current);
		scope.Stream.Write(bytes);
	}

	public static void InsertBytes(this ClusteredStorage storage, long index, ReadOnlySpan<byte> bytes) {
		using var _ = storage.EnterAccessScope();
		using var scope = storage.Insert(index);
		if (bytes != null) {
			scope.Stream.Seek(scope.Stream.Length, SeekOrigin.Current);
			scope.Stream.Write(bytes);
		}
	}

	public static void SaveItem<TItem>(this ClusteredStorage storage, long index, TItem item, IItemSerializer<TItem> serializer, ListOperationType operationType) {
		using var _ = storage.EnterAccessScope();
		using var scope = storage.EnterSaveItemScope(index, item, serializer, operationType);
	}

	public static TItem LoadItem<TItem>(this ClusteredStorage storage, long index, IItemSerializer<TItem> serializer) {
		using var _ = storage.EnterAccessScope();
		using var scope = storage.EnterLoadItemScope(index, serializer, out var item);
		return item;
	}
}
