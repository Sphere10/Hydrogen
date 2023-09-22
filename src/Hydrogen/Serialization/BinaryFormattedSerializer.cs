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
public sealed class BinaryFormattedSerializer<TItem> : ItemSerializer<TItem> {

	public BinaryFormattedSerializer() 
		: base(SizeDescriptorStrategy.UseCVarInt) {
	}

	public override bool SupportsNull => true;

	public override long CalculateSize(TItem item) {
		var formatter = new BinaryFormatter();
		using var memoryStream = new MemoryStream();
		formatter.Serialize(memoryStream, item);
		var objectRawBytes = memoryStream.ToArray();
		return ByteArraySerializer.Instance.CalculateSize(objectRawBytes);
	}

	public override void SerializeInternal(TItem item, EndianBinaryWriter writer) {
		var formatter = new BinaryFormatter();
		using var memoryStream = new MemoryStream();
		formatter.Serialize(memoryStream, item);
		var objectRawBytes = memoryStream.ToArray();
		ByteArraySerializer.Instance.SerializeInternal(objectRawBytes, writer);
	}

	public override TItem DeserializeInternal(EndianBinaryReader reader) {
		var rawBytes = ByteArraySerializer.Instance.DeserializeInternal(reader);
		var formatter = new BinaryFormatter();
		using var memoryStream = new MemoryStream(rawBytes);
		return (TItem)formatter.Deserialize(memoryStream);
	}

}
