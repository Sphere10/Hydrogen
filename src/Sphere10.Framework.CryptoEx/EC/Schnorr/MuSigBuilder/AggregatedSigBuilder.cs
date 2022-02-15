using System;
using System.Linq;

namespace Sphere10.Framework.CryptoEx.EC.MuSigBuilder;

public class AggregatedSigBuilder {
	private readonly MuSig _muSig;
	private readonly MuSigSessionCache _sessionCache;
	private readonly byte[][] _partialSigs;
	public AggregatedSigBuilder(MuSig muSig, MuSigSessionCache sessionCache, byte[][] partialSigs) {
		_muSig = muSig ?? throw new ArgumentNullException(nameof(muSig));
		_sessionCache = sessionCache ?? throw new ArgumentNullException(nameof(sessionCache));
		_partialSigs = partialSigs ?? throw new ArgumentNullException(nameof(partialSigs));
	}

	public byte[] Build() {
		if (!_partialSigs.Any()) {
			throw new Exception("partial sigs cannot be empty");
		}
		return _muSig.CombinePartialSigs(_sessionCache.FinalNonce, _partialSigs.Select(x => Schnorr.BytesToBigInt(x)).ToArray());
	}

}
