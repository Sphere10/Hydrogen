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

	public abstract class NHDatabaseManagerBase : DatabaseManagerDecorator, INHDatabaseManager {

		protected NHDatabaseManagerBase(IDatabaseManager internalDatabaseGenerator)
			: base(internalDatabaseGenerator) {
		}

		public override void CreateApplicationDatabase(string connectionString, DatabaseGenerationDataPolicy dataPolicy, string databaseName) {
			var sessionConfig =
				GetFluentConfig(connectionString)
				.ExposeConfiguration(config => SetCreateDatabaseConfiguration(connectionString, databaseName, config));

			using var sessionFactory = sessionConfig.BuildSessionFactory();
			this.OnDatabaseCreated(connectionString, false);
			IDataGenerator dataPopulator = dataPolicy switch {
				DatabaseGenerationDataPolicy.NoData => new EmptyDataGenerator(),
				DatabaseGenerationDataPolicy.DemoData => CreateDataGenerator(sessionFactory, databaseName, dataPolicy),
				DatabaseGenerationDataPolicy.PrimingData => CreateDataGenerator(sessionFactory, databaseName, dataPolicy),
				_ => null
			};
			dataPopulator?.Populate();
		}

		public ISessionFactory OpenDatabase(string connectionString) 
			=> GetFluentConfig(connectionString).BuildSessionFactory();

		protected abstract IDataGenerator CreateDataGenerator(ISessionFactory sessionFactory, string databaseName, DatabaseGenerationDataPolicy policy);

		protected abstract FluentConfiguration GetFluentConfig(string connectionString);

		protected abstract void SetCreateDatabaseConfiguration(string connectionString, string databaseName, Configuration configuration);

	
	}
}
