using System;
using System.Collections.Generic;

namespace Hydrogen.Communications {
	public class ProtocolModeBuilder : ProtocolBuilderMain {

		public ProtocolModeBuilder(int number, ProtocolBuilder parent, ProtocolMode mode): base(parent) {
			Guard.ArgumentInRange(number, 0, int.MaxValue, nameof(number));
			Guard.ArgumentNotNull(parent, nameof(parent));
			Number = number;
			Requests = new ProtocolRequestBuilder(this, mode);
			Responses = new ProtocolResponseBuilder(this, mode);
			Commands = new ProtocolCommandBuilder(this, mode);
			Messages = new ProtocolMessageBuilder(this, mode, mode.MessageSerializer);
		}

		public int Number { get; init; }

		public override ProtocolRequestBuilder Requests { get; }

		public override ProtocolResponseBuilder Responses { get; }

		public override ProtocolCommandBuilder Commands { get; }

		public override ProtocolMessageBuilder Messages { get; }

	}
}
