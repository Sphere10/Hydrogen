using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Hydrogen {

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

		public bool IsStaticSize => _concreteSerializer.IsStaticSize;

		public int StaticSize => _concreteSerializer.StaticSize;

		public int CalculateTotalSize(IEnumerable<TBase> items, bool calculateIndividualItems, out int[] itemSizes)
			=> _concreteSerializer.CalculateTotalSize(items.Cast<TConcrete>(), calculateIndividualItems, out itemSizes);

		public int CalculateSize(TBase item) => _concreteSerializer.CalculateSize((TConcrete)item);

		public bool TrySerialize(TBase item, EndianBinaryWriter writer, out int bytesWritten) 
			=> _concreteSerializer.TrySerialize((TConcrete)item, writer, out bytesWritten);

		public bool TryDeserialize(int byteSize, EndianBinaryReader reader, out TBase item) {
			var result =_concreteSerializer.TryDeserialize(byteSize, reader, out var concrete);
			item = concrete;
			return result;
		}
		
	}
}
