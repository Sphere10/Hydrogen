namespace Hydrogen;

internal class StreamMappedDictionaryKeyValueHasher : IItemHasher<long> {
	private readonly IStreamMappedDictionary _smpDict;
	private readonly CHF _chf;

	public StreamMappedDictionaryKeyValueHasher(IStreamMappedDictionary smpDict,  CHF chf) {
		_smpDict = smpDict;
		_chf = chf;
		DigestLength = Hashers.GetDigestSizeBytes(chf);	
	}

	public int DigestLength { get; }

	public byte[] Hash(long index) {
		var descriptor = _smpDict.ObjectStream.GetItemDescriptor(index);
		if (descriptor.Traits.HasFlag(ClusteredStreamTraits.Reaped))
			return Hashers.ZeroHash(_chf);

		var keyBytes = _smpDict.ReadKeyBytes(index);
		var keyDigest = Hashers.HashWithNullSupport(_chf, keyBytes);
		var valueBytes = _smpDict.ReadValueBytes(index);
		var valueDigest = Hashers.HashWithNullSupport(_chf, valueBytes);
		return Hashers.JoinHash(_chf, keyDigest, valueDigest);
	}
	
}
