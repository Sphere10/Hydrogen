using System;
using System.Runtime.CompilerServices;

namespace Sphere10.Framework {
    public class SaltedSignatureScheme<TPrivateKey, TPublicKey> : DigitalSignatureSchemeDecorator<TPrivateKey, TPublicKey>
    where TPrivateKey : IPrivateKey
    where TPublicKey : IPublicKey {
        private readonly int _saltSizeBytes;

        public SaltedSignatureScheme(CHF saltingCHF, int saltSizeBytes, IDigitalSignatureScheme internalScheme)
            : base(internalScheme) {
            SaltingCHF = saltingCHF;
            _saltSizeBytes = saltSizeBytes;
        }
        public CHF SaltingCHF { get; }

        public override DigitalSignatureSchemeTraits Traits => Internal.Traits & DigitalSignatureSchemeTraits.Salted;

        public virtual byte[] GenerateSalt(ReadOnlySpan<byte> message) 
            => Tools.Crypto.GenerateCryptographicallyRandomBytes(_saltSizeBytes);

        public virtual byte[] JoinSalt(ReadOnlySpan<byte> other, ReadOnlySpan<byte> salt)
              => Tools.Array.Concat(other, salt);

        public virtual void SplitSalt(ReadOnlySpan<byte> saltedSignature, out ReadOnlySpan<byte> other, out ReadOnlySpan<byte> salt) {
            other = saltedSignature.Slice(0, saltedSignature.Length - _saltSizeBytes);
            salt = saltedSignature.Slice(^_saltSizeBytes);
        }

        public override byte[] CalculateMessageDigest(ReadOnlySpan<byte> message) {
            var salt = GenerateSalt(message);
            var messageDigest = SMAC(message, salt);
            // Note: The salt aggregated to end of message digest here, but the salt is
            // extracted duing internal signing and verifying!
            return JoinSalt(messageDigest, salt);
        }

        public override byte[] SignDigest(IPrivateKey privateKey, ReadOnlySpan<byte> messageDigest, ulong signerNonce) {
            Guard.ArgumentNotNull(privateKey, nameof(privateKey));
            SplitSalt(messageDigest, out var actualMessageDigest, out var salt);
            var signature = Internal.SignDigest(privateKey, actualMessageDigest, signerNonce);
            var saltedSignature = JoinSalt(signature, salt);
            return saltedSignature;
        }

        public override bool VerifyDigest(ReadOnlySpan<byte> saltedSignature, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> publicKey) {
            SplitSalt(messageDigest, out var actualDigest, out var saltInDigest);
            SplitSalt(saltedSignature, out var signature, out var saltInSignature);
            if (!saltInDigest.SequenceEqual(saltInSignature))
                return false;
            return Internal.VerifyDigest(signature, actualDigest, publicKey);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte[] SMAC(ReadOnlySpan<byte> message, ReadOnlySpan<byte> seed)
            => HMAC(Internal.CalculateMessageDigest(message), seed);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte[] HMAC(ReadOnlySpan<byte> digest, ReadOnlySpan<byte> seed) {
            using (Hashers.BorrowHasher(SaltingCHF, out var hasher)) {
                hasher.Transform(seed);
                hasher.Transform(digest);
                var innerHash = hasher.GetResult();
                hasher.Transform(seed);
                hasher.Transform(innerHash);
                return hasher.GetResult();
            }
        }


    }


}
