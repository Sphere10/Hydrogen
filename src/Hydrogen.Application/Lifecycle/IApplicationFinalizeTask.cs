namespace Hydrogen.Application {
	public interface IApplicationFinalizeTask {

		int Sequence { get; }

		void Finalize();
	}
}
