using System;
using Sphere10.Helium.Saga;

namespace Sphere10.Helium.BlueService.SagaData {
	public class BlueServiceSaga1SagaData : ISagaDataForSaga {
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }
		public Guid MyUniqueSagaIdToFindSaga { set; get; }
	}
}
