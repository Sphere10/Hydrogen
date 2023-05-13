// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Communications {

	public interface IHandshakeHandler {

		object GenerateHandshake(ProtocolOrchestrator orchestrator);

		HandshakeOutcome ReceiveHandshake(ProtocolOrchestrator orchestrator, object handshake, out object acknowledgement);

		HandshakeOutcome VerifyHandshake(ProtocolOrchestrator orchestrator, object handshake, object acknowledgement, out object verifyAcknowledgement);

		bool AcknowledgeHandshake(ProtocolOrchestrator orchestrator, object handshake, object acknowledgement, object verifyAcknowledgement);

	}
}
