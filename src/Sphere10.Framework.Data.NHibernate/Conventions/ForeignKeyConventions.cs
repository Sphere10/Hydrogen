using System;
using FluentNHibernate;
using FluentNHibernate.Conventions;

namespace Sphere10.Framework.Data.NHibernate {
    public class ForeignKeyConventions : ForeignKeyConvention {
        protected override string GetKeyName(Member property, Type type) {
            return property == null ? type.Name + "ID" : property.Name + "ID";
        }
    }
}