//-----------------------------------------------------------------------
// <copyright file="DataTableSurrogate.cs" company="Sphere 10 Software">
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

using System.Data;
using System.IO;
using System.Xml.Serialization;

using System.Runtime.Serialization;
using Sphere10.Framework.Data;


namespace Sphere10.Framework.Services {


	[DataContract]
	[XmlRoot]
	public class DataTableSurrogate {

		public DataTableSurrogate(DataTable table) {
			DataTable = table;
		}


		[DataMember]
		[XmlIgnore]
		public DataTable DataTable { get; set; }


		[DataMember]
		[XmlElement]
		public string SerializedDataSet {
			get {
				if (DataTable == null)
					return string.Empty;

				var sw = new StringWriter();
				DataTable.SetDateTimeMode(DataSetDateTime.Unspecified);
				DataTable.WriteXml(sw, XmlWriteMode.WriteSchema);
				return sw.ToString();
			}
			set {
				DataTable = new DataTable();

				if (!string.IsNullOrEmpty(value))
					DataTable.ReadXml(new StringReader(value));
			}
		}

	}
}
