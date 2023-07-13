// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen;
using Hydrogen.Communications;
using System;

namespace AbstractProtocol.UDPSimple;

public class AppProtocol {
	public static Protocol Build() {
		return new ProtocolBuilder()
			.Handshake
			.ThreeWay
			.InitiatedBy(CommunicationRole.Client)
			.HandleWith<Sync, Ack, Verack>(InitiateHandshake, ReceiveHandshake, VerifyHandshake)
			.Requests
			.ForRequest<RequestBytes>().RespondWith(RequestBytes)
			.Responses
			.ForResponse<ReturnBytes>().ToRequest<RequestBytes>().HandleWith(HandleReturnBytes)
			//.Commands
			//	.ForCommand<NotifyNewTransaction>().Execute(HandleNewTransaction)
			.Messages
			.For<RequestBytes>().SerializeWith(new BinaryFormattedSerializer<RequestBytes>())
			.For<ReturnBytes>().SerializeWith(new BinaryFormattedSerializer<ReturnBytes>())
			.For<Sync>().SerializeWith(new BinaryFormattedSerializer<Sync>())
			.For<Ack>().SerializeWith(new BinaryFormattedSerializer<Ack>())
			.For<Verack>().SerializeWith(new BinaryFormattedSerializer<Verack>())
			.Build();
	}

	// This creates the handshake object by the initiator (client). The client can pass in arguments to convince server to accept connection.
	private static Sync InitiateHandshake(ProtocolOrchestrator orchestrator) {
		return new Sync {
			ClientID = "client name",
			Timestamp = DateTime.UtcNow
		};
	}

	// This handles the handshake by the receiver (server). Server can bail-out of this connection by return value.
	private static HandshakeOutcome ReceiveHandshake(ProtocolOrchestrator orchestrator, Sync handshake, out Ack ack) {
		// here we handle the handshake on server, we can check handshake for compliance
		ack = new Ack {
			ServerID = "server name",
			Timestamp = DateTime.UtcNow
		};
		return HandshakeOutcome.Accepted;
	}

	// This verifies the handshake by the initiator (client). Client can bail-out of this connection by return value.
	private static HandshakeOutcome VerifyHandshake(ProtocolOrchestrator orchestrator, Sync handshake, Ack response,
	                                                out Verack verack) {
		// here the client
		// can finalize the handshake (for 2-way)
		verack = new Verack();
		return HandshakeOutcome.Accepted;
	}


	private static ReturnBytes RequestBytes(ProtocolOrchestrator orchestrator, RequestBytes requestBytes) {
		SystemLog.Info("GetBytes");
		return new ReturnBytes();
	}

	private static void HandleReturnBytes(ProtocolOrchestrator orchestrator, RequestBytes requestBytes,
	                                      ReturnBytes returnBytes) {
		SystemLog.Info($"HandleReturnBytes: RequestBytes: {requestBytes}, ReturnBytes {returnBytes} ");
	}
}
