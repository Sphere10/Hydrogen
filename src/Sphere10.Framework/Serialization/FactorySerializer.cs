using System;
using System.Collections.Generic;

namespace Sphere10.Framework {
	/// <summary>
	/// A serializer for a base-class which delegates the actual serialization to registered serializers for concrete-classes. Serialization is prefixed with a TypeCode to determine which concrete class.
	/// </summary>
	/// <typeparam name="TBase"></typeparam>
	public class FactorySerializer<TBase> : ItemSerializerBase<TBase> {

		public IDictionary<uint, IItemSerializer<TBase>> ConcreteLookup { get; init; }

		public Func<TBase, uint> GetTypeCode { get; init; }

		public void RegisterSerializer<TConcrete>(uint typeCode, IItemSerializer<TConcrete> concreteSerializer) where TConcrete : TBase {
			ConcreteLookup.Add(typeCode, new CastedSerializer<TBase,TConcrete>(concreteSerializer));
		}

		public override int CalculateSize(TBase item)
			=> GetConcreteSerializer(item).CalculateSize(item);

		public override TBase Deserialize(int size, EndianBinaryReader reader) {
			// TODO: add checks here
			var typeCode = reader.ReadUInt32();
			return GetConcreteSerializer(typeCode).Deserialize(size, reader);
		}

		public override int Serialize(TBase item, EndianBinaryWriter writer) {
			var typeCode = GetTypeCode(item);
			writer.Write(typeCode);
			return GetConcreteSerializer(typeCode).Serialize(item, writer);
		}

		private IItemSerializer<TBase> GetConcreteSerializer(TBase item) 
			=> GetConcreteSerializer(GetTypeCode(item));
		

		private IItemSerializer<TBase> GetConcreteSerializer(uint typeCode) 
			=> ConcreteLookup[typeCode];
		
	}


}
