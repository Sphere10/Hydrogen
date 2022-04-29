//-----------------------------------------------------------------------
// <copyright file="ProductLicenseFlags.cs" company="Sphere 10 Software">
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
using System.Reflection;

namespace Sphere10.Framework.Application {

	[Obfuscation(Exclude = true)]
    [Flags]
    public enum ProductLicenseFlags {
        DoesNotExpire				= 0,
        LimitedFeatureSet			= 1,
        DisableOnExpiration			= 2,
        CrippleOnExpiration			= 4,
        ExpiresAfterDate			= 8,
        ExpiresAfterDays			= 16,
        ExpiresAfterLoads			= 32,
        LimitSimultaneousUsers		= 64,
        VersionAware				= 128,
    }
}
