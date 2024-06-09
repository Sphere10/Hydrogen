// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Communications;

public class ProtocolMessageEnvelope {
	public ProtocolDispatchType DispatchType { get; init; }
	public int RequestID { get; init; }
	public object Message { get; init; }

	public override string ToString() => $"[Protocol Message Envelope] DispatchType: {DispatchType}, RequestID: {RequestID}, Message: {Message ?? "NULL"}";
}
