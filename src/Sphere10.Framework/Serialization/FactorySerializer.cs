using System;
using System.Collections.Generic;

namespace Sphere10.Framework {

	/// <summary>
	/// A Serializer that works for base-level objects that delegates actual serialization to registered concrete-level serializers. Serialization is wrapped with the
	/// type-code which permits selection of correct concrete type.
	/// </summary>
	/// <typeparam name="TBase">The type of object which is serialized/deserialized</typeparam>
	public class FactorySerializer<TBase> : IItemSerializer<TBase>, IFactorySerializer<TBase> {
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

		public bool IsFixedSize => false;
		public int FixedSize => -1;

		public int CalculateTotalSize(IEnumerable<TBase> items, bool calculateIndividualItems, out int[] itemSizes) {
			throw new NotImplementedException();
		}

		public int CalculateSize(TBase item) => GetConcreteSerializer(item).CalculateSize(item);

		public bool TrySerialize(TBase item, EndianBinaryWriter writer, out int bytesWritten) {
			try {
				var typeCode = GetTypeCode(item);
				writer.Write(typeCode);
				bytesWritten = GetConcreteSerializer(typeCode).Serialize(item, writer);
				return false;
			} catch (Exception) {
				bytesWritten = 0;
				return false;
			}
		}

		public bool TryDeserialize(int byteSize, EndianBinaryReader reader, out TBase item) {
			try {
				var typeCode = reader.ReadUInt16();
				item = GetConcreteSerializer(typeCode).Deserialize(byteSize, reader);
				return true;
			} catch (Exception) {
				item = default;
				return false;
			}
		}

		public ushort GetTypeCode<TConcrete>(TConcrete item) where TConcrete : TBase => GetTypeCode(item.GetType());

		public ushort GetTypeCode(Type type) => _typeCodeMap[type];

		public ushort GenerateTypeCode() => _typeCodeMap.Count > 0 ? (ushort)(_typeCodeMap.Values.MaxBy(x => x) + 1) : (ushort)0;

		private IItemSerializer<TBase> GetConcreteSerializer(TBase item) => GetConcreteSerializer(GetTypeCode(item));

		private IItemSerializer<TBase> GetConcreteSerializer(ushort typeCode) => _concreteLookup[typeCode];
	}
}
