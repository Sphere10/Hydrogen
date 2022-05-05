//-----------------------------------------------------------------------
// <copyright file="NetApiException.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Windows {
    public class NetApiException : WindowsException {

        public NetApiException(WinAPI.NETAPI32.NET_API_STATUS status, string errMsg, params object[] formatArgs ) : base((int) status, errMsg, formatArgs) {
        }
    }
}
