using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using Hydrogen.Data;

namespace Hydrogen.Data.NHibernate {


	public abstract class NHibernateDatabaseManagerBase : DatabaseManagerDecorator, INHibernateSessionProvider {

        public NHibernateDatabaseManagerBase(IDatabaseManager internalDatabaseGenerator)
			: base(internalDatabaseGenerator) {
		}

		public override void CreateApplicationDatabase(string connectionString, DatabaseGenerationDataPolicy dataPolicy, string databaseName) {
			var sessionConfig =
				GetFluentConfig(connectionString)
				.ExposeConfiguration(config => SetCreateDatabaseConfiguration(connectionString, databaseName, config));

			using (var sessionFactory = sessionConfig.BuildSessionFactory()) {
				this.OnDatabaseCreated(connectionString, false);
				IDataGenerator dataPopulator = null;
				switch (dataPolicy) {
					case DatabaseGenerationDataPolicy.NoData:
						dataPopulator = new EmptyDataGenerator();
						break;
					case DatabaseGenerationDataPolicy.DemoData:
					case DatabaseGenerationDataPolicy.PrimingData:
						dataPopulator = CreateDataGenerator(sessionFactory, databaseName, dataPolicy);
						break;
				}
				dataPopulator?.Populate();
			}
		}

		public ISessionFactory OpenDatabase(string connectionString) 
			=> GetFluentConfig(connectionString).BuildSessionFactory();

		protected abstract IDataGenerator CreateDataGenerator(ISessionFactory sessionFactory, string databaseName, DatabaseGenerationDataPolicy policy);

		protected abstract FluentConfiguration GetFluentConfig(string connectionString);

		protected abstract void SetCreateDatabaseConfiguration(string connectionString, string databaseName, Configuration configuration);

	
	}
}
