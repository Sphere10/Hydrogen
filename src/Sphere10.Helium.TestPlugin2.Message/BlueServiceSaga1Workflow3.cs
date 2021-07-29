using System;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.TestPlugin2.Message {
	[Serializable]
	public record BlueServiceSaga1Workflow3 : IMessage
	{
		public string Id { get; set; }
	}
}
