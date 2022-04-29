//-----------------------------------------------------------------------
// <copyright file="DataRowSurrogate.cs" company="Sphere 10 Software">
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

using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System;
using Sphere10.Framework.Data;


namespace Sphere10.Framework.Services {

	[DataContract]
	[XmlRoot]
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class DataRowSurrogate {

		public DataRowSurrogate() : this(null) {			
		}

		public DataRowSurrogate(DataRow dataRow) {
			DataRow = dataRow;
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public DataRow DataRow { get; set; }

		[DataMember]
		[XmlElement]
		public string SerializedDataSet {
			get {
				try {
					if (DataRow == null)
						return string.Empty;

					var sw = new StringWriter();
					DataRow.Table.SetDateTimeMode(DataSetDateTime.Unspecified);
					DataRow.Table.WriteXml(sw, XmlWriteMode.WriteSchema);
					var stringValue = sw.ToString();
					return stringValue;
				} catch(Exception error) {
					SystemLog.Exception(error);
					throw error;
				}
			}
			set {
				if (value == null) {
					DataRow = null;
				} else {
					var dataTable = new DataTable();
					dataTable.ReadXml(new StringReader(value));
					DataRow = dataTable.Rows[0];

				}
			}
		}

	}
}
