// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;

namespace Hydrogen.Data;

public static class DataSetExtensions {


	public static IEnumerable<DataColumn> GetDependentColumns(this DataTable table) {
		if (table == null)
			throw new ArgumentNullException("table");

		var dependencies = (
				from r in table.DataSet.Relations.Cast<DataRelation>()
				where r.ParentTable.TableName == table.TableName
				select r.ChildColumns
			)
			.Cast<IEnumerable<DataColumn>>()
			.Unpartition();

		// get the transitive dependencies
		//	 e.g 	A.ColID -> B.ID -> table.ID
		// TODO: changed Union to Concat -- will this introduce errors?
		foreach (var col in dependencies)
			if (col.Table.PrimaryKey.Contains(col) && col.Table != table)
				dependencies = dependencies.Concat(GetDependentColumns(col.Table));

		return dependencies;
	}

	public static void SetDateTimeMode(this DataSet dataSet, DataSetDateTime mode) {
		foreach (DataTable table in dataSet.Tables)
			table.SetDateTimeMode(mode);
	}

	public static void WriteXml(this DataSet dataSet, XmlWriteMode writeMode, out string xmlSerializedDataSet) {
		var sw = new StringWriter();
		dataSet.WriteXml(sw, writeMode);
		xmlSerializedDataSet = sw.ToString();
	}

	public static string WriteXml(this DataSet dataSet, XmlWriteMode writeMode) {
		string xmlSerializedDataSet;
		dataSet.WriteXml(writeMode, out xmlSerializedDataSet);
		return xmlSerializedDataSet;
	}
}
