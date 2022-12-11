using NHibernate;

namespace Hydrogen.Data.NHibernate {
	public interface INHDatabaseManager : IDatabaseManager {
		ISessionFactory OpenDatabase(string connectionString);
	}
}
