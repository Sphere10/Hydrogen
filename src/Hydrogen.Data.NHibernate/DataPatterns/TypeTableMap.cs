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
