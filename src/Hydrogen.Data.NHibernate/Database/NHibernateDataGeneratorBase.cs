using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using System.Threading.Tasks;
using Hydrogen;

namespace Hydrogen.Data.NHibernatee {
    public abstract class NHibernateDataGeneratorBase : IDataGenerator {
	    protected NHibernateDataGeneratorBase(ISessionFactory sessionFactory)  {
			SessionFactory = sessionFactory;
	    }

		protected ISessionFactory SessionFactory { get; set; }

        public void Populate() {
            using (var session = SessionFactory.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    var data = CreateData();
                    data.ForEach(session.SaveOrUpdate);
                    transaction.Commit();
                }
                session.Flush();
                session.Close();
            } 
        }

        protected abstract IEnumerable<object> CreateData();
    }
}