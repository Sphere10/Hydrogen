using System;
using Sphere10.Helium.Saga;

namespace Sphere10.Helium.TestPlugin1 {
	public class BlueSagaData : ISagaData {
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }
	}
}
