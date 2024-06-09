// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Communications;

public abstract class HandshakeHandlerBase<THandshake, TAck, TVerack> : HandshakeHandlerBase, IHandshakeHandler<THandshake, TAck, TVerack> {

	public sealed override object GenerateHandshake(ProtocolOrchestrator orchestrator)
		=> ((IHandshakeHandler<THandshake, TAck, TVerack>)this).GenerateHandshake(orchestrator);

	public sealed override HandshakeOutcome ReceiveHandshake(ProtocolOrchestrator orchestrator, object handshake, out object acknowledgement) {
		var result = ReceiveHandshake(orchestrator, (THandshake)handshake, out var ack);
		acknowledgement = ack;
		return result;
	}

	public sealed override HandshakeOutcome VerifyHandshake(ProtocolOrchestrator orchestrator, object handshake, object acknowledgement, out object verifyAcknowledgement) {
		var result = VerifyHandshake(orchestrator, (THandshake)handshake, (TAck)acknowledgement, out var verack);
		verifyAcknowledgement = verack;
		return result;
	}

	public sealed override bool AcknowledgeHandshake(ProtocolOrchestrator orchestrator, object handshake, object acknowledgement, object verify)
		=> AcknowledgeHandshake(orchestrator, (THandshake)handshake, (TAck)acknowledgement, (TVerack)verify);

	THandshake IHandshakeHandler<THandshake, TAck, TVerack>.GenerateHandshake(ProtocolOrchestrator orchestrator)
		=> GenerateHandshakeInternal(orchestrator);

	public abstract HandshakeOutcome ReceiveHandshake(ProtocolOrchestrator orchestrator, THandshake handshake, out TAck acknowledgement);

	public abstract HandshakeOutcome VerifyHandshake(ProtocolOrchestrator orchestrator, THandshake handshake, TAck acknowledgement, out TVerack verifyAcknowledgement);

	public abstract bool AcknowledgeHandshake(ProtocolOrchestrator orchestrator, THandshake handshake, TAck acknowledgement, TVerack verifyAcknowledgement);

	protected abstract THandshake GenerateHandshakeInternal(ProtocolOrchestrator orchestrator);
}
