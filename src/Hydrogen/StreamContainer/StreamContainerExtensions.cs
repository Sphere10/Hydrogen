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

}
