using NHibernate;

namespace Hydrogen.Data.NHibernate {
	public interface INHibernateSessionProvider {
		ISessionFactory OpenDatabase(string connectionString);
	}
}
