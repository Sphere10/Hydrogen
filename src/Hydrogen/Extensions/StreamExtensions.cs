//-----------------------------------------------------------------------
// <copyright file="StreamExtensions.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Hydrogen {

	public static class StreamExtensions {

		public static byte[] ToArray(this Stream stream) {
			Guard.ArgumentNotNull(stream, nameof(stream));
			Guard.ArgumentLTE(stream.Length, int.MaxValue, nameof(string.Length));
			return stream.ReadAll((int)stream.Length);
		}

		public static bool CanRead(this Stream stream, int byteCount) =>
			stream.CanRead && (stream.Length - stream.Position) >= byteCount;

		public static byte[] ReadBytes(this Stream stream, int count) {
			var result = new byte[count];
			var bytesRead = stream.Read(result, 0, count);
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

		public static byte[] ReadAll(this Stream stream, int blockSize = Tools.Streams.DefaultBufferReadBlockSize) {
			long originalPosition = 0;
			if (stream.CanSeek) {
				originalPosition = stream.Position;
				stream.Position = 0;
			}
			try {
				return Tools.Streams.ReadByteArray(stream, blockSize, false);
			} finally {
				if (stream.CanSeek) {
					stream.Position = originalPosition;
				}
			}
		}

		public static byte[] ReadAllAndDispose(this Stream stream, int blockSize = Tools.Streams.DefaultBufferReadBlockSize) {
			try {
				return stream.ReadAll(blockSize);
			} finally { 
				stream.Dispose();
			}
		}


		public static T RouteTo<T>(this Stream stream, T writeStream, int blockSizeInBytes = Tools.Streams.DefaultBufferReadBlockSize) where T : Stream {
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

		public static Stream OnDispose(this Stream stream, Action action) {
			return new OnDisposeStream(stream, action);
		}

		public static Stream OnDispose(this Stream stream, Action<Stream> action) {
			return new OnDisposeStream(stream, action);
		}

	}
}
