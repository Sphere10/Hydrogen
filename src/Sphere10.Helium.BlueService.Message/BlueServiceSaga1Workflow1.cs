using System;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.BlueService.Message {
	[Serializable]
	public record BlueServiceSaga1Workflow1 : IMessage {
		public string Id { get; set; }
	}
}
