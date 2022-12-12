namespace Hydrogen.Application {
	public interface IApplicationFinalizer {

		int Priority { get; }

		void Finalize();
	}
}
