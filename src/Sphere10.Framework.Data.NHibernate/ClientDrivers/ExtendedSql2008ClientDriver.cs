using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.SqlTypes;

namespace Sphere10.Framework.Data.NHibernate {
    public class ExtendedSql2008ClientDriver : Sql2008ClientDriver {
        protected override void InitializeParameter(DbParameter dbParam, string name, SqlType sqlType)
        {
            if (Equals(sqlType, SqlTypeFactory.Byte)) sqlType = SqlTypeFactory.Int16;
            if (Equals(sqlType, SqlTypeFactory.UInt16)) sqlType = SqlTypeFactory.Int32;
            if (Equals(sqlType, SqlTypeFactory.UInt32)) sqlType = SqlTypeFactory.Int64;
            if (Equals(sqlType, SqlTypeFactory.UInt64)) sqlType = SqlTypeFactory.Decimal;
            base.InitializeParameter(dbParam, name, sqlType);
        }
    }
}
