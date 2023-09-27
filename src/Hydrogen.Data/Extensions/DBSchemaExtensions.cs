// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.Data;

public static class DBSchemaExtensions {

	public static IEnumerable<object> ValuesToObjectFormat(this IEnumerable<DBColumnSchema> columns, IEnumerable<string> keys) {
		return keys.Zip(columns, (key, col) => Convert.ChangeType(key, col.Type));
	}

	public static IEnumerable<string> ConvertToStorableFormat(this DBSchema schema, IEnumerable<object> values) {
		return values.Select(v => Convert.ChangeType(v, typeof(string)) as string);
	}

	public static EnumerableKeyDictionary<object, object[]> ConvertDataToMultiKeyDictionary(this DBTableSchema tableSchema, IEnumerable<IEnumerable<object>> dataRows) {
		return ConvertRowToMultiKeyDictionaryInternal(tableSchema, dataRows, (x) => x);
	}

	public static EnumerableKeyDictionary<string, string[]> ConvertDataToStringBasedMultiKeyDictionary(this DBTableSchema tableSchema, IEnumerable<IEnumerable<object>> dataRows) {
		return ConvertRowToMultiKeyDictionaryInternal(tableSchema, dataRows, (x) => x != null ? Convert.ChangeType(x, typeof(string)) as string : "NULL");
	}

	private static EnumerableKeyDictionary<T, T[]> ConvertRowToMultiKeyDictionaryInternal<T>(
		this DBTableSchema tableSchema,
		IEnumerable<IEnumerable<object>> dataRows,
		Func<object, T> baseTypeTransformer
	) {
		return
			dataRows
				.ToMultiKeyDictionary2Ex(
					tableSchema
						.PrimaryKeyColumns
						.Select(
							col => (Func<IEnumerable<object>, T>)(row => baseTypeTransformer(row.ElementAt(col.Position - 1)))
						),
					row => row.Select(baseTypeTransformer).ToArray()
				);
	}

}
