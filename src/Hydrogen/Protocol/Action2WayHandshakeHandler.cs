// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Communications;

public class Action2WayHandshakeHandler<THandshake, TAck> : HandshakeHandlerBase<THandshake, TAck, Void> {

	private readonly GenerateHandshakeDelegate<THandshake> _generate;
	private readonly ReceiveHandshakeDelegate<THandshake, TAck> _receive;
	private readonly Verify2WayHandshakeDelegate<THandshake, TAck> _verify;

	public Action2WayHandshakeHandler(GenerateHandshakeDelegate<THandshake> generateHandshake, ReceiveHandshakeDelegate<THandshake, TAck> receiveHandshake, Verify2WayHandshakeDelegate<THandshake, TAck> verifyHandshake) {
		Guard.ArgumentNotNull(generateHandshake, nameof(generateHandshake));
		Guard.ArgumentNotNull(receiveHandshake, nameof(receiveHandshake));
		Guard.ArgumentNotNull(verifyHandshake, nameof(verifyHandshake));
		_generate = generateHandshake;
		_receive = receiveHandshake;
		_verify = verifyHandshake;

	}

	protected override THandshake GenerateHandshakeInternal(ProtocolOrchestrator orchestrator)
		=> _generate(orchestrator);

	public override HandshakeOutcome ReceiveHandshake(ProtocolOrchestrator orchestrator, THandshake handshake, out TAck acknowledgement)
		=> _receive(orchestrator, handshake, out acknowledgement);

	public override HandshakeOutcome VerifyHandshake(ProtocolOrchestrator orchestrator, THandshake handshake, TAck acknowledgement, out Void verifyAcknowledgement) {
		verifyAcknowledgement = default;
		return _verify(orchestrator, handshake, acknowledgement);
	}

	public override bool AcknowledgeHandshake(ProtocolOrchestrator orchestrator, THandshake handshake, TAck acknowledgement, Void verifyAcknowledgement)
		=> throw new NotSupportedException("2-way handshakes do not involve acknowledgement verifications");

}
