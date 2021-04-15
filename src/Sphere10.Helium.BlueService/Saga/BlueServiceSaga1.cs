using System;
using Sphere10.Helium.BlueService.Message;
using Sphere10.Helium.BlueService.SagaData;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Handler;
using Sphere10.Helium.Message;
using Sphere10.Helium.Saga;

namespace Sphere10.Helium.BlueService.Saga {
	public class BlueServiceSaga1 : Saga<BlueServiceSaga1SagaData>,
		IStartSaga<BlueServiceSaga1Start>,
		IEndSaga<BlueServiceSaga1End>,
		IHandleMessage<BlueServiceSaga1Workflow1>,
		IHandleMessage<BlueServiceSaga1Workflow2>,
		IHandleTimeout<BlueServiceSaga1Workflow3> {

		public BlueServiceSaga1(IBus bus) 
			: base(bus) {
		}

		protected override void ConfigureHowToFindSaga(SagaPropertyMapper<BlueServiceSaga1SagaData> mapper) {
			mapper.ConfigureMapping<BlueServiceSaga1Start>(m => m.MyUniqueSagaIdToFindSaga)
				.ToSaga(s => s.MyUniqueSagaIdToFindSaga);

			mapper.ConfigureMapping<BlueServiceSaga1End>(m => m.MyUniqueSagaIdToFindSaga)
				.ToSaga(s => s.MyUniqueSagaIdToFindSaga);
		}

		public void Handle(BlueServiceSaga1Start message) {
			var testDataProperty = Data.MyUniqueSagaIdToFindSaga.ToString();
			
			throw new NotImplementedException();
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
