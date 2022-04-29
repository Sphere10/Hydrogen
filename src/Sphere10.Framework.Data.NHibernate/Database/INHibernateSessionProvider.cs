using NHibernate;

namespace Sphere10.Framework.Data.NHibernate {
	public interface INHibernateSessionProvider {
		ISessionFactory OpenDatabase(string connectionString);
	}
}
