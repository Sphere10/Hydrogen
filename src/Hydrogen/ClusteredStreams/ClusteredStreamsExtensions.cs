// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;

namespace Hydrogen;

internal static class ClusteredStreamsExtensions {

	//public static bool IsTraits(this StreamContainer streams, long index) {
	//	using var _ = streams.EnterAccessScope();
	//	return streams.GetStreamDescriptor(index).Traits.HasFlag(ClusteredStreamTraits.Null);
	//}

	//public static bool IsReaped(this StreamContainer streams, long index) {
	//	using var _ = streams.EnterAccessScope();
	//	return streams.GetStreamDescriptor(index).Traits.HasFlag(ClusteredStreamTraits.Reaped);
	//}

	/// <summary>
	/// Reads the entire contents of the stream at <paramref name="index"/> into memory.
	/// </summary>
	public static byte[] ReadAll(this ClusteredStreams streams, long index) {
		using var _ = streams.EnterAccessScope();
		using var stream = streams.OpenRead(index);
		if (stream.IsNull)
			return null;
		return stream.ReadAll();
	}

	/// <summary>
	/// Appends a new stream containing the supplied bytes.
	/// </summary>
	public static void AddBytes(this ClusteredStreams streams, ReadOnlySpan<byte> bytes) {
		using var _ = streams.EnterAccessScope();
		using var stream = streams.Add();
		stream.Write(bytes);
	}

	/// <summary>
	/// Replaces the contents of the stream at <paramref name="index"/> with <paramref name="bytes"/>.
	/// </summary>
	public static void UpdateBytes(this ClusteredStreams streams, long index, ReadOnlySpan<byte> bytes) {
		using var _ = streams.EnterAccessScope();
		using var stream = streams.OpenWrite(index);
		stream.SetLength(0);
		stream.Write(bytes);
	}

	/// <summary>
	/// Appends <paramref name="bytes"/> to the end of the stream at <paramref name="index"/>.
	/// </summary>
	public static void AppendBytes(this ClusteredStreams streams, long index, ReadOnlySpan<byte> bytes) {
		using var _ = streams.EnterAccessScope();
		using var stream = streams.OpenWrite(index);
		stream.Seek(stream.Length, SeekOrigin.Current);
		stream.Write(bytes);
	}

	/// <summary>
	/// Inserts a new stream at <paramref name="index"/> populated with <paramref name="bytes"/>.
	/// </summary>
	public static void InsertBytes(this ClusteredStreams streams, long index, ReadOnlySpan<byte> bytes) {
		using var _ = streams.EnterAccessScope();
		using var stream = streams.Insert(index);
		if (bytes != null) {
			stream.Seek(stream.Length, SeekOrigin.Current);
			stream.Write(bytes);
		}
	}

}
