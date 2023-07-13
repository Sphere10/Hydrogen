// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Xml.Serialization;

namespace Hydrogen.Data;

[XmlRoot("ConnectionString")]
public struct DBReference {

	public DBReference(DBMSType dbmsType, string connectionString) {
		DBMSType = dbmsType;
		ConnectionString = connectionString;
	}

	[XmlAttribute("DBMS")] public DBMSType DBMSType { get; set; }

	[XmlAttribute("Value")] public string ConnectionString { get; set; }
}
