using System;

namespace VelocityNET.ProtocolObjects {
	public enum AccountType : UInt16 {
		// 0 - 999 are reserved
		Standard = 1,
		Delegate = 2,
		Candidate = 3,
		// from 1000+ are for layer2 applications
	}
}