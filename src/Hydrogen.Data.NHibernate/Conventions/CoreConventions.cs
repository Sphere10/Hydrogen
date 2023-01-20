using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using Hydrogen;


namespace Hydrogen.Data.NHibernate {
    public class CoreConventions : IReferenceConvention, IHasManyConvention, IHasManyToManyConvention {
        public void Apply(IOneToManyCollectionInstance instance) {
			//instance.Inverse();
			//instance.Cascade.All();
		}

        public void Apply(IManyToManyCollectionInstance instance) {
            instance.Table(
                ((ICollectionInspector) instance).Inverse
                    ? instance.ChildType.Name + Inflector.Pluralize(instance.EntityType.Name)
                    : instance.EntityType.Name + Inflector.Pluralize(instance.ChildType.Name)
                );
			//instance.Cascade.All();
		}

        public void Apply(IManyToOneInstance instance) {
			//instance.Cascade.All();
		}
    }
}