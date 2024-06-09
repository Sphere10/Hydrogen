// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
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
public class TaggedTypeCollectionSerializer<TTag> : ItemSerializerBase<IEnumerable<(Type, TTag)>> {
	private readonly IItemSerializer<IEnumerable<IGrouping<Assembly, (Type, TTag)>>> _groupedTypesSerializer;
	private readonly IItemSerializer<IEnumerable<(Type, TTag)>> _crossAssemblyTypesSerializer;

	public TaggedTypeCollectionSerializer(IItemSerializer<TTag> tagSerializer) 
		: this(tagSerializer, SizeDescriptorStrategy.UseCVarInt) {
	}
	
	public TaggedTypeCollectionSerializer(IItemSerializer<TTag> tagSerializer, SizeDescriptorStrategy sizeDescriptorStrategy) {
		_groupedTypesSerializer = new EnumerableSerializer<IGrouping<Assembly, (Type, TTag)>>(new AssemblyTypeGroupingSerializer(tagSerializer, sizeDescriptorStrategy), sizeDescriptorStrategy);
		_crossAssemblyTypesSerializer = new EnumerableSerializer<(Type, TTag)>(new ValueTupleSerializer<Type, TTag>(new TypeSerializer(sizeDescriptorStrategy), tagSerializer), sizeDescriptorStrategy);
	}

	public override long CalculateSize(SerializationContext context, IEnumerable<(Type, TTag)> item) {
		var (crossAssemblyTypes, singleAssemblyTypes) = item.SeparateBy(x => x.Item1.IsCrossAssemblyType());
		return 
			_groupedTypesSerializer.CalculateSize(context, singleAssemblyTypes.GroupBy(x => x.Item1.Assembly)) +
			_crossAssemblyTypesSerializer.CalculateSize(context, crossAssemblyTypes);
	}

	public override void Serialize(IEnumerable<(Type, TTag)> item, EndianBinaryWriter writer, SerializationContext context) {
		var (crossAssemblyTypes, singleAssemblyTypes) = item.SeparateBy(x => x.Item1.IsCrossAssemblyType());
		_groupedTypesSerializer.Serialize(singleAssemblyTypes.GroupBy(x => x.Item1.Assembly), writer, context);
		_crossAssemblyTypesSerializer.Serialize(crossAssemblyTypes, writer, context);
	}

	public override IEnumerable<(Type, TTag)> Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var groupAssemblyTypes = _groupedTypesSerializer.Deserialize(reader, context);
		var crossAssemblyTypes = _crossAssemblyTypesSerializer.Deserialize(reader, context);
		return groupAssemblyTypes.SelectMany(x => x).Concat(crossAssemblyTypes);
	}

	private class AssemblyTypeGroupingSerializer : ItemSerializerBase<IGrouping<Assembly, (Type, TTag)>> {
		private readonly IItemSerializer<TTag> _tagSerializer;
		private readonly AssemblySerializer _assemblySerializer;
		private readonly SizeDescriptorStrategy _sizeStrategy;

		public AssemblyTypeGroupingSerializer(IItemSerializer<TTag> tagSerializer, SizeDescriptorStrategy sizeDescriptorStrategy) {
			_tagSerializer = tagSerializer;
			_assemblySerializer = new AssemblySerializer(sizeDescriptorStrategy);
			_sizeStrategy = sizeDescriptorStrategy;
		}

		public override long CalculateSize(SerializationContext context, IGrouping<Assembly, (Type, TTag)> item) {
			var assembly = item.Key;
			var size = _assemblySerializer.CalculateSize(context, assembly);
			var typesSerializer = new EnumerableSerializer<(Type, TTag)>(new ValueTupleSerializer<Type, TTag>(new AssemblyTypeSerializer(assembly, _sizeStrategy), _tagSerializer), _sizeStrategy);
			return size + typesSerializer.CalculateSize(context, item);
		}

		public override void Serialize(IGrouping<Assembly, (Type, TTag)> item, EndianBinaryWriter writer, SerializationContext context) {
			_assemblySerializer.Serialize(item.Key, writer, context);
			var typesSerializer = new EnumerableSerializer<(Type, TTag)>(new ValueTupleSerializer<Type, TTag>(new AssemblyTypeSerializer(item.Key, _sizeStrategy), _tagSerializer), _sizeStrategy);
			typesSerializer.Serialize(item, writer, context);
		}

		public override IGrouping<Assembly, (Type, TTag)> Deserialize(EndianBinaryReader reader, SerializationContext context) {
			var assembly = _assemblySerializer.Deserialize(reader, context);
			var typesSerializer = new EnumerableSerializer<(Type, TTag)>(new ValueTupleSerializer<Type, TTag>(new AssemblyTypeSerializer(assembly, _sizeStrategy), _tagSerializer), _sizeStrategy);
			var types = typesSerializer.Deserialize(reader, context);
			return new Grouping<Assembly, (Type, TTag)>(assembly, types);
		}
	}

}

