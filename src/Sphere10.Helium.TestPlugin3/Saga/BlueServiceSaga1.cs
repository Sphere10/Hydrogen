using System;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Handle;
using Sphere10.Helium.Message;
using Sphere10.Helium.Saga;
using Sphere10.Helium.TestPlugin3.Message;
using Sphere10.Helium.TestPlugin3.SagaData;

namespace Sphere10.Helium.TestPlugin3.Saga {
	public class BlueServiceSaga1 : Saga<BlueServiceSaga1SagaData>,
		IStartSaga<BlueServiceSaga1Start>,
		IEndSaga<BlueServiceSaga1End>,
		IHandleMessage<BlueServiceSaga1Workflow1>,
		IHandleMessage<BlueServiceSaga1Workflow2>,
		IHandleTimeout<BlueServiceSaga1Workflow3> {
		
		private readonly IBus _bus;

		public BlueServiceSaga1(IBus bus) 
			: base(bus) {

			_bus = bus;
		}

		protected override void ConfigureHowToFindSaga(SagaPropertyMapper<BlueServiceSaga1SagaData> mapper) {
			mapper.ConfigureMapping<BlueServiceSaga1Start>(m => m.MyUniqueSagaIdToFindSaga)
				.ToSaga(s => s.MyUniqueSagaIdToFindSaga);

			mapper.ConfigureMapping<BlueServiceSaga1End>(m => m.MyUniqueSagaIdToFindSaga)
				.ToSaga(s => s.MyUniqueSagaIdToFindSaga);
		}

		public void Handle(BlueServiceSaga1Start message) {
			var testDataProperty = Data.MyUniqueSagaIdToFindSaga.ToString();

			var blueServiceSaga1Workflow1 = new BlueServiceSaga1Workflow1 {
				Id = Guid.NewGuid().ToString()
			};

			_bus.SendLocal(blueServiceSaga1Workflow1);
		}

		public void Handle(BlueServiceSaga1End message) {
			throw new NotImplementedException();
		}

		public void Handle(BlueServiceSaga1Workflow1 message) {
			throw new NotImplementedException();
		}

		public void Handle(BlueServiceSaga1Workflow2 message) {
			throw new NotImplementedException();
		}

		public void Timeout(BlueServiceSaga1Workflow3 state) {
			throw new NotImplementedException();
		}
	}
}
