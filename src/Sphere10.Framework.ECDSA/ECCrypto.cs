using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Sphere10.Framework
{

    public static class ECCrypto
    {
        private static readonly ECDSA_SIG CT_TECDSA_SIG_Nul = default;

        public static bool IsHexString(string hexString) => hexString.All("0123456789abcdefABCDEF".Contains);

        public static byte[] HexaToRaw(string hexString)
        {
            var result = Array.Empty<byte>();
            HexaToRaw(hexString, ref result);
            return result;
        }

        public static bool HexaToRaw(string hexString, ref byte[] raw)
        {
            if (hexString.Length == 0)
            {
                raw = Array.Empty<byte>();
                return true;
            }

            if (!IsHexString(hexString)) return false;
            if ((hexString.Length & 1) != 0) return false; // odd string
            raw = HexEncoding.Decode(hexString);
            return true;
        }

        public static string RawToHex(byte[] raw)
        {
            var result = "";
            RawToHex(raw, ref result);
            return result;
        }

        public static bool RawToHex(byte[] raw, ref string hexString)
        {
            if (raw.Length == 0)
            {
                hexString = "";
                return true;
            }

            hexString = HexEncoding.Encode(raw);
            return true;
        }

        public static byte[] DoSha256(byte[] message) => CryptoLibHelper.DoSHA256(message);

        public static void DoSha256(byte[] message, out byte[] resultSha256) => resultSha256 = DoSha256(message);

        public static byte[] DoDoubleSha256(byte[] message) => DoSha256(DoSha256(message));

        public static byte[] DoRipeMD160AsRaw(byte[] message) => CryptoLibHelper.DoRIPEMD160(message);

        public static byte[] DoRipeMD160_HEXASTRING(byte[] message) =>
            ByteArrayUtils.FromString(DoRipeMD160AsRaw(message).ToHexaString().Substring(0, 40));

        //public static byte[] DoRandomHash(byte[] message) => RandomHashFast.Compute(message);

        //  public static byte[] DoRandomHash2(byte[] message) => RandomHash2Fast.Compute(message);

        public static string PrivateKey2Hexa(ECPrivateKeyInfo privateKeyInfo) =>
            privateKeyInfo.RAW_PrivKey.ToHexaString();

        public static ECDSA_SIG ECDSASign(ECPrivateKeyInfo keyInfo, byte[] digest)
        {
            ECDSA_SIG result = default;
            CryptoLibHelper.DoECDSASign(keyInfo.EC_OpenSSL_NID, keyInfo.RAW_PrivKey, digest, ref result);
            return result;
        }

        public static bool ECDSAVerify(ECDSA_Public pubKey, byte[] digest, ECDSA_SIG signature) =>
            CryptoLibHelper.DoECDSAVerify(pubKey, digest, signature);

        public static bool IsHumanReadable(byte[] readableText) => readableText.All(t => t >= 32 && t < 127);

        public static byte[] EncodeSignature(ECDSA_SIG signature)
        {
            byte[] result;
            using (var ms = new MemoryStream())
            {
                StreamOp.WriteBytes(ms, ref signature.R);
                StreamOp.WriteBytes(ms, ref signature.S);
                result = StreamOp.SaveStreamToRaw(ms);
            }

            return result;
        }

        public static bool DecodeSignature(byte[] rawSignature, out ECDSA_SIG signature)
        {
            signature = CT_TECDSA_SIG_Nul;
            using (var ms = new MemoryStream())
            {
                StreamOp.LoadStreamFromRaw(ms, rawSignature);
                ms.Position = 0;
                if (StreamOp.ReadBytes(ms, ref signature.R) < 0) return false;
                if (StreamOp.ReadBytes(ms, ref signature.S) < 0) return false;
                if (ms.Position < ms.Length) return false; // Invalid position
            }

            return true;
        }

        public static byte[] DoPascalCoinAESEncrypt(byte[] message, byte[] password) =>
            CryptoLibHelper.DoPascalCoinAESEncrypt(message, password);

        public static bool DoPascalCoinAESDecrypt(byte[] encryptedMessage, byte[] password,
            ref byte[] decryptedMessage) => CryptoLibHelper.DoPascalCoinAESDecrypt(encryptedMessage, password,
            ref decryptedMessage);

        public static bool DoPascalCoinECIESEncrypt(ECDSA_Public publicKey, byte[] message,
            ref byte[] encryptedMessage) => CryptoLibHelper.DoPascalCoinECIESEncrypt(publicKey, message,
            ref encryptedMessage);

        public static bool DoPascalCoinECIESDecrypt(
            ushort ecOpensslNid, byte[] privateKey, byte[] encryptedMessage, ref byte[] decryptedMessage) =>
            CryptoLibHelper.DoPascalCoinECIESDecrypt(
                ecOpensslNid, privateKey, encryptedMessage, ref decryptedMessage);
    }
}