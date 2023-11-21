using System;
using System.Collections;

namespace Hydrogen;

public class ArrayListSerializer : ProjectedSerializer<object[], ArrayList> {


	public ArrayListSerializer(SerializerFactory factory) 
		: this (
			  new ArraySerializer<object>(new ObjectSerializer(factory)),
			  (objects) => new ArrayList(objects),
			  (arrayList) => arrayList.ToArray()
		) {
	}

	public ArrayListSerializer(
		IItemSerializer<object[]> sourceSerializer, 
		Func<object[], ArrayList> projection, 
		Func<ArrayList, object[]> inverseProjection) 
		: base(sourceSerializer, projection, inverseProjection) {
	}
}
