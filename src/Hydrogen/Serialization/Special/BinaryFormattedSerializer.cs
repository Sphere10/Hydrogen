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
public sealed class BinaryFormattedSerializer<TItem> : ItemSerializerBase<TItem> {

	public override bool SupportsNull => true;

	public override long CalculateSize(SerializationContext context, TItem item) {
		var formatter = new BinaryFormatter();
		using var memoryStream = new MemoryStream();
		formatter.Serialize(memoryStream, item);
		var objectRawBytes = memoryStream.ToArray();
		return ByteArraySerializer.Instance.CalculateSize(context, objectRawBytes);
	}

	public override void Serialize(TItem item, EndianBinaryWriter writer, SerializationContext context) {
		var formatter = new BinaryFormatter();
		using var memoryStream = new MemoryStream();
		formatter.Serialize(memoryStream, item);
		var objectRawBytes = memoryStream.ToArray();
		ByteArraySerializer.Instance.Serialize(objectRawBytes, writer, context);
	}

	public override TItem Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var rawBytes = ByteArraySerializer.Instance.Deserialize(reader, context);
		var formatter = new BinaryFormatter();
		using var memoryStream = new MemoryStream(rawBytes);
		return (TItem)formatter.Deserialize(memoryStream);
	}

}