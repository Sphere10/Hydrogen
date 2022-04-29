//-----------------------------------------------------------------------
// <copyright file="IDAC.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace Hydrogen.Data {
	public interface IDAC {

	    event EventHandlerEx<IDAC, string> Executing;
        event EventHandlerEx<IDAC, string> Executed;
            
        Guid InstanceID { get; }

        bool UseScopeOsmosis { get; set; }

        IsolationLevel DefaultIsolationLevel { get; set; }

        DBMSType DBMSType { get; }

		string ConnectionString { get; }

		ArtificialKeys ArtificialKeys { get; set;  }

		ILogger Log { get; set; }

		IDbConnection CreateConnection();

		ISQLBuilder CreateSQLBuilder();

        void EnlistInSystemTransaction(IDbConnection connection, System.Transactions.Transaction transaction);

		int ExecuteNonQuery(string query);

		object ExecuteScalar(string query);

		DataTable[] ExecuteBatch(ISQLBuilder sqlBuilder);

		IDataReader ExecuteReader(string query);
	
        // TODO: Make ext method
		long Insert(string tableName, IEnumerable<ColumnValue> values);

        // TODO: Make ext method
        long Update(string tableName, IEnumerable<ColumnValue> setValues, IEnumerable<ColumnValue> whereValues);

        // TODO: Make ext method
        long Delete(string tableName, IEnumerable<ColumnValue> matchColumns);

	    void BulkInsert(DataTable table, BulkInsertOptions bulkInsertOptions, TimeSpan timeout);
		DBSchema GetSchema();


	}
}
