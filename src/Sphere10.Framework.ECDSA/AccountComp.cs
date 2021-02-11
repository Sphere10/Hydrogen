using System;
using System.Linq;

namespace Sphere10.Framework
{
    internal static class AccountComp
    {
        internal static bool IsValidEC_OpenSSL_NID(ushort nid) =>
            new[]
            {
                Constants.CT_NID_secp256k1, Constants.CT_NID_secp384r1, Constants.CT_NID_secp521r1,
                Constants.CT_NID_sect283k1
            }.Contains(nid);
    }
}