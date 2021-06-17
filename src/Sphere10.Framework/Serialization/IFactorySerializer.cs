using System;
using System.Collections.Generic;

namespace Sphere10.Framework {
    /// <summary>
    /// A Serializer that works for base-level objects that delegates actual serialization to registered concrete-level serializers. 
    /// </summary>
    /// <typeparam name="TBase">The type of object which is serialized/deserialized</typeparam>
    public interface IFactorySerializer<TBase> : IItemSerializer<TBase> {

        IEnumerable<Type> RegisteredTypes { get; }

		public void RegisterSerializer<TConcrete>(ushort typeCode, IItemSerializer<TConcrete> concreteSerializer) where TConcrete : TBase;

        ushort GetTypeCode<TConcrete>(TConcrete item) where TConcrete : TBase => GetTypeCode(item.GetType());

        ushort GetTypeCode(Type type);

        ushort GenerateTypeCode();

    }
}
