// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//using System;
//using System.Collections.Generic;
//using System.Numerics;
//using System.Text;
//using Hydrogen;

//namespace Hydrogen.DApp.Core.Maths {
//    public static class CryptoTool {

//        /// <summary>
//        /// Derives a checksum for a secret without revealing any information about that secret.
//        /// </summary>
//        /// <param name="secret">secret to checksum</param>
//        /// <returns>A 32 bit checksum (secure)</returns>
//        public static uint DeriveSecureChecksum(byte[] secret) {
//            // Checksum's are public and used for fast lookups of secret, yet do not reveal any
//            // information about secret.
//            // Checksum = CastToUInt32( Last4BytesLE( SHA2-256( SHA2-256( secret || secret ) ) )
//            return
//                EndianBitConverter.Little.ToUInt32(
//                    Hashers.SHA2_256D(Tools.Array.Concat(secret, secret)),
//                    32 - 4 - 1
//                );
//        }

//        public static byte[] DeriveChildDigest(byte[] digest, ulong index) {
//            // DerivedKey_i = H(H(i || seed))
//            // Knowing the set DerivedKey_0..DerivedKey_i reveals no info about seed, double hashing prevents
//            // length extension attacks.
//            return Hashers.SHA2_256D(Tools.Array.Concat(EndianBitConverter.Little.GetBytes((ulong)index), digest));
//        }

//    }
//}


