﻿using System;

namespace Hydrogen.ObjectSpace.MetaData;

internal class ContainerMetaDataListener<TItem, TKey> {
	private readonly ObjectContainer _container;
	private readonly Func<object, TKey> _projection;
	private readonly IMetaDataStore<TKey> _metaDataStoreStore;

	public ContainerMetaDataListener(ObjectContainer container, IMetaDataStore<TKey> metaDataStoreStore, Func<TItem, TKey> projection) {
		Guard.ArgumentNotNull(container, nameof(container));
		Guard.ArgumentNotNull(metaDataStoreStore, nameof(metaDataStoreStore));
		Guard.ArgumentNotNull(projection, nameof(projection));
		_container = container;
		_metaDataStoreStore = metaDataStoreStore;
		_projection = item => projection((TItem)item);
		_container.PostItemOperation += ObjectContainer_PostItemOperation;
	}

	private void ObjectContainer_PostItemOperation(object source, long index, object item, ObjectContainerOperationType operationType) {
		switch (operationType) {
			case ObjectContainerOperationType.Read:
				break;
			case ObjectContainerOperationType.Add:
				var key = _projection(item);
				_metaDataStoreStore.Add(index, key);
				break;
			case ObjectContainerOperationType.Insert:
				key = _projection(item);
				_metaDataStoreStore.Insert(index, key);
				break;
			case ObjectContainerOperationType.Update:
				var newKey = _projection(item);
				_metaDataStoreStore.Update(index, newKey);
				break;
			case ObjectContainerOperationType.Remove:
				_metaDataStoreStore.Remove(index);
				break;
			case ObjectContainerOperationType.Reap:
				_metaDataStoreStore.Reap(index);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null);
		}
	}

}
