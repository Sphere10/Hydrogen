// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.Communications;

public class ProtocolHandshake {

	public ProtocolHandshakeType Type { get; set; } = ProtocolHandshakeType.None;

	public CommunicationRole Initiator { get; set; } = CommunicationRole.Client;

	public IHandshakeHandler Handler { get; set; }

	public IList<Type> MessageTypes { get; } = new List<Type>();

	public Type SyncMessageType {
		get {
			Guard.Ensure(Type != ProtocolHandshakeType.None, "No handshake defined");
			Guard.Ensure(MessageTypes.Count > 0, "No SYNC Message is defined");
			return MessageTypes[0];
		}
	}

	public Type AckMessageType {
		get {
			Guard.Ensure(Type != ProtocolHandshakeType.None, "No handshake defined");
			Guard.Ensure(MessageTypes.Count > 1, "No ACK Message is defined");
			return MessageTypes[1];
		}
	}

	public Type VerackMessageType {
		get {
			Guard.Ensure(Type == ProtocolHandshakeType.ThreeWay, "Verack message only exists in 3-way handshakes");
			Guard.Ensure(MessageTypes.Count > 2, "No VERACK Message is defined");
			return MessageTypes[2];
		}
	}


}
