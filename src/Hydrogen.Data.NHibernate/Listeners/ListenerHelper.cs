// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;
using NHibernate.Persister.Entity;


namespace Hydrogen.Data.NHibernate;

internal static class ListenerHelper {
	private static readonly SynchronizedDictionary<string, IDictionary<string, int>> EntityPropertyIndexCache;

	static ListenerHelper() {
		EntityPropertyIndexCache = new SynchronizedDictionary<string, IDictionary<string, int>>();
	}


	public static void SetProperty(IEntityPersister persister, object[] state, string propertyName, object value) {
		state[FastGetEntityPropertyIndices(persister)[propertyName]] = value;
	}

	public static object GetProperty(IEntityPersister persister, object[] state, string propertyName) {
		return state[FastGetEntityPropertyIndices(persister)[propertyName]];
	}

	public static IDictionary<string, int> FastGetEntityPropertyIndices(IEntityPersister persister) {
		if (!EntityPropertyIndexCache.ContainsKey(persister.EntityName)) {
			using (EntityPropertyIndexCache.EnterWriteScope()) {
				if (!EntityPropertyIndexCache.ContainsKey(persister.EntityName)) {
					EntityPropertyIndexCache.Add(
						persister.EntityName,
						persister.PropertyNames.WithDescriptions().ToDictionary(d => d.Item, d => d.Index)
					);
				}
			}
		}
		return EntityPropertyIndexCache[persister.EntityName];
	}

}
