// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Hydrogen.Data.NHibernate;

public class EnumConvention :
	IPropertyConvention,
	IPropertyConventionAcceptance {

	#region IPropertyConvention Members

	public void Apply(IPropertyInstance instance) {
		instance.CustomType(instance.Property.PropertyType);
	}

	#endregion

	#region IPropertyConventionAcceptance Members

	public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria) {
		criteria.Expect(x => x.Property.PropertyType.IsEnum ||
		                     (x.Property.PropertyType.IsGenericType &&
		                      x.Property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
		                      x.Property.PropertyType.GetGenericArguments()[0].IsEnum)
		);
	}

	#endregion

}
