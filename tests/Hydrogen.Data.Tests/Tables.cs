// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hydrogen.Data;

namespace Hydrogen.Data.Tests {
    public static class TestTables {


        public static TableSpecification BasicTable {
            get {
                return new TableSpecification {
                    Name = "BasicTable",
                    Type = TableType.Persistent,
                    Columns = new[] {
                        new ColumnSpecification {
                            Name = "ID",
                            Type = typeof (int),
                            DataType = "INTEGER",
                            Nullable =  false
                        },
                        new ColumnSpecification {
                            Name = "Text",
                            Type = typeof (string),
                            DataType = "VARCHAR(1000)",
                            Nullable =  true
                        },
                        new ColumnSpecification {
                            Name = "Number",
                            Type = typeof (int),
                            DataType = "VARCHAR(1000)",
                            Nullable =  true
                        }
                    },
                    PrimaryKey = new PrimaryKeySpecification {
                        Columns = new[] {  "ID"}
                    }
                };
            }
        }
    }
}
