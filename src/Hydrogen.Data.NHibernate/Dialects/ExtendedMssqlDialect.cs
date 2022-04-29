using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Dialect;

namespace Sphere10.Framework.Data.NHibernate {
    public class ExtendedMssqlDialect : MsSql2008Dialect {
        protected override void RegisterNumericTypeMappings() {
            base.RegisterNumericTypeMappings();
            RegisterColumnType(DbType.Byte, "SMALLINT");
            RegisterColumnType(DbType.UInt16, "INT");
            RegisterColumnType(DbType.UInt32, "BIGINT");
            RegisterColumnType(DbType.UInt64, "DECIMAL(28)");
        }
    }
}
