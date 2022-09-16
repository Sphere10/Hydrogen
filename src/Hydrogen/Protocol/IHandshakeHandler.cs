namespace Hydrogen.Communications {

	public interface IHandshakeHandler {

		object GenerateHandshake(ProtocolOrchestrator orchestrator);

		HandshakeOutcome ReceiveHandshake(ProtocolOrchestrator orchestrator, object handshake, out object acknowledgement);

		HandshakeOutcome VerifyHandshake(ProtocolOrchestrator orchestrator, object handshake, object acknowledgement, out object verifyAcknowledgement);

		bool AcknowledgeHandshake(ProtocolOrchestrator orchestrator, object handshake, object acknowledgement, object verifyAcknowledgement);

	}
}
