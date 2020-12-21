//-----------------------------------------------------------------------
// <copyright file="CDataString.cs" company="Sphere 10 Software">
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
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sphere10.Framework {
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
}
