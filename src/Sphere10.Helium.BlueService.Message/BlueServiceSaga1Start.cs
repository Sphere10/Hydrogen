using System;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.BlueService.Message {
	[Serializable]
	public record BlueServiceSaga1Start : ICommand {
		public string Id { get; set; }
		public Guid MyUniqueSagaIdToFindSaga { set; get; }
	}
}
