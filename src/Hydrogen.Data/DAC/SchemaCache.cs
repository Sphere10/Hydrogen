//-----------------------------------------------------------------------
// <copyright file="SchemaCache.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;

namespace Sphere10.Framework.Data {
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
}
