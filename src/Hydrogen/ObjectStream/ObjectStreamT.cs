// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


using Hydrogen.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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

	public bool TryGetUniqueIndexFor<TMember>(Expression<Func<T, TMember>> memberExpression, out IUniqueKeyIndex<TMember> index) {
		throw new NotImplementedException();
		//if (Streams.TryFindAttachment<UniqueKeyChecksumIndex<T, TMember>>(out var uniqueKeyChecksumIndex)) {
		//	index = uniqueKeyChecksumIndex;
		//	return true;
		//}

		//if (Streams.TryFindAttachment<UniqueKeyIndex<T, TMember>>(out var uniqueKeyIndex)) {
		//	index = uniqueKeyIndex;
		//	return true;
		//}
		//index = default;
		//return false;
	}

	public IUniqueKeyIndex<TMember>  GetUniqueIndexFor<TMember>(Expression<Func<T, TMember>> memberExpression) {
		if (!TryGetUniqueIndexFor(memberExpression, out var index))  
			throw new InvalidOperationException($"No unique member index was found for {memberExpression.ToMember()}");

		return index;
	}
	
	public bool TryGetIndexFor<TMember>(Expression<Func<T, TMember>> memberExpression, out IKeyIndex<TMember> index) {
		throw new NotImplementedException();
		//if (Streams.TryFindAttachment<KeyChecksumIndex<T, TMember>>(out var keyChecksumIndex)) {
		//	index = keyChecksumIndex;
		//	return true;
		//}

		//if (Streams.TryFindAttachment<KeyIndex<T, TMember>>(out var keyIndex)) {
		//	index = keyIndex;
		//	return true;
		//}
		//index = default;
		//return false;
	}

	public IKeyIndex<TMember> GetIndexFor<TMember>(Expression<Func<T, TMember>> memberExpression) {
		if (!TryGetIndexFor(memberExpression, out var index))  
			throw new InvalidOperationException($"No member index was found for {memberExpression.ToMember()}");

		return index;
	}
	

	internal ClusteredStream SaveItemAndReturnStream(long index, T item, ObjectStreamOperationType operationType) 
		=> SaveItemAndReturnStream(index, item as object, operationType);

	internal new ClusteredStream LoadItemAndReturnStream(long index, out T item)  {
		 var result = base.LoadItemAndReturnStream(index, out var obj);
		item = (T)obj;
		return result;
	}
	
}