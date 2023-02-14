using NHibernate.SqlCommand;

namespace Hydrogen.Data.NHibernate;

public class EncryptedJsonColumnTypeBase<T> : JsonColumnTypeBase<T> where T : class  {
	public EncryptedJsonColumnTypeBase(string password)
		: base(new EncryptedJsonSerializer(new HydrogenJsonSerializer(), password)) {
	}

}
