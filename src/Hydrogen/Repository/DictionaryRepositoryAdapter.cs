// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public class DictionaryRepositoryAdapter<TEntity, TIdentity, TDict> : SyncRepositoryBase<TEntity, TIdentity> where TDict : IDictionary<TIdentity, TEntity> {

	private readonly TDict _dictionary;
	private readonly Func<TEntity, TIdentity> _identityFunc;

	public DictionaryRepositoryAdapter(TDict dictionary, Func<TEntity, TIdentity> identityFunc) {
		_dictionary = dictionary;
		_identityFunc = identityFunc;
	}

	public override bool Contains(TIdentity identity)
		=> _dictionary.ContainsKey(identity);

	public override bool TryGet(TIdentity identity, out TEntity entity)
		=> _dictionary.TryGetValue(identity, out entity);

	public override void Create(TEntity entity) {
		var identity = _identityFunc(entity);
		if (_dictionary.ContainsKey(identity))
			throw new Exception($"Item with identity '{identity}' already exists");
		_dictionary.Add(identity, entity);
	}

	public override void Update(TEntity entity) {
		var identity = _identityFunc(entity);
		if (!_dictionary.ContainsKey(identity))
			throw new Exception($"Item with identity '{identity}' not found");
		_dictionary[identity] = entity;
	}

	public override void Delete(TIdentity identity) {
		if (!_dictionary.ContainsKey(identity))
			throw new Exception($"Item with identity '{identity}' not found");
		_dictionary.Remove(identity);
	}

	public override void Clear()
		=> _dictionary.Clear();

	protected override void FreeManagedResources() {
		if (_dictionary is IDisposable disposableDictionary)
			disposableDictionary.Dispose();
	}
}

public class DictionaryRepositoryAdapter<TEntity, TIdentity> : DictionaryRepositoryAdapter<TEntity, TIdentity, IDictionary<TIdentity, TEntity>> {

	public DictionaryRepositoryAdapter(Func<TEntity, TIdentity> identityFunc) 
		: this(new Dictionary<TIdentity, TEntity>(), identityFunc) {
	}

	public DictionaryRepositoryAdapter(IDictionary<TIdentity, TEntity> dictionary, Func<TEntity, TIdentity> identityFunc)
		: base(dictionary, identityFunc) {
	}

}
