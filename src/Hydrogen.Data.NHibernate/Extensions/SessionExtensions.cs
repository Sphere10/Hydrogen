// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;

namespace Hydrogen.Data.NHibernate;

public static class SessionExtensions {
	public static T Attach<T>(this ISession session, T entity, LockMode mode = null) {
		//mode = mode ?? LockMode.None;

		IEntityPersister persister = session.GetSessionImplementation().GetEntityPersister(NHibernateProxyHelper.GuessClass(entity).FullName, entity);
		Object[] fields = persister.GetPropertyValues(entity /*, session.ActiveEntityMode*/);
		Object id = persister.GetIdentifier(entity /*, session.ActiveEntityMode*/);
		EntityEntry entry = session.GetSessionImplementation().PersistenceContext.AddEntry(entity, Status.Loaded, fields, null, id, null, LockMode.None, true, persister, true, false);

		return (entity);
	}
}
