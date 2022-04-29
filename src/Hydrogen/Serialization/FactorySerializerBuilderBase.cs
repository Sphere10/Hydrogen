using System;

namespace Hydrogen {
    public class FactorySerializerBuilderBase<TBase, TFactorySerializerBuilder> where TFactorySerializerBuilder : FactorySerializerBuilderBase<TBase, TFactorySerializerBuilder> {

		public FactorySerializerBuilderBase(IFactorySerializer<TBase> serializer) {
			Serializer = serializer;
		}

		public IFactorySerializer<TBase> Serializer { get; protected set; }
		
		protected ushort TypeCode { get; private set; }

		public SerializerBuilder<TConcrete> For<TConcrete>() where TConcrete : TBase 
			=> For<TConcrete>(Serializer.GenerateTypeCode());

		public SerializerBuilder<TConcrete> For<TConcrete>(Enum value) where TConcrete : TBase 
			=> For<TConcrete>(Convert.ToUInt16(value));


		public SerializerBuilder<TConcrete> For<TConcrete>(ushort typeCode) where TConcrete : TBase {
			TypeCode = typeCode;
			return new(this as TFactorySerializerBuilder);
		}

		public class SerializerBuilder<TConcrete>
			where TConcrete : TBase {
			private readonly TFactorySerializerBuilder _parentBuilder;

			public SerializerBuilder(TFactorySerializerBuilder parentBuilder) {
				_parentBuilder = parentBuilder;
			}

			public TFactorySerializerBuilder SerializeWith(IItemSerializer<TConcrete> serializer) {
				_parentBuilder.Serializer.RegisterSerializer(_parentBuilder.TypeCode, serializer);
				return _parentBuilder;
			}
		}
	}
}
