// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Reflection;
using System.Text;

namespace Hydrogen;

public sealed class AssemblySerializer : ItemSerializerBase<Assembly> {
	
	private readonly StringSerializer _fqnSerializer;
	
	public AssemblySerializer(SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) {
		_fqnSerializer = new StringSerializer(Encoding.Unicode, sizeDescriptorStrategy);
	}

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
