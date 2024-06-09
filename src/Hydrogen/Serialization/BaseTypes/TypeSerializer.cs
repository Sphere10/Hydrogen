// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Text;

namespace Hydrogen;

public sealed class TypeSerializer : ItemSerializerBase<Type> {
	
	private readonly StringSerializer _fqnSerializer;
	
	public TypeSerializer(SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) {
		_fqnSerializer = new StringSerializer(Encoding.Unicode, sizeDescriptorStrategy);	
	}

	public static TypeSerializer Instance { get; } = new ();

	public override long CalculateSize(SerializationContext context, Type item) 
		=> _fqnSerializer.CalculateSize(item.AssemblyQualifiedName);

	public override void Serialize(Type item, EndianBinaryWriter writer, SerializationContext context) {
		_fqnSerializer.Serialize(item.AssemblyQualifiedName, writer, context);
	}

	public override Type Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var fqn = _fqnSerializer.Deserialize(reader, context);
		return Type.GetType(fqn);
	}
		
}
