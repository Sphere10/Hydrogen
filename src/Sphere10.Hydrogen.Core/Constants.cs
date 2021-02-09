using System;
using System.Collections.Generic;
using System.Text;
using Sphere10.Framework;

namespace Sphere10.Hydrogen.Core {

	public static class Constants {

		public static readonly Func<byte[], byte[]> Hasher = (b) => Hashers.Hash(CHF.SHA2_256, b);

		public static readonly byte[] ZeroHash = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

		public static readonly Func<byte[], byte[]> SealHash = (x) => Hashers.Hash(CHF.Blake2b_160, Hashers.Hash(CHF.SHA2_256, x));

		public static readonly byte[] SealGenesis = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

		public static readonly byte[] SealNull = new byte[0];

		public const int HashLength = 32;

		public const int Winternitz_W_Param = 8;
		
	}
}
