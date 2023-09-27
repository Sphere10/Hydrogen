// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Communications;

public class ProtocolModeBuilder : ProtocolBuilderMain {

	public ProtocolModeBuilder(int number, ProtocolBuilder parent, ProtocolMode mode) : base(parent) {
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
