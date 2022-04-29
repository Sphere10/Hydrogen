//-----------------------------------------------------------------------
// <copyright file="UserOperatorPrivileges.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Windows.Security {

    [Flags]
    public enum UserOperatorPriviliges {
        /// <summary>
        /// The print operator privilege is enabled.
        /// </summary>
        PrintOperator = 0x1,

        /// <summary>
        /// The communications operator privilege is enabled.
        /// </summary>
        CommunicationOperator = 0x2,

        /// <summary>
        /// The server operator privilege is enabled.
        /// </summary>
        ServerOperator = 0x4,

        /// <summary>
        /// The accounts operator privilege is enabled.
        /// </summary>
        AccountOperator = 0x8
    }

}
