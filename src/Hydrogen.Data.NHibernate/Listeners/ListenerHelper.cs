using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Persister.Entity;


namespace Hydrogen.Data.NHibernate {
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
}