using NHibernate.SqlCommand;

namespace Hydrogen.Data.NHibernate;

public class JsonColumnType<T> : JsonColumnTypeBase<T> 	where T : class  {
	public JsonColumnType()
		: base(new HydrogenJsonSerializer()) {
	}

}