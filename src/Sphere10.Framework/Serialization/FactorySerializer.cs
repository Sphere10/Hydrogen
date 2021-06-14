using System;
using System.Collections.Generic;

namespace Sphere10.Framework {

    /// <summary>
    /// A Serializer that works for base-level objects that delegates actual serialization to registered concrete-level serializers. Serialization is wrapped with the
    /// type-code which permits selection of correct concrete type.
    /// </summary>
    /// <typeparam name="TBase">The type of object which is serialized/deserialized</typeparam>
    public class FactorySerializer<TBase> : ItemSerializerBase<TBase>, IFactorySerializer<TBase> {
		private readonly IDictionary<ushort, IItemSerializer<TBase>> _concreteLookup;
		private readonly IBijectiveDictionary<Type, ushort> _typeCodeMap;

        public FactorySerializer() {
			_concreteLookup = new Dictionary<ushort, IItemSerializer<TBase>>();
			_typeCodeMap = new BijectiveDictionary<Type, ushort>();
		}

		public IEnumerable<Type> RegisteredTypes => _typeCodeMap.Keys;

		public void RegisterSerializer<TConcrete>(IItemSerializer<TConcrete> concreteSerializer) where TConcrete : TBase 
			=> RegisterSerializer(GenerateTypeCode(), concreteSerializer);

		public void RegisterSerializer<TConcrete>(ushort typeCode, IItemSerializer<TConcrete> concreteSerializer) where TConcrete : TBase {
			Guard.Argument(!_typeCodeMap.ContainsValue(typeCode), nameof(typeCode), "Type code is already used for another serializer");
			var concreteType = typeof(TConcrete);
			Guard.Argument(!_typeCodeMap.ContainsKey(concreteType), nameof(TConcrete), "Type already registered");
			_concreteLookup.Add(typeCode, new CastedSerializer<TBase, TConcrete>(concreteSerializer));
			_typeCodeMap.Add(typeof(TConcrete), typeCode);
		}

		public override int CalculateSize(TBase item) => GetConcreteSerializer(item).CalculateSize(item);

		public override TBase Deserialize(int size, EndianBinaryReader reader) {
			// TODO: add checks here
			var typeCode = reader.ReadUInt16();
			return GetConcreteSerializer(typeCode).Deserialize(size, reader);
		}

		public override int Serialize(TBase item, EndianBinaryWriter writer) {
			var typeCode = GetTypeCode(item);
			writer.Write(typeCode);
			return GetConcreteSerializer(typeCode).Serialize(item, writer);
		}

		public ushort GetTypeCode<TConcrete>(TConcrete item) where TConcrete : TBase => GetTypeCode(item.GetType());

		public ushort GetTypeCode(Type type) => _typeCodeMap[type];

		public ushort GenerateTypeCode() => _typeCodeMap.Count > 0 ? (ushort)(_typeCodeMap.Values.MaxBy(x => x) + 1) : (ushort)0;

		private IItemSerializer<TBase> GetConcreteSerializer(TBase item) => GetConcreteSerializer(GetTypeCode(item));
		
		private IItemSerializer<TBase> GetConcreteSerializer(ushort typeCode) => _concreteLookup[typeCode];
		
	}

}
