﻿namespace Hydrogen.Communications {
	public interface IHandshakeHandler<THandshake, TAck, TVerack> : IHandshakeHandler {
		new THandshake GenerateHandshake(ProtocolOrchestrator orchestrator);

		HandshakeOutcome ReceiveHandshake(ProtocolOrchestrator orchestrator, THandshake handshake, out TAck acknowledgement);

		HandshakeOutcome VerifyHandshake(ProtocolOrchestrator orchestrator, THandshake handshake, TAck acknowledgement, out TVerack verifyAcknowledgement);

		bool AcknowledgeHandshake(ProtocolOrchestrator orchestrator, THandshake handshake, TAck acknowledgement, TVerack verifyAcknowledgement);
	}
}
