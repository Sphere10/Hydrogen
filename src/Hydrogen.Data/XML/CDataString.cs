// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Hydrogen.Data;

public class CDataString : IXmlSerializable {
	private string _value;
	public CDataString() : this(null) {
	}

	public CDataString(string value) {
		_value = value;
	}
	public XmlSchema GetSchema() {
		return null;

	}
	public void ReadXml(XmlReader reader) {
		_value = reader.ReadElementContentAsString();
	}

	public void WriteXml(XmlWriter writer) {
		writer.WriteCData(_value);
	}

	public static implicit operator CDataString(string s) {
		var cdatastring = new CDataString(s);
		return cdatastring;
	}

	public static implicit operator string(CDataString cdata) {
		return cdata._value;
	}

}
