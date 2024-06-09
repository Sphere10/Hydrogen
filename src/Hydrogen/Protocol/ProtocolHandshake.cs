// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Communications;

public class ProtocolHandshake {

	public ProtocolHandshakeType Type { get; init; } = ProtocolHandshakeType.None;

	public CommunicationRole Initiator { get; init; } = CommunicationRole.Client;

	public IHandshakeHandler Handler { get; init; }

	public Type[] MessageTypes { get; init;} = Array.Empty<Type>();

	public Type SyncMessageType {
		get {
			Guard.Ensure(Type != ProtocolHandshakeType.None, "No handshake defined");
			Guard.Ensure(MessageTypes.Length > 0, "No SYNC Message is defined");
			return MessageTypes[0];
		}
	}

	public Type AckMessageType {
		get {
			Guard.Ensure(Type != ProtocolHandshakeType.None, "No handshake defined");
			Guard.Ensure(MessageTypes.Length > 1, "No ACK Message is defined");
			return MessageTypes[1];
		}
	}

	public Type VerackMessageType {
		get {
			Guard.Ensure(Type == ProtocolHandshakeType.ThreeWay, "Verack message only exists in 3-way handshakes");
			Guard.Ensure(MessageTypes.Length > 2, "No VERACK Message is defined");
			return MessageTypes[2];
		}
	}


}
