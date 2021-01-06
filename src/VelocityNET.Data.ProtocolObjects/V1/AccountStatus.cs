using System;

namespace VelocityNET.ProtocolObjects {
	public enum AccountStatus : Byte {
		Standard,
		FloatingSale,
		PublicSale,
		PrivateSale,
		AtomicAccountSwap,
		AtomicCoinSwap,
		Staking
	}
}