// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Hydrogen.Data.NHibernate;

public abstract class NHDatabaseManagerBase : DatabaseManagerDecorator, INHDatabaseManager {

	protected NHDatabaseManagerBase(IDatabaseManager internalDatabaseGenerator)
		: base(internalDatabaseGenerator) {
	}

	public override void CreateApplicationDatabase(string connectionString, DatabaseGenerationDataPolicy dataPolicy, string databaseName) {
		var sessionConfig =
			GetFluentConfig(connectionString)
				.ExposeConfiguration(config => SetCreateDatabaseConfiguration(connectionString, databaseName, config));

		using var sessionFactory = sessionConfig.BuildSessionFactory();
		this.OnDatabaseSchemasCreated(connectionString);
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

	protected virtual void SetCreateDatabaseConfiguration(string connectionString, string databaseName, Configuration configuration) {
		var schemaExport = new SchemaExport(configuration);
		//schemaExport.Drop(false, true);
		schemaExport.Create(false, true);
	}


}
