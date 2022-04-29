//-----------------------------------------------------------------------
// <copyright file="DataSetSurrogate.cs" company="Sphere 10 Software">
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
	public class DataSetSurrogate {

		public DataSetSurrogate() : this(new DataSet()) {
		}

		public DataSetSurrogate(DataSet dataSet) {
			DataSet = dataSet;
		}


		[IgnoreDataMember]
		[XmlIgnore]
		public DataSet DataSet { get; set; }


		[DataMember]
		[XmlElement]
		public string SerializedDataSet {
			get {
				if (DataSet == null)
					return string.Empty;

				var sw = new StringWriter();
				DataSet.SetDateTimeMode(DataSetDateTime.Unspecified);
				DataSet.WriteXml(sw, XmlWriteMode.WriteSchema);
				return sw.ToString();
			}
			set {
				DataSet = new DataSet();
				if (!string.IsNullOrEmpty(value))
					DataSet.ReadXml(new StringReader(value));

			}
		}

	}
}
