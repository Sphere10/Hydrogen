using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Sphere10.Framework.Data.NHibernate {

    public class AnsiStringConvention : IPropertyConvention {
        public void Apply(IPropertyInstance instance) {
            if (instance.Property.PropertyType == typeof(string))
                instance.CustomType("AnsiString");
        }
    }
}
