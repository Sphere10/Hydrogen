using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;

namespace Hydrogen.Data.NHibernate {
	public static class SessionExtensions {
	 public static T Attach<T>(this ISession session, T entity, LockMode mode = null)
	 {
		     //mode = mode ?? LockMode.None;
		  
		     IEntityPersister persister = session.GetSessionImplementation().GetEntityPersister(NHibernateProxyHelper.GuessClass(entity).FullName, entity);
		     Object[] fields = persister.GetPropertyValues(entity/*, session.ActiveEntityMode*/);
		     Object id = persister.GetIdentifier(entity/*, session.ActiveEntityMode*/);
		     EntityEntry entry = session.GetSessionImplementation().PersistenceContext.AddEntry(entity, Status.Loaded, fields, null, id, null, LockMode.None, true, persister, true, false);
		     
		    return (entity);
		 }
	}
}
