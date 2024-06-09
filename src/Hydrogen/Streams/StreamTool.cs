// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using Hydrogen;

// ReSharper disable CheckNamespace
namespace Tools;

public static class Streams {


	public static void ShiftBytes(Stream stream, long fromIndex, long count, long toIndex, int blockSize = HydrogenDefaults.DefaultBufferOperationBlockSize) {
		Guard.ArgumentNotNull(stream, nameof(stream));
		Guard.ArgumentGT(blockSize, 0, nameof(blockSize));
		if (count == 0)
			return;

		// SeekTo to the initial read position
		stream.Seek(fromIndex, SeekOrigin.Begin);

		// In cases where the copy/paste overlaps, isolate the overlapping region
		// TODO: this can result in memory bloat if overlap region is large (i.e. insert 1 byte at beginning of stream will result in overlap block the size of of entire stream)
		// The solution to this is complex but possible using the two-variable-XOR swap trick such that:
		// - when writing to overlap region, write the XOR'd value as follows (NEW VALUE XOR OLD VALUE)
		// - when reading an overlap region byte, the original value ios recovered by (NEW VALUE) XOR (NEW VALUE XOR OLD VALUE)
		// - the memory bloat complexity is traded-off with computational and seek complexity
		// - wrapping the above in a block-based approach would be ideal

		var rightOverlapSection = System.Array.Empty<byte>();
		if (toIndex >= fromIndex) {
			var fromEndIndex = fromIndex + count - 1;
			if (toIndex <= fromEndIndex) {
				var rightOverlapSectionLength = fromEndIndex - toIndex + 1;
				var rightOverLapSectionLengthI = Tools.Collection.CheckNotImplemented64bitAddressingLength(rightOverlapSectionLength);
				rightOverlapSection = stream.ReadBytes(rightOverLapSectionLengthI);
				if (rightOverlapSection.Length != rightOverlapSectionLength)
					throw new InternalErrorException("Unable to read overlapping section");
			}
		}

		// Shift the bytes in blocks read from left-to-right
		var readPosition = stream.Position;
		var writePosition = (long)toIndex + rightOverlapSection.Length;
		var block = new byte[blockSize];

		foreach (var nextBlockSize in Tools.Collection.Partition(count - rightOverlapSection.LongLength, blockSize)) {
			Debug.Assert(nextBlockSize <= blockSize);
			stream.Seek(readPosition, SeekOrigin.Begin);
			if (stream.Read(block, 0, unchecked((int)nextBlockSize)) != nextBlockSize)
				throw new InternalErrorException("Failed to read bytes from stream");
			readPosition = stream.Position;
			stream.Seek(writePosition, SeekOrigin.Begin);
			stream.Write(block, 0, unchecked((int)nextBlockSize));
			writePosition = stream.Position;
		}

		// Copy the isolated overlap block if required
		if (rightOverlapSection.Length > 0) {
			stream.Seek(toIndex, SeekOrigin.Begin);
			stream.WriteBytes(rightOverlapSection);
		}

	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RouteStream(Stream readStream, Stream writeStream, int blockSizeInBytes = HydrogenDefaults.DefaultBufferOperationBlockSize, bool closeReadStream = false, bool closeWriteStream = false) {
		Guard.Argument(writeStream != readStream, nameof(writeStream), "Cannot route to same stream");
		// Optimized for reading a stream of unknown length
		var buffer = new byte[blockSizeInBytes];
		int bytesRead;
		while ((bytesRead = readStream.Read(buffer, 0, buffer.Length)) > 0) {
			writeStream.Write(buffer, 0, bytesRead);
		}
		if (closeReadStream)
			readStream.Close();

		if (closeWriteStream)
			writeStream.Close();
	}

	public static void RouteStream(Stream readStream, Stream writeStream, long length, int blockSizeInBytes = HydrogenDefaults.DefaultBufferOperationBlockSize, bool closeReadStream = false, bool closeWriteStream = false) {
		Guard.Argument(writeStream != readStream, nameof(writeStream), "Cannot route to same stream");
		// Optimized for reading a known length of bytes
		var buffer = new byte[blockSizeInBytes];
		var reads = length / blockSizeInBytes;
		for (var i = 0; i < reads; i++) {
			var bytesRead = readStream.Read(buffer, 0, blockSizeInBytes);
			writeStream.Write(buffer, 0, bytesRead);
			length -= bytesRead;
			if (bytesRead < blockSizeInBytes || length == 0) {
				// encountered last block early
				length = 0;
				break;
			}
		}

		// left-over 
		if (length > 0) {
			var bytesRead = readStream.Read(buffer, 0, (int)length);
			writeStream.Write(buffer, 0, bytesRead);

		}

		if (closeReadStream)
			readStream.Close();

		if (closeWriteStream)
			writeStream.Close();
	}

	public static byte[] ReadByteArray(Stream stream, int blockSizeInBytes = HydrogenDefaults.DefaultBufferOperationBlockSize, bool closeStream = true) {
		using (var memoryStream = new MemoryStream()) {
			RouteStream(stream, memoryStream, blockSizeInBytes, closeStream, true);
			return memoryStream.ToArray();
		}
	}

	public static void WriteStreamToFile(Stream readStream, string filePath, FileMode fileMode = FileMode.Create, bool createDirectories = false, int blockSizeInBytes = HydrogenDefaults.DefaultBufferOperationBlockSize, bool closeReadStream = true) {

		#region Argument Validaton

		if (String.IsNullOrEmpty(filePath)) {
			throw new ArgumentException("Invalid file path", "filePath");
		}
		if (readStream == null) {
			throw new ArgumentNullException("readStream");
		}

		#endregion

		try {
			if (createDirectories) {
				var directory = Path.GetDirectoryName(filePath);
				if (!Directory.Exists(directory)) {
					Directory.CreateDirectory(directory);
				}
			}
			using (var writeStream = new FileStream(filePath, fileMode, FileAccess.Write)) {
				RouteStream(readStream, writeStream, blockSizeInBytes, closeReadStream, false);
			}
		} catch (Exception error) {
			throw new Exception("Failed to write file '{0}'".FormatWith(filePath), error);
		}
	}


	public static bool CompareFileStreams(FileStream fs1, FileStream fs2) {
		Debug.Assert(fs1 != null);
		Debug.Assert(fs2 != null);

		// compare lengths
		if (fs1.Length != fs2.Length) {
			return false;
		}

		return CompareStreams(fs1, fs2);
	}

	public static bool CompareStreams(Stream s1, Stream s2) {
		Debug.Assert(s1 != null);
		Debug.Assert(s2 != null);
		var BufferLength = 32768;

		// A buffer for each stream
		var buffer1 = new byte[BufferLength];
		var buffer2 = new byte[BufferLength];

		// Number of bytes valid within each buffer
		var buffer1Valid = 0;
		var buffer2Valid = 0;

		// Index within the buffer for each stream
		var buffer1Index = 0;
		var buffer2Index = 0;

		while (true) {
			// Read any more data if we need to
			if (buffer1Index == buffer1Valid) {
				buffer1Valid = s1.Read(buffer1, 0, BufferLength);
				buffer1Index = 0;
			}

			if (buffer2Index == buffer2Valid) {
				buffer2Valid = s2.Read(buffer2, 0, BufferLength);
				buffer2Index = 0;
			}

			// We've read to the end of both streams simultaneously
			if (buffer1Valid == 0 && buffer2Valid == 0) {
				return true;
			}

			// We've read to the end of one stream but not the other
			if (buffer1Valid == 0 || buffer2Valid == 0) {
				return false;
			}

			// compare each byte in buffer
			if (buffer1[buffer1Index] != buffer2[buffer2Index]) {
				return false;
			}

			buffer1Index++;
			buffer2Index++;
		}
	}

	public static void GZipCompress(Stream input, Stream output) {
		using (var compressor = new GZipStream(output, CompressionMode.Compress, true))
			RouteStream(input, compressor, blockSizeInBytes: HydrogenDefaults.OptimalCompressWriteBlockSize);
	}

	public static void GZipDecompress(Stream input, Stream output) {
		using (var decompressor = new GZipStream(input, CompressionMode.Decompress, true)) {
			// WARNING: do not seek to beginnning here! Client code responsible for that
			RouteStream(decompressor, output, blockSizeInBytes: HydrogenDefaults.OptimalCompressWriteBlockSize);
		}
	}


	public static void Encrypt<TSymmetricAlgorithm>(Stream input, Stream output, string password, byte[] salt = null, PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC)
		where TSymmetricAlgorithm : SymmetricAlgorithm, new() {
		Tools.Crypto.EncryptStream(input, output, Crypto.PrepareSymmetricAlgorithm<TSymmetricAlgorithm>(password, salt, paddingMode, cipherMode));
	}

	public static void Decrypt<TSymmetricAlgorithm>(Stream input, Stream output, string password, byte[] salt = null, PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC)
		where TSymmetricAlgorithm : SymmetricAlgorithm, new() {
		Tools.Crypto.DecryptStream(input, output, Crypto.PrepareSymmetricAlgorithm<TSymmetricAlgorithm>(password, salt, paddingMode, cipherMode));
	}
}
