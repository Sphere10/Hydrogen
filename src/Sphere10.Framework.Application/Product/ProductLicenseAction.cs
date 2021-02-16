//-----------------------------------------------------------------------
// <copyright file="ProductLicenseAction.cs" company="Sphere 10 Software">
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

/// WARNING: Make sure that these two files are synchronized
///     SchoenfeldSoftware.Application/Licensing/ProductLicenseAction.cs
///     www.sphere10.com/OnlineServices/ProductLicenseAction.cs
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Sphere10.Framework.Application {

    [Obfuscation(Exclude = true)]
    public enum ProductLicenseAction {
        Enable=0,
        DisableSoftware=1,
        CrippleSoftware=2
    }

}
