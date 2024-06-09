// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Reflection;
using System.Text;

namespace Hydrogen;

public sealed class AssemblySerializer : ItemSerializerBase<Assembly> {
	
	private readonly StringSerializer _fqnSerializer;

	public AssemblySerializer(SizeDescriptorStrategy sizeDescriptorStrategy  = SizeDescriptorStrategy.UseCVarInt) {
		_fqnSerializer = new StringSerializer(Encoding.Unicode, sizeDescriptorStrategy);
	}

	public static AssemblySerializer Instance { get; } = new ();

	public override long CalculateSize(SerializationContext context, Assembly item) 
		=> _fqnSerializer.CalculateSize(item.FullName);

	public override void Serialize(Assembly item, EndianBinaryWriter writer, SerializationContext context) {
		_fqnSerializer.Serialize(item.FullName, writer, context);
	}

	public override Assembly Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var fqn = _fqnSerializer.Deserialize(reader, context);
		return Assembly.Load(fqn);
	}
		
}
