//-----------------------------------------------------------------------
// <copyright file="ConnectionStringSettingsCollectionExtension.cs" company="Sphere 10 Software">
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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Framework.Application {
    public static class Extension {
        public static bool HasConnectionString(this ConnectionStringSettingsCollection value, string key) {
            try {
                return value[key].ConnectionString.Length > 0;
            } catch {
                return false;
            }
        }
    }
}
