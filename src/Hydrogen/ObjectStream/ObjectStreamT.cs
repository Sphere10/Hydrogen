// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


using Hydrogen.Mapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

	public bool TryGetUniqueIndexFor<TMember>(Expression<Func<T, TMember>> memberExpression, out IUniqueProjectionIndex<TMember> index) 
		=> TryGetUniqueIndex(memberExpression.ToMember().Name, out index);

	public bool TryGetUniqueIndex<TMember>(string indexName, out IUniqueProjectionIndex<TMember> index) {
		if (!Streams.Attachments.TryGetValue(indexName, out var attachment)) {
			index = default;
			return false;
		}
		index = (IUniqueProjectionIndex<TMember>)attachment;
		return true;
	}

	public IUniqueProjectionIndex<TMember> GetUniqueIndexFor<TMember>(Expression<Func<T, TMember>> memberExpression) {
		if (!TryGetUniqueIndexFor(memberExpression, out var index))  
			throw new InvalidOperationException($"No unique index was found for {memberExpression.ToMember()}");

		return index;
	}


	public bool TryGetIndexFor<TMember>(Expression<Func<T, TMember>> memberExpression, out IProjectionIndex<TMember> index) 
		=> TryGetIndex(memberExpression.ToMember().Name, out index);

	public bool TryGetIndex<TMember>(string indexName, out IProjectionIndex<TMember> index) {
		if (!Streams.Attachments.TryGetValue(indexName, out var attachment)) {
			index = default;
			return false;
		}
		index = (IProjectionIndex<TMember>)attachment;
		return true;
	}

	public IProjectionIndex<TMember> GetIndexFor<TMember>(Expression<Func<T, TMember>> memberExpression) {
		if (!TryGetIndexFor(memberExpression, out var index))  
			throw new InvalidOperationException($"No index was found for {memberExpression.ToMember()}");

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