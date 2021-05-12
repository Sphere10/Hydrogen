namespace Sphere10.Framework {
	public class FactorySerializerBuilder<TBase> {
		private readonly FactorySerializer<TBase> _factorySerializer;

		public FactorySerializerBuilder() {
			_factorySerializer = new FactorySerializer<TBase>();
		}

		public void For<TConcrete>(uint typeCode, IItemSerializer<TConcrete> concreteSerializer) where TConcrete : TBase {
			_factorySerializer.RegisterSerializer(typeCode, concreteSerializer);
		}

		public FactorySerializer<TBase> Build() {
			return _factorySerializer;
		}
	}
}
