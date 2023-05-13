//-----------------------------------------------------------------------
// <copyright file="AuthenticationResult.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

namespace Hydrogen {
	public class AuthenticationResult {

		public AuthenticationErrorCode ResultCode { get; set; }

		public object UserObject { get; set; }

		public static AuthenticationResult From(AuthenticationErrorCode resultCode, object userObject)
			=> new AuthenticationResult {
				ResultCode = resultCode,
				UserObject = userObject
			};
	}
}
