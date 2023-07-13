// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Data;

public static class SchemaCache {
	private static readonly SynchronizedDictionary<string, DBSchema> _schemaCache;


	static SchemaCache() {
		_schemaCache = new SynchronizedDictionary<string, DBSchema>();
	}

	public static DBSchema GetSchemaCached(this IDAC dac) {
		using (_schemaCache.EnterWriteScope()) {
			var connectionString = dac.ConnectionString;
			if (!_schemaCache.ContainsKey(connectionString))
				_schemaCache[connectionString] = dac.GetSchema();
			return _schemaCache[connectionString];
		}
	}

	public static void InvalidateCachedSchema(this IDAC dac) {
		using (_schemaCache.EnterWriteScope()) {
			var connectionString = dac.ConnectionString;
			if (_schemaCache.ContainsKey(connectionString))
				_schemaCache.Remove(connectionString);
		}
	}


	public static void InvalidateSchema(string connectionString) {
		_schemaCache[connectionString] = null;
	}

	public static void InvalidateAllSchemas() {
		_schemaCache.Clear();
	}


}
