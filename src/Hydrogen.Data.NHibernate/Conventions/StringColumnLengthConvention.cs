// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Hydrogen.Data.NHibernate;

public class StringColumnLengthConvention : IPropertyConvention, IPropertyConventionAcceptance {
	public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria) {
		criteria.Expect(x => x.Type == typeof(string)).Expect(x => x.Length == 0);
	}
	public void Apply(IPropertyInstance instance) {
		instance.Length(4000);
	}
}
