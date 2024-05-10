// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hydrogen;

/// <summary>
/// Efficiently serializes a collection of types, grouping them by assembly. 
/// </summary>
/// <remarks>Serialized and deserialized order are not the same</remarks>
public sealed class TypeCollectionSerializer : ItemSerializerBase<IEnumerable<Type>> {

	private readonly IItemSerializer<IEnumerable<IGrouping<Assembly, Type>>> _groupedTypesSerializer;
	private readonly IItemSerializer<IEnumerable<Type>> _crossAssemblyTypesSerializer;

	public TypeCollectionSerializer() 
		: this(SizeDescriptorStrategy.UseCVarInt) {
	}

	public TypeCollectionSerializer(SizeDescriptorStrategy sizeDescriptorStrategy) {
		_groupedTypesSerializer = new EnumerableSerializer<IGrouping<Assembly, Type>>(new AssemblyTypeGroupingSerializer(sizeDescriptorStrategy), sizeDescriptorStrategy);
		_crossAssemblyTypesSerializer = new EnumerableSerializer<Type>(new TypeSerializer(sizeDescriptorStrategy), sizeDescriptorStrategy);
	}

	public override long CalculateSize(SerializationContext context, IEnumerable<Type> item) {
		var (crossAssemblyTypes, singleAssemblyTypes) = item.SelectInto(x => x.IsCrossAssemblyType());
		return _groupedTypesSerializer.CalculateSize(context, singleAssemblyTypes) + _crossAssemblyTypesSerializer.CalculateSize(context, crossAssemblyTypes);
	}

	public override void Serialize(IEnumerable<Type> item, EndianBinaryWriter writer, SerializationContext context) {
		var (crossAssemblyTypes, singleAssemblyTypes) = item.SelectInto(x => x.IsCrossAssemblyType());
		_groupedTypesSerializer.Serialize(singleAssemblyTypes.GroupBy(x => x.Assembly), writer, context);
		_crossAssemblyTypesSerializer.Serialize(crossAssemblyTypes, writer, context);
	}

	public override IEnumerable<Type> Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var groupAssemblyTypes = _groupedTypesSerializer.Deserialize(reader, context);
		var crossAssemblyTypes = _crossAssemblyTypesSerializer.Deserialize(reader, context);
		return groupAssemblyTypes.SelectMany(x => x).Concat(crossAssemblyTypes);
	}

	private class AssemblyTypeGroupingSerializer : ItemSerializerBase<IGrouping<Assembly, Type>> {
		private readonly AssemblySerializer _assemblySerializer;
		private readonly SizeDescriptorStrategy _sizeStrategy;

		public AssemblyTypeGroupingSerializer(SizeDescriptorStrategy sizeDescriptorStrategy) {
			_assemblySerializer = new AssemblySerializer(sizeDescriptorStrategy);
			_sizeStrategy = sizeDescriptorStrategy;
		}

		public override long CalculateSize(SerializationContext context, IGrouping<Assembly, Type> item) {
			var assembly = item.Key;
			var size = _assemblySerializer.CalculateSize(context, assembly);
			var typesSerializer = new EnumerableSerializer<Type>(new AssemblyTypeSerializer(assembly, _sizeStrategy), _sizeStrategy);
			return size + typesSerializer.CalculateSize(context, item);
		}

		public override void Serialize(IGrouping<Assembly, Type> item, EndianBinaryWriter writer, SerializationContext context) {
			_assemblySerializer.Serialize(item.Key, writer, context);
			var typesSerializer = new EnumerableSerializer<Type>(new AssemblyTypeSerializer(item.Key, _sizeStrategy), _sizeStrategy);
			typesSerializer.Serialize(item, writer, context);
		}

		public override IGrouping<Assembly, Type> Deserialize(EndianBinaryReader reader, SerializationContext context) {
			var assembly = _assemblySerializer.Deserialize(reader, context);
			var typesSerializer = new EnumerableSerializer<Type>(new AssemblyTypeSerializer(assembly, _sizeStrategy), _sizeStrategy);
			var types = typesSerializer.Deserialize(reader, context);
			return new Grouping<Assembly, Type>(assembly, types);
		}
	}



}
