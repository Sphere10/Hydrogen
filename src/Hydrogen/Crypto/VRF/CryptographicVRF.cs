using System;

namespace Hydrogen;

public class CryptographicVRF : VRFAlgorithmBase {

	public CryptographicVRF(DSS dss, CHF chf) {
		CHF = chf;
		DSS = dss;
	}
	
	public CHF CHF { get; }

	public DSS DSS { get; }

	public override int OutputLength => Hashers.GetDigestSizeBytes(CHF);

	
	protected override void RunInternal(ReadOnlySpan<byte> seed, IPrivateKey privateKey, ulong nonce, out byte[] proof) {
		// Step 1: Hash the seed to create a digest
		Span<byte> seedDigest = stackalloc byte[Hashers.GetDigestSizeBytes(CHF)];
		using (Hashers.BorrowHasher(CHF, out var hasher)) {
			hasher.Transform(seed);
			hasher.GetResult(seedDigest);
		}

		// Step 2: Sign the seed digest using the private key to create the proof
		proof = Signers.Sign(DSS, privateKey, seedDigest, nonce);
	}

	protected override bool TryVerifyInternal(ReadOnlySpan<byte> seed, ReadOnlySpan<byte> proof, IPublicKey publicKey) {
		// Step 1: Recompute the message digest from the seed
		Span<byte> seedDigest = stackalloc byte[Hashers.GetDigestSizeBytes(CHF)];
		using (Hashers.BorrowHasher(CHF, out var hasher)) {
			hasher.Transform(seed);
			hasher.GetResult(seedDigest);
		}

		// Step 2: Verify the signature (proof) using the recomputed seed digest and public key
		var isSignatureValid = Signers.Verify(DSS, proof, seedDigest, publicKey);
		if (!isSignatureValid)
			return false;

		return true;

	}

	public override void CalculateOutput(ReadOnlySpan<byte> proof, Span<byte> output)
		=> Hashers.Hash(CHF, proof, output);

	public override string ToString() 
		=> $"{nameof(CHF)}: DSS = {Tools.Enums.GetHumanReadableName(DSS)}, CHF = {Tools.Enums.GetHumanReadableName(CHF)}]";



}
