using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Hydrogen;


/// <summary>
/// A container that stores objects in a stream using a <see cref="StreamContainer"/>. This can also maintain
/// object metadata such as indexes, timestamps, merkle-trees, etc. This class is used by collections which
/// store their items in a stream.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObjectContainer<T> : ObjectContainer {
	private readonly ProjectionMemoizer _eventHandlerProjections = new ();

	public new event EventHandlerEx<object, long, T, ListOperationType> PreItemOperation {
		add => base.PreItemOperation += _eventHandlerProjections.RememberProjection<EventHandlerEx<object, long, T, ListOperationType>, EventHandlerEx<object, long, object, ListOperationType>>(value, handler => (arg1, arg2, arg3, arg4) => handler(arg1, arg2, (T)arg3, arg4));
		remove => base.PreItemOperation -= _eventHandlerProjections.ForgetProjection<EventHandlerEx<object, long, T, ListOperationType>, EventHandlerEx<object, long, object, ListOperationType>>(value);
	}
	
	public new event EventHandlerEx<object, long, T, ListOperationType> PostItemOperation {
		add => base.PostItemOperation += _eventHandlerProjections.RememberProjection<EventHandlerEx<object, long, T, ListOperationType>, EventHandlerEx<object, long, object, ListOperationType>>(value, handler => (arg1, arg2, arg3, arg4) => handler(arg1, arg2, (T)arg3, arg4));
		remove => base.PostItemOperation -= _eventHandlerProjections.ForgetProjection<EventHandlerEx<object, long, T, ListOperationType>, EventHandlerEx<object, long, object, ListOperationType>>(value);
	}

	public ObjectContainer(StreamContainer streamContainer, IItemSerializer<T> serializer = null, bool preAllocateOptimization = true, bool autoLoad = false) 
		: base(typeof(T), streamContainer, (serializer ?? ItemSerializer<T>.Default).AsPacked(), preAllocateOptimization, autoLoad) {
	}

	public void SaveItem(long index, T item, ListOperationType operationType) => SaveItem(index, item as object, operationType);

	public new T LoadItem(long index) => (T)base.LoadItem(index);

	internal ClusteredStream SaveItemAndReturnStream(long index, T item, ListOperationType operationType) 
		=> SaveItemAndReturnStream(index, item as object, operationType);

	internal new ClusteredStream LoadItemAndReturnStream(long index, out T item)  {
		 var result = base.LoadItemAndReturnStream(index, out var obj);
		item = (T)obj;
		return result;
	}
}