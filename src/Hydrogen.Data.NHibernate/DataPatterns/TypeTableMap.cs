// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using FluentNHibernate.Mapping;

namespace Hydrogen.Data.NHibernate;

public abstract class TypeTableMap<T> : ClassMap<TypeTable<T>> where T : Enum {
	protected TypeTableMap() {
		Table(TableName);
		Id(x => x.ID).Column("ID").GeneratedBy.Assigned();
		var nameMap = Map(x => x.Name).Column("Name").Not.Nullable().CustomType("AnsiString");
		if (UniqueName) {
			nameMap.Unique();
		}
		Map(x => x.Description).Column("Description").Nullable();
	}

	public virtual string TableName => typeof(T).Name;

	public virtual bool UniqueName => true;

}
