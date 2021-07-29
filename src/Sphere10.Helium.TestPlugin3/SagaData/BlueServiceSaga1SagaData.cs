using System;
using Sphere10.Helium.Saga;

namespace Sphere10.Helium.TestPlugin3.SagaData {
	public class BlueServiceSaga1SagaData : ISagaData {
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }
		public Guid MyUniqueSagaIdToFindSaga { set; get; }
	}
}
