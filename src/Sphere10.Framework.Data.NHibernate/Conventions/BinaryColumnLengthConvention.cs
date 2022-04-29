using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Sphere10.Framework.Data.NHibernate {
    public class BinaryColumnLengthConvention : IPropertyConvention, IPropertyConventionAcceptance {
		public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria) {
			criteria.Expect(x => x.Property.PropertyType == typeof(byte[]));
		}

		public void Apply(IPropertyInstance instance) {
			instance.Length(2147483647);
			instance.CustomSqlType("varbinary(MAX)");
		}
	}
}
