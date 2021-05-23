using System;

namespace Sphere10.Framework {
	public class FactorySerializerBuilder<TBase> {
		private readonly FactorySerializer<TBase> _factorySerializer;

		public FactorySerializerBuilder(Func<TBase, uint> getTypeCode) {
			_factorySerializer = new FactorySerializer<TBase>(getTypeCode);
		}

		public FactorySerializerBuilder<TBase> For<TConcrete>(uint typeCode, IItemSerializer<TConcrete> concreteSerializer) where TConcrete : TBase {
			_factorySerializer.RegisterSerializer(typeCode, concreteSerializer);
			return this;
		}

		public FactorySerializer<TBase> Build() {
			return _factorySerializer;
		}
	}
}
