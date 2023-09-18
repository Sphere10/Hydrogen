// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


using System.Collections.Generic;

namespace Hydrogen;

/// <summary>
/// A container that stores objects in a stream using a <see cref="StreamContainer"/>. This can also maintain
/// object metadata such as indexes, timestamps, merkle-trees, etc. This is like a "table" within a "database".
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObjectContainer<T> : ObjectContainer {
	private readonly ProjectionMemoizer _eventHandlerProjections = new ();

	public new event EventHandlerEx<object, long, T, ObjectContainerOperationType> PreItemOperation {
		add => base.PreItemOperation += _eventHandlerProjections.RememberProjection<EventHandlerEx<object, long, T, ObjectContainerOperationType>, EventHandlerEx<object, long, object, ObjectContainerOperationType>>(value, handler => (arg1, arg2, arg3, arg4) => handler(arg1, arg2, (T)arg3, arg4));
		remove => base.PreItemOperation -= _eventHandlerProjections.ForgetProjection<EventHandlerEx<object, long, T, ObjectContainerOperationType>, EventHandlerEx<object, long, object, ObjectContainerOperationType>>(value);
	}
	
	public new event EventHandlerEx<object, long, T, ObjectContainerOperationType> PostItemOperation {
		add => base.PostItemOperation += _eventHandlerProjections.RememberProjection<EventHandlerEx<object, long, T, ObjectContainerOperationType>, EventHandlerEx<object, long, object, ObjectContainerOperationType>>(value, handler => (arg1, arg2, arg3, arg4) => handler(arg1, arg2, (T)arg3, arg4));
		remove => base.PostItemOperation -= _eventHandlerProjections.ForgetProjection<EventHandlerEx<object, long, T, ObjectContainerOperationType>, EventHandlerEx<object, long, object, ObjectContainerOperationType>>(value);
	}

	public ObjectContainer(StreamContainer streamContainer, IItemSerializer<T> serializer = null, bool preAllocateOptimization = true) 
		: base(typeof(T), streamContainer, (serializer ?? ItemSerializer<T>.Default).AsPacked(), preAllocateOptimization) {
	}

	public IItemSerializer<T> ItemSerializer => base.ItemSerializer.Unpack<T>();
	
	public void SaveItem(long index, T item, ObjectContainerOperationType operationType) => SaveItem(index, item as object, operationType);

	public new T LoadItem(long index) => (T)base.LoadItem(index);

	internal ClusteredStream SaveItemAndReturnStream(long index, T item, ObjectContainerOperationType operationType) 
		=> SaveItemAndReturnStream(index, item as object, operationType);

	internal new ClusteredStream LoadItemAndReturnStream(long index, out T item)  {
		 var result = base.LoadItemAndReturnStream(index, out var obj);
		item = (T)obj;
		return result;
	}
}