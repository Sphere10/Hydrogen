using System;
using System.IO;
using System.Linq;
using Sphere10.Framework;

namespace Sphere10.Framework
{
    public class ECPrivateKey
    {
        private static readonly ECDSA_Public CT_TECDSA_Public_Nul = default;

        private ECDSA_Public _bufferedPublicKey;
        private ECPrivateKeyInfo _privateKeyInfo;

        public ECPrivateKey()
        {
            _privateKeyInfo.RAW_PrivKey = null;
            _privateKeyInfo.EC_OpenSSL_NID = Constants.CT_Default_EC_OpenSSL_NID;
            _bufferedPublicKey = CT_TECDSA_Public_Nul;
        }

        public ushort EC_OpenSSL_NID => _privateKeyInfo.EC_OpenSSL_NID;
        public ECPrivateKeyInfo PrivateKey => _privateKeyInfo;

        public ECDSA_Public PublicKey
        {
            get
            {
                if (_bufferedPublicKey.EC_OpenSSL_NID != CT_TECDSA_Public_Nul.EC_OpenSSL_NID) return _bufferedPublicKey;
                var result = CryptoLibHelper.DoGetPublicKey(EC_OpenSSL_NID, _privateKeyInfo.RAW_PrivKey);
                _bufferedPublicKey = result;
                return result;
            }
        }

        ~ECPrivateKey() => _privateKeyInfo.RAW_PrivKey = null;

        private void SetPrivateKeyInfo(ECPrivateKeyInfo value)
        {
            _privateKeyInfo = value;
            _bufferedPublicKey = CT_TECDSA_Public_Nul;
        }

        public ECPrivateKey GenerateRandomPrivateKey(ushort ecOpensslNid)
        {
            _privateKeyInfo.EC_OpenSSL_NID = ecOpensslNid;
            _privateKeyInfo.RAW_PrivKey = CryptoLibHelper.DoGetRandomPrivateKey(EC_OpenSSL_NID);
            _bufferedPublicKey = CT_TECDSA_Public_Nul;
            return this;
        }

        internal bool SetPrivateKeyFromHexa(ushort ecOpensslNid, string hexString)
        {
            var tmp = Array.Empty<byte>();
            if (!ECCrypto.HexaToRaw(hexString, ref tmp)) throw new CryptoException("Unable to parse HexString");
            _privateKeyInfo.EC_OpenSSL_NID = ecOpensslNid;
            _privateKeyInfo.RAW_PrivKey = tmp;
            _bufferedPublicKey = CT_TECDSA_Public_Nul;
            return true;
        }

        internal static bool IsValidPublicKey(ECDSA_Public pubKey, ushort currentProtocol, ref string errors)
        {
            if (!AccountComp.IsValidEC_OpenSSL_NID(pubKey.EC_OpenSSL_NID))
            {
                errors = $"Invalid NID {pubKey.EC_OpenSSL_NID}";
                return false;
            }

            if (pubKey.X == null) return false;
            if (pubKey.Y == null) return false;
            var result = pubKey.X.Length < 100 && pubKey.Y.Length < 100;
            if (currentProtocol >= Constants.CT_PROTOCOL_5) return result;
            return true;
        }

        internal static bool IsValidPublicKey(ECDSA_Public pubKey, ushort currentProtocol)
        {
            var tmp = "";
            return IsValidPublicKey(pubKey, currentProtocol, ref tmp);
        }

        // Exports a Private key in a RAW saving 2 bytes for EC_OpenSSL_NID, 2 bytes for private key length and private key as a RAW
        public byte[] ExportToRaw()
        {
            byte[] result;
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(_privateKeyInfo.EC_OpenSSL_NID);
                    var aux = _privateKeyInfo.RAW_PrivKey;
                    StreamOp.WriteBytes(ms, ref aux);
                    result = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(result, 0, (int) ms.Length);
                }
            }

            return result;
        }

        // Imports a Private key saved with "ExportToRaw" format
        public static ECPrivateKey ImportFromRaw(byte[] raw)
        {
            ECPrivateKeyInfo npki = default;
            npki.EC_OpenSSL_NID = 0;
            npki.RAW_PrivKey = null;
            var aux = Array.Empty<byte>();

            using (var ms = new MemoryStream())
            {
                using (var br = new BinaryReader(ms))
                {
                    ms.Write(raw, 0, raw.Length);
                    ms.Position = 0;
                    npki.EC_OpenSSL_NID = br.ReadUInt16();
                    if (!AccountComp.IsValidEC_OpenSSL_NID(npki.EC_OpenSSL_NID)) return null;
                    if (StreamOp.ReadBytes(ms, ref aux) < 0) return null;
                }
            }

            npki.RAW_PrivKey = aux;
            var result = new ECPrivateKey();
            result.SetPrivateKeyInfo(npki);
            return result;
        }

        // Exports only the private key as a Raw, without info of EC_OpenSSL_NID
        // Return only Private key without info of curve used
        // NOTE: Only returns private key as a RAW without info of EC_OPENSSL_NID
        public byte[] PrivateKeyAsRaw() =>
            !HasPrivateKey() ? Array.Empty<byte>() : _privateKeyInfo.RAW_PrivKey.DeepCopy();

        public bool HasPrivateKey() => _privateKeyInfo.RAW_PrivKey.Any();
    }
}