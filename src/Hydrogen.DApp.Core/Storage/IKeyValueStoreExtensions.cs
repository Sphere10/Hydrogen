// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Linq;


namespace Hydrogen.DApp.Core.Storage;

public static class IKeyValueStoreExtensions {

	public static void WriteStream<T>(this IKeyValueStore<T> kvs, T key, Stream contents) {
		using (var stream = kvs.OpenWrite(key)) {
			contents.CopyTo(stream);
		}
	}

	public static void AppendStream<T>(this IKeyValueStore<T> kvs, T key, Stream contents) {
		using (var stream = kvs.OpenWrite(key)) {
			stream.Seek(0, SeekOrigin.End);
			contents.CopyTo(stream);
		}
	}

	public static string ReadAllText<T>(this IKeyValueStore<T> kvs, T key) {
		return ReadAllText(kvs, key, System.Text.Encoding.UTF8);
	}

	public static string ReadAllText<T>(this IKeyValueStore<T> kvs, T key, System.Text.Encoding encoding) {
		using (var stream = kvs.OpenRead(key)) {
			return encoding.GetString(stream.ReadAll());
		}
	}

	public static void WriteAllText<T>(this IKeyValueStore<T> kvs, T key, string contents) {
		WriteAllText(kvs, key, contents, System.Text.Encoding.UTF8);
	}

	public static void WriteAllText<T>(this IKeyValueStore<T> kvs, T key, string contents, System.Text.Encoding encoding) {
		using (var stream = kvs.OpenWrite(key)) {
			stream.Write(encoding.GetBytes(contents));
		}
	}

	public static void AppendAllText<T>(this IKeyValueStore<T> kvs, T key, string contents) {
		AppendAllText(kvs, key, contents, System.Text.Encoding.UTF8);
	}

	public static void AppendAllText<T>(this IKeyValueStore<T> kvs, T key, string contents, System.Text.Encoding encoding) {
		AppendAllBytes(kvs, key, encoding.GetBytes(contents));
	}

	public static byte[] ReadAllBytes<T>(this IKeyValueStore<T> kvs, T key, int blockSize = 32768) {
		using (var stream = kvs.OpenRead(key)) {
			return stream.ReadAll(blockSize);
		}
	}

	public static void WriteAllBytes<T>(this IKeyValueStore<T> kvs, T key, byte[] bytes) {
		using (var stream = new MemoryStream(bytes)) {
			WriteStream(kvs, key, stream);
		}
	}

	public static void AppendAllBytes<T>(this IKeyValueStore<T> kvs, T key, byte[] bytes) {
		using (var stream = new MemoryStream(bytes)) {
			AppendStream(kvs, key, stream);
		}

	}

	public static string[] ReadAllLines<T>(this IKeyValueStore<T> kvs, T key) {
		return ReadAllLines(kvs, key, System.Text.Encoding.UTF8);
	}

	public static string[] ReadAllLines<T>(this IKeyValueStore<T> kvs, T key, System.Text.Encoding encoding) {
		return ReadAllText(kvs, key, encoding).Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
	}

	public static void WriteAllLines<T>(this IKeyValueStore<T> kvs, T key, string[] contents) {
		WriteAllLines(kvs, key, contents, System.Text.Encoding.UTF8);
	}

	public static void WriteAllLines<T>(this IKeyValueStore<T> kvs, T key, string[] contents, System.Text.Encoding encoding) {
		WriteAllText(kvs, key, contents.ToDelimittedString(Environment.NewLine), encoding);
	}

	public static void AppendAllLines<T>(this IKeyValueStore<T> kvs, T key, string[] contents) {
		AppendAllLines(kvs, key, contents, System.Text.Encoding.UTF8);
	}

	public static void AppendAllLines<T>(this IKeyValueStore<T> kvs, T key, string[] contents, System.Text.Encoding encoding) {
		AppendAllText(kvs, key, contents.ToDelimittedString(Environment.NewLine), encoding);
	}

}
