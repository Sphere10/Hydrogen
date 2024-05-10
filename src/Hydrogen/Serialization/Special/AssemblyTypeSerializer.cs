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

public sealed class AssemblyTypeSerializer : ItemSerializerBase<Type> {
	private readonly Assembly _containingAssembly;
	private readonly StringSerializer _fqnSerializer;
	
	public AssemblyTypeSerializer(Assembly assembly, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) {
		_containingAssembly = assembly;
		_fqnSerializer = new StringSerializer(Encoding.Unicode, sizeDescriptorStrategy);
	}

	public override long CalculateSize(SerializationContext context, Type item) => _fqnSerializer.CalculateSize(GetName(item));

	public override void Serialize(Type item, EndianBinaryWriter writer, SerializationContext context) {
		_fqnSerializer.Serialize(GetName(item), writer, context);
	}

	public override Type Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var typeName = _fqnSerializer.Deserialize(reader, context);
		var type = _containingAssembly.GetType(typeName);
		Guard.Ensure(type is not null, $"Type '{typeName}' not found in assembly '{_containingAssembly.FullName}'");
		return type;
	}

	private string GetName(Type type) {
		Guard.Argument(type.Assembly == _containingAssembly, nameof(type), $"Type '{type.Name}' was not contained in assembly '{_containingAssembly.FullName}'");
		return type.FullName;
	}
		
}
