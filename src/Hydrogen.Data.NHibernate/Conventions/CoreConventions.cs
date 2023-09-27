// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;


namespace Hydrogen.Data.NHibernate;

public class CoreConventions : IReferenceConvention, IHasManyConvention, IHasManyToManyConvention {
	public void Apply(IOneToManyCollectionInstance instance) {
		// 2023-01-20 Removed since not one case fit all
		//instance.Inverse();
		//instance.Cascade.All();
	}

	public void Apply(IManyToManyCollectionInstance instance) {
		instance.Table(
			((ICollectionInspector)instance).Inverse
				? instance.ChildType.Name + Inflector.Pluralize(instance.EntityType.Name)
				: instance.EntityType.Name + Inflector.Pluralize(instance.ChildType.Name)
		);
		// 2023-01-20 Removed since not one case fit all
		//instance.Cascade.All();
	}

	public void Apply(IManyToOneInstance instance) {
		// 2023-01-20 Removed since not one case fit all
		//instance.Cascade.All();
	}
}
