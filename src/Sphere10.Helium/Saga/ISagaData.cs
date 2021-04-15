using System;

namespace Sphere10.Helium.Saga {
	public interface ISagaData {
		Guid Id { get; set; }

		string Originator { get; set; }

		string OriginalMessageId { get; set; }
	}
}
