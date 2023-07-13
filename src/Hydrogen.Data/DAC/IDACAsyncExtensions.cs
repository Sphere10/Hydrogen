// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen.Data;

public static class IDACAsyncExtensions {

	public static async Task<object> ExecuteScalarAsync(this IDAC dac, string query) {
		return await Task.Run(() => dac.ExecuteScalar(query));
	}

	public static async Task<T> ExecuteScalarAsync<T>(this IDAC dac, string query) {
		return await Task.Run(() => dac.ExecuteScalar<T>(query));
	}


	public static async Task<int> ExecuteNonQueryAsync(this IDAC dac, string query) {
		return await Task.Run(() => dac.ExecuteNonQuery(query));
	}

	public static async Task<long> GetMaxIDAsync(this IDAC dac, string tableName, string idColName) {
		return await Task.Run(() => dac.GetMaxID(tableName, idColName));
	}

	public static async Task<long> CountAsync(this IDAC dac, string tableName, IEnumerable<ColumnValue> columnMatches = null, string whereClause = null) {
		return await Task.Run(() => dac.Count(tableName, columnMatches, whereClause));
	}

	public static async Task<DataTable> SelectAsync(this IDAC dac, string tableName, IEnumerable<string> columns = null, bool distinct = false, int? limit = null, int? offset = null, IEnumerable<ColumnValue> columnMatches = null,
	                                                string whereClause = null, string orderByClause = null) {
		return await Task.Run(() => dac.Select(tableName, columns, distinct, limit, offset, columnMatches, whereClause, orderByClause));
	}

	public static async Task<DataTable> ExecuteQueryAsync(this IDAC dac, string query) {
		return await Task.Run(() => dac.ExecuteQuery(query));
	}

	public static async Task<DataTable> ExecuteQueryAsync(this IDAC dac, string query, params object[] args) {
		return await Task.Run(() => dac.ExecuteQuery(query, args));
	}

	public static async Task<long> UpdateAsync(this IDAC dac, string tableName, IEnumerable<ColumnValue> setValues, IEnumerable<ColumnValue> whereValues) {
		return await Task.Run(() => dac.Update(tableName, setValues, whereValues));
	}

	public static async Task<long> DeleteAsync(this IDAC dac, string tableName, IEnumerable<ColumnValue> matchColumns) {
		return await Task.Run(() => dac.Delete(tableName, matchColumns));
	}

	public static async Task<long> InsertAsync(this IDAC dac, string tableName, IEnumerable<ColumnValue> values) {
		return await Task.Run(() => dac.Insert(tableName, values));
	}

	public static async Task BulkInsertAsync(this IDAC dac, DataTable table, BulkInsertOptions bulkInsertOptions, TimeSpan timeout, CancellationToken cancellationToken) {
		// Begin a scope to make sure it's disposed if cancellationToken is turned on
		using (var scope = dac.BeginScope()) {
			await Task.Run(() => dac.BulkInsert(table, bulkInsertOptions, timeout), cancellationToken);
		}
	}
}
