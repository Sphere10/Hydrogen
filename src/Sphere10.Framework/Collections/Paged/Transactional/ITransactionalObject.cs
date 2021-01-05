namespace Sphere10.Framework {

	public interface ITransactionalObject {

		void Commit();

		void Rollback();

	}

}