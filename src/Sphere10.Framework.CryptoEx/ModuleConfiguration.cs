//-----------------------------------------------------------------------
// <copyright file="ModuleConfiguration.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Digests;
using Sphere10.Framework;

namespace Sphere10.Framework.CryptoEx {
	public static class ModuleConfiguration  {

		public static void Initialize() {
			Hashers.Register(CHF.RIPEMD_160, () => new GeneralDigestAdapter(new RipeMD160Digest()));

		}

		public static void Finalize() {

		}
		
	}
}
