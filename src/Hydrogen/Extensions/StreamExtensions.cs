// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Hydrogen;

public static class StreamExtensions {

	public static int PeekNextByte(this Stream stream) {
		Guard.Argument(stream.CanSeek, nameof(stream), "Stream must be seekable");
		var nextByte = stream.ReadByte(); // Read the next byte
		if (nextByte != -1) { // If the end of the stream has not been reached
			stream.Position--; // Set the position back
		}
		return nextByte;
	}

	public static IDisposable EnterRestorePositionScope(this Stream stream) => EnterRestorePositionSeek(stream, stream.Position, SeekOrigin.Begin);

	public static IDisposable EnterRestorePositionSeek(this Stream stream, long offset, SeekOrigin origin) {
		Guard.Argument(stream.CanSeek, nameof(stream), "Stream must be seekable");
		var position = stream.Position;
		stream.Seek(offset, origin);
		return new ActionDisposable(() => stream.Position = position);
	}

	public static byte[] ToArray(this Stream stream) {
		Guard.ArgumentNotNull(stream, nameof(stream));
		Guard.ArgumentLTE(stream.Length, int.MaxValue, nameof(string.Length));
		return stream.ReadAll((int)stream.Length);
	}

	public static bool CanRead(this Stream stream, int byteCount) =>
		stream.CanRead && (stream.Length - stream.Position) >= byteCount;

	public static byte[] ReadBytes(this Stream stream, long count) {
		var result = new byte[count];
		var countI = Tools.Collection.CheckNotImplemented64bitAddressingLength(count);
		var bytesRead = stream.Read(result, 0, countI);
		if (bytesRead != result.Length)
			Array.Resize(ref result, bytesRead);
		return result;
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte[] ReadBytes(this Stream stream, long position, int count) {
		stream.Seek(position, SeekOrigin.Begin);
		return stream.ReadBytes(count);
	}

	public static void WriteBytes(this Stream stream, byte[] buffer) {
		Guard.ArgumentNotNull(buffer, nameof(buffer));
		stream.Write(buffer, 0, buffer.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBytes(this Stream stream, long position, byte[] buffer) {
		stream.Seek(position, SeekOrigin.Begin);
		stream.WriteBytes(buffer);
	}

	public static byte[] ReadAll(this Stream stream, int blockSize = HydrogenDefaults.DefaultBufferOperationBlockSize) {
		// TODO: remove this seek resetting (anti-pattern)
		long originalPosition = 0L;
		if (stream.CanSeek) {
			originalPosition = stream.Position;
			stream.Seek(0L, SeekOrigin.Begin);
		}
		try {
			return Tools.Streams.ReadByteArray(stream, blockSize, false);
		} finally {
			if (stream.CanSeek) {
				stream.Position = originalPosition;
			}
		}
	}

	public static byte[] ReadAllAndDispose(this Stream stream, int blockSize = HydrogenDefaults.DefaultBufferOperationBlockSize) {
		try {
			return stream.ReadAll(blockSize);
		} finally {
			stream.Dispose();
		}
	}


	public static T RouteTo<T>(this Stream stream, T writeStream, int blockSizeInBytes = HydrogenDefaults.DefaultBufferOperationBlockSize) where T : Stream {
		long originalPosition = 0;
		if (stream.CanSeek) {
			originalPosition = stream.Position;
			stream.Position = 0;
		}
		try {
			Tools.Streams.RouteStream(stream, writeStream, blockSizeInBytes, false, false);
			return writeStream;
		} finally {
			if (stream.CanSeek) {
				stream.Position = originalPosition;
			}
		}

	}

	public static Stream WriteToFile(this Stream stream, string filepath, FileMode fileMode = FileMode.Create) {
		long originalPosition = 0;
		if (stream.CanSeek) {
			originalPosition = stream.Position;
			stream.Position = 0;
		}
		try {
			Tools.Streams.WriteStreamToFile(stream, filepath, fileMode, false, closeReadStream: false);
			return stream;
		} finally {
			if (stream.CanSeek) {
				stream.Position = originalPosition;
			}
		}
	}
}
