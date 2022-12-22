using System;
using System.Collections.Generic;
using System.Text;

namespace Hydrogen.Data.NHibernate;

public abstract class TypeTable {
	public virtual int ID { get; set; }

	public virtual string Name { get; set; }

	public virtual string Description { get; set; }

}
public class TypeTable<T> : TypeTable where T : Enum {
}