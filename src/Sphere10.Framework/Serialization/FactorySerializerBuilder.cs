using System;

namespace Sphere10.Framework {

    public class FactorySerializerBuilder<TBase> : FactorySerializerBuilderBase<TBase, FactorySerializerBuilder<TBase>> {

		public FactorySerializerBuilder() : this(new FactorySerializer<TBase>()) {
        }

		public FactorySerializerBuilder(IFactorySerializer<TBase> serializer) 
			: base(serializer) {
        }

		public IFactorySerializer<TBase> Build() {
			return Serializer;
		}

	}
}
