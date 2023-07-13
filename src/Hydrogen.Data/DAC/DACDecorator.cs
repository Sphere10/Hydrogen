// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Data;
using System.Collections.Generic;

namespace Hydrogen.Data;

public abstract class DACDecorator : IDAC {
	protected readonly IDAC DecoratedDAC;

	public event EventHandlerEx<IDAC, string> Executing;
	public event EventHandlerEx<IDAC, string> Executed;


	protected DACDecorator(IDAC decoratedDAC) {
		DecoratedDAC = decoratedDAC;
		decoratedDAC.Executing += (dac, s) => OnExecuting(s);
		decoratedDAC.Executed += (dac, s) => OnExecuted(s);
	}


	#region IDAC Implementation

	public Guid InstanceID {
		get { return DecoratedDAC.InstanceID; }
	}

	public virtual bool UseScopeOsmosis {
		get { return DecoratedDAC.UseScopeOsmosis; }
		set { DecoratedDAC.UseScopeOsmosis = value; }
	}

	public IsolationLevel DefaultIsolationLevel {
		get { return DecoratedDAC.DefaultIsolationLevel; }
		set { DecoratedDAC.DefaultIsolationLevel = value; }
	}

	public virtual DBMSType DBMSType {
		get { return DecoratedDAC.DBMSType; }
	}

	public virtual string ConnectionString {
		get { return DecoratedDAC.ConnectionString; }
	}

	public virtual ArtificialKeys ArtificialKeys {
		get { return DecoratedDAC.ArtificialKeys; }
		set { DecoratedDAC.ArtificialKeys = value; }
	}

	public virtual ILogger Log {
		get { return DecoratedDAC.Log; }
		set { DecoratedDAC.Log = value; }
	}

	public virtual IDbConnection CreateConnection() {
		return DecoratedDAC.CreateConnection();
	}

	public virtual ISQLBuilder CreateSQLBuilder() {
		return DecoratedDAC.CreateSQLBuilder();
	}

	public virtual void EnlistInSystemTransaction(IDbConnection connection, System.Transactions.Transaction transaction) {
		DecoratedDAC.EnlistInSystemTransaction(connection, transaction);
	}

	public virtual DataTable ExecuteQuery(string query) {
		return DecoratedDAC.ExecuteQuery(query);
	}

	public virtual int ExecuteNonQuery(string query) {
		return DecoratedDAC.ExecuteNonQuery(query);
	}

	public virtual object ExecuteScalar(string query) {
		return DecoratedDAC.ExecuteScalar(query);
	}

	public virtual DataTable[] ExecuteBatch(ISQLBuilder sqlBuilder) {
		return DecoratedDAC.ExecuteBatch(sqlBuilder);
	}

	public virtual IDataReader ExecuteReader(string query) {
		return DecoratedDAC.ExecuteReader(query);
	}

	public virtual DataTable Select(string tableName, IEnumerable<string> columns = null, bool distinct = false, int? limit = null, int? offset = null, IEnumerable<ColumnValue> columnMatches = null, string whereClause = null,
	                                string orderByClause = null) {
		return DecoratedDAC.Select(tableName, columns: columns, distinct: distinct, limit: limit, offset: offset, columnMatches: columnMatches, whereClause: whereClause, orderByClause: orderByClause);
	}

	public virtual long Insert(string tableName, IEnumerable<ColumnValue> values) {
		return DecoratedDAC.Insert(tableName, values);
	}

	public virtual long Update(string tableName, IEnumerable<ColumnValue> setValues, IEnumerable<ColumnValue> whereValues) {
		return DecoratedDAC.Update(tableName, setValues, whereValues);
	}

	public virtual long Delete(string tableName, IEnumerable<ColumnValue> matchColumns) {
		return DecoratedDAC.Delete(tableName, matchColumns);
	}

	public virtual void BulkInsert(DataTable table, BulkInsertOptions bulkInsertOptions, TimeSpan timeout) {
		DecoratedDAC.BulkInsert(table, bulkInsertOptions, timeout);
	}

	public virtual DBSchema GetSchema() {
		return DecoratedDAC.GetSchema();
	}

	#endregion

	protected virtual void OnExecuting(string sql) {
	}

	protected virtual void OnExecuted(string sql) {
	}

}
