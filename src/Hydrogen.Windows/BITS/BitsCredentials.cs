//-----------------------------------------------------------------------
// <copyright file="BitsCredentials.cs" company="Sphere 10 Software">
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
using System.Text;
using System.Security;


namespace Hydrogen.Windows.BITS
{
    public class BitsCredentials
    {
        AuthenticationScheme authenticationScheme;
        AuthenticationTarget authenticationTarget;
        string  userName;
        string password;

        public AuthenticationScheme AuthenticationScheme
        {
            get { return this.authenticationScheme; }
            set { this.authenticationScheme = value; }
        }

        public AuthenticationTarget AuthenticationTarget
        {
            get { return this.authenticationTarget; }
            set { this.authenticationTarget = value; }
        }

        public string UserName
        {
            get { return this.userName; }
            set { this.userName = value; }
        }

        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }
    }
}
