// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.ObjectSpaces;

/// <summary>
///  Used to track instances of fetched objects within an <see cref="ObjectSpace"/>.
/// </summary>
/// <remarks>Not thread-safe by design</remarks>
internal class InstanceTracker {

	private readonly Dictionary<Type, BijectiveDictionary<long, object>> _objectsByType;
	private int _newInstances;

	public InstanceTracker() {
		_objectsByType = new Dictionary<Type, BijectiveDictionary<long, object>>(TypeEquivalenceComparer.Instance);
		_newInstances = 0;
	}


	public TItem Get<TItem>(long index) {
		if (!TryGet<TItem>(index, out var item))
			throw new InvalidOperationException($"No instance of {typeof(TItem).ToStringCS()} an index {index} was tracked");
		return item;
	}

	public bool TryGet<TItem>(long index, out TItem item) {
		if (TryGet(typeof(TItem), index, out var itemO)) {
			item = (TItem)itemO;
			return true;
		}
		item = default;
		return false;
	}

	public bool TryGet(Type itemType, long index, out object item) {
		if (!_objectsByType.TryGetValue(itemType, out var instances)) {
			item = default;
			return false;
		}

		if (!instances.TryGetValue(index, out item))
			return false;
		
		return true;
	}

	public IEnumerable<object> GetInstances() 
		=> _objectsByType.Values.SelectMany(instances => instances.Values);


	public IEnumerable<object> GetInstances(Type itemType) 
		=> _objectsByType.TryGetValue(itemType, out var instances) ? instances.Values : Array.Empty<object>();


	public int TrackNew(object item) {
		var newIndex = -(++_newInstances);
		Track(item, newIndex);
		return newIndex;
	}

	public void Track(object item, long index) {
		var itemType = item.GetType();
		
		if (!_objectsByType.TryGetValue(itemType, out var instances)) {
			instances = CreateInstanceDictionary();
			_objectsByType.Add(itemType, instances);
		}

		// check if the index is already in use
		if (instances.TryGetValue(index, out var _)) 
			throw new InvalidOperationException($"An instance of {itemType.ToStringCS()} with index {index} has already been tracked");

		// if the item is already tracked, update the index
		if (instances.Bijection.ContainsKey(item))
			instances.Bijection[item] = index;
		else
			// otherwise, add the item
			instances.Add(index, item);
	}

	public void Untrack(object item) {
		var itemType = item.GetType();
		
		if (!_objectsByType.TryGetValue(itemType, out var instances) || !instances.TryGetKey(item, out var index)) 
			throw new InvalidOperationException("Object instance was not tracked");

		instances.Remove(index);
		if (instances.Count == 0)
			_objectsByType.Remove(itemType);
	}

	public long GetIndexOf(object item) {
		if (!TryGetIndexOf(item, out var index))
			throw new InvalidOperationException($"Instance of {item.GetType().ToStringCS()} was not tracked");
		return index;
	}

	public bool TryGetIndexOf(object item, out long index) {
		var itemType = item.GetType();

		if (!_objectsByType.TryGetValue(itemType, out var instances)) {
			index = default;
			return false;
		}

		if (!instances.TryGetKey(item, out index)) 
			return false;

		return true;
	}

	public void Clear() {
		_objectsByType.Clear();
		_newInstances = 0;
	}

	private BijectiveDictionary<long, object> CreateInstanceDictionary() => new(EqualityComparer<long>.Default, ReferenceEqualityComparer.Instance);

}
