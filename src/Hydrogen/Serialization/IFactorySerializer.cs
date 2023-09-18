//// Copyright (c) Sphere 10 Software 2018 - Present. All rights reserved. (https://sphere10.com) 
//// Author: Herman Schoenfeld  <herman@sphere.com>
////
//// Distributed under the MIT software license, see the accompanying file
//// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
////
//// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//using System;
//using System.Collections.Generic;

//namespace Hydrogen;

///// <summary>
///// A Serializer that works for base-level objects by delegating actual serialization to registered concrete-level serializers. 
///// </summary>
///// <typeparam name="TBase">The type of object which is serialized/deserialized</typeparam>
//public interface IFactorySerializer<TBase> : IItemSerializer<TBase> {

//	IEnumerable<Type> RegisteredTypes { get; }

//	//void RegisterSerializer<TConcrete>(long typeCode, IItemSerializer<TConcrete> concreteSerializer) where TConcrete : TBase;

//	void RegisterSerializer<TConcrete, TArg>(long typeCode, Func<TArg, IItemSerializer<TConcrete>> concreteSerializerFactory) where TConcrete : TBase;

//	//void RegisterSerializer<TConcrete, TArg1, TArg2>(long typeCode, Func<TArg1, TArg2, IItemSerializer<TConcrete>> concreteSerializerFactory) where TConcrete : TBase;

//	long GetTypeCode<TConcrete>(TConcrete item) where TConcrete : TBase => GetTypeCode(item.GetType());

//	long GetTypeCode(Type type);

//	long GenerateTypeCode();


//	//public void RegisterSerializer<TConcrete>(long typeCode, IItemSerializer<TConcrete> concreteSerializer) where TConcrete : TBase {
//	//	Guard.Argument(!_typeCodeMap.ContainsValue(typeCode), nameof(typeCode), $"Type code {typeCode} for type '{typeof(TConcrete).Name}' is already used for another serializer");
//	//	var concreteType = typeof(TConcrete);
//	//	Guard.Argument(!_typeCodeMap.ContainsKey(concreteType), nameof(TConcrete), "Type already registered");
//	//	_concreteLookup.Add(typeCode, concreteSerializer.AsProjection(x => (TBase)x, x => (TConcrete)x));
//	//}


//}

//public static class FactorySerializerExtensions {
//	public static void RegisterSerializer<TBase, TConcrete>(this IFactorySerializer<TBase> factorySerializer, IItemSerializer<TConcrete> concreteSerializer) where TConcrete : TBase
//		//=> factorySerializer.RegisterSerializer(factorySerializer.GenerateTypeCode(), concreteSerializer);
//		=> throw new NotImplementedException();

//}