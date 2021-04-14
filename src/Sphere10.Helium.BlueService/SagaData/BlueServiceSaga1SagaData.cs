using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sphere10.Helium.Saga;

namespace Sphere10.Helium.BlueService.SagaData {
	public class BlueServiceSaga1SagaData : ISagaDataForSaga {
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }
	}
}
