// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.BITS;

public class BitsCredentials {
	AuthenticationScheme authenticationScheme;
	AuthenticationTarget authenticationTarget;
	string userName;
	string password;

	public AuthenticationScheme AuthenticationScheme {
		get { return this.authenticationScheme; }
		set { this.authenticationScheme = value; }
	}

	public AuthenticationTarget AuthenticationTarget {
		get { return this.authenticationTarget; }
		set { this.authenticationTarget = value; }
	}

	public string UserName {
		get { return this.userName; }
		set { this.userName = value; }
	}

	public string Password {
		get { return this.password; }
		set { this.password = value; }
	}
}
