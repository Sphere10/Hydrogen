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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Sphere10.Framework {

	public static class StreamExtensions {

		public static byte[] ReadBytes(this Stream stream, int count) {
			var result = new byte[count];
			var bytesRead = stream.Read(result, 0, count);
			if (bytesRead != result.Length)
				Array.Resize(ref result, bytesRead);
			return result;
		}

		public static void WriteBytes(this Stream stream, byte[] buffer) {
			Guard.ArgumentNotNull(buffer, nameof(buffer));
			stream.Write(buffer, 0, buffer.Length);
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

		public static StreamDecorator OnDispose(this Stream stream, Action action) {
			return new OnDisposeStream(stream, action);
		}

		public static StreamDecorator OnDispose(this Stream stream, Action<Stream> action) {
			return new OnDisposeStream(stream, action);
		}

	}
}
