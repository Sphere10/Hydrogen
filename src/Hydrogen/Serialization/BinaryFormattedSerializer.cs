// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Hydrogen;

/// <summary>
/// A serializer that uses the .NET <see cref="BinaryFormatter"/> under the hood.
/// </summary>
/// <remarks>Due to limitations of <see cref="BinaryFormatter"/> this class performs a serialization on <see cref="CalculateSize"/>.</remarks>
/// <typeparam name="TItem"></typeparam>
public sealed class BinaryFormattedSerializer<TItem> : IItemSerializer<TItem> {

	public bool SupportsNull => true;

	public bool IsConstantLength => false;

	public long ConstantLength => -1;

	public long CalculateTotalSize(IEnumerable<TItem> items, bool calculateIndividualItems, out long[] itemSizes) {
		var itemsArr = items as TItem[] ?? items.ToArray();

		if (calculateIndividualItems) {
			itemSizes = new long[itemsArr.Length];
			for (int i = 0; i < itemsArr.Length; i++) {
				itemSizes[i] = CalculateSize(itemsArr[i]);
			}
		}

		var sum = 0L;
		for (int i = 0; i < itemsArr.Length; i++) {
			sum += CalculateSize(itemsArr[i]);
		}
		itemSizes = Array.Empty<long>();
		return sum;
	}

	public long CalculateSize(TItem item) {
		var formatter = new BinaryFormatter();
		using var memoryStream = new MemoryStream();
		formatter.Serialize(memoryStream, item);
		var objectRawBytes = memoryStream.ToArray();
		return objectRawBytes.Length;
	}

	public void SerializeInternal(TItem item, EndianBinaryWriter writer) {
		var formatter = new BinaryFormatter();
		using var memoryStream = new MemoryStream();
		formatter.Serialize(memoryStream, item);
		var objectRawBytes = memoryStream.ToArray();
		writer.Write(objectRawBytes);
	}

	public TItem DeserializeInternal(long byteSize, EndianBinaryReader reader) {
		var bytes = reader.ReadBytes(byteSize);
		var formatter = new BinaryFormatter();
		using var memoryStream = new MemoryStream(bytes);
		var result = formatter.Deserialize(memoryStream);
		if (result is null)
			throw new InvalidOperationException("Unexpected null");
		if (result is not TItem t)
			throw new InvalidOperationException($"Unexpected type '{result.GetType().Name}'");

		return t;
	}

}
