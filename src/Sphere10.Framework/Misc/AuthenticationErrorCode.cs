//-----------------------------------------------------------------------
// <copyright file="AuthenticationErrorCode.cs" company="Sphere 10 Software">
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

using System.ComponentModel;

namespace Sphere10.Framework {
	public enum AuthenticationErrorCode {
		[Description("Logged on")]
		Success,

		[Description("Logon aborted")]
		Aborted,

		[Description("Wrong username and/or password")]
		InvalidCredentials,

		[Description("Server was unavailable")]
		ServerUnavailable
	}
}
