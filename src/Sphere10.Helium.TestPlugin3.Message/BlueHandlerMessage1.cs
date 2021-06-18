using System;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.TestPlugin3.Message {
	[Serializable]
	public record BlueHandlerMessage1 : IMessage {
		public string Id { get; set; }
		public string TheName { get; set; }
	}
}
