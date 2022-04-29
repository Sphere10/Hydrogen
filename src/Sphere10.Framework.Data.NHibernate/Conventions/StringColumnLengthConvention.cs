using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Sphere10.Framework.Data.NHibernate {
    public class StringColumnLengthConvention : IPropertyConvention, IPropertyConventionAcceptance {
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria) {
            criteria.Expect(x => x.Type == typeof(string)).Expect(x => x.Length == 0);
        }
        public void Apply(IPropertyInstance instance) {
            instance.Length(4000);
        }
    }
}
