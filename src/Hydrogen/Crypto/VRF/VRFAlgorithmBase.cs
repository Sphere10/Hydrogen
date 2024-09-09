using System;

namespace Hydrogen;

public abstract class VRFAlgorithmBase : IVRFAlgorithm {
	public abstract int OutputLength { get; }

	public void Run(ReadOnlySpan<byte> seed, IPrivateKey privateKey, ulong nonce, Span<byte> output, out byte[] proof) {
		Guard.ArgumentNotNull(privateKey, nameof(privateKey));
		RunInternal(seed, privateKey, nonce, out proof);
		CalculateOutput(proof, output);
	}

	protected abstract void RunInternal(ReadOnlySpan<byte> seed, IPrivateKey privateKey, ulong nonce, out byte[] proof);

	public bool TryVerify(ReadOnlySpan<byte> seed, ReadOnlySpan<byte> output, ReadOnlySpan<byte> proof, IPublicKey publicKey) {
		Guard.Argument(!output.IsEmpty, nameof(output), "Is null or empty");
		Guard.Argument(!proof.IsEmpty, nameof(proof), "Is null or empty");
		Guard.ArgumentNotNull(publicKey, nameof(publicKey));
		Span<byte> verifiedOutput = stackalloc byte[OutputLength];
		CalculateOutput(proof, verifiedOutput);

		if (!output.SequenceEqual(verifiedOutput)) 
			return false;

		return TryVerifyInternal(seed, proof, publicKey);
	}

	protected abstract bool TryVerifyInternal(ReadOnlySpan<byte> seed, ReadOnlySpan<byte> proof, IPublicKey publicKey);

	public abstract void CalculateOutput(ReadOnlySpan<byte> proof, Span<byte> output); 

	public byte[] CalculateOutput(ReadOnlySpan<byte> proof) {
		Guard.Argument(!proof.IsEmpty, nameof(proof), "Is null or empty");
		var output = new byte[OutputLength];
		CalculateOutput(proof, output);
		return output;
	}
}
