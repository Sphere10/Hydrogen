//-----------------------------------------------------------------------
// <copyright file="StreamTool.cs" company="Sphere 10 Software">
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO.Compression;
using Sphere10.Framework;

// ReSharper disable CheckNamespace
namespace Tools {


    public static class Streams {
        public const int DefaultBufferReadBlockSize = 32768;
        public const int OptimalCompressWriteBlockSize = 8192;

        public static void RouteStream(Stream readStream, Stream writeStream, int blockSizeInBytes = DefaultBufferReadBlockSize, bool closeReadStream = false, bool closeWriteStream = false) {
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


        public static byte[] ReadByteArray(Stream stream, int blockSizeInBytes = DefaultBufferReadBlockSize, bool closeStream = true) {
            using (var memoryStream = new MemoryStream()) {
                RouteStream(stream, memoryStream, blockSizeInBytes, closeStream, true);
                return memoryStream.ToArray();
            }
        }

        public static void WriteStreamToFile(Stream readStream, string filePath, FileMode fileMode = FileMode.Create, bool createDirectories = false, int blockSizeInBytes = DefaultBufferReadBlockSize, bool closeReadStream = true) {
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
                RouteStream(input, compressor, blockSizeInBytes: OptimalCompressWriteBlockSize);
        }

        public static void GZipDecompress(Stream input, Stream output) {
            using (var decompressor = new GZipStream(input, CompressionMode.Decompress, true)) {
                // WARNING: do not seek to beginnning here! Client code responsible for that
                RouteStream(decompressor, output, blockSizeInBytes: OptimalCompressWriteBlockSize);
            }
        }



        public static void Encrypt<TSymmetricAlgorithm>(Stream input, Stream output, string password, byte[] salt = null, PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC) where TSymmetricAlgorithm : SymmetricAlgorithm, new() {
            Tools.Crypto.EncryptStream(input, output, Crypto.PrepareSymmetricAlgorithm<TSymmetricAlgorithm>(password, salt, paddingMode, cipherMode));
        }

        public static void Decrypt<TSymmetricAlgorithm>(Stream input, Stream output, string password, byte[] salt = null, PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC) where TSymmetricAlgorithm : SymmetricAlgorithm, new() {
            Tools.Crypto.DecryptStream(input, output, Crypto.PrepareSymmetricAlgorithm<TSymmetricAlgorithm>(password, salt, paddingMode, cipherMode));
        }
    }
}
