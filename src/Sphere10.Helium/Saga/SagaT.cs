using Sphere10.Helium.Bus;

namespace Sphere10.Helium.Saga {
	public abstract class Saga<T> : Saga where T : ISagaData, new() {

		public T Data {
			get => (T)SagaDataBase;
			set => SagaDataBase = (ISagaData)value;
		}

		protected Saga(IBus bus) : base(bus) {
		}

		protected internal override void ConfigureHowToFindSaga(IFindSaga sagaFindMap) {
			ConfigureHowToFindSaga(new SagaPropertyMapper<T>(sagaFindMap));
		}

		protected abstract void ConfigureHowToFindSaga(SagaPropertyMapper<T> mapper);

	}
}