//-----------------------------------------------------------------------
// <copyright file="TableSpecification2.cs" company="Sphere 10 Software">
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
using System.Data;
using System.Linq;
using System.Text;

namespace Hydrogen.Data {
    public class TableSpecification2 : BaseSQLSpecificationObject {

        public string Schema { get; set; }

		public string Name { get; set; }
		public TableType Type { get; set; }

		public IEnumerable<Column> Columns { get; set; }
		public IEnumerable<Constraint> Constraints { get; set; }

        public class Column : BaseSQLSpecificationObject {
            public string Name { get; set; }
            public DbType Type { get; set; }
            public bool Nullable { get; set; }
            public int Length { get; set; }
            public int Scale { get; set; }
            public int Precision { get; set; }
            public TextEncoding? Collation { get; set; }
            public object DefaultValue { get; set; }
        }

        public abstract class Constraint : BaseSQLSpecificationObject {

            public string Name { get; set; }

            public abstract ConstraintType ConstraintType { get; }

        }

        public class PrimaryKeyConstraint : Constraint {

            public string Name { get; set; }

            public IEnumerable<string> Columns { get; set; }

            public override ConstraintType ConstraintType {
                get { return ConstraintType.PrimaryKey; }
            }

            public SQLSortDirection? SortDirection { get; set; }

        }
        public class UniqueConstraint : Constraint {

            public override ConstraintType ConstraintType {
                get { return ConstraintType.Unique; }
            }

        }
        public class CheckConstraint : Constraint {

            public SQLExpression Expression { get; set; }

            public override ConstraintType ConstraintType {
                get { return ConstraintType.Check; }
            }

        }

	}
}
