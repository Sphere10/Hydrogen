﻿using System;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.TestPlugin3.Message {
	[Serializable]
	public record BlueServiceSaga1Workflow1 : IMessage {
		public string Id { get; set; }
	}
}
