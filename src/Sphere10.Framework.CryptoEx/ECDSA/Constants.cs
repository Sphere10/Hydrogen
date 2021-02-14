namespace Sphere10.Framework.CryptoEx
{
    public static class Constants
    {
        public const ushort CT_NID_secp256k1 = 714;
        public const ushort CT_NID_secp384r1 = 715;
        public const ushort CT_NID_sect283k1 = 729;
        public const ushort CT_NID_secp521r1 = 716;
        internal const ushort CT_Default_EC_OpenSSL_NID = CT_NID_secp256k1;

        internal const ushort CT_PROTOCOL_1 = 1;
        internal const ushort CT_PROTOCOL_2 = 2;
        internal const ushort CT_PROTOCOL_3 = 3;
        internal const ushort CT_PROTOCOL_4 = 4;
        internal const ushort CT_PROTOCOL_5 = 5;
        internal const ushort CT_PROTOCOL_6 = 6;
        internal const ushort CT_PROTOCOL_MAX = CT_PROTOCOL_6;
        internal const ushort CT_BUILD_PROTOCOL = CT_PROTOCOL_5;
    }
}