using System;

namespace Sphere10.Framework {

	[Flags]
	public enum DigitalSignatureSchemeTraits {
		None	     = 0,
		OTS          = 1 << 0,
		Salted       = 1 << 1,
		Stateless    = 1 << 2,
		Stateful     = 1 << 3,
		SupportsIES  = 1 << 4,
		ECDSA        = 1 << 5,
		Schnorr      = 1 << 6,
		PQC          = 1 << 7,

	}

}
