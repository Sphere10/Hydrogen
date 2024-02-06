// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Hydrogen.ObjectSpaces;

/// <summary>
///  Used to track instances of fetched objects within an <see cref="ObjectSpace"/>.
/// </summary>
/// <remarks>Not thread-safe by design</remarks>
internal class InstanceTracker {

	private readonly Dictionary<Type, BijectiveDictionary<long, object>> _objects;

	public InstanceTracker() {
		_objects = new Dictionary<Type, BijectiveDictionary<long, object>>(TypeEquivalenceComparer.Instance);
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
		if (!_objects.TryGetValue(itemType, out var instances)) {
			item = default;
			return false;
		}

		if (!instances.TryGetValue(index, out item))
			return false;
		
		return true;
	}


	public void Track(object item, long index) {
		var itemType = item.GetType();
		
		if (!_objects.TryGetValue(itemType, out var instances)) {
			instances = CreateInstanceDictionary();
			_objects.Add(itemType, instances);
		}

		if (instances.TryGetValue(index, out var _)) 
			throw new InvalidOperationException($"An instance of {itemType.ToStringCS()} with index {index} has already been tracked"); 

		instances.Add(index, item);
	}

	public void Untrack(object item) {
		var itemType = item.GetType();
		
		if (!_objects.TryGetValue(itemType, out var instances) || !instances.TryGetKey(item, out var index)) 
			throw new InvalidOperationException("Object instance was not tracked");

		instances.Remove(index);
		if (instances.Count == 0)
			_objects.Remove(itemType);
	}

	public long GetIndexOf(object item) {
		if (!TryGetIndexOf(item, out var index))
			throw new InvalidOperationException($"Instance of {item.GetType().ToStringCS()} was not tracked");
		return index;
	}

	public bool TryGetIndexOf(object item, out long index) {
		var itemType = item.GetType();

		if (!_objects.TryGetValue(itemType, out var instances)) {
			index = default;
			return false;
		}

		if (!instances.TryGetKey(item, out index)) 
			return false;

		return true;
	}

	public void Clear() {
		_objects.Clear();
	}

	private BijectiveDictionary<long, object> CreateInstanceDictionary() => new(EqualityComparer<long>.Default, ReferenceEqualityComparer.Instance);

}
