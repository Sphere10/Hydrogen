namespace Sphere10.Framework.Application {
	public interface IApplicationFinalizeTask {

		int Sequence { get; }

		void Finalize();
	}
}
