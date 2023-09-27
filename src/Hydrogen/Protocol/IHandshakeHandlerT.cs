// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Communications;

public interface IHandshakeHandler<THandshake, TAck, TVerack> : IHandshakeHandler {
	new THandshake GenerateHandshake(ProtocolOrchestrator orchestrator);

	HandshakeOutcome ReceiveHandshake(ProtocolOrchestrator orchestrator, THandshake handshake, out TAck acknowledgement);

	HandshakeOutcome VerifyHandshake(ProtocolOrchestrator orchestrator, THandshake handshake, TAck acknowledgement, out TVerack verifyAcknowledgement);

	bool AcknowledgeHandshake(ProtocolOrchestrator orchestrator, THandshake handshake, TAck acknowledgement, TVerack verifyAcknowledgement);
}
