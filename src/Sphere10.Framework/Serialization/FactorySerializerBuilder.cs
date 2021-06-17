using System;

namespace Sphere10.Framework {

    public class FactorySerializerBuilder<TBase> : FactorySerializerBuilderBase<TBase, FactorySerializerBuilder<TBase>> {

		public FactorySerializerBuilder() : this(new FactorySerializer<TBase>()) {
        }

		internal FactorySerializerBuilder(IFactorySerializer<TBase> serializer) 
			: base(serializer) {
        }

		public IFactorySerializer<TBase> Build() {
			return Serializer;
		}

	}
}
