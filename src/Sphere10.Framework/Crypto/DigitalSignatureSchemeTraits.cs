using System;

namespace Sphere10.Framework {

	[Flags]
	public enum DigitalSignatureSchemeTraits {
		None	  = 0,
		OTS       = 1 << 0,
		Salted	  = 1 << 1,
		Stateless = 1 << 2,
		Stateful  = 1 << 3,
		ECDSA     = 1 << 4,
		Schnorr   = 1 << 5,
		PQC       = 1 << 6,
	}

}
