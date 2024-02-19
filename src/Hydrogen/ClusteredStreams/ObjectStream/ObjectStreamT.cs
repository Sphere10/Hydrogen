// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


namespace Hydrogen;

/// <summary>
/// A objectStream that stores objects in a stream using a <see cref="ClusteredStreams"/>. This can also maintain
/// object metadata such as indexes, timestamps, merkle-trees, etc. This is like a "table" within a "database".
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObjectStream<T> : ObjectStream {
	public ObjectStream(ClusteredStreams streams, IItemSerializer<T> serializer = null, bool preAllocateOptimization = true) 
		: base(typeof(T), streams, (serializer ?? ItemSerializer<T>.Default), preAllocateOptimization) {
	}

	public new IItemSerializer<T> ItemSerializer => (IItemSerializer<T>) base.ItemSerializer;
	
	public void SaveItem(long index, T item, ObjectStreamOperationType operationType) => SaveItem(index, item as object, operationType);

	public new T LoadItem(long index) => (T)base.LoadItem(index);

	internal ClusteredStream SaveItemAndReturnStream(long index, T item, ObjectStreamOperationType operationType) 
		=> SaveItemAndReturnStream(index, item as object, operationType);

	internal new ClusteredStream LoadItemAndReturnStream(long index, out T item)  {
		 var result = base.LoadItemAndReturnStream(index, out var obj);
		item = (T)obj;
		return result;
	}
	
}