// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

[Flags]
public enum DigitalSignatureSchemeTraits {
	None = 0,
	OTS = 1 << 0,
	Salted = 1 << 1,
	Stateless = 1 << 2,
	Stateful = 1 << 3,
	SupportsIES = 1 << 4,
	ECDSA = 1 << 5,
	Schnorr = 1 << 6,
	PQC = 1 << 7,

}
