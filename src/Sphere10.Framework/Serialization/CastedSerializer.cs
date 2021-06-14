using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// Converts a serializer for <see cref="TConcrete"/> to a serializer of it's base-type <see cref="TBase"/>
	/// </summary>
	/// <typeparam name="TBase">The type that this serializer will serialize</typeparam>
	/// <typeparam name="TConcrete">The concrete type of the supplied serializer which is converted</typeparam>
	public class CastedSerializer<TBase, TConcrete> : IItemSerializer<TBase> where TConcrete : TBase {
		private readonly IItemSerializer<TConcrete> _concreteSerializer;

		public CastedSerializer(IItemSerializer<TConcrete> concreteSerializer) {
			_concreteSerializer = concreteSerializer;
		}

		public bool IsFixedSize => _concreteSerializer.IsFixedSize;

		public int FixedSize => _concreteSerializer.FixedSize;

		public int CalculateTotalSize(IEnumerable<TBase> items, bool calculateIndividualItems, out int[] itemSizes) 
			=> _concreteSerializer.CalculateTotalSize(items.Cast<TConcrete>(), calculateIndividualItems, out itemSizes);

		public int CalculateSize(TBase item) => _concreteSerializer.CalculateSize((TConcrete)item);

		public int Serialize(TBase @object, EndianBinaryWriter writer) => _concreteSerializer.Serialize((TConcrete)@object, writer);

		public TBase Deserialize(int size, EndianBinaryReader reader) => _concreteSerializer.Deserialize(size, reader);
	}
}
