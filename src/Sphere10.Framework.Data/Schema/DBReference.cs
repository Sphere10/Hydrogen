//-----------------------------------------------------------------------
// <copyright file="DBReference.cs" company="Sphere 10 Software">
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
using System.Xml.Serialization;

namespace Sphere10.Framework.Data {

    [XmlRoot("ConnectionString")]
    public struct DBReference {

        public DBReference(DBMSType dbmsType, string connectionString) {
            DBMSType = dbmsType;
            ConnectionString = connectionString;
        }

        [XmlAttribute("DBMS")]
        public DBMSType DBMSType { get; set; }

        [XmlAttribute("Value")]
        public string ConnectionString { get; set; }
    }
}
