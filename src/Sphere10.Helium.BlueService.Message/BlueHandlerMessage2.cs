using System;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.BlueService.Message {
	[Serializable]
	public record BlueHandlerMessage2 : IMessage {
		public string Id { get; set; }
		public string TheName { get; set; }
	}
}
